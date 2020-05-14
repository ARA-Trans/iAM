using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class SectionContext : CalculateEvaluateArgument
    {
        public SectionContext(SectionHistory history, SimulationRunner simulationRunner)
        {
            History = history ?? throw new ArgumentNullException(nameof(history));
            SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));

            Initialize();
        }

        public SectionContext(SectionContext original) : base(original)
        {
            History = original.History;
            SimulationRunner = original.SimulationRunner;
            LastYearOfShadowForAnyTreatment = original.LastYearOfShadowForAnyTreatment;
            LastYearOfShadowForSameTreatment.CopyFrom(original.LastYearOfShadowForSameTreatment);
            EventSchedule.CopyFrom(original.EventSchedule);
            NumberCache.CopyFrom(original.NumberCache);
        }

        public IDictionary<int, Choice<Treatment, TreatmentProgress>> EventSchedule { get; } = new Dictionary<int, Choice<Treatment, TreatmentProgress>>();

        public SectionHistory History { get; }

        public SimulationRunner SimulationRunner { get; }

        private AnalysisMethod AnalysisMethod => SimulationRunner.Simulation.AnalysisMethod;

        public void ApplyPassiveTreatment(int year)
        {
            var cost = GetCostOfTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment);
            if (cost != 0)
            {
                throw SimulationErrors.CostOfPassiveTreatmentIsNonZero;
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

            foreach (var scheduling in treatment.GetSchedulings())
            {
                var schedulingYear = year + scheduling.OffsetToFutureYear;

                if (EventSchedule.ContainsKey(schedulingYear))
                {
                    throw SimulationErrors.YearIsAlreadyScheduled;
                }

                EventSchedule.Add(schedulingYear, scheduling.Treatment);
            }

            LastYearOfShadowForAnyTreatment = year + treatment.ShadowForAnyTreatment;
            LastYearOfShadowForSameTreatment[treatment.Name] = year + treatment.ShadowForSameTreatment;
        }

        public double GetAreaOfSection() => GetNumber(AnalysisMethod.AreaAttribute.Name);

        public double GetBenefit()
        {
            var rawBenefit = GetNumber(AnalysisMethod.Benefit.Name);
            var benefit = AnalysisMethod.LimitBenefit(rawBenefit);
            var weight = GetNumber(AnalysisMethod.Weighting.Name);
            var weightedBenefit = weight * benefit;
            return weightedBenefit;
        }

        public double GetCostOfTreatment(Treatment treatment) => treatment.GetCost(this, AnalysisMethod.AgeAttribute, AnalysisMethod.ShouldApplyMultipleFeasibleCosts);

        public override double GetNumber(string key)
        {
            if (!NumberCache.TryGetValue(key, out var number))
            {
                number = base.GetNumber(key);
                NumberCache[key] = number;
            }

            return number;
        }

        public void RollForward()
        {
            IEnumerable<int?> getMostRecentYearPerAttribute<T>(IEnumerable<Attribute<T>> attributes) =>
                attributes.Select(attribute => History.GetAttributeHistory(attribute).Keys.AsNullables().Max());

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

                    SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                    SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

                    ApplyPassiveTreatment(year);
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

            foreach (var committedProject in SimulationRunner.CommittedProjectsPerSection[History.Section])
            {
                EventSchedule.Add(committedProject.Year, committedProject);
            }
        }

        private void SetHistoricalValues<T>(int referenceYear, bool useMostRecent, IEnumerable<Attribute<T>> attributes, Action<string, T> setValue)
        {
            foreach (var attribute in attributes)
            {
                var attributeHistory = History.GetAttributeHistory(attribute);
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
