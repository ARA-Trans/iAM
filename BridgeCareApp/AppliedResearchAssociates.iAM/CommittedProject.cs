using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CommittedProject : Treatment
    {
        public Budget Budget { get; }

        public List<UnconditionalTreatmentConsequence> Consequences { get; }

        public double Cost { get; }

        public Section Section { get; }

        public SelectableTreatment TemplateTreatment
        {
            get => _TemplateTreatment;
            set
            {
                _TemplateTreatment = value;

                Name = TemplateTreatment.Name;

                Consequences.Clear();
                foreach (var templateConsequence in TemplateTreatment.Consequences)
                {
                    void addConsequence(AttributeValueChange templateChange)
                    {
                        var change = new AttributeValueChange
                        {
                            Expression = templateChange.Expression
                        };
                        var consequence = new UnconditionalTreatmentConsequence
                        {
                            Attribute = templateConsequence.Attribute,
                            Change = change,
                        };
                        Consequences.Add(consequence);
                    }

                    templateConsequence.Recalculation.Handle(addConsequence, Static.DoNothing);
                }
            }
        }

        public int Year { get; }

        public override bool CanUseBudget(Budget budget) => budget == Budget;

        public override IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute) => Consequences.Select(consequence => consequence.GetRecalculator(argument, ageAttribute)).ToArray();

        public override double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts) => Cost;

        private SelectableTreatment _TemplateTreatment;
    }
}
