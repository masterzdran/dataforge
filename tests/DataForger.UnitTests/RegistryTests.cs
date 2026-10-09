using System;
using FluentAssertions;
using Xunit;

namespace DataForger.UnitTests
{
    public sealed class RegistryTests
    {
        [Fact]
        public void CustomProvider_ReplacesBuiltIn()
        {
            var registry = new CountryDataProviderRegistry(new ICountryDataProvider[] { new StubCountryDataProvider() });
            var forge = new Forge(Country.PT, registry);

            forge.Create<Company>().TaxIdentifier.Should().Be("STUB");
        }

        [Fact]
        public void UnknownCountry_Throws()
        {
            Action act = () => new CountryDataProviderRegistry().Get((Country)123);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void DuplicateProvider_Throws()
        {
            var providers = new ICountryDataProvider[] { new StubCountryDataProvider(), new StubCountryDataProvider() };

            Action act = () => new CountryDataProviderRegistry(providers);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NullProviderCollection_Throws()
        {
            Action act = () => new CountryDataProviderRegistry(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NullProviderEntry_Throws()
        {
            var providers = new ICountryDataProvider[] { null };

            Action act = () => new CountryDataProviderRegistry(providers);

            act.Should().Throw<ArgumentException>();
        }
    }
}
