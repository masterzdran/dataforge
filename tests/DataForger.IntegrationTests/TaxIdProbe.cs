namespace DataForger.IntegrationTests
{
    /// <summary>Sample entity produced by the integration custom generator.</summary>
    public sealed class TaxIdProbe
    {
        public string Value { get; set; }
    }

    /// <summary>Custom generator that consumes the ambient country provider and seed.</summary>
    public sealed class TaxIdProbeGenerator : IDataGenerator<TaxIdProbe>
    {
        private readonly IGenerationContext _context;

        public TaxIdProbeGenerator(IGenerationContext context)
        {
            _context = context;
        }

        public TaxIdProbe Generate()
        {
            var provider = (IPersonTaxIdentifierProvider)_context.Provider;
            return new TaxIdProbe { Value = provider.GetPersonTaxIdentifier(_context) };
        }
    }
}
