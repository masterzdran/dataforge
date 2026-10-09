using System;

namespace DataForge
{
    /// <summary>
    /// Generates synthetic <see cref="Company"/> instances.
    /// </summary>
    public sealed class CompanyGenerator : IDataGenerator<Company>
    {
        private readonly IGenerationContext _context;

        /// <summary>Initializes a new instance of the <see cref="CompanyGenerator"/> class.</summary>
        /// <param name="context">Generation context.</param>
        public CompanyGenerator(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public Company Generate()
        {
            var context = _context;
            var provider = context.Provider;
            var name = provider.GetCompanyName(context);

            return new Company
            {
                Name = name,
                TaxIdentifier = provider.GetTaxIdentifier(context),
                Email = SyntheticText.CompanyEmail(context, name),
                PhoneNumber = provider.GetPhoneNumber(context),
                Address = new AddressGenerator(context).Generate()
            };
        }
    }
}
