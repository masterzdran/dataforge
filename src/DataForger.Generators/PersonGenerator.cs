using System;

namespace DataForger
{
    /// <summary>
    /// Generates synthetic <see cref="Person"/> instances.
    /// </summary>
    public sealed class PersonGenerator : IDataGenerator<Person>
    {
        private readonly IGenerationContext _context;

        /// <summary>Initializes a new instance of the <see cref="PersonGenerator"/> class.</summary>
        /// <param name="context">Generation context.</param>
        public PersonGenerator(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public Person Generate()
        {
            var context = _context;
            var provider = context.Provider;
            var gender = context.NextBool() ? Gender.Male : Gender.Female;
            var firstName = provider.GetFirstName(context, gender);
            var lastName = provider.GetLastName(context);

            return new Person
            {
                FirstName = firstName,
                LastName = lastName,
                FullName = firstName + " " + lastName,
                Email = SyntheticText.PersonEmail(context, firstName, lastName),
                PhoneNumber = provider.GetPhoneNumber(context),
                DateOfBirth = GenerationDates.DateOfBirth(context),
                Gender = gender
            };
        }
    }
}
