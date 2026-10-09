namespace DataForge
{
    /// <summary>
    /// Optional capability implemented by country providers that can generate a
    /// person-level tax identifier (NIF, DNI, CPF, SSN-like, ...).
    /// </summary>
    /// <remarks>
    /// Capability interfaces keep <see cref="ICountryDataProvider"/> stable while new identifier
    /// generators (passport, citizen card, IBAN, ...) are added over time.
    /// </remarks>
    public interface IPersonTaxIdentifierProvider
    {
        /// <summary>Returns a synthetic person tax identifier in the country format.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic person tax identifier.</returns>
        string GetPersonTaxIdentifier(IGenerationContext context);
    }
}
