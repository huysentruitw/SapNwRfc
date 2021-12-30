using System;

namespace SapNwRfc
{
    /// <summary>
    /// Interface for an SAP RFC server.
    /// </summary>
    public interface ISapServer : IDisposable
    {
        /// <summary>
        /// Event which is called when an error occurs.
        /// </summary>
        event EventHandler<SapServerErrorEventArgs> Error;

        /// <summary>
        /// Launches the server.
        /// </summary>
        void Launch();

        /// <summary>
        /// Shuts the server down.
        /// </summary>
        /// <param name="timeout">timeout in seconds.</param>
        void Shutdown(uint timeout);
    }
}
