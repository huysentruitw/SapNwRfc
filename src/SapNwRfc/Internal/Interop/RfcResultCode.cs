using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc.Internal.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    internal enum RfcResultCode
    {
        RFC_OK,
        RFC_COMMUNICATION_FAILURE,
        RFC_LOGON_FAILURE,
        RFC_ABAP_RUNTIME_FAILURE,
        RFC_ABAP_MESSAGE,
        RFC_ABAP_EXCEPTION,
        RFC_CLOSED,
        RFC_CANCELED,
        RFC_TIMEOUT,
        RFC_MEMORY_INSUFFICIENT,
        RFC_VERSION_MISMATCH,
        RFC_INVALID_PROTOCOL,
        RFC_SERIALIZATION_FAILURE,
        RFC_INVALID_HANDLE,
        RFC_RETRY,
        RFC_EXTERNAL_FAILURE,
        RFC_EXECUTED,
        RFC_NOT_FOUND,
        RFC_NOT_SUPPORTED,
        RFC_ILLEGAL_STATE,
        RFC_INVALID_PARAMETER,
        RFC_CODEPAGE_CONVERSION_FAILURE,
        RFC_CONVERSION_FAILURE,
        RFC_BUFFER_TOO_SMALL,
        RFC_TABLE_MOVE_BOF,
        RFC_TABLE_MOVE_EOF,
        RFC_START_SAP_GUI_FAILURE,
        RFC_ABAP_CLASS_EXCEPTION,
        RFC_UNKNOWN_ERROR,
        RFC_AUTHORIZATION_FAILURE,
        RFC_RC_MAX_VALUE,
    }
}
