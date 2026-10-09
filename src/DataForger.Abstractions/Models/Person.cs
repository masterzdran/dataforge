using System;

namespace DataForger
{
    /// <summary>
    /// A synthetic person. No real personal data is ever used.
    /// </summary>
    /// <remarks>
    /// Settable properties keep custom <see cref="IDataGenerator{T}"/> implementations simple.
    /// </remarks>
    public sealed class Person
    {
        /// <summary>Gets or sets the first name.</summary>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        public string LastName { get; set; }

        /// <summary>Gets or sets the composed full name.</summary>
        public string FullName { get; set; }

        /// <summary>Gets or sets the e-mail address. Uses reserved example domains only.</summary>
        public string Email { get; set; }

        /// <summary>Gets or sets the phone number, including the country calling code.</summary>
        public string PhoneNumber { get; set; }

        /// <summary>Gets or sets the date of birth.</summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>Gets or sets the gender.</summary>
        public Gender Gender { get; set; }
    }
}
