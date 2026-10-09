using System.Globalization;

namespace DataForger
{
    /// <summary>
    /// Provides Spanish data. Company identifiers follow the CIF rule and person identifiers the DNI/NIF rule.
    /// </summary>
    public sealed class SpainDataProvider : CountryDataProvider, IPersonTaxIdentifierProvider
    {
        /// <summary>Initializes a new instance of the <see cref="SpainDataProvider"/> class.</summary>
        public SpainDataProvider()
            : base(
                Country.ES,
                new[]
                {
                    "Alejandro", "Daniel", "Pablo", "Javier", "Carlos", "Miguel", "Sergio", "Adrián", "Álvaro", "Iván", "Rubén", "Diego"
                },
                new[]
                {
                    "Lucía", "María", "Carmen", "Ana", "Laura", "Elena", "Marta", "Paula", "Sara", "Cristina", "Nuria", "Isabel"
                },
                new[]
                {
                    "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez", "Gómez", "Martín", "Jiménez", "Ruiz", "Hernández", "Díaz", "Moreno", "Álvarez"
                },
                new[]
                {
                    "Madrid", "Barcelona", "Valencia", "Sevilla", "Zaragoza", "Málaga", "Bilbao", "Granada", "Murcia", "Alicante", "Valladolid", "Vigo"
                },
                new[]
                {
                    "Mayor", "Real", "de Alcalá", "Gran Vía", "de la Constitución", "del Sol", "de Cervantes", "de Goya", "Colón", "de la Paz", "Nueva", "de Aragón"
                },
                "Calle {0}",
                new[] { "S.L.", "S.A.", "e Hijos", "y Cía", "Group", "Solutions" },
                "#####",
                "+34 6## ### ###")
        {
        }

        /// <inheritdoc />
        public override string GetTaxIdentifier(IGenerationContext context)
        {
            var typeLetter = context.Pick(new[] { 'A', 'B' });
            var body = RandomDigits(context, 7);
            var control = CheckDigits.SpanishCifControl(typeLetter, body);
            return typeLetter + ToDigits(body) + control;
        }

        /// <inheritdoc />
        public string GetPersonTaxIdentifier(IGenerationContext context)
        {
            var number = context.Next(1, 99999999);
            var letter = CheckDigits.SpanishDniLetter(number);
            return number.ToString("D8", CultureInfo.InvariantCulture) + letter;
        }
    }
}
