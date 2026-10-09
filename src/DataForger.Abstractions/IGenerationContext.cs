using System.Collections.Generic;

namespace DataForger
{
    /// <summary>
    /// Ambient state handed to generators and country providers: the selected country, the
    /// matching <see cref="ICountryDataProvider"/> and a deterministic random source.
    /// </summary>
    /// <remarks>
    /// A fresh context is created for every generation call, so a context instance is never
    /// shared between threads. Individual <c>Forge</c> instances are safe to use concurrently.
    /// </remarks>
    public interface IGenerationContext
    {
        /// <summary>Gets the country this context generates data for.</summary>
        Country Country { get; }

        /// <summary>Gets the country data provider backing this context.</summary>
        ICountryDataProvider Provider { get; }

        /// <summary>Returns a random integer in the range [<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>).</summary>
        /// <param name="minInclusive">Inclusive lower bound.</param>
        /// <param name="maxExclusive">Exclusive upper bound.</param>
        /// <returns>A pseudo-random integer.</returns>
        int Next(int minInclusive, int maxExclusive);

        /// <summary>Returns a random double in the range [0.0, 1.0).</summary>
        /// <returns>A pseudo-random double.</returns>
        double NextDouble();

        /// <summary>Returns a random boolean.</summary>
        /// <returns>A pseudo-random boolean.</returns>
        bool NextBool();

        /// <summary>Returns a random element from a non-empty list.</summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="items">Source list. Must not be null or empty.</param>
        /// <returns>A pseudo-random element.</returns>
        T Pick<T>(IReadOnlyList<T> items);

        /// <summary>Returns a string of random decimal digits.</summary>
        /// <param name="count">Number of digits. Must be non-negative.</param>
        /// <returns>A digit string of length <paramref name="count"/>.</returns>
        string Digits(int count);

        /// <summary>Fills the buffer with pseudo-random bytes.</summary>
        /// <param name="buffer">Destination buffer. Must not be null.</param>
        void NextBytes(byte[] buffer);
    }
}
