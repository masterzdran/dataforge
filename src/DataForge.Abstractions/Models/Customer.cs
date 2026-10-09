using System;

namespace DataForge
{
    /// <summary>
    /// A synthetic customer.
    /// </summary>
    public sealed class Customer
    {
        /// <summary>Gets or sets the unique identifier, deterministically derived from the seed.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the human-readable customer number.</summary>
        public string CustomerNumber { get; set; }

        /// <summary>Gets or sets the customer name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the contact e-mail address. Uses reserved example domains only.</summary>
        public string Email { get; set; }

        /// <summary>Gets or sets the phone number, including the country calling code.</summary>
        public string PhoneNumber { get; set; }

        /// <summary>Gets or sets the billing address.</summary>
        public Address Address { get; set; }
    }
}
