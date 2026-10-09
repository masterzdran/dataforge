namespace DataForge
{
    /// <summary>
    /// Provides Brazilian data. Company identifiers follow the CNPJ modulo-11 rule and
    /// person identifiers the CPF modulo-11 rule.
    /// </summary>
    public sealed class BrazilDataProvider : CountryDataProvider, IPersonTaxIdentifierProvider
    {
        /// <summary>Initializes a new instance of the <see cref="BrazilDataProvider"/> class.</summary>
        public BrazilDataProvider()
            : base(
                Country.BR,
                new[]
                {
                    "João", "José", "Pedro", "Lucas", "Gabriel", "Rafael", "Bruno", "Felipe", "Thiago", "Matheus", "Carlos", "Diego"
                },
                new[]
                {
                    "Maria", "Ana", "Juliana", "Fernanda", "Camila", "Larissa", "Beatriz", "Patrícia", "Amanda", "Letícia", "Bruna", "Carolina"
                },
                new[]
                {
                    "Silva", "Santos", "Oliveira", "Souza", "Rodrigues", "Ferreira", "Alves", "Pereira", "Lima", "Gomes", "Costa", "Ribeiro", "Martins", "Carvalho", "Almeida", "Lopes"
                },
                new[]
                {
                    "São Paulo", "Rio de Janeiro", "Brasília", "Salvador", "Fortaleza", "Belo Horizonte", "Curitiba", "Recife", "Manaus", "Porto Alegre", "Goiânia", "Belém"
                },
                new[]
                {
                    "das Flores", "dos Santos", "Brasil", "da Independência", "do Comércio", "das Palmeiras", "da Praia", "Getúlio Vargas", "da Bahia", "São João", "das Acácias", "do Sol"
                },
                "Rua {0}",
                new[] { "Ltda", "S.A.", "Comércio", "& Cia", "Grupo", "Serviços" },
                "#####-###",
                "+55 (##) 9####-####")
        {
        }

        /// <inheritdoc />
        public override string GetTaxIdentifier(IGenerationContext context)
        {
            var baseDigits = RandomDigits(context, 12);
            var check = CheckDigits.Cnpj(baseDigits);
            var value = ToDigits(baseDigits) + ToDigits(check);
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}.{1}.{2}/{3}-{4}",
                value.Substring(0, 2),
                value.Substring(2, 3),
                value.Substring(5, 3),
                value.Substring(8, 4),
                value.Substring(12, 2));
        }

        /// <inheritdoc />
        public string GetPersonTaxIdentifier(IGenerationContext context)
        {
            var baseDigits = RandomDigits(context, 9);
            var check = CheckDigits.Cpf(baseDigits);
            var value = ToDigits(baseDigits) + ToDigits(check);
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}.{1}.{2}-{3}",
                value.Substring(0, 3),
                value.Substring(3, 3),
                value.Substring(6, 3),
                value.Substring(9, 2));
        }
    }
}
