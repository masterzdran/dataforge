using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DataForger.UnitTests
{
    public sealed class ThreadSafetyTests
    {
        [Fact]
        public void SharedBuilder_SameSeed_ParallelGeneratesIdenticalSequences()
        {
            var forge = Forge.ForCountry(Country.PT).WithSeed(7);
            var results = new IReadOnlyCollection<Customer>[8];

            Parallel.For(0, results.Length, index => results[index] = forge.Create<Customer>(200));

            var expected = results[0].Select(Signatures.Of).ToArray();
            foreach (var result in results)
            {
                result.Select(Signatures.Of).Should().Equal(expected);
            }
        }

        [Fact]
        public void IndependentBuilders_Parallel_ProduceValidData()
        {
            var countries = new[] { Country.PT, Country.ES, Country.US, Country.BR, Country.DE, Country.FR, Country.UK };

            Action act = () => Parallel.For(
                0,
                countries.Length,
                index => Forge.ForCountry(countries[index]).WithSeed(index).Create<Customer>(100));

            act.Should().NotThrow();
        }

        [Fact]
        public void ConcurrentRegistrationAndGeneration_DoesNotThrow()
        {
            var forge = Forge.ForCountry(Country.PT).WithSeed(0);

            Action act = () => Parallel.For(
                0,
                64,
                _ =>
                {
                    forge.Register<FixedCustomerGenerator>();
                    forge.Create<Customer>(10);
                });

            act.Should().NotThrow();
        }
    }
}
