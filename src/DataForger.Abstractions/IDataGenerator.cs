namespace DataForger
{
    /// <summary>
    /// Produces instances of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The generated type. Must be a reference type.</typeparam>
    /// <remarks>
    /// Implement this interface to plug custom generation into DataForger, then register the
    /// implementation with <c>Forge.Register&lt;TGenerator&gt;()</c>. A generator may declare
    /// either a parameterless constructor or a constructor taking an <see cref="IGenerationContext"/>.
    /// </remarks>
    public interface IDataGenerator<T>
        where T : class
    {
        /// <summary>Generates a single instance.</summary>
        /// <returns>A newly generated instance.</returns>
        T Generate();
    }
}
