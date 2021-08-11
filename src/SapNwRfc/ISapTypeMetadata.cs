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
        string GetTypeName();

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <returns>The field count.</returns>
        uint GetFieldCount();

        /// <summary>
        /// Gets field metadata by index.
        /// </summary>
        /// <param name="index">The index of the field.</param>
        /// <returns>The field metadata.</returns>
        ISapFieldMetadata GetFieldByIndex(uint index);

        /// <summary>
        /// Gets field metadata by name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>The field metadata.</returns>
        ISapFieldMetadata GetFieldByName(string name);
    }
}
