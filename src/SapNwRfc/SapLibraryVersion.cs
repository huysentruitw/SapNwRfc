using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc
{
    /// <summary>
    /// Represents a SAP RFC library version.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class SapLibraryVersion
    {
        /// <summary>
        /// Gets the major version value.
        /// </summary>
        public uint Major { get; internal set; }

        /// <summary>
        /// Gets the minor version value.
        /// </summary>
        public uint Minor { get; internal set; }

        /// <summary>
        /// Gets the patch version value.
        /// </summary>
        public uint Patch { get; internal set; }
    }
}
