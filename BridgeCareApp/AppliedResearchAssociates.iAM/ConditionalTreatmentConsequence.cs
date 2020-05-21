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

        public override ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (Change.ExpressionIsBlank == Equation.ExpressionIsBlank)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, MessageStrings.ChangeAndEquationAreEitherBothSetOrBothUnset));
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
                throw new InvalidOperationException(MessageStrings.ChangeAndEquationAreEitherBothSetOrBothUnset);
            }
        }
    }
}
