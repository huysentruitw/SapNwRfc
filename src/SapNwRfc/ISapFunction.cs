using System;

namespace SapNwRfc
{
    /// <summary>
    /// Interface for an SAP RFC function.
    /// </summary>
    public interface ISapFunction : IDisposable
    {
        /// <summary>
        /// Invokes the remote function.
        /// </summary>
        void Invoke();

        /// <summary>
        /// Invokes the remote function with the given input parameters.
        /// </summary>
        /// <param name="input">The input parameters.</param>
        void Invoke(object input);

        /// <summary>
        /// Invokes the remote function and returns the output.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output model.</typeparam>
        /// <returns>The output.</returns>
        TOutput Invoke<TOutput>();

        /// <summary>
        /// Invokes the remote function with the given input parameters and returns the output.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output model.</typeparam>
        /// <param name="input">The input parameters.</param>
        /// <returns>The output.</returns>
        TOutput Invoke<TOutput>(object input);
    }
}
