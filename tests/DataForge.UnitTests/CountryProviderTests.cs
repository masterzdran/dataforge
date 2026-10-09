using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace DataForge.UnitTests
{
    public sealed class CountryProviderTests
    {
        [Theory]
        [InlineData(Country.PT, "^[0-9]{4}-[0-9]{3}$", "^\\+351 9[0-9]{2} [0-9]{3} [0-9]{3}$")]
        [InlineData(Country.ES, "^[0-9]{5}$", "^\\+34 6[0-9]{2} [0-9]{3} [0-9]{3}$")]
        [InlineData(Country.UK, "^[A-Z0-9]+ [0-9][A-Z]{2}$", "^\\+44 7[0-9]{3} [0-9]{6}$")]
        [InlineData(Country.FR, "^[0-9]{5}$", "^\\+33 6 [0-9]{2} [0-9]{2} [0-9]{2} [0-9]{2}$")]
        [InlineData(Country.DE, "^[0-9]{5}$", "^\\+49 1[0-9]{2} [0-9]{7}$")]
        [InlineData(Country.US, "^[0-9]{5}(-[0-9]{4})?$", "^\\+1 \\([0-9]{3}\\) [0-9]{3}-[0-9]{4}$")]
        [InlineData(Country.BR, "^[0-9]{5}-[0-9]{3}$", "^\\+55 \\([0-9]{2}\\) 9[0-9]{4}-[0-9]{4}$")]
        public void PostalCodeAndPhone_FollowCountryFormat(Country country, string postalPattern, string phonePattern)
        {
            var forge = Forge.ForCountry(country).WithSeed(21);

            var addresses = forge.Create<Address>(50);
            var people = forge.Create<Person>(50);

            foreach (var address in addresses)
            {
                address.PostalCode.Should().MatchRegex(postalPattern);
            }

            foreach (var person in people)
            {
                person.PhoneNumber.Should().MatchRegex(phonePattern);
                person.Email.Should().MatchRegex("^[a-z0-9]+\\.[a-z0-9]+@example\\.(com|net|org)$");
            }
        }

        [Fact]
        public void FirstNames_RespectRequestedGender()
        {
            var provider = Forge.ForCountry(Country.PT).Provider;
            var forge = Forge.ForCountry(Country.PT).WithSeed(22);

            var males = forge.Create<Person>(100).Where(person => person.Gender == Gender.Male).ToArray();
            var females = forge.Create<Person>(100).Where(person => person.Gender == Gender.Female).ToArray();

            males.Should().NotBeEmpty();
            females.Should().NotBeEmpty();
            provider.Should().NotBeNull();
        }

        [Fact]
        public void StreetNumber_ContainsDigits()
        {
            var addresses = Forge.ForCountry(Country.US).WithSeed(23).Create<Address>(50);

            addresses.Should().OnlyContain(address => Regex.IsMatch(address.Number, "^[0-9]{1,3}[A-Z]?$"));
        }

        [Fact]
        public void Registry_DefaultProviders_CoverEveryCountry()
        {
            var registry = new CountryDataProviderRegistry();

            foreach (var country in EntityTests.AllCountries)
            {
                registry.Get(country).Country.Should().Be(country);
            }
        }

        [Fact]
        public void BaseProvider_NullPool_Throws()
        {
            Action act = () => new NullPoolProvider();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BaseProvider_EmptyPool_Throws()
        {
            Action act = () => new EmptyPoolProvider();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void BaseProvider_NullFormat_Throws()
        {
            Action act = () => new NullFormatProvider();

            act.Should().Throw<ArgumentException>();
        }
    }
}
