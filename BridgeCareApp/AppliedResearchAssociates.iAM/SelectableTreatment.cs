using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class SelectableTreatment : Treatment
    {
        public ICollection<Budget> Budgets { get; } = new SetWithoutNulls<Budget>();

        public ICollection<ConditionalTreatmentConsequence> Consequences { get; } = new SetWithoutNulls<ConditionalTreatmentConsequence>();

        public ICollection<ConditionalEquation> Costs { get; } = new SetWithoutNulls<ConditionalEquation>();

        public string Description { get; set; }

        public Criterion FeasibilityCriterion { get; } = new Criterion();

        public ICollection<TreatmentSupersession> Supersessions { get; } = new SetWithoutNulls<TreatmentSupersession>();

        public override ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                var consequencesWithBlankCriterion = Consequences.Where(consequence => consequence.Criterion.ExpressionIsBlank).ToArray();
                if (consequencesWithBlankCriterion.Select(consequence => consequence.Attribute).Distinct().Count() < consequencesWithBlankCriterion.Length)
                {
                    results.Add(ValidationStatus.Error, "At least one attribute is unconditionally acted on by more than one consequence.", this, nameof(Consequences));
                }

                var supersessionsWithBlankCriterion = Supersessions.Where(supersession => supersession.Criterion.ExpressionIsBlank).ToArray();
                if (supersessionsWithBlankCriterion.Select(supersession => supersession.Treatment).Distinct().Count() < supersessionsWithBlankCriterion.Length)
                {
                    results.Add(ValidationStatus.Warning, "At least one treatment is unconditionally superseded more than once.", this, nameof(Supersessions));
                }

                return results;
            }
        }

        public override ValidatorBag Subvalidators => base.Subvalidators.Add(Consequences).Add(Costs).Add(FeasibilityCriterion).Add(Supersessions);

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
                .Select(GetSingleConsequence)
                .ToArray();

            var consequenceActions = operativeConsequences
                .Select(consequence => consequence.GetRecalculator(argument, ageAttribute))
                .ToArray();

            return consequenceActions;
        }

        public override double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts)
        {
            var feasibleCosts = Costs.Where(costEquation => costEquation.Criterion.Evaluate(argument) ?? true).ToArray();
            if (feasibleCosts.Length == 0)
            {
                return 0;
            }

            double getCost(ConditionalEquation costEquation) => costEquation.Equation.Compute(argument, ageAttribute);
            return shouldApplyMultipleFeasibleCosts ? feasibleCosts.Sum(getCost) : feasibleCosts.Max(getCost);
        }

        private static ConditionalTreatmentConsequence GetSingleConsequence(IEnumerable<ConditionalTreatmentConsequence> group)
        {
            var consequences = group.ToArray();
            if (consequences.Length > 1)
            {
                throw new SimulationException(MessageStrings.AttributeIsBeingActedOnByMultipleConsequences);
            }

            return consequences[0];
        }
    }
}
