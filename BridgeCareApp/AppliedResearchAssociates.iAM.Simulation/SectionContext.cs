using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class SectionContext : CalculateEvaluateArgument
    {
        // An instance of this type is conditionally thread-safe, i.e. multiple threads using only
        // "get" operations is safe, and multiple threads using only "set" operations is safe, but
        // multiple threads concurrently using both "get" and "set" operations could produce
        // inconsistent "get" results.

        public SectionContext(Section section, Simulation simulation)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        }

        public SectionContext(SectionContext original) : base(original)
        {
            Section = original.Section;
            Simulation = original.Simulation;
            LastYearOfShadowForAnyTreatment = original.LastYearOfShadowForAnyTreatment;
            LastYearsOfShadowForSameTreatment.CopyFrom(original.LastYearsOfShadowForSameTreatment);
            TreatmentSchedule.CopyFrom(original.TreatmentSchedule);
            NumberCache.CopyFrom(original.NumberCache);
        }

        public Section Section { get; }

        public Simulation Simulation { get; }

        public IDictionary<int, Treatment> TreatmentSchedule { get; } = new Dictionary<int, Treatment>();

        public void ApplyTreatment(Treatment treatment, int year, out double costPerUnitArea)
        {
            // TODO: Needs to handle cashflow/split treatments/projects.

            TreatmentSchedule[year] = treatment;

            var area = GetNumber(Simulation.AnalysisMethod.AreaAttribute.Name);
            var totalCost = treatment.CostEquations
                .Where(costEquation => costEquation.Criterion.Evaluate(this) ?? true)
                .Sum(costEquation => costEquation.Equation.Compute(this, Simulation.AnalysisMethod.AgeAttribute));

            costPerUnitArea = totalCost / area;

            treatment.Consequences.Channel(
                consequence => consequence.Criterion.Evaluate(this),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableConsequences,
                out var defaultConsequences);

            var operativeConsequences = applicableConsequences.Count > 0 ? applicableConsequences : defaultConsequences;

            operativeConsequences = operativeConsequences
                .GroupBy(consequence => consequence.Attribute)
                .Select(group => group.Single()) // It's (currently) an error when one attribute has multiple valid consequences.
                .ToArray();

            var consequenceActions = operativeConsequences
                .Select(consequence => consequence.GetRecalculator(this, Simulation.AnalysisMethod.AgeAttribute))
                .ToArray();

            foreach (var consequenceAction in consequenceActions)
            {
                consequenceAction();
            }

            foreach (var scheduling in treatment.Schedulings)
            {
                var schedulingYear = year + scheduling.OffsetToFutureYear;
                TreatmentSchedule.Add(schedulingYear, scheduling.Treatment);

                // Unclear what is correct when two treatments write into the same schedule slot. Or
                // when one scheduled treatment is slotted to begin before the end of another.
            }

            LastYearOfShadowForAnyTreatment = year + treatment.ShadowForAnyTreatment;
            LastYearsOfShadowForSameTreatment[treatment] = year;
        }

        public double GetBenefit()
        {
            var rawBenefit = GetNumber(Simulation.AnalysisMethod.Benefit.Name);
            var benefit = Simulation.AnalysisMethod.LimitBenefit(rawBenefit);
            var weight = GetNumber(Simulation.AnalysisMethod.Weighting.Name);
            var weightedBenefit = weight * benefit;
            return weightedBenefit;
        }

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

        public bool YearHasOngoingTreatment(int year) => TreatmentSchedule.TryGetValue(year, out var treatment) && treatment == null;

        public bool YearIsWithinShadowForAnyTreatment(int year) => year <= LastYearOfShadowForAnyTreatment;

        public bool YearIsWithinShadowForSameTreatment(int year, Treatment treatment) => LastYearsOfShadowForSameTreatment.TryGetValue(treatment, out var lastYearOfShadow) ? year <= lastYearOfShadow : false;

        private readonly IDictionary<Treatment, int> LastYearsOfShadowForSameTreatment = new Dictionary<Treatment, int>();

        private readonly IDictionary<string, double> NumberCache = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        private int? LastYearOfShadowForAnyTreatment;
    }
}
