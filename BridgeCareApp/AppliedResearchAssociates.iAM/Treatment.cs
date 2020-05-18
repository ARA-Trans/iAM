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

        public virtual ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                if (Schedulings.Select(scheduling => scheduling.OffsetToFutureYear).Distinct().Count() < Schedulings.Count)
                {
                    results.Add(ValidationStatus.Error.Describe("At least one future year has more than one scheduling."));
                }

                if (ShadowForAnyTreatment < 0)
                {
                    results.Add(ValidationStatus.Error.Describe("\"Any\" shadow is less than zero."));
                }

                if (ShadowForSameTreatment < 0)
                {
                    results.Add(ValidationStatus.Error.Describe("\"Same\" shadow is less than zero."));
                }

                if (ShadowForSameTreatment <= ShadowForAnyTreatment)
                {
                    results.Add(ValidationStatus.Warning.Describe("\"Same\" shadow is less than or equal to \"any\" shadow."));
                }

                return results;
            }
        }

        public abstract bool CanUseBudget(Budget budget);

        public abstract ICollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute);

        public abstract double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts);
    }
}
