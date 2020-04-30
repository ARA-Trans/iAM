using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    internal sealed class TreatmentOutlook
    {
        public TreatmentOutlook(SectionContext templateContext, Treatment initialTreatment, int initialYear, IEnumerable<RemainingLifeCalculator.Factory> remainingLifeCalculatorFactories)
        {
            CumulativeContext = new SectionContext(templateContext ?? throw new ArgumentNullException(nameof(templateContext)));
            InitialTreatment = initialTreatment ?? throw new ArgumentNullException(nameof(initialTreatment));
            InitialYear = initialYear;

            RemainingLifeCalculators = remainingLifeCalculatorFactories.Select(factory => factory.Create(CumulativeContext)).ToArray();

            Run();
        }

        public double CumulativeBenefit { get; private set; }

        public SectionContext CumulativeContext { get; }

        public double CumulativeCostPerUnitArea { get; private set; }

        public Treatment InitialTreatment { get; }

        public double? RemainingLife { get; private set; }

        private readonly int InitialYear;

        private readonly IReadOnlyCollection<RemainingLifeCalculator> RemainingLifeCalculators;

        private void AccumulateBenefit()
        {
            var benefit = CumulativeContext.GetBenefit();
            CumulativeBenefit += benefit;
        }

        private void ApplyTreatment(Treatment treatment, int year)
        {
            CumulativeContext.ApplyTreatment(treatment, year, out var totalCost);
            var area = CumulativeContext.GetArea();
            var costPerUnitArea = totalCost / area;
            CumulativeCostPerUnitArea += costPerUnitArea;
        }

        private void Run()
        {
            ApplyTreatment(InitialTreatment, InitialYear);
            AccumulateBenefit();

            Action updateRemainingLife = null;

            if (RemainingLifeCalculators.Count > 0)
            {
                RemainingLife = 0;

                updateRemainingLife = delegate
                {
                    foreach (var calculator in RemainingLifeCalculators)
                    {
                        calculator.UpdateValue();
                    }

                    var minimumFractionalRemainingLife = Enumerable.Min(
                        from calculator in RemainingLifeCalculators
                        where calculator.CurrentValueIsBeyondLimit()
                        select calculator.GetLimitLocationRelativeToLatestValues().AsNullable());

                    if (minimumFractionalRemainingLife.HasValue)
                    {
                        RemainingLife += minimumFractionalRemainingLife.Value;
                        updateRemainingLife = null;
                    }
                    else
                    {
                        RemainingLife += 1;
                    }
                };
            }

            const int MAXIMUM_NUMBER_OF_YEARS_OF_OUTLOOK = 100;
            var maximumYearOfOutlook = InitialYear + MAXIMUM_NUMBER_OF_YEARS_OF_OUTLOOK;

            foreach (var year in Static.Count(InitialYear + 1))
            {
                if (year > maximumYearOfOutlook)
                {
                    throw new SimulationException($"Treatment outlook must terminate naturally within {MAXIMUM_NUMBER_OF_YEARS_OF_OUTLOOK} years.");
                }

                if (CumulativeContext.TreatmentSchedule.ContainsKey(year))
                {
                    continue;
                }

                CumulativeContext.ApplyPerformanceCurves();

                if (CumulativeContext.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment) && scheduledTreatment != null)
                {
                    ApplyTreatment(scheduledTreatment, year);
                }

                var previousCumulativeBenefit = CumulativeBenefit;
                AccumulateBenefit();

                updateRemainingLife?.Invoke();

                if (CumulativeBenefit == previousCumulativeBenefit && CumulativeContext.TreatmentSchedule.Keys.All(scheduledYear => scheduledYear <= year))
                {
                    break;
                }
            }
        }
    }
}
