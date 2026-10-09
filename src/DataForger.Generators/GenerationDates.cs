using System;

namespace DataForger
{
    /// <summary>
    /// Date anchors for deterministic generation. Relative dates are derived from a fixed epoch,
    /// so the same seed yields the same dates regardless of the wall clock.
    /// </summary>
    internal static class GenerationDates
    {
        internal static readonly DateTime Epoch = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private const int DaysPerYear = 365;

        internal static DateTime DateOfBirth(IGenerationContext context)
        {
            return Epoch.AddDays(-context.Next(18 * DaysPerYear, (76 * DaysPerYear) + 1));
        }

        internal static DateTime ContractStart(IGenerationContext context)
        {
            return Epoch.AddDays(-context.Next(0, (5 * DaysPerYear) + 1));
        }
    }
}
