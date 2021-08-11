namespace SapNwRfc
{
    /// <summary>
    /// Interface for SAP RFC function metadata.
    /// </summary>
    public interface ISapFunctionMetadata
    {
        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <returns>The function name.</returns>
        string GetName();

        /// <summary>
        /// Gets the parameter count.
        /// </summary>
        /// <returns>The parameter count.</returns>
        uint GetParameterCount();

        /// <summary>
        /// Gets parameter metadata by index.
        /// </summary>
        /// <param name="index">The index of the parameter.</param>
        /// <returns>The parameter metadata.</returns>
        ISapParameterMetadata GetParameterByIndex(uint index);

        /// <summary>
        /// Gets parameter metadata by name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter metadata.</returns>
        ISapParameterMetadata GetParameterByName(string name);

        /// <summary>
        /// Gets the exception count.
        /// </summary>
        /// <returns>The exception count.</returns>
        uint GetExceptionCount();

        /// <summary>
        /// Gets exception metadata by index.
        /// </summary>
        /// <param name="index">The index of the exception.</param>
        /// <returns>The exception metadata.</returns>
        ISapExceptionMetadata GetExceptionByIndex(uint index);

        /// <summary>
        /// Gets exception metadata by name.
        /// </summary>
        /// <param name="name">The name of the exception.</param>
        /// <returns>The exception metadata.</returns>
        ISapExceptionMetadata GetExceptionByName(string name);
    }
}
