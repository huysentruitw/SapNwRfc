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
        /// Gets the metadata for the specified type name.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="cancellationToken">The cancellation token for cancelling the underlying operation.</param>
        /// <returns>The metadata for the type name.</returns>
        ISapTypeMetadata GetTypeMetadata(string typeName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the metadata for the specified function name.
        /// </summary>
        /// <param name="functionName">The function name.</param>
        /// <param name="cancellationToken">The cancellation token for cancelling the underlying operation.</param>
        /// <returns>The matadata for the function name.</returns>
        ISapFunctionMetadata GetFunctionMetadata(string functionName, CancellationToken cancellationToken = default);

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
