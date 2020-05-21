using System;
using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class TreatmentConsequence : IValidator
    {
        public Attribute Attribute { get; set; }

        public virtual ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Attribute == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Attribute is unset."));
                }

                return results;
            }
        }

        public abstract Action GetRecalculator(CalculateEvaluateArgument argument, NumberAttribute ageAttribute);
    }
}
