using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            BudgetContexts = Simulation.InvestmentPlan.Budgets.Select(budget => new BudgetContext(budget)).ToArray();

            SectionContexts = Simulation.Network.Sections
                .Select(CreateContext)
                // Can jurisdiction criterion be blank?
                .Where(context => Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context) ?? true)
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

            var spendingStrategy = SpendingStrategyBehaviorProvider.GetInstance(Simulation.AnalysisMethod.SpendingStrategy);

            foreach (var currentYear in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                foreach (var context in BudgetContexts)
                {
                    context.MoveToNextYear();
                }

                var idleContexts = ApplyTreatmentActivities(SectionContexts, currentYear);
                ApplyPerformanceCurves(idleContexts);
                var untreatedContexts = ApplyScheduledTreatments(idleContexts, currentYear);

                var targetConditionActuals = GetTargetConditionActuals(currentYear);
                var deficientConditionActuals = GetDeficientConditionActuals();

                var treatmentOptions = GetTreatmentOptionsInOptimalOrder(untreatedContexts, currentYear);

                if (spendingStrategy.SpendingIsAllowed && !spendingStrategy.GoalsAreMet(targetConditionActuals, deficientConditionActuals))
                {
                    var applicablePriorities = new List<BudgetPriority>();
                    foreach (var levelPriorities in Simulation.AnalysisMethod.BudgetPriorities.GroupBy(priority => priority.PriorityLevel))
                    {
                        var priority = Option
                            .Of(levelPriorities.Where(p => p.Year == currentYear).SingleOrDefault())
                            .Coalesce(levelPriorities.Where(p => p.Year == null).SingleOrDefault);

                        priority.Handle(applicablePriorities.Add, Static.DoNothing);
                    }

                    applicablePriorities.Sort(SelectionComparer<BudgetPriority>.Create(priority => priority.PriorityLevel));

                    foreach (var priority in applicablePriorities)
                    {
                        var applicableOptions = treatmentOptions
                            .Where(option => priority.Criterion.Evaluate(option.Context) ?? true)
                            .ToArray();

                        foreach (var option in applicableOptions)
                        {
                            if (untreatedContexts.Contains(option.Context))
                            {
                                continue;
                            }

                            var remainingCost = (decimal)option.Context.GetCostOfTreatment(option.CandidateTreatment);

                            var costAllocators = new List<Action>();
                            foreach (var context in BudgetContexts)
                            {
                                if (!option.CandidateTreatment.Budgets.Contains(context.Budget))
                                {
                                    continue;
                                }

                                bool conditionMatchesAndIsSatisfied(BudgetCondition condition) =>
                                    condition.Budget == context.Budget &&
                                    (condition.Criterion.Evaluate(option.Context) ?? true);

                                if (!Simulation.InvestmentPlan.BudgetConditions.Any(conditionMatchesAndIsSatisfied))
                                {
                                    continue;
                                }

                                // for non-fixed budget spending, create allocator against this budget and break loop.

                                var budgetAmount = Simulation.InvestmentPlan.GetBudgetAmount(context, currentYear);
                                var prioritizedFraction = priority.BudgetPercentages[context] / 100;
                                var prioritizedBudgetAmount = budgetAmount * prioritizedFraction;

                                // if the remaining cost is covered, break loop.

                                if (Simulation.AnalysisMethod.UseExtraFundsAcrossBudgets)
                                {
                                    // create an extra allocator.
                                    // decrease remaining cost.
                                }
                            }

                            if (remainingCost > 0) // replace with flag set in budgeting loop
                            {
                                foreach (var costAllocator in costAllocators)
                                {
                                    costAllocator();
                                }

                                option.Context.ApplyTreatment(option.CandidateTreatment, currentYear);
                                _ = untreatedContexts.Remove(option.Context);

                                targetConditionActuals = GetTargetConditionActuals(currentYear);
                                deficientConditionActuals = GetDeficientConditionActuals();

                                if (spendingStrategy.GoalsAreMet(targetConditionActuals, deficientConditionActuals))
                                {
                                    goto goalsAreMet; // Convert to "return" when this is in its own function.
                                }
                            }
                        }
                    }
                }

            goalsAreMet:

                ApplyPassiveTreatment(untreatedContexts, currentYear);

                // TODO: create/finalize another SimulationYear output object.
            }

            // TODO: return sequence of SimulationYear objects.
        }

        private void ApplyPassiveTreatment(IEnumerable<SectionContext> contexts, int year)
        {
            foreach (var context in contexts)
            {
                var cost = context.GetCostOfTreatment(Simulation.DesignatedPassiveTreatment);
                if (cost != 0)
                {
                    throw new InvalidOperationException("Cost of passive treatment is non-zero.");
                }

                context.ApplyTreatment(Simulation.DesignatedPassiveTreatment, year);
            }
        }

        internal ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; private set; }

        internal void OnInformation(InformationEventArgs e) => Information?.Invoke(this, e);

        internal void OnWarning(WarningEventArgs e) => Warning?.Invoke(this, e);

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments;

        private ILookup<Section, CommittedProject> CommittedProjectsPerSection;

        private IReadOnlyCollection<SectionContext> SectionContexts;

        private IReadOnlyCollection<BudgetContext> BudgetContexts;

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

        private ISet<SectionContext> ApplyTreatmentActivities(IEnumerable<SectionContext> contexts, int year)
        {
            var idleContexts = contexts.ToHashSet();

            foreach (var context in contexts)
            {
                if (context.ProjectSchedule.TryGetValue(year, out var project) && project.IsT2(out var activity))
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

        private IReadOnlyCollection<TreatmentOption> GetTreatmentOptionsInOptimalOrder(IEnumerable<SectionContext> contexts, int year)
        {
            Func<TreatmentOption, double> objectiveFunction;
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

            var treatmentOptionsBag = new ConcurrentBag<TreatmentOption>();
            void addTreatmentOptions(SectionContext context)
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
                        var option = outlook.GetOptionRelativeToBaseline(baselineOutlook);
                        treatmentOptionsBag.Add(option);
                    }
                }
            }

            _ = Parallel.ForEach(
                contexts.Where(context => !context.YearIsWithinShadowForAnyTreatment(year)),
                addTreatmentOptions);

            var treatmentOptions = treatmentOptionsBag.OrderByDescending(objectiveFunction).ToArray();
            return treatmentOptions;
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
