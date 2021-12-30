using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    public sealed class SapErrorInfo
    {
        private readonly RfcErrorInfo _errorInfo;

        internal SapErrorInfo(RfcErrorInfo errorInfo)
        {
            _errorInfo = errorInfo;
        }

        public int Code => (int)_errorInfo.Code;

        public int ErrorGroup => (int)_errorInfo.ErrorGroup;

        public string Key => _errorInfo.Key;

        public string Message => _errorInfo.Message;

        public string AbapMsgClass => _errorInfo.AbapMsgClass;

        public string AbapMsgType => _errorInfo.AbapMsgType;

        public string AbapMsgNumber => _errorInfo.AbapMsgNumber;

        public string AbapMsgV1 => _errorInfo.AbapMsgV1;

        public string AbapMsgV2 => _errorInfo.AbapMsgV2;

        public string AbapMsgV3 => _errorInfo.AbapMsgV3;

        public string AbapMsgV4 => _errorInfo.AbapMsgV4;
    }
}
