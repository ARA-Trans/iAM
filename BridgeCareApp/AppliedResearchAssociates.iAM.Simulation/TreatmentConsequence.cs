using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class TreatmentConsequence
    {
        public Attribute Attribute { get; }

        public Choice<AttributeValueChange, Equation> Recalculation { get; }

        public Criterion Criterion { get; }

        public Action GetRecalculator(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
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
