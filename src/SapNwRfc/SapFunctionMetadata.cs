using System;
using System.Collections.Generic;
using SapNwRfc.Internal;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents SAP RFC function metadata.
    /// </summary>
    public sealed class SapFunctionMetadata : ISapFunctionMetadata
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _functionDescHandle;
        private MetadataList<ISapParameterMetadata> _parameters;
        private MetadataList<ISapExceptionMetadata> _exceptions;

        internal SapFunctionMetadata(RfcInterop interop, IntPtr functionDescHandle)
        {
            _interop = interop;
            _functionDescHandle = functionDescHandle;
        }

        /// <inheritdoc cref="ISapFunctionMetadata"/>
        public string GetName()
        {
            RfcResultCode resultCode = _interop.GetFunctionName(
                rfcHandle: _functionDescHandle,
                funcName: out string funcName,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return funcName;
        }

        /// <inheritdoc cref="ISapFunctionMetadata"/>
        public IReadOnlyList<ISapParameterMetadata> Parameters => _parameters ??
            (_parameters = new MetadataList<ISapParameterMetadata>(GetParameterByIndex, GetParameterCount));

        private ISapParameterMetadata GetParameterByIndex(int index)
        {
            RfcResultCode resultCode = _interop.GetParameterDescByIndex(
                funcDesc: _functionDescHandle,
                index: (uint)index,
                paramDesc: out RfcParameterDescription paramDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapParameterMetadata(_interop, paramDesc);
        }

        private int GetParameterCount()
        {
            RfcResultCode resultCode = _interop.GetParameterCount(
                funcDesc: _functionDescHandle,
                count: out uint count,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return (int)count;
        }

        /// <inheritdoc cref="ISapFunctionMetadata"/>
        public ISapParameterMetadata GetParameterByName(string name)
        {
            RfcResultCode resultCode = _interop.GetParameterDescByName(
                funcDesc: _functionDescHandle,
                name: name,
                paramDesc: out RfcParameterDescription paramDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapParameterMetadata(_interop, paramDesc);
        }

        /// <inheritdoc cref="ISapFunctionMetadata"/>
        public IReadOnlyList<ISapExceptionMetadata> Exceptions => _exceptions ??
            (_exceptions = new MetadataList<ISapExceptionMetadata>(GetExceptionByIndex, GetExceptionCount));

        private ISapExceptionMetadata GetExceptionByIndex(int index)
        {
            RfcResultCode resultCode = _interop.GetExceptionDescByIndex(
                funcDesc: _functionDescHandle,
                index: (uint)index,
                excDesc: out RfcExceptionDescription excDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapExceptionMetadata(excDesc);
        }

        private int GetExceptionCount()
        {
            RfcResultCode resultCode = _interop.GetExceptionCount(
                funcDesc: _functionDescHandle,
                count: out uint count,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return (int)count;
        }

        /// <inheritdoc cref="ISapFunctionMetadata"/>
        public ISapExceptionMetadata GetExceptionByName(string name)
        {
            RfcResultCode resultCode = _interop.GetExceptionDescByName(
                funcDesc: _functionDescHandle,
                name: name,
                excDesc: out RfcExceptionDescription excDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapExceptionMetadata(excDesc);
        }
    }
}
