namespace DataForge
{
    /// <summary>
    /// Resolves the <see cref="ICountryDataProvider"/> for a given <see cref="Country"/>.
    /// </summary>
    public interface ICountryDataProviderRegistry
    {
        /// <summary>Returns the provider registered for the specified country.</summary>
        /// <param name="country">The requested country.</param>
        /// <returns>The matching provider.</returns>
        ICountryDataProvider Get(Country country);
    }
}
