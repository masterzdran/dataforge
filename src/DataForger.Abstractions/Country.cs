namespace DataForger
{
    /// <summary>
    /// Identifies a country for which synthetic data can be generated.
    /// </summary>
    /// <remarks>
    /// New countries can be added by extending this enumeration and supplying a matching
    /// <see cref="ICountryDataProvider"/> implementation. Existing generators do not change.
    /// </remarks>
    public enum Country
    {
        /// <summary>Portugal.</summary>
        PT,

        /// <summary>Spain.</summary>
        ES,

        /// <summary>United Kingdom.</summary>
        UK,

        /// <summary>France.</summary>
        FR,

        /// <summary>Germany.</summary>
        DE,

        /// <summary>United States.</summary>
        US,

        /// <summary>Brazil.</summary>
        BR
    }
}
