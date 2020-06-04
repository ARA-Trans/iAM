using System;
using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class Treatment : IValidator
    {
        public string Name { get; set; }

        public int ShadowForAnyTreatment { get; set; }

        public int ShadowForSameTreatment { get; set; }

        public virtual ValidatorBag Subvalidators => new ValidatorBag();

        public abstract bool CanUseBudget(Budget budget);

        public abstract IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument);

        public abstract double GetCost(CalculateEvaluateArgument argument, bool shouldApplyMultipleFeasibleCosts);

        public virtual ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            if (ShadowForAnyTreatment < 0)
            {
                results.Add(ValidationStatus.Error, "\"Any\" shadow is less than zero.", this, nameof(ShadowForAnyTreatment));
            }

            if (ShadowForSameTreatment < 0)
            {
                results.Add(ValidationStatus.Error, "\"Same\" shadow is less than zero.", this, nameof(ShadowForSameTreatment));
            }

            if (ShadowForSameTreatment < ShadowForAnyTreatment)
            {
                results.Add(ValidationStatus.Warning, "\"Same\" shadow is less than \"any\" shadow.", this);
            }

            return results;
        }

        public abstract IEnumerable<TreatmentScheduling> GetSchedulings();
    }
}
