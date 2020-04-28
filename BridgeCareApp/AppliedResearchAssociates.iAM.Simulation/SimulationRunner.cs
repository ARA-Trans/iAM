using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class SimulationRunner
    {
        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public event EventHandler<InformationEventArgs> Information;

        public event EventHandler<WarningEventArgs> Warning;

        public void Run() => CompileSimulation();

        private readonly Simulation Simulation;

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments { get; set; }

        private ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; set; }

        private IReadOnlyCollection<SectionContext> SectionContexts { get; set; }

        private void ApplyPerformanceCurves(SectionContext context)
        {
            var dataUpdates = CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, curves =>
            {
                curves.Channel(
                    curve => curve.Criterion.Evaluate(context),
                    result => result ?? false,
                    result => !result.HasValue,
                    out var applicableCurves,
                    out var defaultCurves);

                var operativeCurves = applicableCurves.Count > 0 ? applicableCurves : defaultCurves;

                if (operativeCurves.Count > 1)
                {
                    OnWarning(new WarningEventArgs("Two or more performance curves are simultaneously valid."));
                }

                double calculate(PerformanceCurve curve) => curve.Equation.Compute(context, Simulation.AnalysisMethod.AgeAttribute);

                switch (curves.Key.Deterioration)
                {
                case Deterioration.Decreasing:
                    return operativeCurves.Min(calculate);

                case Deterioration.Increasing:
                    return operativeCurves.Max(calculate);

                default:
                    throw new InvalidOperationException("Invalid deterioration.");
                }
            });

            foreach (var (key, value) in dataUpdates)
            {
                context.SetNumber(key, value);
            }
        }

        private void CompileSimulation()
        {
            // fill OCI weights.

            ActiveTreatments = Simulation.GetActiveTreatments();

            CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);

            SectionContexts = Simulation.Network.Sections
                .Select(CreateContext)
                .Where(context => Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context) ?? true) // Can jurisdiction criterion be blank?
                .ToArray();

            foreach (var context in SectionContexts)
            {
                //- fill in with ROLL-FORWARD data and committed projects.
            }

            // drop previous simulation.

            // get simulation method.

            // if conditional RSL, load conditional RSL.

            // get simulation attributes.

            RunSimulation();
        }

        private SectionContext CreateContext(Section section)
        {
            var context = new SectionContext(section, Simulation);

            //- fill in with NON-ROLL-FORWARD data (per Gregg's memo, to apply jurisdiction criterion).

            foreach (var calculatedField in Simulation.Network.Explorer.CalculatedFields)
            {
                double calculate() => calculatedField.Calculate(context, Simulation.AnalysisMethod.AgeAttribute);
                context.SetNumber(calculatedField.Name, calculate);
            }

            return context;
        }

        private void DetermineBenefitCost(int year)
        {
            foreach (var context in SectionContexts)
            {
                if (context.YearHasOngoingTreatment(year))
                {
                    continue;
                }

                ApplyPerformanceCurves(context);

                var baselineOutlook = GetTreatmentOutlook(context, Simulation.DesignatedPassiveTreatment, year);
                var selectedOutlook = baselineOutlook;

                if (!context.YearIsWithinShadowForAnyTreatment(year))
                {
                    var feasibleTreatments = ActiveTreatments.ToHashSet();

                    _ = feasibleTreatments.RemoveWhere(treatment => context.YearIsWithinShadowForSameTreatment(year, treatment));
                    _ = feasibleTreatments.RemoveWhere(treatment => treatment.FeasibilityCriterion.Evaluate(context) ?? false);

                    var supersededTreatments = feasibleTreatments
                        .SelectMany(treatment => treatment.Supersessions
                            .Where(supersession => supersession.Criterion.Evaluate(context) ?? false)
                            .Select(supersession => supersession.Treatment));

                    feasibleTreatments.ExceptWith(supersededTreatments);

                    foreach (var treatment in feasibleTreatments)
                    {
                        var treatmentOutlook = GetTreatmentOutlook(context, treatment, year);

                        // switch on optimization strategy. re-assign selectedOutlook if this one's better.
                    }
                }

                // rank outlooks using optimization strategy.
            }

            // later, rank sections based on best outlook, using spending strategy.
        }

        private TreatmentOutlook GetTreatmentOutlook(SectionContext context, Treatment treatment, int initialYear)
        {
            var outlookContext = new SectionContext(context);
            var outlook = new TreatmentOutlook(outlookContext);

            outlook.ApplyTreatment(treatment, initialYear);
            outlook.AccumulateBenefit();

            // need to aggregate for targets, deficient, remaining life, etc.

            const int MAXIMUM_NUMBER_OF_YEARS_OF_OUTLOOK = 100;
            var maximumYearOfOutlook = initialYear + MAXIMUM_NUMBER_OF_YEARS_OF_OUTLOOK;

            foreach (var year in Static.Count(initialYear + 1))
            {
                if (year > maximumYearOfOutlook)
                {
                    throw new SimulationException($"Treatment outlook must terminate naturally within {MAXIMUM_NUMBER_OF_YEARS_OF_OUTLOOK} years.");
                }

                if (context.YearHasOngoingTreatment(year))
                {
                    continue;
                }

                ApplyPerformanceCurves(context);

                if (context.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment) && scheduledTreatment != null)
                {
                    outlook.ApplyTreatment(scheduledTreatment, year);
                }

                var previousCumulativeBenefit = outlook.CumulativeBenefit;

                outlook.AccumulateBenefit();

                if (outlook.CumulativeBenefit == previousCumulativeBenefit && context.TreatmentSchedule.Keys.All(scheduleYear => scheduleYear <= year))
                {
                    break;
                }
            }

            return outlook;
        }

        private void OnInformation(InformationEventArgs e) => Information?.Invoke(this, e);

        private void OnWarning(WarningEventArgs e) => Warning?.Invoke(this, e);

        private void RunSimulation()
        {
            //FillSectionList();

            //FillCommittedProjects();

            //CalculateAreaOfEachSection();

            //UpdateSimulationAttributes();

            //SpendAllCommittedProjects();

            foreach (var currentYear in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                DetermineBenefitCost(currentYear);

                // load & apply committed projects.

                // calculate network averages and "deficient base" (after committed).

                // either (a) spend as budget permits or (b) spend until targets/deficient met.

                // report targets/deficient.
            }

            // create simulation table.

            // "write simulation" for each section.

            // database bulk load for each "simulation table".

            // if multi-year, "solve". (?)
        }
    }
}
