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
            ActiveTreatments = Simulation.GetActiveTreatments();
            CommittedProjectsPerSection = Simulation.CommittedProjects.ToLookup(committedProject => committedProject.Section);
            CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);
            BudgetContexts = Simulation.InvestmentPlan.Budgets.Select(budget => new BudgetContext(budget)).ToArray();

            SectionContexts = Simulation.Network.Sections
                .AsParallel()
                .Select(CreateContext)
                .Where(context => Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context) ?? true)
                .ToArray();

            _ = Parallel.ForEach(SectionContexts, context =>
            {
                // TODO: fill in with ROLL-FORWARD data and committed projects.
            });

            switch (Simulation.AnalysisMethod.SpendingStrategy)
            {
            case SpendingStrategy.NoSpending:
                AllowedSpending = Spending.None;
                ConditionGoalsAreMet = ValueGetter.Of(false).Delegate;
                break;

            case SpendingStrategy.UnlimitedSpending:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = ValueGetter.Of(false).Delegate;
                break;

            case SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = () => GoalsAreMet(TargetConditionActuals) && GoalsAreMet(DeficientConditionActuals);
                break;

            case SpendingStrategy.UntilTargetConditionGoalsMet:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = () => GoalsAreMet(TargetConditionActuals);
                break;

            case SpendingStrategy.UntilDeficientConditionGoalsMet:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = () => GoalsAreMet(DeficientConditionActuals);
                break;

            case SpendingStrategy.AsBudgetPermits:
                AllowedSpending = Spending.Budgeted;
                ConditionGoalsAreMet = ValueGetter.Of(false).Delegate;
                break;

            default:
                throw SimulationErrors.InvalidSpendingStrategy;
            }

            foreach (var year in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                MoveBudgetsToNextYear();

                var unhandledContexts = ApplyRequiredEvents(year);
                ConsiderSelectableTreatments(unhandledContexts, year);
                ApplyPassiveTreatment(unhandledContexts, year);
                UpdateConditionActuals(year);

                // TODO: create/finalize another SimulationYear output object.
            }

            // TODO: return sequence of SimulationYear objects.
        }

        internal ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; private set; }

        internal void Inform(string message) => OnInformation(new InformationEventArgs(message));

        internal void Warn(string message) => OnWarning(new WarningEventArgs(message));

        private static readonly IComparer<BudgetPriority> BudgetPriorityComparer = SelectionComparer<BudgetPriority>.Create(priority => priority.PriorityLevel);

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments;

        private Spending AllowedSpending;

        private IReadOnlyCollection<BudgetContext> BudgetContexts;

        private ILookup<Section, CommittedProject> CommittedProjectsPerSection;

        private Func<bool> ConditionGoalsAreMet;

        private IReadOnlyCollection<ConditionActual> DeficientConditionActuals;

        private IReadOnlyCollection<SectionContext> SectionContexts;

        private IReadOnlyCollection<ConditionActual> TargetConditionActuals;

        private enum Spending
        {
            None,
            Budgeted,
            Unlimited,
        }

        private static bool GoalsAreMet(IEnumerable<ConditionActual> conditionActuals) => conditionActuals.All(actual => actual.GoalIsMet);

        private void ApplyPassiveTreatment(IEnumerable<SectionContext> contexts, int year)
        {
            _ = Parallel.ForEach(contexts, context =>
            {
                var cost = context.GetCostOfTreatment(Simulation.DesignatedPassiveTreatment);
                if (cost != 0)
                {
                    throw SimulationErrors.CostOfPassiveTreatmentIsNonZero;
                }

                context.ProjectSchedule[year] = Simulation.DesignatedPassiveTreatment;
                context.ApplyTreatment(Simulation.DesignatedPassiveTreatment, year);
            });
        }

        private ICollection<SectionContext> ApplyRequiredEvents(int year)
        {
            var unhandledContexts = new ConcurrentBag<SectionContext>();
            _ = Parallel.ForEach(SectionContexts, applyRequiredEvents);
            return unhandledContexts.ToHashSet();

            void applyRequiredEvents(SectionContext context)
            {
                var yearIsScheduled = context.ProjectSchedule.TryGetValue(year, out var project);

                if (yearIsScheduled && project.IsT2(out var progress))
                {
                    if (!TryToPayForTreatment(context, progress.Treatment, budgetContext => budgetContext.CurrentAmount, progress.Cost))
                    {
                        throw SimulationErrors.CostOfScheduledEventCannotBeCovered;
                    }

                    if (progress.IsComplete)
                    {
                        context.ApplyTreatment(progress.Treatment, year);
                    }
                }
                else
                {
                    context.ApplyPerformanceCurves();

                    if (yearIsScheduled && project.IsT1(out var treatment))
                    {
                        if (!TryToPayForTreatment(context, treatment, budgetContext => budgetContext.CurrentAmount))
                        {
                            throw SimulationErrors.CostOfScheduledEventCannotBeCovered;
                        }

                        context.ApplyTreatment(treatment, year);
                    }
                    else
                    {
                        unhandledContexts.Add(context);
                    }
                }
            }
        }

        private void ConsiderSelectableTreatments(ICollection<SectionContext> sectionContexts, int year)
        {
            // TODO: Needs to return or otherwise produce output, e.g. a SimulationYear object.

            var treatmentOptions = GetTreatmentOptionsInOptimalOrder(sectionContexts, year);

            UpdateConditionActuals(year);

            if (AllowedSpending != Spending.None && !ConditionGoalsAreMet())
            {
                var applicablePriorities = new List<BudgetPriority>();
                foreach (var levelPriorities in Simulation.AnalysisMethod.BudgetPriorities.GroupBy(priority => priority.PriorityLevel))
                {
                    var priority = Option
                        .Of(levelPriorities.SingleOrDefault(p => p.Year == year))
                        .Coalesce(() => levelPriorities.SingleOrDefault(p => p.Year == null));

                    priority.Handle(applicablePriorities.Add, Inaction.Delegate);
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
                        if (!sectionContexts.Contains(option.Context) || !(priority.Criterion.Evaluate(option.Context) ?? true))
                        {
                            continue;
                        }

                        if (TryToPayForTreatment(option.Context, option.CandidateTreatment, context => context.CurrentPrioritizedAmount.Value))
                        {
                            option.Context.ProjectSchedule[year] = option.CandidateTreatment;
                            option.Context.ApplyTreatment(option.CandidateTreatment, year);
                            _ = sectionContexts.Remove(option.Context);
                        }

                        UpdateConditionActuals(year);

                        if (ConditionGoalsAreMet())
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
                var goalContexts = SectionContexts.AsParallel().Where(context => goal.Criterion.Evaluate(context) ?? true).ToArray();
                var goalArea = goalContexts.Sum(context => context.GetAreaOfSection());
                var deficientContexts = goalContexts.Where(context => goal.LevelIsDeficient(context.GetNumber(goal.Attribute.Name)));
                var deficientArea = deficientContexts.Sum(context => context.GetAreaOfSection());
                var deficientPercentageActual = deficientArea / goalArea * 100;

                results.Add(new ConditionActual(goal, deficientPercentageActual));
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

                var goalContexts = SectionContexts.AsParallel().Where(context => goal.Criterion.Evaluate(context) ?? true).ToArray();
                var goalAreaValues = goalContexts.Select(context => context.GetAreaOfSection()).ToArray();
                var averageArea = goalAreaValues.Average();
                var goalAreaWeights = goalAreaValues.Select(area => area / averageArea);
                var averageActual = goalContexts.Zip(goalAreaWeights, (context, weight) => context.GetNumber(goal.Attribute.Name) * weight).Average();

                results.Add(new ConditionActual(goal, averageActual));
            }

            return results;
        }

        private IReadOnlyCollection<TreatmentOption> GetTreatmentOptionsInOptimalOrder(IEnumerable<SectionContext> contexts, int year)
        {
            Func<TreatmentOption, double> objectiveFunction;
            switch (Simulation.AnalysisMethod.OptimizationStrategy)
            {
            case OptimizationStrategy.Benefit:
                objectiveFunction = option => option.Benefit;
                break;

            case OptimizationStrategy.BenefitToCostRatio:
                objectiveFunction = option => option.Benefit / option.CostPerUnitArea;
                break;

            case OptimizationStrategy.RemainingLife:
                ValidateRemainingLifeOptimization();
                objectiveFunction = option => option.RemainingLife.Value;
                break;

            case OptimizationStrategy.RemainingLifeToCostRatio:
                ValidateRemainingLifeOptimization();
                objectiveFunction = option => option.RemainingLife.Value / option.CostPerUnitArea;
                break;

            default:
                throw SimulationErrors.InvalidOptimizationStrategy;
            }

            var treatmentOptionsBag = new ConcurrentBag<TreatmentOption>();
            void addTreatmentOptions(SectionContext context)
            {
                if (context.YearIsWithinShadowForAnyTreatment(year))
                {
                    return;
                }

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

            _ = Parallel.ForEach(contexts, addTreatmentOptions);
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

        private enum CostCoverage
        {
            None,
            Full,
            CashFlow,
        }

        private CostCoverage TryToPayForTreatment(SectionContext sectionContext, Treatment treatment, Func<BudgetContext, decimal> getAvailableAmount, double? atomicCost = null)
        {
            // [TODO] if atomicCost is null && exactly one cashflow criterion is met, use
            // distribution rule with correct cost ceiling. basically, set remaining cost to the
            // first year's amount, then schedule other progress entries.

            var remainingCost = (decimal)(atomicCost ?? sectionContext.GetCostOfTreatment(treatment));

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

                if (AllowedSpending == Spending.Unlimited)
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
                throw SimulationErrors.RemainingCostIsNegative;
            }

            if (remainingCost > 0)
            {
                return false;
            }

            foreach (var costAllocator in costAllocators)
            {
                costAllocator();
            }

            return true;
        }

        private void UpdateConditionActuals(int year)
        {
            TargetConditionActuals = GetTargetConditionActuals(year);
            DeficientConditionActuals = GetDeficientConditionActuals();
        }

        private void ValidateRemainingLifeOptimization()
        {
            if (Simulation.AnalysisMethod.RemainingLifeLimits.Count == 0)
            {
                throw SimulationErrors.RemainingLifeOptimizationHasNoLimits;
            }
        }
    }
}
