using System;
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
        private SapMetadataCollection<ISapParameterMetadata> _parameters;
        private SapMetadataCollection<ISapExceptionMetadata> _exceptions;

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
        public ISapMetadataCollection<ISapParameterMetadata> Parameters => _parameters ??
            (_parameters = new SapMetadataCollection<ISapParameterMetadata>(GetParameterByIndex, GetParameterByName, GetParameterCount));

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

        private ISapParameterMetadata GetParameterByName(string name)
        {
            RfcResultCode resultCode = _interop.GetParameterDescByName(
                funcDesc: _functionDescHandle,
                name: name,
                paramDesc: out RfcParameterDescription paramDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            if (resultCode == RfcResultCode.RFC_INVALID_PARAMETER)
                return null;

            errorInfo.ThrowOnError();

            return new SapParameterMetadata(_interop, paramDesc);
        }

        /// <inheritdoc cref="ISapFunctionMetadata"/>
        public ISapMetadataCollection<ISapExceptionMetadata> Exceptions => _exceptions ??
            (_exceptions = new SapMetadataCollection<ISapExceptionMetadata>(GetExceptionByIndex, GetExceptionByName, GetExceptionCount));

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

        private ISapExceptionMetadata GetExceptionByName(string name)
        {
            RfcResultCode resultCode = _interop.GetExceptionDescByName(
                funcDesc: _functionDescHandle,
                name: name,
                excDesc: out RfcExceptionDescription excDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            if (resultCode == RfcResultCode.RFC_INVALID_PARAMETER)
                return null;

            errorInfo.ThrowOnError();

            return new SapExceptionMetadata(excDesc);
        }
    }
}
