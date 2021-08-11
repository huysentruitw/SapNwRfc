using System;

namespace SapNwRfc
{
    /// <summary>
    /// Interface for an SAP RFC connection.
    /// </summary>
    public interface ISapConnection : IDisposable
    {
        /// <summary>
        /// Connect with the SAP application server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from the SAP application server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets a value indicating whether the underlying connection handle is still valid.
        /// </summary>
        /// <remarks>This does not test the actual network connection, therefore the <see cref="Ping"/> method can be used.</remarks>
        bool IsValid { get; }

        /// <summary>
        /// Pings the remote SAP host to check if the connection is still alive.
        /// </summary>
        /// <returns>True when the connection is still alive. False when not.</returns>
        bool Ping();

        /// <summary>
        /// Gets the connection attributes of an connected connection.
        /// </summary>
        /// <returns>The connection attributes.</returns>
        SapConnectionAttributes GetAttributes();

        /// <summary>
        /// Gets the metadata for the specified type name.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <returns>The metadata for the type name.</returns>
        ISapTypeMetadata GetTypeMetadata(string typeName);

        /// <summary>
        /// Gets the metadata for the specified function name.
        /// </summary>
        /// <param name="functionName">The function name.</param>
        /// <returns>The matadata for the function name.</returns>
        ISapFunctionMetadata GetFunctionMetadata(string functionName);

        /// <summary>
        /// Creates a <see cref="SapFunction"/> object for invoking the remote function.
        /// </summary>
        /// <param name="name">The name of the remote function.</param>
        /// <returns>The <see cref="SapFunction"/> object.</returns>
        ISapFunction CreateFunction(string name);
    }
}
