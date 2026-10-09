using System;

namespace DataForger
{
    /// <summary>
    /// Generates synthetic <see cref="Address"/> instances.
    /// </summary>
    public sealed class AddressGenerator : IDataGenerator<Address>
    {
        private readonly IGenerationContext _context;

        /// <summary>Initializes a new instance of the <see cref="AddressGenerator"/> class.</summary>
        /// <param name="context">Generation context.</param>
        public AddressGenerator(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public Address Generate()
        {
            var context = _context;
            var provider = context.Provider;

            return new Address
            {
                Street = provider.GetStreet(context),
                Number = provider.GetStreetNumber(context),
                PostalCode = provider.GetPostalCode(context),
                City = provider.GetCity(context),
                Country = context.Country
            };
        }
    }
}
