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

        public ICollection<TreatmentScheduling> Schedulings { get; } = new SetWithoutNulls<TreatmentScheduling>();

        public int ShadowForAnyTreatment { get; set; }

        public int ShadowForSameTreatment { get; set; }

        public virtual ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
                }

                if (Schedulings.Select(scheduling => scheduling.OffsetToFutureYear).Distinct().Count() < Schedulings.Count)
                {
                    results.Add(ValidationStatus.Error, "At least one future year has more than one scheduling.", this, nameof(Schedulings));
                }

                if (ShadowForAnyTreatment < 0)
                {
                    results.Add(ValidationStatus.Error, "\"Any\" shadow is less than zero.", this, nameof(ShadowForAnyTreatment));
                }

                if (ShadowForSameTreatment < 0)
                {
                    results.Add(ValidationStatus.Error, "\"Same\" shadow is less than zero.", this, nameof(ShadowForSameTreatment));
                }

                if (ShadowForSameTreatment <= ShadowForAnyTreatment)
                {
                    results.Add(ValidationStatus.Warning, "\"Same\" shadow is less than or equal to \"any\" shadow.", this);
                }

                return results;
            }
        }

        public virtual ValidatorBag Subvalidators => new ValidatorBag { Schedulings };

        public abstract bool CanUseBudget(Budget budget);

        public abstract IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute);

        public abstract double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts);
    }
}
