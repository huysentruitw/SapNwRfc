using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Exceptions
{
    public sealed class SapInvalidParameterException : SapException
    {
        public SapInvalidParameterException(string message)
            : base(RfcResultCode.RFC_INVALID_PARAMETER, message)
        {
        }
    }
}
