using System;

namespace SapNwRfc.Internal.Interop
{
    internal static class RfcErrorInfoExtensions
    {
        public static void ThrowOnError(this RfcErrorInfo errorInfo, Action beforeThrow = null)
            => errorInfo.Code.ThrowOnError(errorInfo, beforeThrow);
    }
}
