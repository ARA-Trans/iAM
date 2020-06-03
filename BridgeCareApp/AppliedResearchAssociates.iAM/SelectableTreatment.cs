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

        public IReadOnlyCollection<ConditionalTreatmentConsequence> Consequences => _Consequences;

        public IReadOnlyCollection<TreatmentCost> Costs => _Costs;

        public string Description { get; set; }

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

        public IReadOnlyCollection<Criterion> FeasibilityCriteria => _FeasibilityCriteria;

        public IReadOnlyCollection<TreatmentSupersession> Supersessions => _Supersessions;

        public override ValidatorBag Subvalidators => base.Subvalidators.Add(Consequences).Add(Costs).Add(FeasibilityCriteria).Add(Supersessions);

        public ConditionalTreatmentConsequence AddConsequence() => _Consequences.GetAdd(new ConditionalTreatmentConsequence(Simulation.Network.Explorer));

        public TreatmentCost AddCost() => _Costs.GetAdd(new TreatmentCost(Simulation.Network.Explorer));

        public Criterion AddFeasibilityCriterion() => _FeasibilityCriteria.GetAdd(new Criterion(Simulation.Network.Explorer));

        public TreatmentSupersession AddSupersession() => _Supersessions.GetAdd(new TreatmentSupersession(Simulation.Network.Explorer));

        public override bool CanUseBudget(Budget budget) => Budgets.Contains(budget);

        public void DesignateAsPassiveForSimulation()
        {
            if (!Simulation.Treatments.Contains(this))
            {
                throw new InvalidOperationException("Simulation does not contain this treatment.");
            }

            Simulation.DesignatedPassiveTreatment = this;
        }

        public override IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument)
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
                .Select(consequence => consequence.GetRecalculator(argument))
                .ToArray();

            return consequenceActions;
        }

        public override double GetCost(CalculateEvaluateArgument argument, bool shouldApplyMultipleFeasibleCosts)
        {
            var feasibleCosts = Costs.Where(cost => cost.Criterion.EvaluateOrDefault(argument)).ToArray();
            if (feasibleCosts.Length == 0)
            {
                return 0;
            }

            double getCost(TreatmentCost cost) => cost.Equation.Compute(argument);
            return shouldApplyMultipleFeasibleCosts ? feasibleCosts.Sum(getCost) : feasibleCosts.Max(getCost);
        }

        public bool IsFeasible(CalculateEvaluateArgument argument) => FeasibilityCriteria.Any(feasibility => feasibility.EvaluateOrDefault(argument));

        public void Remove(TreatmentSupersession supersession) => _Supersessions.Remove(supersession);

        public void Remove(ConditionalTreatmentConsequence consequence) => _Consequences.Remove(consequence);

        public void Remove(TreatmentCost cost) => _Costs.Remove(cost);

        public void RemoveFeasibilityCriterion(Criterion criterion) => _FeasibilityCriteria.Remove(criterion);

        internal SelectableTreatment(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        private readonly List<ConditionalTreatmentConsequence> _Consequences = new List<ConditionalTreatmentConsequence>();

        private readonly List<TreatmentCost> _Costs = new List<TreatmentCost>();

        private readonly List<Criterion> _FeasibilityCriteria = new List<Criterion>();

        private readonly List<TreatmentSupersession> _Supersessions = new List<TreatmentSupersession>();

        private readonly Simulation Simulation;

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
