using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Analysis
{
    internal sealed class SectionContext : CalculateEvaluateArgument
    {
        public SectionContext(Section section, SimulationRunner simulationRunner)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));

            Initialize();
        }

        public SectionContext(SectionContext original) : base(original)
        {
            Section = original.Section;
            SimulationRunner = original.SimulationRunner;
            LastYearOfShadowForAnyTreatment = original.LastYearOfShadowForAnyTreatment;
            LastYearOfShadowForSameTreatment.CopyFrom(original.LastYearOfShadowForSameTreatment);
            EventSchedule.CopyFrom(original.EventSchedule);
            NumberCache.CopyFrom(original.NumberCache);
        }

        public SectionDetail Detail { get; private set; }

        public IDictionary<int, Choice<Treatment, TreatmentProgress>> EventSchedule { get; } = new Dictionary<int, Choice<Treatment, TreatmentProgress>>();

        public Section Section { get; }

        public SimulationRunner SimulationRunner { get; }

        private AnalysisMethod AnalysisMethod => SimulationRunner.Simulation.AnalysisMethod;

        public void ApplyPassiveTreatment(int year)
        {
            var cost = GetCostOfTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment);
            if (cost != 0)
            {
                throw new SimulationException(MessageStrings.CostOfPassiveTreatmentIsNonZero);
            }

            ApplyTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment, year);
        }

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
                    SimulationRunner.Warn("Two or more performance curves are simultaneously valid.");
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
            var consequenceActions = treatment.GetConsequenceActions(this, AnalysisMethod.AgeAttribute);
            foreach (var consequenceAction in consequenceActions)
            {
                consequenceAction();
            }

            foreach (var scheduling in treatment.Schedulings)
            {
                var schedulingYear = year + scheduling.OffsetToFutureYear;

                if (EventSchedule.ContainsKey(schedulingYear))
                {
                    throw new SimulationException(MessageStrings.YearIsAlreadyScheduled);
                }

                EventSchedule.Add(schedulingYear, scheduling.Treatment);
            }

            LastYearOfShadowForAnyTreatment = year + treatment.ShadowForAnyTreatment;
            LastYearOfShadowForSameTreatment[treatment.Name] = year + treatment.ShadowForSameTreatment;

            Detail.NameOfAppliedTreatment = treatment.Name;
        }

        public void CopyAttributeValuesToDetail()
        {
            foreach (var attribute in SimulationRunner.Simulation.Network.Explorer.NumberAttributes)
            {
                Detail.ValuePerNumberAttribute.Add(attribute.Name, GetNumber(attribute.Name));
            }

            foreach (var attribute in SimulationRunner.Simulation.Network.Explorer.TextAttributes)
            {
                Detail.ValuePerTextAttribute.Add(attribute.Name, GetText(attribute.Name));
            }
        }

        public double GetAreaOfSection() => GetNumber(AnalysisMethod.AreaAttribute.Name);

        public double GetBenefit()
        {
            var rawBenefit = GetNumber(AnalysisMethod.Benefit.Attribute.Name);
            var benefit = AnalysisMethod.Benefit.LimitBenefit(rawBenefit);

            if (AnalysisMethod.Weighting != null)
            {
                var weight = GetNumber(AnalysisMethod.Weighting.Name);
                benefit *= weight;
            }

            return benefit;
        }

        public double GetCostOfTreatment(Treatment treatment) => treatment.GetCost(this, AnalysisMethod.AgeAttribute, AnalysisMethod.ShouldApplyMultipleFeasibleCosts);

        public override double GetNumber(string key)
        {
            if (!NumberCache.TryGetValue(key, out var number))
            {
                number = base.GetNumber(key);

                if (SimulationRunner.NumberAttributeByName.TryGetValue(key, out var attribute))
                {
                    if (attribute.Minimum.HasValue)
                    {
                        number = Math.Max(number, attribute.Minimum.Value);
                    }

                    if (attribute.Maximum.HasValue)
                    {
                        number = Math.Min(number, attribute.Maximum.Value);
                    }
                }

                NumberCache[key] = number;
            }

            return number;
        }

        public void ResetDetail() => Detail = new SectionDetail(Section.Name, Section.Facility.Name);

        public void RollForward()
        {
            IEnumerable<int?> getMostRecentYearPerAttribute<T>(IEnumerable<Attribute<T>> attributes) =>
                attributes.Select(attribute => Section.GetAttributeHistory(attribute).Keys.AsNullables().Max());

            var earliestYearOfMostRecentValue = Enumerable.Concat(
                getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.NumberAttributes),
                getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.TextAttributes)
                ).Min();

            if (earliestYearOfMostRecentValue.HasValue)
            {
                SetHistoricalValues(earliestYearOfMostRecentValue.Value, true, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                SetHistoricalValues(earliestYearOfMostRecentValue.Value, true, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

                var startYear = earliestYearOfMostRecentValue.Value + 1;
                foreach (var year in Enumerable.Range(startYear, SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod - startYear))
                {
                    ApplyPerformanceCurves();
                    ApplyPassiveTreatment(year);

                    SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                    SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);
                }
            }
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

        private void Initialize()
        {
            var yearBeforeAnalysisPeriod = SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod - 1;

            SetHistoricalValues(yearBeforeAnalysisPeriod, true, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
            SetHistoricalValues(yearBeforeAnalysisPeriod, true, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

            foreach (var calculatedField in SimulationRunner.Simulation.Network.Explorer.CalculatedFields)
            {
                double calculate() => calculatedField.Calculate(this, AnalysisMethod.AgeAttribute);
                SetNumber(calculatedField.Name, calculate);
            }

            foreach (var committedProject in SimulationRunner.CommittedProjectsPerSection[Section])
            {
                EventSchedule.Add(committedProject.Year, committedProject);
            }
        }

        private void SetHistoricalValues<T>(int referenceYear, bool useMostRecent, IEnumerable<Attribute<T>> attributes, Action<string, T> setValue)
        {
            foreach (var attribute in attributes)
            {
                var attributeHistory = Section.GetAttributeHistory(attribute);
                if (attributeHistory.TryGetValue(referenceYear, out var value))
                {
                    setValue(attribute.Name, value);
                }
                else if (useMostRecent)
                {
                    var mostRecentYear = attributeHistory.Keys.Where(year => year < referenceYear).AsNullables().Max();
                    if (mostRecentYear.HasValue)
                    {
                        setValue(attribute.Name, attributeHistory[mostRecentYear.Value]);
                    }
                    else
                    {
                        setValue(attribute.Name, attribute.DefaultValue);
                    }
                }
            }
        }
    }
}
