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

        public ICollection<UnconditionalTreatmentConsequence> Consequences { get; } = new ListWithoutNulls<UnconditionalTreatmentConsequence>();

        public double Cost { get; set; }

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
                    var consequence = new UnconditionalTreatmentConsequence { Attribute = templateConsequence.Attribute };
                    consequence.Change.Expression = templateConsequence.Change.Expression;
                    Consequences.Add(consequence);
                }
            }
        }

        public override ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (Budget == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Budget is unset."));
                }

                if (Cost < 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Cost is less than zero."));
                }
                else if (Cost == 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Warning, this, "Cost is zero."));
                }

                if (Consequences.Select(consequence => consequence.Attribute).Distinct().Count() < Consequences.Count)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "At least one attribute is acted on by more than one consequence."));
                }

                return results;
            }
        }

        public int Year { get; }

        public override bool CanUseBudget(Budget budget) => budget == Budget;

        public override ICollection<Action> GetConsequenceActions(CalculateEvaluateArgument argument, NumberAttribute ageAttribute) => Consequences.Select(consequence => consequence.GetRecalculator(argument, ageAttribute)).ToArray();

        public override double GetCost(CalculateEvaluateArgument argument, NumberAttribute ageAttribute, bool shouldApplyMultipleFeasibleCosts) => Cost;

        private SelectableTreatment _TemplateTreatment;
    }
}
