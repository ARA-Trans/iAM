using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class Treatment : IValidator
    {
        public string Name { get; set; }

        public ICollection<TreatmentScheduling> Schedulings { get; } = new List<TreatmentScheduling>();

        public int ShadowForAnyTreatment { get; }

        public int ShadowForSameTreatment { get; }

        public virtual ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Name is blank."));
                }

                if (Schedulings.Select(scheduling => scheduling.OffsetToFutureYear).Distinct().Count() < Schedulings.Count)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "At least one future year has more than one scheduling."));
                }

                if (ShadowForAnyTreatment < 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "\"Any\" shadow is less than zero."));
                }

                if (ShadowForSameTreatment < 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "\"Same\" shadow is less than zero."));
                }

                if (ShadowForSameTreatment <= ShadowForAnyTreatment)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Warning, this, "\"Same\" shadow is less than or equal to \"any\" shadow."));
                }

                return results;
            }
        }

        public abstract bool CanUseBudget(Budget budget);

        public abstract ICollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute);

        public abstract double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts);
    }
}
