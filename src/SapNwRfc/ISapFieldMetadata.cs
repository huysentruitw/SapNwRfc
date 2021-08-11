namespace SapNwRfc
{
    /// <summary>
    /// Interface for SAP RFC field metadata.
    /// </summary>
    public interface ISapFieldMetadata
    {
        /// <summary>
        /// Gets the field name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        SapRfcType Type { get; }

        /// <summary>
        /// Gets the field length in bytes in a 1-byte-per-SAP_CHAR system.
        /// </summary>
        uint NucLength { get; }

        /// <summary>
        /// Gets the field offset in bytes in a 1-byte-per-SAP_CHAR system.
        /// </summary>
        uint NucOffset { get; }

        /// <summary>
        /// Gets the field length in bytes in a 2-byte-per-SAP_CHAR system.
        /// </summary>
        uint UcLength { get; }

        /// <summary>
        /// Gets the field offset in bytes in a 2-byte-per-SAP_CHAR system.
        /// </summary>
        uint UcOffset { get; }

        /// <summary>
        /// Gets the number of decimals in case of a packed number (BCD).
        /// </summary>
        uint Decimals { get; }

        /// <summary>
        /// Gets the type metadata if this is a <see cref="SapRfcType.RFCTYPE_STRUCTURE"/> or <see cref="SapRfcType.RFCTYPE_TABLE"/>.
        /// </summary>
        /// <returns>The type metadata or <c>null</c>.</returns>
        ISapTypeMetadata GetTypeMetadata();
    }
}
