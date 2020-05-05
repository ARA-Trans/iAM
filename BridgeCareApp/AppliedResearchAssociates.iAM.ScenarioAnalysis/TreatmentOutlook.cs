using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentOutlook
    {
        public TreatmentOutlook(SectionContext templateContext, SelectableTreatment initialTreatment, int initialYear, IEnumerable<RemainingLifeCalculator.Factory> remainingLifeCalculatorFactories)
        {
            TemplateContext = templateContext ?? throw new ArgumentNullException(nameof(templateContext));
            InitialTreatment = initialTreatment ?? throw new ArgumentNullException(nameof(initialTreatment));
            InitialYear = initialYear;

            RemainingLifeCalculators = remainingLifeCalculatorFactories.Select(factory => factory.Create(AccumulationContext)).ToArray();

            AccumulationContext = new SectionContext(TemplateContext);

            Run();
        }

        public TreatmentOutlookSummary GetSummaryRelativeToBaseline(TreatmentOutlook baseline)
        {
            if (TemplateContext != baseline.TemplateContext)
            {
                throw new ArgumentException("Template context does not match.", nameof(baseline));
            }

            if (InitialTreatment != baseline.InitialTreatment)
            {
                throw new ArgumentException("Initial treatment does not match.", nameof(baseline));
            }

            return new TreatmentOutlookSummary(
                TemplateContext,
                InitialTreatment,
                CumulativeCostPerUnitArea - baseline.CumulativeCostPerUnitArea,
                CumulativeBenefit - baseline.CumulativeBenefit,
                RemainingLife - baseline.RemainingLife);
        }

        private readonly SectionContext TemplateContext;
        private readonly SectionContext AccumulationContext;
        private readonly SelectableTreatment InitialTreatment;
        private readonly int InitialYear;
        private readonly IReadOnlyCollection<RemainingLifeCalculator> RemainingLifeCalculators;

        private double CumulativeBenefit;
        private double CumulativeCostPerUnitArea;
        private double? RemainingLife; // REVIEW: Should this default to zero so that per-context lack of valid RLLs doesn't crash the optimization ordering?

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
                    throw new InvalidOperationException($"Treatment outlook did not terminate naturally within {MAXIMUM_NUMBER_OF_YEARS_OF_OUTLOOK} years.");
                }

                if (AccumulationContext.ProgressSchedule.TryGetValue(year, out var treatmentProgress))
                {
                    // TODO: apply progress (accumulating cost & applying consequences)
                    continue;
                }

                AccumulationContext.ApplyPerformanceCurves();

                if (AccumulationContext.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment))
                {
                    ApplyTreatment(scheduledTreatment, year);
                }

                var previousCumulativeBenefit = CumulativeBenefit;
                AccumulateBenefit();

                updateRemainingLife?.Invoke();

                if (CumulativeBenefit == previousCumulativeBenefit &&
                    AccumulationContext.TreatmentSchedule.Keys.All(scheduledYear => scheduledYear <= year) &&
                    AccumulationContext.ProgressSchedule.Keys.All(scheduleYear => scheduleYear <= year))
                {
                    break;
                }
            }
        }
    }
}
