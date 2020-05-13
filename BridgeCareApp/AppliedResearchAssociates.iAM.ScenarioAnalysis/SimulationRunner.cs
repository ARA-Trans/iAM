﻿using System;
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

        public IEnumerable<SimulationYear> Run()
        {
            ActiveTreatments = Simulation.GetActiveTreatments();
            BudgetContexts = Simulation.InvestmentPlan.Budgets.Select(budget => new BudgetContext(budget)).ToArray();
            CommittedProjectsPerSection = Simulation.CommittedProjects.ToLookup(committedProject => committedProject.Section);
            CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);

            SectionContexts = Simulation.Network.Sections
                .AsParallel()
                .Select(CreateSectionContext)
                .Where(context => Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context) ?? true)
                .ToArray();

            _ = Parallel.ForEach(SectionContexts, context =>
            {
                // TODO: "Roll forward". Applying performance curves and passive treatment.
                //- find earliest year with values. (if none, take all defaults and stop.) set all those values, use defaults for the rest.
                //- starting from next year: apply PCs, set any existing year-values, apply passive treatment. stop before "first year of analysis period".
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
                yield return null;
            }
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

        private enum CostCoverage
        {
            None,
            Full,
            CashFlow,
        }

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
                    var costCoverage = TryToPayForTreatment(
                        context,
                        progress.Treatment,
                        year,
                        budgetContext => budgetContext.CurrentAmount,
                        progress.Cost);

                    if (costCoverage == CostCoverage.None)
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
                        var costCoverage = TryToPayForTreatment(
                            context,
                            treatment,
                            year,
                            budgetContext => budgetContext.CurrentAmount);

                        if (costCoverage == CostCoverage.None)
                        {
                            throw SimulationErrors.CostOfScheduledEventCannotBeCovered;
                        }

                        if (costCoverage == CostCoverage.Full)
                        {
                            context.ApplyTreatment(treatment, year);
                        }
                    }
                    else
                    {
                        unhandledContexts.Add(context);
                    }
                }
            }
        }

        private void ConsiderSelectableTreatments(ICollection<SectionContext> unhandledContexts, int year)
        {
            var treatmentOptions = GetTreatmentOptionsInOptimalOrder(unhandledContexts, year);

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
                        if (unhandledContexts.Contains(option.Context) && (priority.Criterion.Evaluate(option.Context) ?? true))
                        {
                            var costCoverage = TryToPayForTreatment(
                                option.Context,
                                option.CandidateTreatment,
                                year,
                                context => context.CurrentPrioritizedAmount.Value);

                            if (costCoverage != CostCoverage.None)
                            {
                                option.Context.ProjectSchedule[year] = option.CandidateTreatment;
                                _ = unhandledContexts.Remove(option.Context);

                                if (costCoverage == CostCoverage.Full)
                                {
                                    option.Context.ApplyTreatment(option.CandidateTreatment, year);
                                    UpdateConditionActuals(year);

                                    if (ConditionGoalsAreMet())
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private SectionContext CreateSectionContext(SectionHistory sectionHistory)
        {
            var context = new SectionContext(sectionHistory.Section, this);

            foreach (var (attribute, history) in sectionHistory.HistoryPerNumberAttribute)
            {
                FillSectionContext(attribute, history, context.SetNumber);
            }

            foreach (var (attribute, history) in sectionHistory.HistoryPerTextAttribute)
            {
                FillSectionContext(attribute, history, context.SetText);
            }

            foreach (var calculatedField in Simulation.Network.Explorer.CalculatedFields)
            {
                double calculate() => calculatedField.Calculate(context, Simulation.AnalysisMethod.AgeAttribute);
                context.SetNumber(calculatedField.Name, calculate);
            }

            foreach (var committedProject in CommittedProjectsPerSection[context.Section])
            {
                context.ProjectSchedule[committedProject.Year] = committedProject;
            }

            return context;
        }

        private void FillSectionContext<T>(Attribute<T> attribute, IDictionary<int, T> history, Action<string, T> setValue)
        {
            var yearBeforeAnalysisPeriod = Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod - 1;

            if (history.TryGetValue(yearBeforeAnalysisPeriod, out var value))
            {
                setValue(attribute.Name, value);
            }
            else
            {
                var mostRecentYear = history.Keys
                    .Where(year => year < yearBeforeAnalysisPeriod)
                    .Select(year => year.AsNullable())
                    .Max();

                if (mostRecentYear.HasValue)
                {
                    setValue(attribute.Name, history[mostRecentYear.Value]);
                }
                else
                {
                    setValue(attribute.Name, attribute.DefaultValue);
                }
            }
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

        private CostCoverage TryToPayForTreatment(SectionContext sectionContext, Treatment treatment, int year, Func<BudgetContext, decimal> getAvailableAmount, decimal? indivisibleCost = null)
        {
            decimal remainingCost;
            CashFlowRule cashFlowRule = null;

            if (indivisibleCost.HasValue)
            {
                remainingCost = indivisibleCost.Value;
            }
            else
            {
                remainingCost = (decimal)sectionContext.GetCostOfTreatment(treatment);

                cashFlowRule = Simulation.InvestmentPlan.CashFlowRules.SingleOrDefault(rule => rule.Criterion.Evaluate(sectionContext) ?? true);
                if (cashFlowRule != null)
                {
                    var distributionRule = cashFlowRule.DistributionRules
                        .OrderBy(rule => rule.CostCeiling)
                        .TakeWhile(rule => remainingCost <= rule.CostCeiling)
                        .Last();

                    var costPerYear = distributionRule.YearlyPercentages.Select(percentage => percentage / 100 * remainingCost).ToArray();
                    remainingCost = costPerYear[0];

                    var progression = costPerYear.Skip(1).Select(cost => new TreatmentProgress(treatment, cost)).ToArray();
                    if (progression.Length == 0)
                    {
                        cashFlowRule = null;
                    }
                    else
                    {
                        progression[progression.Length - 1].IsComplete = true;
                    }

                    foreach (var (yearProgress, yearOffset) in Zip.Short(progression, Static.Count(1)))
                    {
                        sectionContext.ProjectSchedule[year + yearOffset] = yearProgress;
                    }
                }
            }

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
                return CostCoverage.None;
            }

            foreach (var costAllocator in costAllocators)
            {
                costAllocator();
            }

            return cashFlowRule == null ? CostCoverage.Full : CostCoverage.CashFlow;
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
