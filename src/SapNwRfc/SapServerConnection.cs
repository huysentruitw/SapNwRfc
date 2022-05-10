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
        public SapAttributes GetAttributes()
        {
            RfcResultCode resultCode = _interop.GetConnectionAttributes(
                rfcHandle: _rfcConnectionHandle,
                attributes: out RfcAttributes attributes,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return new SapAttributes(attributes);
        }

        /// <inheritdoc cref="ISapServerConnection"/>
        public ISapTypeMetadata GetTypeMetadata(string typeName)
        {
            IntPtr typeDescriptionHandle = _interop.GetTypeDesc(
               rfcHandle: _rfcConnectionHandle,
               typeName: typeName,
               errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapTypeMetadata(_interop, typeDescriptionHandle);
        }

        /// <inheritdoc cref="ISapServerConnection"/>
        public ISapFunctionMetadata GetFunctionMetadata(string functionName)
        {
            IntPtr functionDescriptionHandle = _interop.GetFunctionDesc(
               rfcHandle: _rfcConnectionHandle,
               funcName: functionName,
               errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapFunctionMetadata(_interop, functionDescriptionHandle);
        }

        /// <inheritdoc cref="ISapServerConnection"/>
        public ISapFunction CreateFunction(string name)
        {
            IntPtr functionDescriptionHandle = _interop.GetFunctionDesc(
                rfcHandle: _rfcConnectionHandle,
                funcName: name,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return SapFunction.CreateFromDescriptionHandle(
                interop: _interop,
                rfcConnectionHandle: _rfcConnectionHandle,
                functionDescriptionHandle: functionDescriptionHandle);
        }
    }
}
