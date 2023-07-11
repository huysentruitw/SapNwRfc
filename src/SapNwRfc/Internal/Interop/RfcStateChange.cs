using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RfcStateChange
    {
        [MarshalAs(UnmanagedType.I4)]
        public SapRfcServerState OldState;

        [MarshalAs(UnmanagedType.I4)]
        public SapRfcServerState NewState;
    }
}
