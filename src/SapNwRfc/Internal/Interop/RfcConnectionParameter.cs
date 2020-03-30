using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct RfcConnectionParameter
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Name;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string Value;
    }
}
