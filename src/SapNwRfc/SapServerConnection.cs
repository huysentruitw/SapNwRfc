using System;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an RFC server connection to the SAP application server.
    /// </summary>
    public sealed class SapServerConnection : ISapServerConnection
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _rfcConnectionHandle;

        internal SapServerConnection(RfcInterop interop, IntPtr rfcConnectionHandle)
        {
            _interop = interop;
            _rfcConnectionHandle = rfcConnectionHandle;
        }

        /// <inheritdoc cref="ISapServerConnection"/>
        public SapConnectionAttributes GetAttributes()
        {
            RfcResultCode resultCode = _interop.GetConnectionAttributes(
                rfcHandle: _rfcConnectionHandle,
                attributes: out RfcAttributes attributes,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return new SapConnectionAttributes(attributes);
        }
    }
}
