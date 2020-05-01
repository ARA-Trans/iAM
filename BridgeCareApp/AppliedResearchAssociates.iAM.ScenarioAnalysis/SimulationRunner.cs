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
                // TODO: fill in with ROLL-FORWARD data and committed projects.
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
                var idleContexts = ApplyProgress(SectionContexts, currentYear);
                ApplyPerformanceCurves(idleContexts);
                var untreatedContexts = ApplyScheduledTreatments(idleContexts, currentYear);

                // calculate network averages and "deficient base" (after committed).
                bool targetConditionGoalsMet;
                bool deficientConditionGoalsMet;

                if (Simulation.AnalysisMethod.SpendingStrategy != SpendingStrategy.NoSpending)
                {
                    // TODO: Don't optimize if target/deficient strategy and it's already satisfied.

                    var optimizedOrderOfTreatments = GetOptimizedOrderOfTreatments(untreatedContexts, currentYear);

                    {
                        // When a treatment is chosen, spent, and applied, remove that context from untreatedContexts.
                    }
                }

                foreach (var untreatedContext in untreatedContexts)
                {
                    untreatedContext.ApplyTreatment(Simulation.DesignatedPassiveTreatment, currentYear, out var cost);

                    if (cost != 0)
                    {
                        throw new NotSupportedException("Cost of passive treatment is non-zero.");
                    }
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

        private void ApplyPerformanceCurves(IEnumerable<SectionContext> idleContexts)
        {
            foreach (var context in idleContexts)
            {
                context.ApplyPerformanceCurves();
            }
        }

        private ISet<SectionContext> ApplyProgress(IEnumerable<SectionContext> allContexts, int year)
        {
            var idleContexts = allContexts.ToHashSet();

            foreach (var context in allContexts)
            {
                if (context.ProgressSchedule.TryGetValue(year, out var treatmentProgress))
                {
                    _ = idleContexts.Remove(context);

                    // TODO: apply the progress (expenses and possibly consequences).
                }
            }

            return idleContexts;
        }

        private ISet<SectionContext> ApplyScheduledTreatments(IEnumerable<SectionContext> idleContexts, int year)
        {
            var untreatedContexts = idleContexts.ToHashSet();

            foreach (var context in idleContexts)
            {
                if (context.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment))
                {
                    _ = untreatedContexts.Remove(context);

                    // TODO: filter the treatment's budget list by the budget criterion for each budget in
                    // that list. spend from the remaining budgets.

                    context.ApplyTreatment(scheduledTreatment, year, out var cost);

                    // TODO: allocate that cost to the right budget(s).
                }
            }

            return untreatedContexts;
        }

        private SectionContext CreateContext(Section section)
        {
            var context = new SectionContext(section, this);

            // TODO: fill in with NON-ROLL-FORWARD data (per Gregg's memo, to apply jurisdiction criterion).

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

        private IReadOnlyCollection<TreatmentOutlookSummary> GetOptimizedOrderOfTreatments(IEnumerable<SectionContext> unscheduledContexts, int year)
        {
            Func<TreatmentOutlookSummary, double> objectiveFunction;
            switch (Simulation.AnalysisMethod.OptimizationStrategy)
            {
            case OptimizationStrategy.Benefit:
                objectiveFunction = summary => summary.Benefit;
                break;

            case OptimizationStrategy.BenefitPerCost:
                objectiveFunction = summary => summary.Benefit / summary.CostPerUnitArea;
                break;

            case OptimizationStrategy.RemainingLife:
                ValidateRemainingLifeOptimization();
                objectiveFunction = summary => summary.RemainingLife.Value;
                break;

            case OptimizationStrategy.RemainingLifePerCost:
                ValidateRemainingLifeOptimization();
                objectiveFunction = summary => summary.RemainingLife.Value / summary.CostPerUnitArea;
                break;

            default:
                throw new InvalidOperationException("Invalid optimization strategy.");
            }

            var outlookSummariesBag = new ConcurrentBag<TreatmentOutlookSummary>();
            void addOutlookSummaries(SectionContext context)
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

                        var baselineOutlook = new TreatmentOutlook(context, Simulation.DesignatedPassiveTreatment, year, remainingLifeCalculatorFactories);

                        foreach (var treatment in feasibleTreatments)
                        {
                            var outlook = new TreatmentOutlook(context, treatment, year, remainingLifeCalculatorFactories);
                            var summary = outlook.GetSummaryRelativeToBaseline(baselineOutlook);
                            outlookSummariesBag.Add(summary);
                        }
                    }
                }
            }

            _ = Parallel.ForEach(unscheduledContexts, addOutlookSummaries);

            var outlookSummaries = outlookSummariesBag.OrderByDescending(objectiveFunction).ToList();
            return outlookSummaries;
        }

        private void ValidateRemainingLifeOptimization()
        {
            if (Simulation.AnalysisMethod.RemainingLifeLimits.Count == 0)
            {
                throw new InvalidOperationException("Simulation is using remaining-life optimization but has no remaining life limits.");
            }
        }
    }
}
