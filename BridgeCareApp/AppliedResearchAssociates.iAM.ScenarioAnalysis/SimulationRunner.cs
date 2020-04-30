using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
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

            CommittedProjectsPerSection = Simulation.CommittedProjects.ToLookup(committedProject => committedProject.Section);

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
                ApplyScheduledTreatments(currentYear, out var untreatedContexts);

                var optimalTreatmentOutlooks = GetOptimalTreatmentOutlooks(currentYear, untreatedContexts);

                // calculate network averages and "deficient base" (after committed).

                // either (a) spend as budget permits or (b) spend until targets/deficient met.
                {
                    // When a treatment is chosen, spent, and applied, remove that context from untreatedContexts.
                }

                foreach (var untreatedContext in untreatedContexts)
                {
                    untreatedContext.ApplyTreatment(Simulation.DesignatedPassiveTreatment, currentYear, out var cost);

                    // TODO: Allocate "cost" appropriately.
                }

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

        private ILookup<Section, CommittedProject> CommittedProjectsPerSection;

        private IReadOnlyCollection<SectionContext> SectionContexts;

        private void ApplyScheduledTreatments(int year, out ISet<SectionContext> untreatedContexts)
        {
            untreatedContexts = SectionContexts.ToHashSet();

            foreach (var sectionContext in SectionContexts)
            {
                if (sectionContext.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment))
                {
                    _ = untreatedContexts.Remove(sectionContext);

                    if (scheduledTreatment != null)
                    {
                        sectionContext.ApplyTreatment(scheduledTreatment, year, out var cost);

                        // TODO: allocate that cost to the right budget(s).

                        // filter the treatment's budget list by the budget criterion for each
                        // budget in that list. spend from the remaining budgets.
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

            foreach (var committedProject in CommittedProjectsPerSection[section])
            {
                context.TreatmentSchedule[committedProject.Year] = committedProject;
            }

            return context;
        }

        private IReadOnlyCollection<TreatmentOutlook> GetOptimalTreatmentOutlooks(int year, IEnumerable<SectionContext> unscheduledContexts)
        {
            var treatmentOutlooks = new ConcurrentBag<TreatmentOutlook>();
            void addTreatmentOutlook(SectionContext context)
            {
                context.ApplyPerformanceCurves();

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

                    if (feasibleTreatments.Count > 0)
                    {
                        var remainingLifeCalculatorFactories = Enumerable.ToArray(
                            from limit in Simulation.AnalysisMethod.RemainingLifeLimits
                            where limit.Criterion.Evaluate(context) ?? true
                            group limit.Value by limit.Attribute into attributeLimitValues
                            select new RemainingLifeCalculator.Factory(attributeLimitValues));

                        var selectedOutlook = new TreatmentOutlook(context, Simulation.DesignatedPassiveTreatment, year, remainingLifeCalculatorFactories);

                        foreach (var treatment in feasibleTreatments)
                        {
                            var treatmentOutlook = new TreatmentOutlook(context, treatment, year, remainingLifeCalculatorFactories);

                            //selectedOutlook = strategy.GetOptimum(selectedOutlook, treatmentOutlook);
                        }

                        treatmentOutlooks.Add(selectedOutlook);
                    }
                }
            }

            _ = Parallel.ForEach(unscheduledContexts, addTreatmentOutlook);
            return treatmentOutlooks;
        }
    }
}
