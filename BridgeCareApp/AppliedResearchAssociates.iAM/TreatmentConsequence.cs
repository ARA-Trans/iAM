using System;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public class TreatmentConsequence : IValidator
    {
        public Attribute Attribute { get; set; }

        public AttributeValueChange Change { get; } = new AttributeValueChange();

        public virtual ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Attribute == null)
                {
                    results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
                }

                return results;
            }
        }

        public virtual ValidatorBag Subvalidators => new ValidatorBag { Change };

        public virtual Action GetRecalculator(CalculateEvaluateArgument argument) => Change.GetApplicator(Attribute, argument);
    }
}
