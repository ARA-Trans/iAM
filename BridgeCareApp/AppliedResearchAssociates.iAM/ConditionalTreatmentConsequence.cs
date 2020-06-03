using System;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class ConditionalTreatmentConsequence : TreatmentConsequence
    {
        public ConditionalTreatmentConsequence(Explorer explorer)
        {
            if (explorer == null)
            {
                throw new ArgumentNullException(nameof(explorer));
            }

            Criterion = new Criterion(explorer);
            Equation = new Equation(explorer);
        }

        public Criterion Criterion { get; }

        public Equation Equation { get; }

        public override ValidatorBag Subvalidators
        {
            get
            {
                var validators = base.Subvalidators.Add(Criterion);
                if (!Equation.ExpressionIsBlank)
                {
                    _ = validators.Remove(Change).Add(Equation);
                }

                return validators;
            }
        }

        public override Action GetRecalculator(CalculateEvaluateArgument argument)
        {
            if (!Equation.ExpressionIsBlank)
            {
                var newValue = Equation.Compute(argument);
                return () => argument.SetNumber(Attribute.Name, newValue);
            }

            return base.GetRecalculator(argument);
        }
    }
}
