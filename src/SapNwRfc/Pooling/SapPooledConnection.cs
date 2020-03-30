using System.Diagnostics.CodeAnalysis;
using System.Threading;
using SapNwRfc.Exceptions;

namespace SapNwRfc.Pooling
{
    public sealed class SapPooledConnection : ISapPooledConnection
    {
        private readonly ISapConnectionPool _pool;
        private ISapConnection _connection;
        private bool _disposed = false;

        public SapPooledConnection(ISapConnectionPool pool, CancellationToken cancellationToken = default)
        {
            _pool = pool;
            _connection = pool.GetConnection(cancellationToken);
        }

        [ExcludeFromCodeCoverage]
        ~SapPooledConnection()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _pool.ReturnConnection(_connection);
            _disposed = true;
        }

        public void InvokeFunction(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke();
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke();
            }
        }

        public void InvokeFunction(string name, object input, CancellationToken cancellationToken = default)
        {
            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke(input);
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke(input);
            }
        }

        public TOutput InvokeFunction<TOutput>(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>();
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>();
            }
        }

        public TOutput InvokeFunction<TOutput>(string name, object input, CancellationToken cancellationToken = default)
        {
            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>(input);
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>(input);
            }
        }
    }
}
