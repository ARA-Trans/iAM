using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public abstract class TreatmentConsequence
    {
        public Attribute Attribute { get; }

        public abstract Action GetRecalculator(CalculateEvaluateArgument argument, NumberAttribute ageAttribute);
    }
}
