using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Exceptions
{
    /// <summary>
    /// Exception throw when communication with the SAP application server has failed.
    /// </summary>
    public sealed class SapCommunicationFailedException : SapException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapCommunicationFailedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SapCommunicationFailedException(string message)
            : base(RfcResultCode.RFC_COMMUNICATION_FAILURE, message)
        {
        }
    }
}
