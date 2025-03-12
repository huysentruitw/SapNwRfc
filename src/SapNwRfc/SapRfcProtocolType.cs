using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc
{
    /// <summary>
    /// Represents the SAP RFC server protocol.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    public enum SapRfcProtocolType
    {
        RFC_UNKOWN,
        RFC_CLIENT,
        RFC_STARTED_SERVER,
        RFC_REGISTERED_SERVER,
        RFC_MULTI_COUNT_REGISTERED_SERVER,
        RFC_TCP_SOCKET_CLIENT,
        RFC_TCP_SOCKET_SERVER,
        RFC_WEBSOCKET_CLIENT,
        RFC_WEBSOCKET_SERVER,
        RFC_PROXY_WEBSOCKET_CLIENT,
    }
}
