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
        /// <param name="errorInfo">The RFC error info object.</param>
        internal SapCommunicationFailedException(RfcErrorInfo errorInfo)
            : base(RfcResultCode.RFC_COMMUNICATION_FAILURE, errorInfo)
        {
        }
    }
}
