namespace SapNwRfc
{
    /// <summary>
    /// Interface for an SAP RFC server connection.
    /// </summary>
    public interface ISapServerConnection
    {
        /// <summary>
        /// Gets the connection attributes of an connected connection.
        /// </summary>
        /// <returns>The connection attributes.</returns>
        SapAttributes GetAttributes();

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
        /// Creates a <see cref="ISapFunction"/> object for invoking the remote function.
        /// </summary>
        /// <param name="name">The name of the remote function.</param>
        /// <returns>The <see cref="ISapFunction"/> object.</returns>
        ISapFunction CreateFunction(string name);
    }
}
