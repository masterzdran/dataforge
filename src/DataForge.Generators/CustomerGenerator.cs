using System;

namespace DataForge
{
    /// <summary>
    /// Generates synthetic <see cref="Customer"/> instances.
    /// </summary>
    public sealed class CustomerGenerator : IDataGenerator<Customer>
    {
        private readonly IGenerationContext _context;

        /// <summary>Initializes a new instance of the <see cref="CustomerGenerator"/> class.</summary>
        /// <param name="context">Generation context.</param>
        public CustomerGenerator(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public Customer Generate()
        {
            var context = _context;
            var provider = context.Provider;
            var gender = context.NextBool() ? Gender.Male : Gender.Female;
            var firstName = provider.GetFirstName(context, gender);
            var lastName = provider.GetLastName(context);
            var idBytes = new byte[16];
            context.NextBytes(idBytes);

            return new Customer
            {
                Id = new Guid(idBytes),
                CustomerNumber = "CUST-" + context.Digits(7),
                Name = firstName + " " + lastName,
                Email = SyntheticText.PersonEmail(context, firstName, lastName),
                PhoneNumber = provider.GetPhoneNumber(context),
                Address = new AddressGenerator(context).Generate()
            };
        }
    }
}
