using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public class Attribute : IValidator
    {
        public virtual ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (!NamePattern.IsMatch(Name))
                {
                    results.Add(ValidationStatus.Error, $"Invalid name. The valid pattern is \"{PatternStrings.Identifier}\".", this, nameof(Name));
                }

                return results;
            }
        }

        public string Name { get; private set; }

        public virtual ValidatorBag Subvalidators => new ValidatorBag();

        public bool UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || NameIsTakenByOtherAttribute(newName))
            {
                return false;
            }

            if (Explorer.Compiler.ParameterTypes.TryGetValue(Name, out var parameterType))
            {
                Explorer.RemoveParameterType(Name);
                Explorer.Compiler.ParameterTypes.Add(newName, parameterType);
            }

            Name = newName;
            return true;
        }

        protected Attribute(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        private static readonly Regex NamePattern = new Regex($@"(?>\A{PatternStrings.Identifier}\z)");

        private readonly Explorer Explorer;

        private bool NameIsTakenByOtherAttribute(string value) => Explorer.AllAttributes.Any(attribute => attribute.Name == value && attribute != this);
    }

    public class Attribute<T> : Attribute
    {
        // [REVIEW] Remove this member in favor of separately defining one or more
        // "AttributeDataProvider"-like types, which would use this type (as metadata) to retrieve
        // the corresponding data with respect to system constraints, e.g. not enough memory to hold
        // all data at once.
        public IEnumerable<AttributeDatum<T>> Data => throw new NotImplementedException();

        public T DefaultValue { get; set; }

        protected Attribute(Explorer explorer) : base(explorer)
        {
        }
    }
}
