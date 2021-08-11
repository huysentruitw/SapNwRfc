using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents SAP RFC exception metadata.
    /// </summary>
    public sealed class SapExceptionMetadata : ISapExceptionMetadata
    {
        private readonly RfcExceptionDescription _exceptionDescription;

        internal SapExceptionMetadata(RfcExceptionDescription exceptionDescription)
        {
            _exceptionDescription = exceptionDescription;
        }

        /// <inheritdoc cref="ISapExceptionMetadata"/>
        public string Key => _exceptionDescription.Key;

        /// <inheritdoc cref="ISapExceptionMetadata"/>
        public string Message => _exceptionDescription.Message;
    }
}
