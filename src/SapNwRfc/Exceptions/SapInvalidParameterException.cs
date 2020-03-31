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
        /// <param name="message">The exception message.</param>
        public SapInvalidParameterException(string message)
            : base(RfcResultCode.RFC_INVALID_PARAMETER, message)
        {
        }
    }
}
