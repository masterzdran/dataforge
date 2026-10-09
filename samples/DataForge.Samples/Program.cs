using System;
using DataForge;

namespace DataForge.Samples
{
    public sealed class Passport
    {
        public string Number { get; set; }
        public Country Country { get; set; }
    }

    public sealed class PassportGenerator : IDataGenerator<Passport>
    {
        private readonly IGenerationContext _context;

        public PassportGenerator(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Passport Generate()
        {
            return new Passport
            {
                Number = "P" + _context.Digits(8),
                Country = _context.Country,
            };
        }
    }

    internal sealed class Program
    {
        private static void Main()
        {
            const int seed = 1234;

            Console.WriteLine("=== DataForge samples (deterministic; same seed) ===");
            Console.WriteLine();

            var countries = new[] { Country.PT, Country.ES, Country.FR, Country.DE, Country.UK, Country.US, Country.BR };
            foreach (var c in countries)
            {
                var person = Forge.ForCountry(c).WithSeed(seed).Create<Person>();
                Console.WriteLine($"{c}: {person.FullName} | {person.Email} | {person.PhoneNumber}");
            }

            Console.WriteLine();
            var customers = Forge.ForCountry(Country.PT).WithSeed(seed).Create<Customer>(3);
            Console.WriteLine("PT customers (seed 1234, count 3):");
            foreach (var cu in customers)
            {
                Console.WriteLine($"  {cu.CustomerNumber} - {cu.Name} - {cu.Email}");
            }

            Console.WriteLine();
            var companies = Forge.ForCountry(Country.BR).WithSeed(42).Create<Company>(2);
            Console.WriteLine("BR companies (seed 42):");
            foreach (var co in companies)
            {
                Console.WriteLine($"  {co.Name} - Tax: {co.TaxIdentifier}");
            }

            Console.WriteLine();
            var a = Forge.ForCountry(Country.US).WithSeed("demo").Create<Person>();
            var b = Forge.ForCountry(Country.US).WithSeed("demo").Create<Person>();
            Console.WriteLine($"Determinism: WithSeed('demo') -> same person? {a.FullName == b.FullName && a.Email == b.Email}");

            Console.WriteLine();
            var passport = Forge.ForCountry(Country.FR).WithSeed(7).Register<PassportGenerator>().Create<Passport>();
            Console.WriteLine($"Custom generator (Passport): {passport.Country} {passport.Number}");
        }
    }
}

