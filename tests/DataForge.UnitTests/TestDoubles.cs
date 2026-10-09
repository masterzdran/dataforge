using System;

namespace DataForge.UnitTests
{
    /// <summary>Sample entity that exposes the generation context so tests can exercise it.</summary>
    public sealed class ContextSample
    {
        public IGenerationContext Context { get; set; }

        public int IntValue { get; set; }

        public double DoubleValue { get; set; }

        public bool BoolValue { get; set; }

        public string Digits { get; set; }

        public string Picked { get; set; }
    }

    /// <summary>Generator that samples every <see cref="IGenerationContext"/> primitive.</summary>
    public sealed class ContextProbeGenerator : IDataGenerator<ContextSample>
    {
        private static readonly string[] Options = { "alpha", "beta", "gamma" };

        private readonly IGenerationContext _context;

        public ContextProbeGenerator(IGenerationContext context)
        {
            _context = context;
        }

        public ContextSample Generate()
        {
            return new ContextSample
            {
                Context = _context,
                IntValue = _context.Next(10, 20),
                DoubleValue = _context.NextDouble(),
                BoolValue = _context.NextBool(),
                Digits = _context.Digits(6),
                Picked = _context.Pick(Options)
            };
        }
    }

    /// <summary>Sample entity carrying a person tax identifier.</summary>
    public sealed class PersonTaxIdentifierSample
    {
        public string Value { get; set; }
    }

    /// <summary>Generator that reaches the person tax identifier capability through the context.</summary>
    public sealed class PersonTaxIdentifierProbeGenerator : IDataGenerator<PersonTaxIdentifierSample>
    {
        private readonly IGenerationContext _context;

        public PersonTaxIdentifierProbeGenerator(IGenerationContext context)
        {
            _context = context;
        }

        public PersonTaxIdentifierSample Generate()
        {
            var provider = (IPersonTaxIdentifierProvider)_context.Provider;
            return new PersonTaxIdentifierSample { Value = provider.GetPersonTaxIdentifier(_context) };
        }
    }

    /// <summary>Simple entity for parameterless generator tests.</summary>
    public sealed class SimpleSample
    {
        public string Value { get; set; }
    }

    /// <summary>Alternative entity for multi-output generator tests.</summary>
    public sealed class OtherSample
    {
        public string Value { get; set; }
    }

    /// <summary>Generator without a context argument.</summary>
    public sealed class ParameterlessGenerator : IDataGenerator<SimpleSample>
    {
        public SimpleSample Generate()
        {
            return new SimpleSample { Value = "fixed" };
        }
    }

    /// <summary>Type that does not implement the generator interface.</summary>
    public sealed class NotAGenerator
    {
    }

    /// <summary>Generator that claims two output types.</summary>
    public sealed class DualOutputGenerator : IDataGenerator<SimpleSample>, IDataGenerator<OtherSample>
    {
        public SimpleSample Generate()
        {
            return new SimpleSample();
        }

        OtherSample IDataGenerator<OtherSample>.Generate()
        {
            return new OtherSample();
        }
    }

    /// <summary>Custom generator that overrides the built-in customer generator.</summary>
    public sealed class FixedCustomerGenerator : IDataGenerator<Customer>
    {
        public Customer Generate()
        {
            return new Customer { Name = "Custom", CustomerNumber = "CUSTOM-1" };
        }
    }

    /// <summary>Minimal provider used to test registry replacement.</summary>
    public sealed class StubCountryDataProvider : CountryDataProvider
    {
        public StubCountryDataProvider()
            : base(
                Country.PT,
                new[] { "Anna" },
                new[] { "Bea" },
                new[] { "Costa" },
                new[] { "City" },
                new[] { "Street" },
                "{0}",
                new[] { "Lda" },
                "###",
                "###")
        {
        }

        public override string GetTaxIdentifier(IGenerationContext context)
        {
            return "STUB";
        }
    }

    /// <summary>Provider with a null pool, used to verify constructor validation.</summary>
    public sealed class NullPoolProvider : CountryDataProvider
    {
        public NullPoolProvider()
            : base(Country.PT, null, new[] { "b" }, new[] { "c" }, new[] { "d" }, new[] { "e" }, "{0}", new[] { "f" }, "###", "###")
        {
        }

        public override string GetTaxIdentifier(IGenerationContext context)
        {
            return "X";
        }
    }

    /// <summary>Provider with an empty pool, used to verify constructor validation.</summary>
    public sealed class EmptyPoolProvider : CountryDataProvider
    {
        public EmptyPoolProvider()
            : base(Country.PT, new string[0], new[] { "b" }, new[] { "c" }, new[] { "d" }, new[] { "e" }, "{0}", new[] { "f" }, "###", "###")
        {
        }

        public override string GetTaxIdentifier(IGenerationContext context)
        {
            return "X";
        }
    }

    /// <summary>Provider with a null format, used to verify constructor validation.</summary>
    public sealed class NullFormatProvider : CountryDataProvider
    {
        public NullFormatProvider()
            : base(Country.PT, new[] { "a" }, new[] { "b" }, new[] { "c" }, new[] { "d" }, new[] { "e" }, null, new[] { "f" }, "###", "###")
        {
        }

        public override string GetTaxIdentifier(IGenerationContext context)
        {
            return "X";
        }
    }

    /// <summary>Builds a comparable signature for determinism assertions.</summary>
    public static class Signatures
    {
        public static string Of(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            return string.Join(
                "|",
                customer.Id,
                customer.CustomerNumber,
                customer.Name,
                customer.Email,
                customer.PhoneNumber,
                customer.Address.Street,
                customer.Address.Number,
                customer.Address.PostalCode,
                customer.Address.City);
        }
    }
}
