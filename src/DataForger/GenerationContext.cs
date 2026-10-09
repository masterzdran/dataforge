using System;
using System.Collections.Generic;
using System.Text;

namespace DataForger
{
    /// <summary>
    /// Default <see cref="IGenerationContext"/> implementation backed by <see cref="DeterministicRandom"/>.
    /// </summary>
    internal sealed class GenerationContext : IGenerationContext
    {
        private readonly DeterministicRandom _random;

        internal GenerationContext(Country country, ICountryDataProvider provider, int seed)
        {
            Country = country;
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _random = new DeterministicRandom(seed);
        }

        /// <inheritdoc />
        public Country Country { get; }

        /// <inheritdoc />
        public ICountryDataProvider Provider { get; }

        /// <inheritdoc />
        public int Next(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }

        /// <inheritdoc />
        public double NextDouble()
        {
            return _random.NextDouble();
        }

        /// <inheritdoc />
        public bool NextBool()
        {
            return _random.NextBool();
        }

        /// <inheritdoc />
        public T Pick<T>(IReadOnlyList<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (items.Count == 0)
            {
                throw new ArgumentException("The list must contain at least one element.", nameof(items));
            }

            return items[_random.Next(0, items.Count)];
        }

        /// <inheritdoc />
        public string Digits(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "The digit count must not be negative.");
            }

            var builder = new StringBuilder(count);
            for (var i = 0; i < count; i++)
            {
                builder.Append((char)('0' + _random.Next(0, 10)));
            }

            return builder.ToString();
        }

        /// <inheritdoc />
        public void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            _random.NextBytes(buffer);
        }
    }
}
