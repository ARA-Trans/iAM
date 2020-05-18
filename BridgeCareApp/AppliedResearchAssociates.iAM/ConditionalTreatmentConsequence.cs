using System;
using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class ConditionalTreatmentConsequence : TreatmentConsequence
    {
        public AttributeValueChange Change { get; } = new AttributeValueChange();

        public Criterion Criterion { get; } = new Criterion();

        public Equation Equation { get; } = new Equation();

        public override ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = base.ValidationResults;

                if (Change.ExpressionIsBlank == Equation.ExpressionIsBlank)
                {
                    results.Add(ValidationStatus.Error.Describe(MessageStrings.ChangeAndEquationAreEitherBothSetOrBothUnset));
                }

                return results;
            }
        }

        public override Action GetRecalculator(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
        {
            if (!Change.ExpressionIsBlank && Equation.ExpressionIsBlank)
            {
                return Change.GetApplicator(Attribute, argument);
            }
            else if (Change.ExpressionIsBlank && !Equation.ExpressionIsBlank)
            {
                var newValue = Equation.Compute(argument, ageAttribute);
                return () => argument.SetNumber(Attribute.Name, newValue);
            }
            else
            {
                throw new SimulationException(MessageStrings.ChangeAndEquationAreEitherBothSetOrBothUnset);
            }
        }
    }
}
