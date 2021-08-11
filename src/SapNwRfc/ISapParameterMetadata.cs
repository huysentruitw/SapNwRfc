namespace SapNwRfc
{
    /// <summary>
    /// Interface for parameter metadata.
    /// </summary>
    public interface ISapParameterMetadata
    {
        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        SapRfcType Type { get; }

        /// <summary>
        /// Gets the parameter direction.
        /// </summary>
        SapRfcDirection Direction { get; }

        /// <summary>
        /// Gets the parameter length in bytes in a 1-byte-per-SAP_CHAR system.
        /// </summary>
        uint NucLength { get; }

        /// <summary>
        /// Gets the parameter length in bytes in a 2-byte-per-SAP_CHAR system.
        /// </summary>
        uint UcLength { get; }

        /// <summary>
        /// Gets the number of decimals in case of a packed number (BCD).
        /// </summary>
        uint Decimals { get; }

        /// <summary>
        /// Gets the default value as defined in SE37.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// Gets a value indicating whether this parameter is defined as optional in SE37.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the type metadata if this is a <see cref="SapRfcType.RFCTYPE_STRUCTURE"/> or <see cref="SapRfcType.RFCTYPE_TABLE"/>.
        /// </summary>
        /// <returns>The type metadata or <c>null</c>.</returns>
        ISapTypeMetadata GetTypeMetadata();
    }
}
