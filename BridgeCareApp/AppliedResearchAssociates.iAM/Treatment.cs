using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public abstract class Treatment
    {
        public string Name { get; set; }

        public int ShadowForAnyTreatment { get; }

        public int ShadowForSameTreatment { get; }

        public abstract bool CanUseBudget(Budget budget);

        public abstract IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute);

        public abstract double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts);

        public virtual IEnumerable<TreatmentScheduling> GetSchedulings() => Enumerable.Empty<TreatmentScheduling>();
    }
}
