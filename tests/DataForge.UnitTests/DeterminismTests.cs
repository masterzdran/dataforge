using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DataForge.UnitTests
{
    public sealed class DeterminismTests
    {
        [Fact]
        public void SameSeed_SingleEntity_ProducesSameResult()
        {
            var first = Forge.ForCountry(Country.PT).WithSeed(123).Create<Customer>();
            var second = Forge.ForCountry(Country.PT).WithSeed(123).Create<Customer>();

            Signatures.Of(first).Should().Be(Signatures.Of(second));
        }

        [Fact]
        public void SameSeed_Collection_ProducesSameSequence()
        {
            var first = Forge.ForCountry(Country.ES).WithSeed(777).Create<Customer>(100).Select(Signatures.Of).ToArray();
            var second = Forge.ForCountry(Country.ES).WithSeed(777).Create<Customer>(100).Select(Signatures.Of).ToArray();

            first.Should().Equal(second);
        }

        [Fact]
        public void DifferentSeed_ProducesDifferentData()
        {
            var first = Forge.ForCountry(Country.PT).WithSeed(1).Create<Customer>(50).Select(Signatures.Of).ToArray();
            var second = Forge.ForCountry(Country.PT).WithSeed(2).Create<Customer>(50).Select(Signatures.Of).ToArray();

            first.Should().NotEqual(second);
        }

        [Fact]
        public void SameBuilder_RepeatedCreate_IsStable()
        {
            var forge = Forge.ForCountry(Country.US).WithSeed(55);

            Signatures.Of(forge.Create<Customer>()).Should().Be(Signatures.Of(forge.Create<Customer>()));
        }

        [Fact]
        public void CustomerIdentifier_IsDeterministic()
        {
            var first = Forge.ForCountry(Country.PT).WithSeed(9).Create<Customer>();
            var second = Forge.ForCountry(Country.PT).WithSeed(9).Create<Customer>();

            first.Id.Should().Be(second.Id);
            first.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Contract_IsDeterministic()
        {
            var first = Forge.ForCountry(Country.FR).WithSeed(2024).Create<Contract>();
            var second = Forge.ForCountry(Country.FR).WithSeed(2024).Create<Contract>();

            first.ContractNumber.Should().Be(second.ContractNumber);
            first.StartDate.Should().Be(second.StartDate);
            first.EndDate.Should().Be(second.EndDate);
            first.Customer.Id.Should().Be(second.Customer.Id);
        }

        [Fact]
        public void WithoutSeed_StillProducesValidData()
        {
            var customer = Forge.ForCountry(Country.DE).Create<Customer>();

            customer.Should().NotBeNull();
            customer.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void DateOfBirth_IsAnchoredToFixedEpoch()
        {
            var epoch = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var people = Forge.ForCountry(Country.PT).WithSeed(3).Create<Person>(200);

            people.Should().OnlyContain(person => person.DateOfBirth < epoch);
            people.Should().OnlyContain(person => person.DateOfBirth >= epoch.AddYears(-76));
            people.Should().OnlyContain(person => person.DateOfBirth <= epoch.AddYears(-18));
        }
    }
}
