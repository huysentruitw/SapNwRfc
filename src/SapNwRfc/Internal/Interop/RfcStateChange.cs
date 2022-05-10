using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RfcStateChange
    {
        [MarshalAs(UnmanagedType.I4)]
        public SapServerState OldState;

        [MarshalAs(UnmanagedType.I4)]
        public SapServerState NewState;
    }
}
