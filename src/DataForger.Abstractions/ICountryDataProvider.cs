namespace DataForger
{
    /// <summary>
    /// Supplies country-specific data and formats used by the built-in generators.
    /// </summary>
    /// <remarks>
    /// Providers are isolated from business entities: they return primitive values only.
    /// Every method must return a non-null, non-empty value.
    /// </remarks>
    public interface ICountryDataProvider
    {
        /// <summary>Gets the country this provider supplies data for.</summary>
        Country Country { get; }

        /// <summary>Returns a first name matching the requested gender.</summary>
        /// <param name="context">Generation context.</param>
        /// <param name="gender">Requested gender.</param>
        /// <returns>A synthetic first name.</returns>
        string GetFirstName(IGenerationContext context, Gender gender);

        /// <summary>Returns a last name.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic last name.</returns>
        string GetLastName(IGenerationContext context);

        /// <summary>Returns a city name.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic city name.</returns>
        string GetCity(IGenerationContext context);

        /// <summary>Returns a street name.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic street name.</returns>
        string GetStreet(IGenerationContext context);

        /// <summary>Returns a street number, optionally with a letter suffix.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic street number.</returns>
        string GetStreetNumber(IGenerationContext context);

        /// <summary>Returns a postal code in the country format.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic postal code.</returns>
        string GetPostalCode(IGenerationContext context);

        /// <summary>Returns a phone number in the country format, including the calling code.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic phone number.</returns>
        string GetPhoneNumber(IGenerationContext context);

        /// <summary>Returns a company name.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic company name.</returns>
        string GetCompanyName(IGenerationContext context);

        /// <summary>Returns a company tax identifier in the country format.</summary>
        /// <param name="context">Generation context.</param>
        /// <returns>A synthetic tax identifier.</returns>
        string GetTaxIdentifier(IGenerationContext context);
    }
}
