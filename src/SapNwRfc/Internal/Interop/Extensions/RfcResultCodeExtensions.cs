using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Exceptions;

namespace SapNwRfc.Internal.Interop
{
    internal static class RfcResultCodeExtensions
    {
        [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement", Justification = "Readability")]
        public static void ThrowOnError(this RfcResultCode resultCode, RfcErrorInfo errorInfo, Action beforeThrow = null)
        {
            if (resultCode == RfcResultCode.RFC_OK)
                return;

            beforeThrow?.Invoke();

            if (resultCode == RfcResultCode.RFC_COMMUNICATION_FAILURE)
                throw new SapCommunicationFailedException(errorInfo);

            if (resultCode == RfcResultCode.RFC_INVALID_PARAMETER)
                throw new SapInvalidParameterException(errorInfo);

            throw new SapException(resultCode, errorInfo);
        }
    }
}
