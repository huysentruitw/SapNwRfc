using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc.Internal.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    internal enum RfcDirection : uint
    {
        RFC_IMPORT = 1u,
        RFC_EXPORT = 2u,
        RFC_CHANGING = 3u,
        RFC_TABLES = 7u,
    }
}
