namespace DataForge
{
    /// <summary>
    /// Provides United Kingdom data. Company numbers use a two-letter region prefix.
    /// </summary>
    public sealed class UnitedKingdomDataProvider : CountryDataProvider
    {
        private static readonly string[] OutwardAreas = { "EC1", "SW1", "M1", "EH1", "B1", "LS1", "BS1", "CF1" };
        private static readonly string[] InwardSuffixes = { "AA", "AB", "DD", "EE", "HN", "LL", "TT", "ZZ" };

        /// <summary>Initializes a new instance of the <see cref="UnitedKingdomDataProvider"/> class.</summary>
        public UnitedKingdomDataProvider()
            : base(
                Country.UK,
                new[]
                {
                    "Oliver", "Harry", "Jack", "George", "Charlie", "Thomas", "Oscar", "William", "James", "Henry", "Alfie", "Arthur"
                },
                new[]
                {
                    "Olivia", "Amelia", "Isla", "Ava", "Emily", "Sophia", "Grace", "Lily", "Mia", "Ella", "Charlotte", "Freya"
                },
                new[]
                {
                    "Smith", "Jones", "Taylor", "Brown", "Williams", "Wilson", "Johnson", "Davies", "Robinson", "Wright", "Thompson", "Evans", "Walker", "White", "Roberts", "Green"
                },
                new[]
                {
                    "London", "Manchester", "Birmingham", "Leeds", "Bristol", "Glasgow", "Edinburgh", "Liverpool", "Sheffield", "Newcastle", "Cardiff", "Belfast"
                },
                new[]
                {
                    "High", "Church", "Station", "Victoria", "Mill", "Park", "Kings", "Queens", "New", "Green", "Gordon", "Alexandra"
                },
                "{0} Street",
                new[] { "Ltd", "Limited", "PLC", "Group", "Holdings", "& Co" },
                "###",
                "+44 7### ######")
        {
        }

        /// <inheritdoc />
        public override string GetPostalCode(IGenerationContext context)
        {
            var outward = context.Pick(OutwardAreas);
            var inward = context.Next(0, 10).ToString(System.Globalization.CultureInfo.InvariantCulture) + context.Pick(InwardSuffixes);
            return outward + " " + inward;
        }

        /// <inheritdoc />
        public override string GetTaxIdentifier(IGenerationContext context)
        {
            var prefix = context.Pick(new[] { "SC", "NI", "OC" });
            return prefix + Expand(context, "######");
        }
    }
}
