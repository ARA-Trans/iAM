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

            SpendingStrategy = SpendingStrategyBehaviorProvider.GetInstance(Simulation.AnalysisMethod.SpendingStrategy);

            foreach (var currentYear in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                var unhandledContexts = SectionContexts.ToHashSet();

                MoveBudgetsToNextYear();
                ApplyProjectActivities(unhandledContexts, currentYear);
                ApplyPerformanceCurves(unhandledContexts);
                ApplyScheduledTreatments(unhandledContexts, currentYear);
                ConsiderSelectableTreatments(unhandledContexts, currentYear);
                ApplyPassiveTreatment(unhandledContexts, currentYear);

                // TODO: create/finalize another SimulationYear output object.
            }

            // TODO: return sequence of SimulationYear objects.
        }

        internal ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; private set; }

        internal void Inform(string message) => OnInformation(new InformationEventArgs(message));

        internal void Warn(string message) => OnWarning(new WarningEventArgs(message));

        private static readonly IComparer<BudgetPriority> BudgetPriorityComparer = SelectionComparer<BudgetPriority>.Create(priority => priority.PriorityLevel);

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments;

        private IReadOnlyCollection<BudgetContext> BudgetContexts;

        private ILookup<Section, CommittedProject> CommittedProjectsPerSection;

        private IReadOnlyCollection<SectionContext> SectionContexts;

        private SpendingStrategyBehaviorProvider SpendingStrategy;

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

        private void ApplyPerformanceCurves(IEnumerable<SectionContext> contexts)
        {
            foreach (var context in contexts)
            {
                context.ApplyPerformanceCurves();
            }
        }

        private void ApplyProjectActivities(ICollection<SectionContext> contexts, int year)
        {
            foreach (var context in contexts)
            {
                if (context.ProjectSchedule.TryGetValue(year, out var project) && project.IsT2(out var activity))
                {
                    _ = contexts.Remove(context);

                    // TODO: apply the progress (expenses and possibly consequences and further schedulings).
                }
            }
        }

        private void ApplyScheduledTreatments(ICollection<SectionContext> contexts, int year)
        {
            foreach (var context in contexts)
            {
                var treatmentWasApplied =
                    context.ProjectSchedule.TryGetValue(year, out var project) &&
                    project.IsT1(out var treatment) &&
                    ApplyTreatmentIfAffordable(context, treatment, year, budgetContext => budgetContext.CurrentAmount);

                if (treatmentWasApplied)
                {
                    _ = contexts.Remove(context);
                }
            }
        }

        private bool ApplyTreatmentIfAffordable(SectionContext sectionContext, Treatment treatment, int year, Func<BudgetContext, decimal> getAvailableAmount)
        {
            var remainingCost = (decimal)sectionContext.GetCostOfTreatment(treatment);

            var costAllocators = new List<Action>();
            void addCostAllocator(decimal cost, BudgetContext context)
            {
                remainingCost -= cost;
                costAllocators.Add(() => context.AllocateCost(cost));
            }

            foreach (var context in BudgetContexts)
            {
                if (remainingCost <= 0)
                {
                    break;
                }

                if (!treatment.CanUseBudget(context.Budget))
                {
                    continue;
                }

                var budgetConditionIsMet = Simulation.InvestmentPlan.BudgetConditions.Any(condition =>
                    condition.Budget == context.Budget &&
                    (condition.Criterion.Evaluate(sectionContext) ?? true));

                if (!budgetConditionIsMet)
                {
                    continue;
                }

                if (SpendingStrategy.AllowedSpending == AllowedSpending.Unlimited)
                {
                    addCostAllocator(remainingCost, context);
                    break;
                }

                var availableAmount = getAvailableAmount(context);
                if (remainingCost <= availableAmount)
                {
                    addCostAllocator(remainingCost, context);
                    break;
                }

                if (Simulation.AnalysisMethod.UseExtraFundsAcrossBudgets)
                {
                    addCostAllocator(availableAmount, context);
                }
            }

            if (remainingCost < 0)
            {
                throw new InvalidOperationException("Remaining cost is negative.");
            }

            if (remainingCost > 0)
            {
                return false;
            }

            foreach (var costAllocator in costAllocators)
            {
                costAllocator();
            }

            sectionContext.ApplyTreatment(treatment, year);

            return true;
        }

        private void ConsiderSelectableTreatments(ICollection<SectionContext> contexts, int year)
        {
            // TODO: Needs to return or otherwise produce output, e.g. a SimulationYear object.

            var targetConditionActuals = GetTargetConditionActuals(year);
            var deficientConditionActuals = GetDeficientConditionActuals();

            var treatmentOptions = GetTreatmentOptionsInOptimalOrder(contexts, year);

            var shouldConsiderTreatments =
                SpendingStrategy.AllowedSpending != AllowedSpending.None &&
                !SpendingStrategy.GoalsAreMet(targetConditionActuals, deficientConditionActuals);

            if (shouldConsiderTreatments)
            {
                var applicablePriorities = new List<BudgetPriority>();
                foreach (var levelPriorities in Simulation.AnalysisMethod.BudgetPriorities.GroupBy(priority => priority.PriorityLevel))
                {
                    var priority = Option
                        .Of(levelPriorities.SingleOrDefault(p => p.Year == year))
                        .Coalesce(() => levelPriorities.SingleOrDefault(p => p.Year == null));

                    priority.Handle(applicablePriorities.Add, Static.DoNothing);
                }

                applicablePriorities.Sort(BudgetPriorityComparer);

                foreach (var priority in applicablePriorities)
                {
                    foreach (var context in BudgetContexts)
                    {
                        context.Priority = priority;
                    }

                    foreach (var option in treatmentOptions)
                    {
                        if (!contexts.Contains(option.Context) || !(priority.Criterion.Evaluate(option.Context) ?? true))
                        {
                            continue;
                        }

                        if (ApplyTreatmentIfAffordable(option.Context, option.CandidateTreatment, year, context => context.CurrentPrioritizedAmount.Value))
                        {
                            _ = contexts.Remove(option.Context);
                        }

                        targetConditionActuals = GetTargetConditionActuals(year);
                        deficientConditionActuals = GetDeficientConditionActuals();

                        if (SpendingStrategy.GoalsAreMet(targetConditionActuals, deficientConditionActuals))
                        {
                            return;
                        }
                    }
                }
            }
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
                var goalContexts = SectionContexts.Where(context => goal.Criterion.Evaluate(context) ?? true).ToArray();
                var goalArea = goalContexts.Sum(context => context.GetAreaOfSection());

                var deficientContexts = goalContexts.Where(context => goal.LevelIsDeficient(context.GetNumber(goal.Attribute.Name)));
                var deficientArea = deficientContexts.Sum(context => context.GetAreaOfSection());

                var actualDeficientPercentage = deficientArea / goalArea * 100;
                results.Add(new ConditionActual(goal, actualDeficientPercentage));
            }

            return results;
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

        private void MoveBudgetsToNextYear()
        {
            foreach (var context in BudgetContexts)
            {
                context.MoveToNextYear();
            }
        }

        private void OnInformation(InformationEventArgs e) => Information?.Invoke(this, e);

        private void OnWarning(WarningEventArgs e) => Warning?.Invoke(this, e);

        private void ValidateRemainingLifeOptimization()
        {
            if (Simulation.AnalysisMethod.RemainingLifeLimits.Count == 0)
            {
                throw new InvalidOperationException("Simulation is using remaining-life optimization but has no remaining life limits.");
            }
        }
    }
}
