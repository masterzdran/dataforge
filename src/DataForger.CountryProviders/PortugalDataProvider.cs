namespace DataForger
{
    /// <summary>
    /// Provides Portuguese data. Tax identifiers follow the NIF/NIPC modulo-11 check-digit rule.
    /// </summary>
    public sealed class PortugalDataProvider : CountryDataProvider, IPersonTaxIdentifierProvider
    {
        /// <summary>Initializes a new instance of the <see cref="PortugalDataProvider"/> class.</summary>
        public PortugalDataProvider()
            : base(
                Country.PT,
                new[]
                {
                    "João", "Miguel", "Pedro", "Tiago", "Ricardo", "Nuno", "André", "Rui", "Bruno", "Diogo", "Fernando", "Paulo"
                },
                new[]
                {
                    "Maria", "Ana", "Inês", "Sofia", "Catarina", "Marta", "Rita", "Beatriz", "Teresa", "Leonor", "Carolina", "Joana"
                },
                new[]
                {
                    "Silva", "Santos", "Ferreira", "Pereira", "Oliveira", "Costa", "Rodrigues", "Martins", "Sousa", "Fernandes", "Gonçalves", "Lopes", "Almeida", "Carvalho", "Ribeiro", "Correia"
                },
                new[]
                {
                    "Lisboa", "Porto", "Braga", "Coimbra", "Aveiro", "Faro", "Setúbal", "Guimarães", "Évora", "Funchal", "Leiria", "Viseu"
                },
                new[]
                {
                    "do Comércio", "da Liberdade", "das Flores", "de Santo António", "do Campo", "da República", "5 de Outubro", "da Praia", "do Sol", "dos Aliados", "de São Bento", "Augusta"
                },
                "Rua {0}",
                new[] { "Lda", "Lda.", "S.A.", "Unipessoal Lda", "& Filhos", "SGPS" },
                "####-###",
                "+351 9## ### ###")
        {
        }

        /// <inheritdoc />
        public override string GetTaxIdentifier(IGenerationContext context)
        {
            return TaxNumber(context, context.Pick(new[] { 5, 6, 8, 9 }));
        }

        /// <inheritdoc />
        public string GetPersonTaxIdentifier(IGenerationContext context)
        {
            return TaxNumber(context, context.Pick(new[] { 1, 2, 4 }));
        }

        private static string TaxNumber(IGenerationContext context, int firstDigit)
        {
            var digits = RandomDigits(context, 8);
            digits[0] = firstDigit;
            var check = CheckDigits.PortugueseTaxNumber(digits);
            return ToDigits(digits) + (char)('0' + check);
        }
    }
}
