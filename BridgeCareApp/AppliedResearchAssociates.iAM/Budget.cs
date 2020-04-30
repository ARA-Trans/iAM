using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Budget
    {
        public string Name { get; set; }

        public IReadOnlyList<double> YearlyAmounts => _YearlyAmounts;

        public void SetYearlyAmount(int index, double amount) => _YearlyAmounts[index] = amount;

        internal void SetNumberOfYears(int numberOfYears) => _YearlyAmounts.RemoveRange(numberOfYears, _YearlyAmounts.Count - numberOfYears);

        private readonly List<double> _YearlyAmounts = new List<double>();
    }
}
