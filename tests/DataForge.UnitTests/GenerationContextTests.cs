using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DataForge.UnitTests
{
    public sealed class GenerationContextTests
    {
        [Fact]
        public void Context_ExposesCountryAndProvider()
        {
            var sample = Forge.ForCountry(Country.PT)
                .WithSeed(41)
                .Register<ContextProbeGenerator>()
                .Create<ContextSample>();

            sample.Context.Should().NotBeNull();
            sample.Context.Country.Should().Be(Country.PT);
            sample.Context.Provider.Should().NotBeNull();
            sample.Context.Provider.Country.Should().Be(Country.PT);
        }

        [Fact]
        public void Context_Primitives_RespectBoundsAndFormats()
        {
            var sample = Forge.ForCountry(Country.ES)
                .WithSeed(42)
                .Register<ContextProbeGenerator>()
                .Create<ContextSample>();

            sample.IntValue.Should().BeGreaterOrEqualTo(10).And.BeLessThan(20);
            sample.DoubleValue.Should().BeGreaterOrEqualTo(0.0).And.BeLessThan(1.0);
            sample.Digits.Should().MatchRegex("^[0-9]{6}$");
            sample.Picked.Should().BeOneOf("alpha", "beta", "gamma");
        }

        [Fact]
        public void Context_Digits_AreDeterministicForSameSeed()
        {
            var first = Forge.ForCountry(Country.PT).WithSeed(43).Register<ContextProbeGenerator>().Create<ContextSample>();
            var second = Forge.ForCountry(Country.PT).WithSeed(43).Register<ContextProbeGenerator>().Create<ContextSample>();

            first.Digits.Should().Be(second.Digits);
            first.IntValue.Should().Be(second.IntValue);
            first.BoolValue.Should().Be(second.BoolValue);
            first.Picked.Should().Be(second.Picked);
        }

        [Fact]
        public void Context_Digits_NegativeCount_Throws()
        {
            var context = CreateContext();

            Action act = () => context.Digits(-1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Context_Pick_NullList_Throws()
        {
            var context = CreateContext();

            Action act = () => context.Pick<string>(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Context_Pick_EmptyList_Throws()
        {
            var context = CreateContext();

            Action act = () => context.Pick(new string[0]);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Context_Next_InvalidRange_Throws()
        {
            var context = CreateContext();

            Action act = () => context.Next(5, 5);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Context_NextBytes_NullBuffer_Throws()
        {
            var context = CreateContext();

            Action act = () => context.NextBytes(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Context_NextBytes_FillsBuffer()
        {
            var context = CreateContext();
            var buffer = new byte[20];

            context.NextBytes(buffer);

            buffer.Should().HaveCount(20);
            buffer.Sum(value => value).Should().BeGreaterThan(0);
        }

        private static IGenerationContext CreateContext()
        {
            return Forge.ForCountry(Country.PT)
                .WithSeed(44)
                .Register<ContextProbeGenerator>()
                .Create<ContextSample>()
                .Context;
        }
    }
}
