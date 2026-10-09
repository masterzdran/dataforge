namespace DataForger
{
    /// <summary>
    /// A synthetic postal address.
    /// </summary>
    public sealed class Address
    {
        /// <summary>Gets or sets the street name.</summary>
        public string Street { get; set; }

        /// <summary>Gets or sets the street number, including an optional letter suffix.</summary>
        public string Number { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        public string PostalCode { get; set; }

        /// <summary>Gets or sets the city.</summary>
        public string City { get; set; }

        /// <summary>Gets or sets the country the address belongs to.</summary>
        public Country Country { get; set; }
    }
}
