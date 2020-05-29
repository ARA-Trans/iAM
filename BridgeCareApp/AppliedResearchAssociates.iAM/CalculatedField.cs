using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CalculatedField : Attribute, IValidator
    {
        public CalculatedField(string name, Explorer explorer) : base(name) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Equations.Count == 0)
                {
                    results.Add(ValidationStatus.Error, "There are no equations.", this, nameof(Equations));
                }
                else
                {
                    var numberOfEquationsWithBlankCriterion = Equations.Count(equation => equation.Criterion.ExpressionIsBlank);
                    if (numberOfEquationsWithBlankCriterion == 0)
                    {
                        results.Add(ValidationStatus.Warning, "There are no equations with a blank criterion.", this, nameof(Equations));
                    }
                    else if (numberOfEquationsWithBlankCriterion > 1)
                    {
                        results.Add(ValidationStatus.Error, "There are multiple equations with a blank criterion.", this, nameof(Equations));
                    }
                }

                return results;
            }
        }

        public IReadOnlyCollection<ConditionalEquation> Equations => _Equations;

        public ValidatorBag Subvalidators => new ValidatorBag { Equations };

        public ConditionalEquation AddEquation()
        {
            var equation = new ConditionalEquation(Explorer);
            _Equations.Add(equation);
            return equation;
        }

        public double Calculate(CalculateEvaluateArgument argument)
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

            return operativeEquations[0].Equation.Compute(argument);
        }

        public bool RemoveEquation(ConditionalEquation equation) => _Equations.Remove(equation);

        private readonly List<ConditionalEquation> _Equations = new List<ConditionalEquation>();

        private readonly Explorer Explorer;
    }
}
