using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CommittedProject : Treatment
    {
        public CommittedProject(Section section, int year)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            Year = year;
        }

        public Budget Budget { get; set; }

        public ICollection<TreatmentConsequence> Consequences { get; } = new SetWithoutNulls<TreatmentConsequence>();

        public double Cost { get; set; }

        public override ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (Budget == null)
                {
                    results.Add(ValidationStatus.Error, "Budget is unset.", this, nameof(Budget));
                }

                if (Cost < 0)
                {
                    results.Add(ValidationStatus.Error, "Cost is less than zero.", this, nameof(Cost));
                }
                else if (Cost == 0)
                {
                    results.Add(ValidationStatus.Warning, "Cost is zero.", this, nameof(Cost));
                }

                if (Consequences.Select(consequence => consequence.Attribute).Distinct().Count() < Consequences.Count)
                {
                    results.Add(ValidationStatus.Error, "At least one attribute is acted on by more than one consequence.", this, nameof(Consequences));
                }

                return results;
            }
        }

        public Section Section { get; }

        public SelectableTreatment TemplateTreatment
        {
            get => _TemplateTreatment;
            set
            {
                _TemplateTreatment = value;

                Name = TemplateTreatment.Name;

                Consequences.Clear();
                foreach (var templateConsequence in TemplateTreatment.Consequences)
                {
                    var consequence = new TreatmentConsequence { Attribute = templateConsequence.Attribute };
                    consequence.Change.Expression = templateConsequence.Change.Expression;
                    Consequences.Add(consequence);
                }
            }
        }

        public int Year { get; }

        public override bool CanUseBudget(Budget budget) => budget == Budget;

        public override ICollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute) => Consequences.Select(consequence => consequence.GetRecalculator(argument, ageAttribute)).ToArray();

        public override double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts) => Cost;

        private SelectableTreatment _TemplateTreatment;
    }
}
