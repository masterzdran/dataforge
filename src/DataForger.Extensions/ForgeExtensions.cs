using System;

namespace DataForger
{
    /// <summary>
    /// Extension methods that complement the core <see cref="Forge"/> API.
    /// </summary>
    public static class ForgeExtensions
    {
        /// <summary>Sets a deterministic seed derived from a text value using the stable FNV-1a hash.</summary>
        /// <param name="forge">Target builder. Must not be null.</param>
        /// <param name="seed">Non-empty seed text, for example a fixture name.</param>
        /// <returns>The same builder, for chaining.</returns>
        public static Forge WithSeed(this Forge forge, string seed)
        {
            if (forge == null)
            {
                throw new ArgumentNullException(nameof(forge));
            }

            if (string.IsNullOrEmpty(seed))
            {
                throw new ArgumentException("The seed text must not be null or empty.", nameof(seed));
            }

            return forge.WithSeed(HashSeed(seed));
        }

        private static int HashSeed(string value)
        {
            unchecked
            {
                var hash = 2166136261u;
                for (var i = 0; i < value.Length; i++)
                {
                    hash ^= value[i];
                    hash *= 16777619u;
                }

                return (int)hash;
            }
        }
    }
}
