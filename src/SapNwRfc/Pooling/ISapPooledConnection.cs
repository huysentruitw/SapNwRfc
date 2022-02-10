using System;
using System.Threading;

namespace SapNwRfc.Pooling
{
    /// <summary>
    /// Interface for the SAP RFC pooled connection.
    /// </summary>
    public interface ISapPooledConnection : IDisposable
    {
        /// <summary>
        /// Invokes the remote function.
        /// </summary>
        /// <param name="name">The name of the remote function.</param>
        /// <param name="cancellationToken">The cancellation token for cancelling the underlying operation.</param>
        void InvokeFunction(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Invokes the remote function with the given input parameters.
        /// </summary>
        /// <param name="name">The name of the remote function.</param>
        /// <param name="input">The input parameters.</param>
        /// <param name="cancellationToken">The cancellation token for cancelling the underlying operation.</param>
        void InvokeFunction(string name, object input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Invokes the remote function and returns the output.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output model.</typeparam>
        /// <param name="name">The name of the remote function.</param>
        /// <param name="cancellationToken">The cancellation token for cancelling the underlying operation.</param>
        /// <returns>The output.</returns>
        TOutput InvokeFunction<TOutput>(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Invokes the remote function with the given input parameters and returns the output.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output model.</typeparam>
        /// <param name="name">The name of the remote function.</param>
        /// <param name="input">The input parameters.</param>
        /// <param name="cancellationToken">The cancellation token for cancelling the underlying operation.</param>
        /// <returns>The output.</returns>
        TOutput InvokeFunction<TOutput>(string name, object input, CancellationToken cancellationToken = default);
    }
}
