namespace SapNwRfc
{
    /// <summary>
    /// Interface for SAP RFC type metadata.
    /// </summary>
    public interface ISapTypeMetadata
    {
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        string GetName();

        /// <summary>
        /// Gets the fields.
        /// </summary>
        ISapMetadataCollection<ISapFieldMetadata> Fields { get; }
    }
}
