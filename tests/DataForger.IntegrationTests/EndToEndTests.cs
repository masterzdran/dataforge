using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DataForger.IntegrationTests
{
    public sealed class EndToEndTests
    {
        [Fact]
        public void LargeDataSet_IsGeneratedCorrectly()
        {
            const int Count = 100000;

            var stopwatch = Stopwatch.StartNew();
            var customers = Forge.ForCountry(Country.PT).WithSeed(99).Create<Customer>(Count);
            stopwatch.Stop();

            customers.Should().HaveCount(Count);
            customers.Should().OnlyContain(customer => !string.IsNullOrWhiteSpace(customer.Name));
            customers.Should().OnlyContain(customer => customer.Address != null);
            customers.Select(customer => customer.Id).Distinct().Should().HaveCount(Count);
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromMinutes(2));
        }

        [Fact]
        public void MultiCountryBatch_IsDeterministic()
        {
            var countries = new[] { Country.PT, Country.ES, Country.UK, Country.FR, Country.DE, Country.US, Country.BR };

            var first = GenerateSnapshot(countries);
            var second = GenerateSnapshot(countries);

            first.Should().Equal(second);
        }

        [Fact]
        public void SeededEnvironment_CreatesConsistentContracts()
        {
            var forge = Forge.ForCountry(Country.US).WithSeed("demo-environment");
            var customers = forge.Create<Customer>(500);
            var contracts = forge.Create<Contract>(500);

            customers.Should().HaveCount(500);
            contracts.Should().OnlyContain(contract => contract.Customer != null);
            contracts.Should().OnlyContain(contract => contract.EndDate > contract.StartDate);
        }

        [Fact]
        public void CustomGenerator_IntegratesWithCountryProviderAndSeed()
        {
            var forge = Forge.ForCountry(Country.PT).WithSeed(5).Register<TaxIdProbeGenerator>();

            var first = forge.Create<TaxIdProbe>();
            var second = Forge.ForCountry(Country.PT).WithSeed(5).Register<TaxIdProbeGenerator>().Create<TaxIdProbe>();

            first.Value.Should().NotBeNullOrWhiteSpace();
            first.Value.Should().Be(second.Value);
        }

        [Fact]
        public void InvalidArguments_AreRejected()
        {
            Action negative = () => Forge.ForCountry(Country.PT).Create<Person>(-5);
            Action unknownCountry = () => Forge.ForCountry((Country)1000);
            Action unknownType = () => Forge.ForCountry(Country.PT).Create<Uri>();

            negative.Should().Throw<ArgumentOutOfRangeException>();
            unknownCountry.Should().Throw<ArgumentOutOfRangeException>();
            unknownType.Should().Throw<InvalidOperationException>();
        }

        private static string[] GenerateSnapshot(IEnumerable<Country> countries)
        {
            var snapshot = new List<string>();
            var seed = 0;
            foreach (var country in countries)
            {
                seed++;
                var forge = Forge.ForCountry(country).WithSeed(seed);
                foreach (var person in forge.Create<Person>(25))
                {
                    snapshot.Add(person.FullName + "|" + person.Email + "|" + person.DateOfBirth.Ticks);
                }
            }

            return snapshot.ToArray();
        }
    }
}
