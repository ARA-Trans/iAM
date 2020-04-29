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

        internal void ApplyPerformanceCurves(SectionContext context)
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

                return curves.Key.IsDecreasingWithDeterioration ? operativeCurves.Min(calculate) : operativeCurves.Max(calculate);
            });

            foreach (var (key, value) in dataUpdates)
            {
                context.SetNumber(key, value);
            }
        }

        private readonly Simulation Simulation;

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments { get; set; }

        private ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; set; }

        private IReadOnlyCollection<SectionContext> SectionContexts { get; set; }

        private void ApplyScheduledTreatments(int year, out ISet<SectionContext> unscheduledContexts)
        {
            unscheduledContexts = SectionContexts.ToHashSet();

            foreach (var sectionContext in SectionContexts)
            {
                if (sectionContext.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment))
                {
                    _ = unscheduledContexts.Remove(sectionContext);

                    if (scheduledTreatment != null)
                    {
                        sectionContext.ApplyTreatment(scheduledTreatment, year, out var totalCost);

                        // TODO: allocate that cost to the right budget(s).
                    }
                }
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

        private IEnumerable<TreatmentOutlook> GetTreatmentOutlooks(int year, IEnumerable<SectionContext> unscheduledContexts)
        {
            foreach (var context in unscheduledContexts)
            {
                ApplyPerformanceCurves(context);

                var remainingLifeCalculatorFactories = Enumerable.ToArray(
                    from limit in Simulation.AnalysisMethod.RemainingLifeLimits
                    where limit.Criterion.Evaluate(context) ?? true
                    group limit.Value by limit.Attribute into attributeLimitValues
                    select new RemainingLifeCalculator.Factory(attributeLimitValues));

                var selectedOutlook = new TreatmentOutlook(this, context, Simulation.DesignatedPassiveTreatment, year, remainingLifeCalculatorFactories);

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
                        var treatmentOutlook = new TreatmentOutlook(this, context, treatment, year, remainingLifeCalculatorFactories);

                        //selectedOutlook = strategy.GetOptimum(selectedOutlook, treatmentOutlook);
                    }
                }

                yield return selectedOutlook;
            }
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
                ApplyScheduledTreatments(currentYear, out var unscheduledContexts);

                var treatmentOutlooks = GetTreatmentOutlooks(currentYear, unscheduledContexts);

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
