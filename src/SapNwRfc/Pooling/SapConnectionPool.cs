using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace SapNwRfc.Pooling
{
    /// <summary>
    /// The SAP Connection pool class.
    /// </summary>
    public sealed class SapConnectionPool : ISapConnectionPool
    {
        private readonly SapConnectionParameters _connectionParameters;
        private readonly int _poolSize;
        private readonly Func<SapConnectionParameters, ISapConnection> _connectionFactory;
        private readonly TimeSpan _connectionIdleTimeout;
        private readonly ConcurrentQueue<(ISapConnection Connection, DateTime ExpiresAtUtc)> _idleConnections = new ConcurrentQueue<(ISapConnection Connection, DateTime ExpiresAtUtc)>();
        private readonly SemaphoreSlim _leases;
        private readonly Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapConnectionPool"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="poolSize">The size of the pool.</param>
        /// <param name="connectionIdleTimeout">The idle timeout after which unused open connections are disposed. Defaults to 30 seconds.</param>
        /// <param name="idleDetectionInterval">The interval at which to look for idling connections. Defaults to 1 second.</param>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "RedundantOverload.Global", Justification = "Public constructor should not expose connection factory")]
        public SapConnectionPool(
            string connectionString,
            int poolSize = 5,
            TimeSpan? connectionIdleTimeout = null,
            TimeSpan? idleDetectionInterval = null)
            : this(SapConnectionParameters.Parse(connectionString), poolSize, connectionIdleTimeout, idleDetectionInterval, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SapConnectionPool"/> class.
        /// </summary>
        /// <param name="connectionParameters">The connection parameters.</param>
        /// <param name="poolSize">The size of the pool.</param>
        /// <param name="connectionIdleTimeout">The idle timeout after which unused open connections are disposed. Defaults to 30 seconds.</param>
        /// <param name="idleDetectionInterval">The interval at which to look for idling connections. Defaults to 1 second.</param>
        [ExcludeFromCodeCoverage]
        public SapConnectionPool(
            SapConnectionParameters connectionParameters,
            int poolSize = 5,
            TimeSpan? connectionIdleTimeout = null,
            TimeSpan? idleDetectionInterval = null)
            : this(connectionParameters, poolSize, connectionIdleTimeout, idleDetectionInterval, null)
        {
        }

        internal SapConnectionPool(
            SapConnectionParameters connectionParameters,
            int poolSize = 5,
            TimeSpan? connectionIdleTimeout = null,
            TimeSpan? idleDetectionInterval = null,
            Func<SapConnectionParameters, ISapConnection> connectionFactory = null)
        {
            _connectionParameters = connectionParameters;
            _poolSize = poolSize;
            _connectionIdleTimeout = connectionIdleTimeout ?? TimeSpan.FromSeconds(30);
            _connectionFactory = connectionFactory ?? (parameters => new SapConnection(parameters));
            _leases = new SemaphoreSlim(poolSize, poolSize);
            _timer = new Timer(
                callback: _ => DisposeIdleConnections(),
                state: null,
                dueTime: idleDetectionInterval ?? TimeSpan.FromSeconds(1),
                period: idleDetectionInterval ?? TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Disposes the SAP Connection pool and all idling connections.
        /// </summary>
        public void Dispose()
        {
            _leases.Dispose();
            _timer.Dispose();
            while (_idleConnections.TryDequeue(out (ISapConnection Connection, DateTime ExpiresAtUtc) idleConnection))
                idleConnection.Connection.Dispose();
        }

        /// <inheritdoc cref="ISapConnectionPool"/>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Don't change double-checked lock")]
        public ISapConnection GetConnection(CancellationToken cancellationToken = default)
        {
            _leases.Wait(cancellationToken);

            while (_idleConnections.TryDequeue(out (ISapConnection Connection, DateTime ExpiresAtUtc) idleConnection))
            {
                if (idleConnection.ExpiresAtUtc > DateTime.UtcNow)
                    return idleConnection.Connection;

                idleConnection.Connection.Dispose();
            }

            ISapConnection connection = _connectionFactory(_connectionParameters);

            if (connection == null)
            {
                _leases.Release();
                throw new InvalidOperationException("The connection factory should never return null");
            }

            try
            {
                connection.Connect();
            }
            catch
            {
                ForgetConnection(connection);
                throw;
            }

            return connection;
        }

        /// <inheritdoc cref="ISapConnectionPool"/>
        public void ReturnConnection(ISapConnection connection)
        {
            _ = connection ?? throw new ArgumentNullException(nameof(connection));
            if (_idleConnections.Count < _poolSize)
            {
                DateTime expiresAtUtc = DateTime.UtcNow + _connectionIdleTimeout;
                _idleConnections.Enqueue((Connection: connection, ExpiresAtUtc: expiresAtUtc));
            }
            else
            {
                connection.Dispose();
            }

            _leases.Release();
        }

        /// <inheritdoc cref="ISapConnectionPool"/>
        public void ForgetConnection(ISapConnection connection)
        {
            _ = connection ?? throw new ArgumentNullException(nameof(connection));
            connection.Dispose();
            _leases.Release();
        }

        [SuppressMessage("ReSharper", "InvertIf", Justification = "Don't change double-checked lock")]
        private void DisposeIdleConnections()
        {
            while (_idleConnections.TryPeek(out (ISapConnection Connection, DateTime ExpiresAtUtc) idleConnection)
                   && idleConnection.ExpiresAtUtc <= DateTime.UtcNow)
            {
                if (_idleConnections.TryDequeue(out idleConnection))
                    idleConnection.Connection.Dispose();
            }
        }
    }
}
