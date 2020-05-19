using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public class Attribute : IValidator
    {
        public string Name { get; set; }

        public virtual ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                return results;
            }
        }
    }

    public class Attribute<T> : Attribute
    {
        // [REVIEW] Consider removing this member in favor of separately defining one or more
        // AttributeDataProvider/AttributeDataSource types, which would use this type (as metadata)
        // to retrieve the corresponding data with respect to system constraints, e.g. not enough
        // memory to hold all data at once.
        public IEnumerable<AttributeDatum<T>> Data => throw new NotImplementedException();

        public T DefaultValue { get; set; }
    }
}
