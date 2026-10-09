using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace DataForger.UnitTests
{
    public sealed class EntityTests
    {
        public static readonly Country[] AllCountries =
        {
            Country.PT, Country.ES, Country.UK, Country.FR, Country.DE, Country.US, Country.BR
        };

        public static TheoryData<Country> Countries()
        {
            var data = new TheoryData<Country>();
            foreach (var country in AllCountries)
            {
                data.Add(country);
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(Countries))]
        public void Person_IsFullyPopulated(Country country)
        {
            var person = Forge.ForCountry(country).WithSeed(11).Create<Person>();

            person.FirstName.Should().NotBeNullOrWhiteSpace();
            person.LastName.Should().NotBeNullOrWhiteSpace();
            person.FullName.Should().Be(person.FirstName + " " + person.LastName);
            person.PhoneNumber.Should().NotBeNullOrWhiteSpace();
            person.Email.Should().MatchRegex("^[a-z0-9]+\\.[a-z0-9]+@example\\.(com|net|org)$");
            person.Gender.Should().BeOneOf(Gender.Female, Gender.Male);
        }

        [Theory]
        [MemberData(nameof(Countries))]
        public void Address_IsFullyPopulated(Country country)
        {
            var address = Forge.ForCountry(country).WithSeed(12).Create<Address>();

            address.Street.Should().NotBeNullOrWhiteSpace();
            address.Number.Should().NotBeNullOrWhiteSpace();
            address.PostalCode.Should().NotBeNullOrWhiteSpace();
            address.City.Should().NotBeNullOrWhiteSpace();
            address.Country.Should().Be(country);
        }

        [Theory]
        [MemberData(nameof(Countries))]
        public void Company_IsFullyPopulated(Country country)
        {
            var company = Forge.ForCountry(country).WithSeed(13).Create<Company>();

            company.Name.Should().NotBeNullOrWhiteSpace();
            company.TaxIdentifier.Should().NotBeNullOrWhiteSpace();
            company.PhoneNumber.Should().NotBeNullOrWhiteSpace();
            company.Email.Should().MatchRegex("^info@[a-z0-9]+\\.example\\.(com|net|org)$");
            company.Address.Should().NotBeNull();
            company.Address.Country.Should().Be(country);
        }

        [Theory]
        [MemberData(nameof(Countries))]
        public void Customer_IsFullyPopulated(Country country)
        {
            var customer = Forge.ForCountry(country).WithSeed(14).Create<Customer>();

            customer.Id.Should().NotBe(Guid.Empty);
            customer.CustomerNumber.Should().MatchRegex("^CUST-[0-9]{7}$");
            customer.Name.Should().NotBeNullOrWhiteSpace();
            customer.PhoneNumber.Should().NotBeNullOrWhiteSpace();
            customer.Address.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(Countries))]
        public void Contract_IsConsistent(Country country)
        {
            var contract = Forge.ForCountry(country).WithSeed(15).Create<Contract>();

            contract.ContractNumber.Should().MatchRegex("^CT-[0-9]{4}-[0-9]{5}$");
            contract.EndDate.Should().BeAfter(contract.StartDate);
            contract.Customer.Should().NotBeNull();
            contract.Status.Should().BeDefined();
        }

        [Fact]
        public void Contract_CompletedWhenEndDateIsInThePast()
        {
            var contracts = Forge.ForCountry(Country.PT).WithSeed(16).Create<Contract>(200);
            var epoch = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            contracts
                .Where(contract => contract.EndDate < epoch)
                .Should()
                .OnlyContain(contract => contract.Status == ContractStatus.Completed);
        }

        [Fact]
        public void Product_IsFullyPopulated()
        {
            var product = Forge.ForCountry(Country.PT).WithSeed(17).Create<Product>();

            product.Sku.Should().MatchRegex("^[A-Z]{4}-[0-9]{5}$");
            product.Name.Should().NotBeNullOrWhiteSpace();
            product.Description.Should().StartWith(product.Name);
            product.Price.Should().BeGreaterThan(0m);
        }
    }
}
