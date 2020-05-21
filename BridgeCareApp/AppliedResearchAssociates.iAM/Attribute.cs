using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public class Attribute : IValidator
    {
        public virtual ICollection<ValidationResult> DirectValidationResults => new List<ValidationResult>();

        public AttributeName Name { get; }

        public ICollection<IValidator> Subvalidators
        {
            get
            {
                var validators = new List<IValidator>();
                validators.Add(Name);
                return validators;
            }
        }

        protected Attribute(Explorer explorer) => Name = new AttributeName(explorer ?? throw new ArgumentNullException(nameof(explorer)));
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
