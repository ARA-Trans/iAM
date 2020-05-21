using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class AttributeName : IValidator
    {
        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (!Pattern.IsMatch(Value))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, $"Invalid value. The valid pattern is \"{PatternStrings.Identifier}\"."));
                }

                return results;
            }
        }

        public ICollection<IValidator> Subvalidators => new List<IValidator>();

        public string Value { get; private set; }

        public bool UpdateValue(string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue) || ValueIsTakenByOtherAttributeName(newValue))
            {
                return false;
            }

            if (Explorer.Compiler.ParameterTypes.TryGetValue(Value, out var parameterType))
            {
                Explorer.RemoveParameterType(Value);
                Explorer.Compiler.ParameterTypes.Add(newValue, parameterType);
            }

            Value = newValue;
            return true;
        }

        internal AttributeName(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        private static readonly Regex Pattern = new Regex($@"(?>\A{PatternStrings.Identifier}\z)");

        private readonly Explorer Explorer;

        private bool ValueIsTakenByOtherAttributeName(string value) => Explorer.AllAttributes.Any(attribute => attribute.Name.Value == value && attribute.Name != this);
    }
}
