using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Budget
    {
        public string Name { get; set; }

        public IReadOnlyList<decimal> YearlyAmounts => _YearlyAmounts;

        public void SetYearlyAmount(int index, decimal amount) => _YearlyAmounts[index] = amount;

        internal int NumberOfYears
        {
            set
            {
                if (value < _YearlyAmounts.Count)
                {
                    _YearlyAmounts.RemoveRange(value, _YearlyAmounts.Count - value);
                }
                else if (value > _YearlyAmounts.Count)
                {
                    _YearlyAmounts.AddRange(Enumerable.Repeat(0m, value - _YearlyAmounts.Count));
                }
            }
        }

        private readonly List<decimal> _YearlyAmounts = new List<decimal>();
    }
}
