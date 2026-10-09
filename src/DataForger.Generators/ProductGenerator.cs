using System;

namespace DataForger
{
    /// <summary>
    /// Generates synthetic <see cref="Product"/> instances.
    /// </summary>
    public sealed class ProductGenerator : IDataGenerator<Product>
    {
        private static readonly string[] Adjectives =
        {
            "Wireless", "Ergonomic", "Compact", "Premium", "Portable", "Smart", "Durable", "Adjustable", "Rechargeable", "Foldable", "Ultra", "Classic"
        };

        private static readonly string[] Nouns =
        {
            "Keyboard", "Mouse", "Monitor", "Headset", "Speaker", "Camera", "Lamp", "Chair", "Backpack", "Charger", "Hub", "Stand"
        };

        private static readonly string[] CategoryCodes = { "ELEC", "HOME", "OFFC", "AUDI", "ACCS", "GAME" };

        private static readonly string[] Blurbs =
        {
            "Engineered for everyday use.",
            "Built with durable, responsibly sourced materials.",
            "Designed for comfort and long sessions.",
            "Backed by a two-year limited warranty.",
            "Tested for reliability and performance.",
            "A refined take on a proven design."
        };

        private readonly IGenerationContext _context;

        /// <summary>Initializes a new instance of the <see cref="ProductGenerator"/> class.</summary>
        /// <param name="context">Generation context.</param>
        public ProductGenerator(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public Product Generate()
        {
            var context = _context;
            var name = context.Pick(Adjectives) + " " + context.Pick(Nouns);
            var price = context.Next(99, 99999) / 100m;

            return new Product
            {
                Sku = context.Pick(CategoryCodes) + "-" + context.Digits(5),
                Name = name,
                Description = name + ". " + context.Pick(Blurbs),
                Price = price
            };
        }
    }
}
