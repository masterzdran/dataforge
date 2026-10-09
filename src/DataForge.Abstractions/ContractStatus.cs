namespace DataForge
{
    /// <summary>
    /// Lifecycle status of a generated <see cref="Contract"/>.
    /// </summary>
    public enum ContractStatus
    {
        /// <summary>Draft, not yet active.</summary>
        Draft,

        /// <summary>Currently active.</summary>
        Active,

        /// <summary>Temporarily suspended.</summary>
        Suspended,

        /// <summary>Completed, end date reached.</summary>
        Completed,

        /// <summary>Terminated before the agreed end date.</summary>
        Cancelled
    }
}
