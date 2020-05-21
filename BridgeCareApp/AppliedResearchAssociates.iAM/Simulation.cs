using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Simulation : IValidator
    {
        public AnalysisMethod AnalysisMethod { get; }

        public ICollection<CommittedProject> CommittedProjects { get; } = new ListWithoutNulls<CommittedProject>();

        public Box<SelectableTreatment> DesignatedPassiveTreatment { get; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; set; }

        public Network Network { get; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; } = 100;

        public ICollection<PerformanceCurve> PerformanceCurves { get; } = new ListWithoutNulls<PerformanceCurve>();

        public ICollection<SimulationYearDetail> Results { get; } = new ListWithoutNulls<SimulationYearDetail>();

        public ICollection<SelectableTreatment> Treatments { get; } = new SetWithoutNulls<SelectableTreatment>();

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (DesignatedPassiveTreatment == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, DesignatedPassiveTreatment, "Designated passive treatment is unset."));
                }
                else if (DesignatedPassiveTreatment.Value.Costs.Count > 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, DesignatedPassiveTreatment, "Designated passive treatment has costs."));
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Name is blank."));
                }

                if (NumberOfYearsOfTreatmentOutlook < 1)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Number of years of treatment outlook is less than one."));
                }

                if (Treatments.Select(treatment => treatment.Name).Distinct().Count() < Treatments.Count)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Multiple selectable treatments have the same name."));
                }

                return results;
            }
        }

        public IReadOnlyCollection<SelectableTreatment> GetActiveTreatments()
        {
            var result = Treatments.ToHashSet();
            _ = result.Remove(DesignatedPassiveTreatment);
            return result;
        }

        internal Simulation(Network network)
        {
            Network = network ?? throw new ArgumentNullException(nameof(network));

            AnalysisMethod = new AnalysisMethod(this);
            InvestmentPlan = new InvestmentPlan(this);

            DesignatedPassiveTreatment = new Box<SelectableTreatment>(() => Treatments.Add(DesignatedPassiveTreatment));
        }
    }
}
