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
        /// Gets the parameters.
        /// </summary>
        ISapMetadataCollection<ISapParameterMetadata> Parameters { get; }

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        ISapMetadataCollection<ISapExceptionMetadata> Exceptions { get; }
    }
}
