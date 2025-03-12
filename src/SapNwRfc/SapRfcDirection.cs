using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc
{
    /// <summary>
    /// Represents the SAP RFC direction.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    public enum SapRfcDirection
    {
        RFC_IMPORT = 0x01,
        RFC_EXPORT = 0x02,
        RFC_CHANGING = RFC_IMPORT | RFC_EXPORT,
        RFC_TABLES = 0x04 | RFC_CHANGING,
    }
}
