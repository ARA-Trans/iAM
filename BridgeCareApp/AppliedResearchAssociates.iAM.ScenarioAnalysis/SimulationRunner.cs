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

            var spendingStrategyConductor = SpendingStrategyConductor.GetInstance(Simulation.AnalysisMethod.SpendingStrategy);

            foreach (var currentYear in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                var idleContexts = ApplyTreatmentProgress(SectionContexts, currentYear);
                ApplyPerformanceCurves(idleContexts);
                var untreatedContexts = ApplyScheduledTreatments(idleContexts, currentYear);

                var targetConditionActuals = GetTargetConditionActuals(currentYear);
                var deficientConditionActuals = GetDeficientConditionActuals();

                var optimizedOrderOfTreatments = GetOptimizedOrderOfTreatments(untreatedContexts, currentYear);

                if (spendingStrategyConductor.SpendingIsAllowed)
                {
                    foreach (var summary in optimizedOrderOfTreatments)
                    {
                        if (spendingStrategyConductor.GoalsAreMet(targetConditionActuals, deficientConditionActuals))
                        {
                            break;
                        }

                        if (untreatedContexts.Contains(summary.Context))
                        {
                            continue;
                        }

                        var cost = summary.Context.GetCostOfTreatment(summary.CandidateTreatment);
                        // determine budget(s) to spend from.

                        if (!spendingStrategyConductor.BudgetIsSufficient())
                        {
                            continue;
                        }

                        // allocate cost to the right budget(s).

                        summary.Context.ApplyTreatment(summary.CandidateTreatment, currentYear);
                        _ = untreatedContexts.Remove(summary.Context);

                        targetConditionActuals = GetTargetConditionActuals(currentYear);
                        deficientConditionActuals = GetDeficientConditionActuals();
                    }
                }

                foreach (var untreatedContext in untreatedContexts)
                {
                    var cost = untreatedContext.GetCostOfTreatment(Simulation.DesignatedPassiveTreatment);
                    if (cost != 0)
                    {
                        throw new NotSupportedException("Cost of passive treatment is non-zero.");
                    }

                    untreatedContext.ApplyTreatment(Simulation.DesignatedPassiveTreatment, currentYear);
                }

                // "Calculate target condition goal statuses"

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

        private void ApplyPerformanceCurves(IEnumerable<SectionContext> contexts)
        {
            foreach (var context in contexts)
            {
                context.ApplyPerformanceCurves();
            }
        }

        private ISet<SectionContext> ApplyScheduledTreatments(IEnumerable<SectionContext> contexts, int year)
        {
            var untreatedContexts = contexts.ToHashSet();

            foreach (var context in contexts)
            {
                if (context.ProjectSchedule.TryGetValue(year, out var project) && project.IsT1(out var treatment))
                {
                    _ = untreatedContexts.Remove(context);

                    var cost = context.GetCostOfTreatment(treatment);

                    // TODO: filter the treatment's budget list by the budget criterion for each budget in
                    // that list. spend from the remaining budgets.

                    // TODO: allocate that cost to the right budget(s).

                    context.ApplyTreatment(treatment, year);
                }
            }

            return untreatedContexts;
        }

        private ISet<SectionContext> ApplyTreatmentProgress(IEnumerable<SectionContext> contexts, int year)
        {
            var idleContexts = contexts.ToHashSet();

            foreach (var context in contexts)
            {
                if (context.ProjectSchedule.TryGetValue(year, out var project) && project.IsT2(out var progress))
                {
                    _ = idleContexts.Remove(context);

                    // TODO: apply the progress (expenses and possibly consequences and further schedulings).
                }
            }

            return idleContexts;
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
                context.ProjectSchedule[committedProject.Year] = committedProject;
            }

            return context;
        }

        private IReadOnlyCollection<ConditionActual> GetDeficientConditionActuals()
        {
            var results = new List<ConditionActual>();

            foreach (var goal in Simulation.AnalysisMethod.DeficientConditionGoals)
            {
                var actualLevels = SectionContexts
                    .Where(context => goal.Criterion.Evaluate(context) ?? true)
                    .Select(context => context.GetNumber(goal.Attribute.Name))
                    .ToArray();

                var numberOfDeficientLevels = goal.Attribute.IsDecreasingWithDeterioration
                    ? actualLevels.Count(level => level < goal.DeficientLevel)
                    : actualLevels.Count(level => level > goal.DeficientLevel);

                var actualDeficientPercentage = (double)numberOfDeficientLevels / actualLevels.Length * 100;

                results.Add(new ConditionActual(goal, actualDeficientPercentage));
            }

            return results;
        }

        private IReadOnlyCollection<TreatmentOutlookSummary> GetOptimizedOrderOfTreatments(IEnumerable<SectionContext> contexts, int year)
        {
            Func<TreatmentOutlookSummary, double> objectiveFunction;
            switch (Simulation.AnalysisMethod.OptimizationStrategy)
            {
            case OptimizationStrategy.Benefit:
                objectiveFunction = summary => summary.Benefit;
                break;

            case OptimizationStrategy.BenefitToCostRatio:
                objectiveFunction = summary => summary.Benefit / summary.CostPerUnitArea;
                break;

            case OptimizationStrategy.RemainingLife:
                ValidateRemainingLifeOptimization();
                objectiveFunction = summary => summary.RemainingLife.Value;
                break;

            case OptimizationStrategy.RemainingLifeToCostRatio:
                ValidateRemainingLifeOptimization();
                objectiveFunction = summary => summary.RemainingLife.Value / summary.CostPerUnitArea;
                break;

            default:
                throw new InvalidOperationException("Invalid optimization strategy.");
            }

            var outlookSummariesBag = new ConcurrentBag<TreatmentOutlookSummary>();
            void addOutlookSummaries(SectionContext context)
            {
                var feasibleTreatments = ActiveTreatments.ToHashSet();

                _ = feasibleTreatments.RemoveWhere(treatment => context.YearIsWithinShadowForSameTreatment(year, treatment));
                _ = feasibleTreatments.RemoveWhere(treatment => treatment.FeasibilityCriterion.Evaluate(context) ?? false);

                var supersededTreatments = Enumerable.ToArray(
                    from treatment in feasibleTreatments
                    from supersession in treatment.Supersessions
                    where supersession.Criterion.Evaluate(context) ?? true
                    select supersession.Treatment);

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

            _ = Parallel.ForEach(
                contexts.Where(context => !context.YearIsWithinShadowForAnyTreatment(year)),
                addOutlookSummaries);

            var outlookSummaries = outlookSummariesBag.OrderByDescending(objectiveFunction).ToList();
            return outlookSummaries;
        }

        private IReadOnlyCollection<ConditionActual> GetTargetConditionActuals(int year)
        {
            var results = new List<ConditionActual>();

            foreach (var goal in Simulation.AnalysisMethod.TargetConditionGoals)
            {
                if (goal.Year.HasValue && goal.Year.Value != year)
                {
                    continue;
                }

                var actual = SectionContexts
                    .Where(context => goal.Criterion.Evaluate(context) ?? true)
                    .Average(context => context.GetNumber(goal.Attribute.Name));

                results.Add(new ConditionActual(goal, actual));
            }

            return results;
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
