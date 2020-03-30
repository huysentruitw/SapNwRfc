using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc.Internal.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    internal enum RfcErrorGroup
    {
        OK,
        ABAP_APPLICATION_FAILURE,
        ABAP_RUNTIME_FAILURE,
        LOGON_FAILURE,
        COMMUNICATION_FAILURE,
        EXTERNAL_RUNTIME_FAILURE,
        EXTERNAL_APPLICATION_FAILURE,
        EXTERNAL_AUTHORIZATION_FAILURE,
    }
}
