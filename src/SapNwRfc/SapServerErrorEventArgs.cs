using System;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an SAP RFC server error event.
    /// </summary>
    public sealed class SapServerErrorEventArgs : EventArgs
    {
        internal SapServerErrorEventArgs(SapAttributes attributes, SapErrorInfo errorInfo)
        {
            Attributes = attributes;
            Error = errorInfo;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        public SapAttributes Attributes { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public SapErrorInfo Error { get; }
    }
}
