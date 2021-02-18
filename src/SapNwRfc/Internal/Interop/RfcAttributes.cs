using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcAttributes
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64 + 1)]
        public string Dest;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100 + 1)]
        public string Host;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100 + 1)]
        public string PartnerHost;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string SysNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 + 1)]
        public string SysId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3 + 1)]
        public string Client;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12 + 1)]
        public string User;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string Language;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string Trace;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string IsoLanguage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string Codepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerCodepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string RfcRole;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string Type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string PartnerType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string Rel;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerRel;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string KernelRel;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 + 1)]
        public string CpicConvId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128 + 1)]
        public string ProgName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string PartnerBytesPerChar;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerSystemCodepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15 + 1)]
        public string PartnerIP;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 45 + 1)]
        public string PartnerIPv6;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string Reserved;
    }
}
