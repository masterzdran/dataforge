using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DataForge.UnitTests
{
    public sealed class ExtensionTests
    {
        [Fact]
        public void WithSeed_String_ProducesDeterministicResults()
        {
            var first = Forge.ForCountry(Country.PT).WithSeed("fixture-a").Create<Customer>(25).Select(Signatures.Of).ToArray();
            var second = Forge.ForCountry(Country.PT).WithSeed("fixture-a").Create<Customer>(25).Select(Signatures.Of).ToArray();

            first.Should().Equal(second);
        }

        [Fact]
        public void WithSeed_DifferentStrings_ProduceDifferentResults()
        {
            var first = Forge.ForCountry(Country.PT).WithSeed("fixture-a").Create<Customer>(25).Select(Signatures.Of).ToArray();
            var second = Forge.ForCountry(Country.PT).WithSeed("fixture-b").Create<Customer>(25).Select(Signatures.Of).ToArray();

            first.Should().NotEqual(second);
        }

        [Fact]
        public void WithSeed_NullForge_Throws()
        {
            Action act = () => ForgeExtensions.WithSeed(null, "seed");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WithSeed_NullText_Throws()
        {
            Action act = () => Forge.ForCountry(Country.PT).WithSeed((string)null);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WithSeed_EmptyText_Throws()
        {
            Action act = () => Forge.ForCountry(Country.PT).WithSeed(string.Empty);

            act.Should().Throw<ArgumentException>();
        }
    }
}
