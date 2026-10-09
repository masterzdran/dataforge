using System;
using System.Collections.Generic;

namespace DataForge
{
    /// <summary>
    /// Default registry holding all built-in country providers. Custom providers can be supplied
    /// to replace built-in ones or to add support for new countries.
    /// </summary>
    public sealed class CountryDataProviderRegistry : ICountryDataProviderRegistry
    {
        private readonly IReadOnlyDictionary<Country, ICountryDataProvider> _providers;

        /// <summary>Initializes a new instance of the <see cref="CountryDataProviderRegistry"/> class containing all built-in providers.</summary>
        public CountryDataProviderRegistry()
            : this(CreateDefaultProviders())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CountryDataProviderRegistry"/> class from an explicit set of providers.</summary>
        /// <param name="providers">Providers to register. Each country must appear at most once.</param>
        public CountryDataProviderRegistry(IEnumerable<ICountryDataProvider> providers)
        {
            if (providers == null)
            {
                throw new ArgumentNullException(nameof(providers));
            }

            var map = new Dictionary<Country, ICountryDataProvider>();
            foreach (var provider in providers)
            {
                if (provider == null)
                {
                    throw new ArgumentException("The provider collection must not contain null entries.", nameof(providers));
                }

                if (map.ContainsKey(provider.Country))
                {
                    throw new ArgumentException("Duplicate provider for country '" + provider.Country + "'.", nameof(providers));
                }

                map.Add(provider.Country, provider);
            }

            _providers = map;
        }

        /// <inheritdoc />
        public ICountryDataProvider Get(Country country)
        {
            if (!_providers.TryGetValue(country, out var provider))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(country),
                    country,
                    "No data provider is registered for country '" + country + "'.");
            }

            return provider;
        }

        private static ICountryDataProvider[] CreateDefaultProviders()
        {
            return new ICountryDataProvider[]
            {
                new PortugalDataProvider(),
                new SpainDataProvider(),
                new UnitedKingdomDataProvider(),
                new FranceDataProvider(),
                new GermanyDataProvider(),
                new UnitedStatesDataProvider(),
                new BrazilDataProvider()
            };
        }
    }
}
