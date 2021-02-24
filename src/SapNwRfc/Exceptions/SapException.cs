using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Exceptions
{
    /// <summary>
    /// Exception throw when an SAP exception occurs.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Allow serialization of ResultCode")]
    public class SapException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapException"/> class.
        /// </summary>
        /// <param name="resultCode">The RFC result code.</param>
        /// <param name="errorInfo">The RFC error info object.</param>
        internal SapException(RfcResultCode resultCode, RfcErrorInfo errorInfo)
            : base(string.IsNullOrEmpty(errorInfo.Message)
                ? $"SAP RFC Error: {resultCode}"
                : $"SAP RFC Error: {resultCode} with message: {errorInfo.Message}")
        {
            ResultCode = SapResultCodeMapper.Map(resultCode);
            ErrorInfo = SapErrorInfo.Map(errorInfo);
        }

        public SapResultCode ResultCode { get; }

        public SapErrorInfo ErrorInfo { get; }
    }

    public sealed class SapErrorInfo
    {
        internal SapErrorInfo(RfcErrorInfo errorInfo)
        {
            ErrorGroup = SapErrorGroupMapper.Map(errorInfo.ErrorGroup);
            Key = errorInfo.Key;
            Message = errorInfo.Message;
            AbapMessageClass = errorInfo.AbapMsgClass;
            AbapMessageType = errorInfo.AbapMsgType;
            AbapMessageNumber = errorInfo.AbapMsgNumber;
            AbapMessageV1 = errorInfo.AbapMsgV1;
            AbapMessageV2 = errorInfo.AbapMsgV2;
            AbapMessageV3 = errorInfo.AbapMsgV3;
            AbapMessageV4 = errorInfo.AbapMsgV4;
        }

        internal static SapErrorInfo Map(RfcErrorInfo errorInfo)
            => new SapErrorInfo(errorInfo);

        public SapErrorGroup ErrorGroup { get; }

        public string Key { get; }

        public string Message { get; }

        public string AbapMessageClass { get; }

        public string AbapMessageType { get; }

        public string AbapMessageNumber { get; }

        public string AbapMessageV1 { get; }

        public string AbapMessageV2 { get; }

        public string AbapMessageV3 { get; }

        public string AbapMessageV4 { get; }
    }
}
