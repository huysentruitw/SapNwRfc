using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc
{
    /// <summary>
    /// Represents the SAP RFC server states.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    public enum SapRfcServerState
    {
        RFC_SERVER_INITIAL,
        RFC_SERVER_STARTING,
        RFC_SERVER_RUNNING,
        RFC_SERVER_BROKEN,
        RFC_SERVER_STOPPING,
        RFC_SERVER_STOPPED,
    }
}
