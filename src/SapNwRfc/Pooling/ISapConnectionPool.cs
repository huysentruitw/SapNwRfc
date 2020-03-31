using System;
using System.Threading;

namespace SapNwRfc.Pooling
{
    /// <summary>
    /// Interface for the SAP RFC connection pool.
    /// </summary>
    public interface ISapConnectionPool : IDisposable
    {
        /// <summary>
        /// This method is used by the <see cref="SapPooledConnection"/> class to get a free <see cref="SapConnection"/> from the pool.
        /// If the maximum pool size has been reached, this method will block until a connection is returned.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> instance that allows cancelling the operation.</param>
        /// <returns>A free connection.</returns>
        ISapConnection GetConnection(CancellationToken cancellationToken = default);

        /// <summary>
        /// This method is used by the <see cref="SapPooledConnection"/> class to return a working connection to the pool and make it available for reuse.
        /// When another <see cref="SapPooledConnection"/> is waiting for a connection, it will receive this returned connection.
        /// </summary>
        /// <param name="connection">The connection to return.</param>
        void ReturnConnection(ISapConnection connection);

        /// <summary>
        /// This method is used by the <see cref="SapPooledConnection"/> class to request the pool to forget a broken connection.
        /// When another <see cref="SapPooledConnection"/> is waiting for a connection, it will receive a new connection replacing this broken connection.
        /// </summary>
        /// <param name="connection">The connection to forget.</param>
        void ForgetConnection(ISapConnection connection);
    }
}
