using System;

namespace DataForger
{
    /// <summary>
    /// Small, dependency-free pseudo-random source with a stable sequence for a given seed
    /// on every runtime (.NET Framework, .NET Core, Mono). <see cref="System.Random"/> is avoided
    /// because its seeded algorithm changed in .NET 6, which would break determinism across runtimes.
    /// </summary>
    /// <remarks>
    /// One instance belongs to one <see cref="IGenerationContext"/> and is used by a single thread at a time.
    /// </remarks>
    internal sealed class DeterministicRandom
    {
        private const ulong Increment = 0x9E3779B97F4A7C15UL;

        private ulong _state;

        internal DeterministicRandom(int seed)
        {
            _state = unchecked(((ulong)(uint)seed << 32) | (uint)seed) + Increment;
            if (_state == 0)
            {
                _state = Increment;
            }
        }

        internal int Next(int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(minInclusive), "The minimum bound must be smaller than the maximum bound.");
            }

            var range = (ulong)((long)maxExclusive - minInclusive);
            return (int)(minInclusive + (long)(NextUInt64() % range));
        }

        internal double NextDouble()
        {
            // 53 significant bits mapped to [0, 1).
            return (NextUInt64() >> 11) * (1.0 / 9007199254740992.0);
        }

        internal bool NextBool()
        {
            return (NextUInt64() & 1UL) == 1UL;
        }

        internal void NextBytes(byte[] buffer)
        {
            var index = 0;
            while (index < buffer.Length)
            {
                var value = NextUInt64();
                for (var i = 0; i < 8 && index < buffer.Length; i++)
                {
                    buffer[index] = (byte)(value & 0xFF);
                    value >>= 8;
                    index++;
                }
            }
        }

        private ulong NextUInt64()
        {
            unchecked
            {
                _state += Increment;
                var z = _state;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }
        }
    }
}
