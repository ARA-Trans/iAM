using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Simulation : IValidator
    {
        public AnalysisMethod AnalysisMethod { get; }

        public ICollection<CommittedProject> CommittedProjects { get; } = new CollectionWithoutNulls<CommittedProject>();

        public SelectableTreatment DesignatedPassiveTreatment
        {
            get => _DesignatedPassiveTreatment;
            set
            {
                _DesignatedPassiveTreatment = value;

                Treatments.Add(DesignatedPassiveTreatment);
            }
        }

        private SelectableTreatment _DesignatedPassiveTreatment;

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; set; }

        public Network Network { get; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; } = 100;

        public ICollection<PerformanceCurve> PerformanceCurves { get; } = new CollectionWithoutNulls<PerformanceCurve>();

        public ICollection<SimulationYearDetail> Results { get; } = new CollectionWithoutNulls<SimulationYearDetail>();

        public ICollection<SelectableTreatment> Treatments { get; } = new CollectionWithoutNulls<SelectableTreatment>();

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (DesignatedPassiveTreatment == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Designated passive treatment is unset."));
                }
                else if (DesignatedPassiveTreatment.Costs.Count > 0)
                {
                    results.Add(ValidationStatus.Error.Describe("Designated passive treatment has costs."));
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                if (NumberOfYearsOfTreatmentOutlook < 1)
                {
                    results.Add(ValidationStatus.Error.Describe("Number of years of treatment outlook is less than one."));
                }

                if (Treatments.Select(treatment => treatment.Name).Distinct().Count() < Treatments.Count)
                {
                    results.Add(ValidationStatus.Error.Describe("Multiple selectable treatments have the same name."));
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
        }
    }
}
