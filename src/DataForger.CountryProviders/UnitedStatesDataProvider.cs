using System.Globalization;

namespace DataForger
{
    /// <summary>
    /// Provides United States data. Person identifiers are synthetic SSN-like values using
    /// the non-issued 9xx area range, so they can never match a real Social Security Number.
    /// </summary>
    public sealed class UnitedStatesDataProvider : CountryDataProvider, IPersonTaxIdentifierProvider
    {
        /// <summary>Initializes a new instance of the <see cref="UnitedStatesDataProvider"/> class.</summary>
        public UnitedStatesDataProvider()
            : base(
                Country.US,
                new[]
                {
                    "James", "Michael", "Robert", "John", "David", "William", "Richard", "Joseph", "Daniel", "Matthew", "Christopher", "Andrew"
                },
                new[]
                {
                    "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Emily", "Ashley"
                },
                new[]
                {
                    "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Wilson", "Anderson", "Thomas", "Taylor"
                },
                new[]
                {
                    "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "Austin", "Seattle", "Denver"
                },
                new[]
                {
                    "Main", "Oak", "Maple", "Cedar", "Pine", "Washington", "Lake", "Hill", "Park", "Sunset", "Franklin", "Lincoln"
                },
                "{0} Street",
                new[] { "Inc", "LLC", "Corp", "Industries", "Group", "Holdings" },
                "#####",
                "+1 (###) ###-####")
        {
        }

        /// <inheritdoc />
        public override string GetPostalCode(IGenerationContext context)
        {
            var zip = Expand(context, "#####");
            return context.NextBool() ? zip + "-" + Expand(context, "####") : zip;
        }

        /// <inheritdoc />
        public override string GetTaxIdentifier(IGenerationContext context)
        {
            var prefix = context.Next(10, 100).ToString(CultureInfo.InvariantCulture);
            return prefix + "-" + Expand(context, "#######");
        }

        /// <inheritdoc />
        public string GetPersonTaxIdentifier(IGenerationContext context)
        {
            var area = "9" + Expand(context, "##");
            return area + "-" + Expand(context, "##") + "-" + Expand(context, "####");
        }
    }
}
