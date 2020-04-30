using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class SimulationRunner
    {
        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public event EventHandler<InformationEventArgs> Information;

        public event EventHandler<WarningEventArgs> Warning;

        public Simulation Simulation { get; }

        public void Run()
        {
            // --- CompileSimulation

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

            // --- RunSimulation

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

        internal ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; private set; }

        internal void OnInformation(InformationEventArgs e) => Information?.Invoke(this, e);

        internal void OnWarning(WarningEventArgs e) => Warning?.Invoke(this, e);

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments;

        private IReadOnlyCollection<SectionContext> SectionContexts;

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

        private SectionContext CreateContext(Section section)
        {
            var context = new SectionContext(section, this);

            //- fill in with NON-ROLL-FORWARD data (per Gregg's memo, to apply jurisdiction criterion).

            foreach (var calculatedField in Simulation.Network.Explorer.CalculatedFields)
            {
                double calculate() => calculatedField.Calculate(context, Simulation.AnalysisMethod.AgeAttribute);
                context.SetNumber(calculatedField.Name, calculate);
            }

            return context;
        }

        private IReadOnlyCollection<TreatmentOutlook> GetTreatmentOutlooks(int year, IEnumerable<SectionContext> unscheduledContexts)
        {
            var treatmentOutlooks = new ConcurrentBag<TreatmentOutlook>();
            void addTreatmentOutlook(SectionContext context)
            {
                context.ApplyPerformanceCurves();

                var remainingLifeCalculatorFactories = Enumerable.ToArray(
                    from limit in Simulation.AnalysisMethod.RemainingLifeLimits
                    where limit.Criterion.Evaluate(context) ?? true
                    group limit.Value by limit.Attribute into attributeLimitValues
                    select new RemainingLifeCalculator.Factory(attributeLimitValues));

                var selectedOutlook = new TreatmentOutlook(context, Simulation.DesignatedPassiveTreatment, year, remainingLifeCalculatorFactories);

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
                        var treatmentOutlook = new TreatmentOutlook(context, treatment, year, remainingLifeCalculatorFactories);

                        //selectedOutlook = strategy.GetOptimum(selectedOutlook, treatmentOutlook);
                    }
                }

                treatmentOutlooks.Add(selectedOutlook);
            }

            _ = Parallel.ForEach(unscheduledContexts, addTreatmentOutlook);
            return treatmentOutlooks;
        }
    }
}
