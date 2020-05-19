using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CalculatedField : IValidator
    {
        public ICollection<ConditionalEquation> Equations { get; } = new List<ConditionalEquation>();

        public string Name { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Equations.Count(equation => equation.Criterion.ExpressionIsBlank) > 1)
                {
                    results.Add(ValidationStatus.Error.Describe("Multiple equations have a blank criterion."));
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                return results;
            }
        }

        public double Calculate(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
        {
            Equations.Channel(
                equation => equation.Criterion.Evaluate(argument),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableEquations,
                out var defaultEquations);

            var operativeEquations = applicableEquations.Count > 0 ? applicableEquations : defaultEquations;

            if (operativeEquations.Count == 0)
            {
                throw new SimulationException(MessageStrings.CalculatedFieldHasNoOperativeEquations);
            }

            if (operativeEquations.Count > 1)
            {
                throw new SimulationException(MessageStrings.CalculatedFieldHasMultipleOperativeEquations);
            }

            return operativeEquations[0].Equation.Compute(argument, ageAttribute);
        }
    }
}
