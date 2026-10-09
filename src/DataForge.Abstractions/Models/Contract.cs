using System;

namespace DataForge
{
    /// <summary>
    /// A synthetic contract between the organization and a <see cref="DataForge.Customer"/>.
    /// </summary>
    public sealed class Contract
    {
        /// <summary>Gets or sets the contract number.</summary>
        public string ContractNumber { get; set; }

        /// <summary>Gets or sets the start date.</summary>
        public DateTime StartDate { get; set; }

        /// <summary>Gets or sets the end date.</summary>
        public DateTime EndDate { get; set; }

        /// <summary>Gets or sets the counterparty.</summary>
        public Customer Customer { get; set; }

        /// <summary>Gets or sets the lifecycle status.</summary>
        public ContractStatus Status { get; set; }
    }
}
