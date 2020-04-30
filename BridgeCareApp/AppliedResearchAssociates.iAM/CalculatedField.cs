using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public class CalculatedField
    {
        public List<ConditionalEquation> Equations { get; }

        public string Name { get; }

        public double Calculate(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
        {
            Equations.Channel(
                equation => equation.Criterion.Evaluate(argument),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableEquations,
                out var defaultEquations);

            var operativeEquations = applicableEquations.Count > 0 ? applicableEquations : defaultEquations;
            var operativeEquation = operativeEquations.Single();

            return operativeEquation.Equation.Compute(argument, ageAttribute);
        }
    }
}
