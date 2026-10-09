namespace DataForger
{
    /// <summary>
    /// Provides French data. Company identifiers follow the SIREN Luhn check-digit rule.
    /// </summary>
    public sealed class FranceDataProvider : CountryDataProvider
    {
        /// <summary>Initializes a new instance of the <see cref="FranceDataProvider"/> class.</summary>
        public FranceDataProvider()
            : base(
                Country.FR,
                new[]
                {
                    "Jean", "Pierre", "Louis", "Antoine", "Nicolas", "Julien", "Thomas", "Maxime", "Hugo", "Lucas", "Alexandre", "Mathieu"
                },
                new[]
                {
                    "Marie", "Camille", "Emma", "Chloé", "Léa", "Manon", "Julie", "Sarah", "Laura", "Céline", "Sophie", "Inès"
                },
                new[]
                {
                    "Martin", "Bernard", "Dubois", "Thomas", "Robert", "Richard", "Petit", "Durand", "Leroy", "Moreau", "Simon", "Laurent", "Lefebvre", "Michel", "Garcia", "David"
                },
                new[]
                {
                    "Paris", "Lyon", "Marseille", "Toulouse", "Nice", "Nantes", "Strasbourg", "Bordeaux", "Lille", "Rennes", "Reims", "Montpellier"
                },
                new[]
                {
                    "de la République", "Victor Hugo", "de la Paix", "Pasteur", "des Écoles", "du Général de Gaulle", "Jean Jaurès", "de Verdun", "Saint-Michel", "des Lilas", "de la Gare", "du Moulin"
                },
                "Rue {0}",
                new[] { "S.A.S.", "S.A.R.L.", "S.A.", "et Fils", "Groupe", "Solutions" },
                "#####",
                "+33 6 ## ## ## ##")
        {
        }

        /// <inheritdoc />
        public override string GetTaxIdentifier(IGenerationContext context)
        {
            var first = context.Next(3, 10);
            var value = first + Expand(context, "#######");
            return value + CheckDigits.Luhn(value);
        }
    }
}
