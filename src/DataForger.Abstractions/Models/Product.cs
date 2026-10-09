namespace DataForger
{
    /// <summary>
    /// A synthetic product.
    /// </summary>
    public sealed class Product
    {
        /// <summary>Gets or sets the stock keeping unit code.</summary>
        public string Sku { get; set; }

        /// <summary>Gets or sets the product name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the product description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the unit price.</summary>
        public decimal Price { get; set; }
    }
}
