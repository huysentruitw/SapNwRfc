using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcSessionChange
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30 + 1)]
        public string SessionId;

        [MarshalAs(UnmanagedType.I4)]
        public RfcSessionEvent Event;
    }
}
