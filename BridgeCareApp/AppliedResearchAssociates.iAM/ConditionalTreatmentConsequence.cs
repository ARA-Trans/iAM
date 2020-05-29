using System;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class ConditionalTreatmentConsequence : TreatmentConsequence
    {
        public Criterion Criterion { get; } = new Criterion();

        public override ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (Change.ExpressionIsBlank == Equation.ExpressionIsBlank)
                {
                    results.Add(ValidationStatus.Error, MessageStrings.ChangeAndEquationAreEitherBothSetOrBothUnset, this);
                }

                return results;
            }
        }

        public Equation Equation { get; } = new Equation();

        public override ValidatorBag Subvalidators => base.Subvalidators.Add(Equation).Add(Criterion);

        public override Action GetRecalculator(CalculateEvaluateArgument argument)
        {
            if (!Change.ExpressionIsBlank && Equation.ExpressionIsBlank)
            {
                return base.GetRecalculator(argument);
            }
            else if (Change.ExpressionIsBlank && !Equation.ExpressionIsBlank)
            {
                var newValue = Equation.Compute(argument);
                return () => argument.SetNumber(Attribute.Name, newValue);
            }
            else
            {
                throw new InvalidOperationException(MessageStrings.ChangeAndEquationAreEitherBothSetOrBothUnset);
            }
        }
    }
}
