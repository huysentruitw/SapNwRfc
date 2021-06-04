using System;
using SapNwRfc.Internal;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an SAP RFC server function.
    /// </summary>
    public sealed class SapServerFunction : ISapServerFunction
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _functionHandle;
        private readonly IntPtr _functionDescriptionHandle;

        internal SapServerFunction(
            RfcInterop interop,
            IntPtr functionHandle,
            IntPtr functionDescriptionHandle)
        {
            _interop = interop;
            _functionHandle = functionHandle;
            _functionDescriptionHandle = functionDescriptionHandle;
        }

        /// <inheritdoc cref="ISapServerFunction"/>
        public string GetName()
        {
            RfcResultCode resultCode = _interop.GetFunctionName(
                rfcHandle: _functionDescriptionHandle,
                funcName: out string functionName,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return functionName;
        }

        /// <inheritdoc cref="ISapFunction"/>
        public bool HasParameter(string parameterName)
        {
            RfcResultCode resultCode = _interop.GetParameterDescByName(
                funcDescHandle: _functionDescriptionHandle,
                parameterName: parameterName,
                parameterDescHandle: out IntPtr _,
                errorInfo: out RfcErrorInfo _);

            return resultCode == RfcResultCode.RFC_OK;
        }

        /// <inheritdoc cref="ISapServerFunction"/>
        public TOutput GetParameters<TOutput>()
        {
            return OutputMapper.Extract<TOutput>(_interop, _functionHandle);
        }

        /// <inheritdoc cref="ISapServerFunction"/>
        public void SetResult(object input)
        {
            InputMapper.Apply(_interop, _functionHandle, input);
        }
    }
}
