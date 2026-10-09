using System;
using System.Globalization;

namespace DataForger
{
    /// <summary>
    /// Generates synthetic <see cref="Contract"/> instances with an embedded <see cref="Customer"/>.
    /// </summary>
    public sealed class ContractGenerator : IDataGenerator<Contract>
    {
        private readonly IGenerationContext _context;

        /// <summary>Initializes a new instance of the <see cref="ContractGenerator"/> class.</summary>
        /// <param name="context">Generation context.</param>
        public ContractGenerator(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public Contract Generate()
        {
            var context = _context;
            var start = GenerationDates.ContractStart(context);
            var end = start.AddDays(context.Next(30, (5 * 365) + 1));
            var status = end < GenerationDates.Epoch
                ? ContractStatus.Completed
                : (ContractStatus)context.Next(0, 3);

            return new Contract
            {
                ContractNumber = "CT-" + start.Year.ToString(CultureInfo.InvariantCulture) + "-" + context.Digits(5),
                StartDate = start,
                EndDate = end,
                Customer = new CustomerGenerator(context).Generate(),
                Status = status
            };
        }
    }
}
