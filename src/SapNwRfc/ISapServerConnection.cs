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
    }
}
