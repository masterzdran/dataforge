using System;
using DataForger;

namespace DataForger.Demo
{
    internal sealed class Program
    {
        private static void Main()
        {
            const int seed = 1234;

            Console.WriteLine("=== DataForger quick demo (deterministic) ===");
            Console.WriteLine();

            var person = Forge.ForCountry(Country.PT).WithSeed(seed).Create<Person>();
            Console.WriteLine("person = Forge.ForCountry(Country.PT).WithSeed(seed).Create<Person>()");
            Console.WriteLine($"  {person.FullName} <{person.Email}>");
            Console.WriteLine($"  {person.PhoneNumber}");

            Console.WriteLine();
            var customers = Forge.ForCountry(Country.PT).WithSeed(seed).Create<Customer>(3);
            Console.WriteLine("customers = Forge.ForCountry(Country.PT).WithSeed(seed).Create<Customer>(3)");
            foreach (var customer in customers)
            {
                Console.WriteLine($"  {customer.CustomerNumber}  {customer.Name}  <{customer.Email}>");
            }

            Console.WriteLine();
            var again = Forge.ForCountry(Country.PT).WithSeed(seed).Create<Person>();
            Console.WriteLine($"Same seed again -> same person? {person.FullName == again.FullName && person.Email == again.Email}");
        }
    }
}