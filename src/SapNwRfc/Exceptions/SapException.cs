using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Exceptions
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Allow serialization of ResultCode")]
    public class SapException : Exception
    {
        public SapException(string message)
            : base(string.IsNullOrEmpty(message) ? "SAP RFC Error" : $"SAP RFC Error with message: {message}")
        {
            ResultCode = RfcResultCode.RFC_UNKNOWN_ERROR;
        }

        internal SapException(RfcResultCode resultCode, string message)
            : base(string.IsNullOrEmpty(message)
                ? $"SAP RFC Error: {resultCode}"
                : $"SAP RFC Error: {resultCode} with message: {message}")
        {
            ResultCode = resultCode;
        }

        internal RfcResultCode ResultCode { get; }
    }
}
