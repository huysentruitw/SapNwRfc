using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Exceptions
{
    public sealed class SapCommunicationFailedException : SapException
    {
        public SapCommunicationFailedException(string message)
            : base(RfcResultCode.RFC_COMMUNICATION_FAILURE, message)
        {
        }
    }
}
