namespace SapNwRfc
{
    /// <summary>
    /// Interface for an SAP RFC server function.
    /// </summary>
    public interface ISapServerFunction
    {
        /// <summary>
        /// Gets the Name of the function.
        /// </summary>
        /// <returns>the name of the function.</returns>
        string GetName();

        /// <summary>
        /// Returns true if the <paramref name="parameterName"/> exists.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>Whether the parameter name exists.</returns>
        bool HasParameter(string parameterName);

        /// <summary>
        /// Gets the metadata for this function.
        /// </summary>
        /// <returns>The metadata for this function.</returns>
        ISapFunctionMetadata GetMetadata();

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        /// <typeparam name="TOutput">the parameters type.</typeparam>
        /// <returns>the parameters.</returns>
        TOutput GetParameters<TOutput>();

        /// <summary>
        /// Sets the result of the function.
        /// </summary>
        /// <param name="input">the result.</param>
        void SetResult(object input);
    }
}
