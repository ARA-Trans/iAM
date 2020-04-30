using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class UnconditionalTreatmentConsequence : TreatmentConsequence
    {
        public AttributeValueChange Change { get; }

        public override Action GetRecalculator(CalculateEvaluateArgument argument, NumberAttribute ageAttribute) => Change.GetApplicator(Attribute, argument);
    }
}
