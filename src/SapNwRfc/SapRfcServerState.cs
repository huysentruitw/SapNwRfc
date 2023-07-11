namespace SapNwRfc
{
    /// <summary>
    /// Represents the SAP RFC server states.
    /// </summary>
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
