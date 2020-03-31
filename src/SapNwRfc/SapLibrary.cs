using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Static class that allows ensuring the SAP RFC binaries are present.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SapLibrary
    {
        /// <summary>
        /// Ensures the SAP RFC binaries are present. Throws an <see cref="SapLibraryNotFoundException"/> exception when the SAP RFC binaries could not be found.
        /// </summary>
        public static void EnsureLibraryPresent()
        {
            try
            {
                new RfcInterop().GetVersion(out uint majorVersion, out uint minorVersion, out uint patchLevel);
            }
            catch (DllNotFoundException ex)
            {
                throw new SapLibraryNotFoundException(ex);
            }
        }
    }
}
