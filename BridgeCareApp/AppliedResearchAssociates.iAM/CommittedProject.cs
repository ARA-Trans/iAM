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

        public int Year { get; }

        public void FillFromTreatment(SelectableTreatment treatment)
        {
            // TODO
        }

        public override IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute) => Consequences.Select(consequence => consequence.GetRecalculator(argument, ageAttribute)).ToArray();

        public override double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute) => Cost;
    }
}
