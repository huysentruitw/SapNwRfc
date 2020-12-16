using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid parameter is being passed.
    /// </summary>
    public sealed class SapInvalidParameterException : SapException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapInvalidParameterException"/> class.
        /// </summary>
        /// <param name="errorInfo">The RFC error info object.</param>
        internal SapInvalidParameterException(RfcErrorInfo errorInfo)
            : base(RfcResultCode.RFC_INVALID_PARAMETER, errorInfo)
        {
        }
    }
}
