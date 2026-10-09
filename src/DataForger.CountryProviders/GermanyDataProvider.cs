namespace DataForger
{
    /// <summary>
    /// Provides German data.
    /// </summary>
    public sealed class GermanyDataProvider : CountryDataProvider
    {
        /// <summary>Initializes a new instance of the <see cref="GermanyDataProvider"/> class.</summary>
        public GermanyDataProvider()
            : base(
                Country.DE,
                new[]
                {
                    "Maximilian", "Alexander", "Lukas", "Felix", "Jonas", "Leon", "Paul", "Elias", "Noah", "Ben", "Finn", "Moritz"
                },
                new[]
                {
                    "Anna", "Lena", "Sophie", "Marie", "Laura", "Lea", "Emilia", "Mia", "Emma", "Hannah", "Clara", "Johanna"
                },
                new[]
                {
                    "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann", "Koch", "Bauer", "Richter", "Klein", "Wolf", "Schröder"
                },
                new[]
                {
                    "Berlin", "Hamburg", "München", "Köln", "Frankfurt", "Stuttgart", "Düsseldorf", "Leipzig", "Dresden", "Hannover", "Nürnberg", "Bremen"
                },
                new[]
                {
                    "Bahnhof", "Haupt", "Garten", "Schul", "Kirch", "Berg", "Wald", "Feld", "Ring", "Linden", "Goethe", "Schiller"
                },
                "{0}straße",
                new[] { "GmbH", "AG", "KG", "GmbH & Co. KG", "e.V.", "Group" },
                "#####",
                "+49 1## #######")
        {
        }

        /// <inheritdoc />
        public override string GetTaxIdentifier(IGenerationContext context)
        {
            return "DE" + Expand(context, "#########");
        }
    }
}
