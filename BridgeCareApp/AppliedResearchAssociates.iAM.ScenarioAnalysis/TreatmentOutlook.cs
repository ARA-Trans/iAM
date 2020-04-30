using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentOutlook
    {
        public TreatmentOutlook(SectionContext templateContext, Treatment initialTreatment, int initialYear, IEnumerable<RemainingLifeCalculator.Factory> remainingLifeCalculatorFactories)
        {
            TemplateContext = templateContext ?? throw new ArgumentNullException(nameof(templateContext));
            AccumulationContext = new SectionContext(TemplateContext);
            InitialTreatment = initialTreatment ?? throw new ArgumentNullException(nameof(initialTreatment));
            InitialYear = initialYear;

            RemainingLifeCalculators = remainingLifeCalculatorFactories.Select(factory => factory.Create(AccumulationContext)).ToArray();

            Run();
        }

        public SectionContext TemplateContext { get; }

        public double CumulativeBenefit { get; private set; }

        public double CumulativeCostPerUnitArea { get; private set; }

        public Treatment InitialTreatment { get; }

        public double? RemainingLife { get; private set; }

        private readonly SectionContext AccumulationContext;

        private readonly int InitialYear;

        private readonly IReadOnlyCollection<RemainingLifeCalculator> RemainingLifeCalculators;

        private void AccumulateBenefit()
        {
            var benefit = AccumulationContext.GetBenefit();
            CumulativeBenefit += benefit;
        }

        private void ApplyTreatment(Treatment treatment, int year)
        {
            AccumulationContext.ApplyTreatment(treatment, year, out var cost);
            var area = AccumulationContext.GetArea();
            var costPerUnitArea = cost / area;
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

                if (AccumulationContext.TreatmentSchedule.ContainsKey(year))
                {
                    continue;
                }

                AccumulationContext.ApplyPerformanceCurves();

                if (AccumulationContext.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment) && scheduledTreatment != null)
                {
                    ApplyTreatment(scheduledTreatment, year);
                }

                var previousCumulativeBenefit = CumulativeBenefit;
                AccumulateBenefit();

                updateRemainingLife?.Invoke();

                if (CumulativeBenefit == previousCumulativeBenefit && AccumulationContext.TreatmentSchedule.Keys.All(scheduledYear => scheduledYear <= year))
                {
                    break;
                }
            }
        }
    }
}
