using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class SelectableTreatment : Treatment
    {
        public ICollection<Budget> Budgets { get; }

        public List<ConditionalTreatmentConsequence> Consequences { get; }

        public List<ConditionalEquation> Costs { get; }

        public string Description { get; }

        public Criterion FeasibilityCriterion { get; }

        public List<TreatmentScheduling> Schedulings { get; }

        public List<TreatmentSupersession> Supersessions { get; }

        public override bool CanUseBudget(Budget budget) => Budgets.Contains(budget);

        public override IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
        {
            Consequences.Channel(
                consequence => consequence.Criterion.Evaluate(argument),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableConsequences,
                out var defaultConsequences);

            var operativeConsequences = applicableConsequences.Count > 0 ? applicableConsequences : defaultConsequences;

            operativeConsequences = operativeConsequences
                .GroupBy(consequence => consequence.Attribute)
                // It's (currently) an error when one attribute has multiple valid consequences. A
                // semantic that might match more closely with legacy intent is to use 'First'
                // instead of 'Single'.
                .Select(group => group.Single())
                .ToArray();

            var consequenceActions = operativeConsequences
                .Select(consequence => consequence.GetRecalculator(argument, ageAttribute))
                .ToArray();

            return consequenceActions;
        }

        public override double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts)
        {
            var feasibleCosts = Costs.Where(costEquation => costEquation.Criterion.Evaluate(argument) ?? true);
            return shouldApplyMultipleFeasibleCosts ? feasibleCosts.Sum(getCost) : feasibleCosts.Max(getCost);

            double getCost(ConditionalEquation costEquation) => costEquation.Equation.Compute(argument, ageAttribute);
        }

        public override IEnumerable<TreatmentScheduling> GetSchedulings() => Schedulings;
    }
}
