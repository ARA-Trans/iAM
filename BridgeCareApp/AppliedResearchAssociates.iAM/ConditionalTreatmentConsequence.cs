using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class ConditionalTreatmentConsequence : TreatmentConsequence
    {
        public Criterion Criterion { get; }

        public Choice<AttributeValueChange, Equation> Recalculation { get; }

        public override Action GetRecalculator(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
        {
            Action getRecalculatorForEquation(Equation equation)
            {
                var newValue = equation.Compute(argument, ageAttribute);
                return () => argument.SetNumber(Attribute.Name, newValue);
            }

            return Recalculation.Reduce(
                change => change.GetApplicator(Attribute, argument),
                getRecalculatorForEquation);
        }
    }
}
