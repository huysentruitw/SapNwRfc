namespace SapNwRfc
{
    /// <summary>
    /// Interface for SAP RFC exception metadata.
    /// </summary>
    public interface ISapExceptionMetadata
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        string Message { get; }
    }
}
