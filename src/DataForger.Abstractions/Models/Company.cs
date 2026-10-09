namespace DataForger
{
    /// <summary>
    /// A synthetic company.
    /// </summary>
    public sealed class Company
    {
        /// <summary>Gets or sets the company name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the country-specific tax identifier (NIPC, CIF, CNPJ, EIN, SIREN, ...).</summary>
        public string TaxIdentifier { get; set; }

        /// <summary>Gets or sets the contact e-mail address. Uses reserved example domains only.</summary>
        public string Email { get; set; }

        /// <summary>Gets or sets the phone number, including the country calling code.</summary>
        public string PhoneNumber { get; set; }

        /// <summary>Gets or sets the registered address.</summary>
        public Address Address { get; set; }
    }
}
