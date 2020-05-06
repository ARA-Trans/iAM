using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class SectionContext : CalculateEvaluateArgument
    {
        public SectionContext(Section section, SimulationRunner simulationRunner)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));
        }

        public SectionContext(SectionContext original) : base(original)
        {
            Section = original.Section;
            SimulationRunner = original.SimulationRunner;
            LastYearOfShadowForAnyTreatment = original.LastYearOfShadowForAnyTreatment;
            LastYearOfShadowForSameTreatment.CopyFrom(original.LastYearOfShadowForSameTreatment);
            ProjectSchedule.CopyFrom(original.ProjectSchedule);
            NumberCache.CopyFrom(original.NumberCache);
        }

        public Section Section { get; }

        public SimulationRunner SimulationRunner { get; }

        public IDictionary<int, Choice<Treatment, TreatmentProgress>> ProjectSchedule { get; } = new Dictionary<int, Choice<Treatment, TreatmentProgress>>();

        private AnalysisMethod AnalysisMethod => SimulationRunner.Simulation.AnalysisMethod;

        public void ApplyPerformanceCurves()
        {
            var dataUpdates = SimulationRunner.CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, curves =>
            {
                curves.Channel(
                    curve => curve.Criterion.Evaluate(this),
                    result => result ?? false,
                    result => !result.HasValue,
                    out var applicableCurves,
                    out var defaultCurves);

                var operativeCurves = applicableCurves.Count > 0 ? applicableCurves : defaultCurves;

                if (operativeCurves.Count > 1)
                {
                    SimulationRunner.OnWarning(new WarningEventArgs("Two or more performance curves are simultaneously valid."));
                }

                double calculate(PerformanceCurve curve) => curve.Equation.Compute(this, AnalysisMethod.AgeAttribute);

                return curves.Key.IsDecreasingWithDeterioration ? operativeCurves.Min(calculate) : operativeCurves.Max(calculate);
            });

            foreach (var (key, value) in dataUpdates)
            {
                SetNumber(key, value);
            }
        }

        public void ApplyTreatment(Treatment treatment, int year)
        {
            ProjectSchedule[year] = treatment;

            // TODO: Handle cash-flow/split treatments.

            var consequenceActions = treatment.GetConsequenceActions(this, AnalysisMethod.AgeAttribute);
            foreach (var consequenceAction in consequenceActions)
            {
                consequenceAction();
            }

            foreach (var scheduling in treatment.GetSchedulings())
            {
                var schedulingYear = year + scheduling.OffsetToFutureYear;

                if (ProjectSchedule.ContainsKey(schedulingYear))
                {
                    // [REVIEW] Unclear what is correct when two treatments write into the same
                    // schedule slot. Or when one scheduled treatment is slotted to begin before the
                    // end of another. According to legacy code, both are eliminated! wtf...
                    throw new InvalidOperationException("Year is already scheduled for other activity.");
                }

                ProjectSchedule.Add(schedulingYear, scheduling.Treatment);
            }

            LastYearOfShadowForAnyTreatment = year + treatment.ShadowForAnyTreatment;
            LastYearOfShadowForSameTreatment[treatment.Name] = year + treatment.ShadowForSameTreatment;
        }

        public double GetArea() => GetNumber(AnalysisMethod.AreaAttribute.Name);

        public double GetBenefit()
        {
            var rawBenefit = GetNumber(AnalysisMethod.Benefit.Name);
            var benefit = AnalysisMethod.LimitBenefit(rawBenefit);
            var weight = GetNumber(AnalysisMethod.Weighting.Name);
            var weightedBenefit = weight * benefit;
            return weightedBenefit;
        }

        public double GetCostOfTreatment(Treatment treatment) => treatment.GetCost(this, AnalysisMethod.AgeAttribute);

        public override double GetNumber(string key)
        {
            if (!NumberCache.TryGetValue(key, out var number))
            {
                number = base.GetNumber(key);
                NumberCache[key] = number;
            }

            return number;
        }

        public override void SetNumber(string key, double value)
        {
            NumberCache.Clear();
            base.SetNumber(key, value);
        }

        public override void SetNumber(string key, Func<double> getValue)
        {
            NumberCache.Clear();
            base.SetNumber(key, getValue);
        }

        public override void SetText(string key, string value)
        {
            NumberCache.Clear();
            base.SetText(key, value);
        }

        public bool YearIsWithinShadowForAnyTreatment(int year) => year <= LastYearOfShadowForAnyTreatment;

        public bool YearIsWithinShadowForSameTreatment(int year, Treatment treatment) => LastYearOfShadowForSameTreatment.TryGetValue(treatment.Name, out var lastYearOfShadow) ? year <= lastYearOfShadow : false;

        private readonly IDictionary<string, int> LastYearOfShadowForSameTreatment = new Dictionary<string, int>();

        private readonly IDictionary<string, double> NumberCache = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        private int? LastYearOfShadowForAnyTreatment;
    }
}
