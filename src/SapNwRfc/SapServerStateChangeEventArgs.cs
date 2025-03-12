using System;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an SAP RFC server state change event.
    /// </summary>
    public sealed class SapServerStateChangeEventArgs : EventArgs
    {
        internal SapServerStateChangeEventArgs(RfcStateChange stateChange)
        {
            OldState = stateChange.OldState;
            NewState = stateChange.NewState;
        }

        /// <summary>
        /// Gets the old server state.
        /// </summary>
        public SapRfcServerState OldState { get; }

        /// <summary>
        /// Gets the new server state.
        /// </summary>
        public SapRfcServerState NewState { get; }
    }
}
