using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public class Attribute : IValidator
    {
        public string Name { get; private set; }

        public virtual ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (!NamePattern.IsMatch(Name))
                {
                    results.Add(ValidationStatus.Error.Describe($"Name is invalid. The valid pattern is \"{PatternStrings.Identifier}\"."));
                }

                return results;
            }
        }

        public bool UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || NameIsTakenByOtherAttribute(newName))
            {
                return false;
            }

            var parameterType = Explorer.Compiler.ParameterTypes[Name];
            Explorer.RemoveParameterType(Name);
            Explorer.Compiler.ParameterTypes.Add(newName, parameterType);
            Name = newName;
            return true;
        }

        protected Attribute(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        private static readonly Regex NamePattern = new Regex($@"(?>\A{PatternStrings.Identifier}\z)");

        private readonly Explorer Explorer;

        private bool NameIsTakenByOtherAttribute(string name) => Explorer.AllAttributes.Any(attribute => attribute != this && attribute.Name == name);
    }

    public class Attribute<T> : Attribute
    {
        // [REVIEW] Consider removing this member in favor of separately defining one or more
        // AttributeDataProvider/AttributeDataSource types, which would use this type (as metadata)
        // to retrieve the corresponding data with respect to system constraints, e.g. not enough
        // memory to hold all data at once.
        public IEnumerable<AttributeDatum<T>> Data => throw new NotImplementedException();

        public T DefaultValue { get; set; }

        protected Attribute(Explorer explorer) : base(explorer)
        {
        }
    }
}
