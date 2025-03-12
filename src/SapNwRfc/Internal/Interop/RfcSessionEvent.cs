using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc.Internal.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    internal enum RfcSessionEvent
    {
        RFC_SESSION_CREATED,
        RFC_SESSION_ACTIVATED,
        RFC_SESSION_PASSIVATED,
        RFC_SESSION_DESTROYED,
    }
}
