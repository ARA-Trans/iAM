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

                if (ValueSources.Count == 0)
                {
                    results.Add(ValidationStatus.Error, "There are no value sources.", this, nameof(ValueSources));
                }
                else
                {
                    var numberOfEquationsWithBlankCriterion = ValueSources.Count(equation => equation.Criterion.ExpressionIsBlank);
                    if (numberOfEquationsWithBlankCriterion == 0)
                    {
                        results.Add(ValidationStatus.Warning, "There are no value sources with a blank criterion.", this, nameof(ValueSources));
                    }
                    else if (numberOfEquationsWithBlankCriterion > 1)
                    {
                        results.Add(ValidationStatus.Error, "There are multiple value sources with a blank criterion.", this, nameof(ValueSources));
                    }
                }

                return results;
            }
        }

        public IReadOnlyCollection<CalculatedFieldValueSource> ValueSources => _ValueSources;

        public ValidatorBag Subvalidators => new ValidatorBag { ValueSources };

        public CalculatedFieldValueSource AddValueSource() => _ValueSources.GetAdd(new CalculatedFieldValueSource(Explorer));

        public double Calculate(CalculateEvaluateArgument argument)
        {
            ValueSources.Channel(
                source => source.Criterion.Evaluate(argument),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableSources,
                out var defaultSources);

            var operativeSources = applicableSources.Count > 0 ? applicableSources : defaultSources;

            if (operativeSources.Count == 0)
            {
                throw new SimulationException(MessageStrings.CalculatedFieldHasNoOperativeEquations);
            }

            if (operativeSources.Count > 1)
            {
                throw new SimulationException(MessageStrings.CalculatedFieldHasMultipleOperativeEquations);
            }

            return operativeSources[0].Equation.Compute(argument);
        }

        public void Remove(CalculatedFieldValueSource source) => _ValueSources.Remove(source);

        private readonly List<CalculatedFieldValueSource> _ValueSources = new List<CalculatedFieldValueSource>();

        private readonly Explorer Explorer;
    }
}
