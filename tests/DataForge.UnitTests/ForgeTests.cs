using System;
using FluentAssertions;
using Xunit;

namespace DataForge.UnitTests
{
    public sealed class ForgeTests
    {
        [Fact]
        public void ForCountry_ReturnsBuilderWithCountryAndProvider()
        {
            var forge = Forge.ForCountry(Country.PT);

            forge.Country.Should().Be(Country.PT);
            forge.Provider.Should().NotBeNull();
            forge.Provider.Country.Should().Be(Country.PT);
        }

        [Fact]
        public void Constructor_WithNullRegistry_Throws()
        {
            Action act = () => new Forge(Country.PT, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ForCountry_UnsupportedValue_Throws()
        {
            Action act = () => Forge.ForCountry((Country)999);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Create_Single_ReturnsAllEntityTypes()
        {
            var forge = Forge.ForCountry(Country.PT);

            forge.Create<Person>().Should().NotBeNull();
            forge.Create<Address>().Should().NotBeNull();
            forge.Create<Company>().Should().NotBeNull();
            forge.Create<Customer>().Should().NotBeNull();
            forge.Create<Contract>().Should().NotBeNull();
            forge.Create<Product>().Should().NotBeNull();
        }

        [Fact]
        public void Create_Collection_ReturnsExactCount()
        {
            Forge.ForCountry(Country.PT).Create<Customer>(5).Should().HaveCount(5);
        }

        [Fact]
        public void Create_Zero_ReturnsEmpty()
        {
            Forge.ForCountry(Country.PT).Create<Customer>(0).Should().BeEmpty();
        }

        [Fact]
        public void Create_Negative_Throws()
        {
            Action act = () => Forge.ForCountry(Country.PT).Create<Customer>(-1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Create_UnregisteredType_Throws()
        {
            Action act = () => Forge.ForCountry(Country.PT).Create<string>();

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void WithSeed_ReturnsSameInstance()
        {
            var forge = Forge.ForCountry(Country.PT);

            forge.WithSeed(1).Should().BeSameAs(forge);
        }

        [Fact]
        public void Register_ReplacesBuiltInGenerator()
        {
            var forge = Forge.ForCountry(Country.PT);

            forge.Register<FixedCustomerGenerator>().Should().BeSameAs(forge);
            forge.Create<Customer>().Name.Should().Be("Custom");
            forge.Create<Customer>(3).Should().OnlyContain(customer => customer.Name == "Custom");
        }

        [Fact]
        public void Register_ParameterlessGenerator_Works()
        {
            var forge = Forge.ForCountry(Country.PT).Register<ParameterlessGenerator>();

            forge.Create<SimpleSample>().Value.Should().Be("fixed");
        }

        [Fact]
        public void Register_NonGenerator_Throws()
        {
            Action act = () => Forge.ForCountry(Country.PT).Register<NotAGenerator>();

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Register_DualOutputGenerator_Throws()
        {
            Action act = () => Forge.ForCountry(Country.PT).Register<DualOutputGenerator>();

            act.Should().Throw<InvalidOperationException>();
        }
    }
}
