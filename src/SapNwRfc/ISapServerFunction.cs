namespace SapNwRfc
{
    /// <summary>
    /// Interface for an SAP RFC server function.
    /// </summary>
    public interface ISapServerFunction
    {
        /// <summary>
        /// Gets the Name of the function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        /// <typeparam name="TOutput">the parameters type.</typeparam>
        /// <returns>the parameters.</returns>
        TOutput GetParameters<TOutput>();

        /// <summary>
        /// Sets the result of the function.
        /// </summary>
        /// <param name="input">the result.</param>
        void SetResult(object input);
    }
}
