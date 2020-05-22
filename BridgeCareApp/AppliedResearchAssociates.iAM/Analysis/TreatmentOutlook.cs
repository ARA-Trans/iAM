using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis
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

        public TreatmentOption GetOptionRelativeToBaseline(TreatmentOutlook baseline)
        {
            if (TemplateContext != baseline.TemplateContext)
            {
                throw new ArgumentException("Template context does not match.", nameof(baseline));
            }

            if (InitialTreatment != baseline.InitialTreatment)
            {
                throw new ArgumentException("Initial treatment does not match.", nameof(baseline));
            }

            return new TreatmentOption(
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
        private double? RemainingLife;

        private void AccumulateBenefit()
        {
            var benefit = AccumulationContext.GetBenefit();
            CumulativeBenefit += benefit;
        }

        private void ApplyTreatment(Treatment treatment, int year)
        {
            var cost = AccumulationContext.GetCostOfTreatment(treatment);
            var area = AccumulationContext.GetAreaOfSection();
            var costPerUnitArea = cost / area;
            CumulativeCostPerUnitArea += costPerUnitArea;
            AccumulationContext.ApplyTreatment(treatment, year);
        }

        private void Run()
        {
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

            ApplyTreatment(InitialTreatment, InitialYear);

            AccumulateBenefit();
            updateRemainingLife?.Invoke();

            foreach (var year in Enumerable.Range(InitialYear + 1, AccumulationContext.SimulationRunner.Simulation.NumberOfYearsOfTreatmentOutlook))
            {
                var yearIsScheduled = AccumulationContext.EventSchedule.TryGetValue(year, out var scheduledEvent);

                if (yearIsScheduled && scheduledEvent.IsT2())
                {
                    throw new SimulationException(MessageStrings.TreatmentOutlookIsConsumingAProgressEvent);
                }

                AccumulationContext.ApplyPerformanceCurves();

                if (yearIsScheduled && scheduledEvent.IsT1(out var treatment))
                {
                    ApplyTreatment(treatment, year);
                }

                AccumulateBenefit();
                updateRemainingLife?.Invoke();
            }
        }
    }
}
