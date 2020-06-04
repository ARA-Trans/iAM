using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Simulation : IValidator
    {
        public Simulation(Network network)
        {
            Network = network ?? throw new ArgumentNullException(nameof(network));

            AnalysisMethod = new AnalysisMethod(this);
            InvestmentPlan = new InvestmentPlan(this);
        }

        public AnalysisMethod AnalysisMethod { get; }

        public ICollection<CommittedProject> CommittedProjects { get; } = new SetWithoutNulls<CommittedProject>();

        public SelectableTreatment DesignatedPassiveTreatment { get; internal set; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; set; }

        public Network Network { get; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; } = 100;

        public IReadOnlyCollection<PerformanceCurve> PerformanceCurves => _PerformanceCurves;

        public ICollection<SimulationYearDetail> Results { get; } = new SetWithoutNulls<SimulationYearDetail>();

        public ValidatorBag Subvalidators => new ValidatorBag { AnalysisMethod, CommittedProjects, InvestmentPlan, PerformanceCurves, Treatments };

        public IReadOnlyCollection<SelectableTreatment> Treatments => _Treatments;

        public PerformanceCurve AddPerformanceCurve() => _PerformanceCurves.GetAdd(new PerformanceCurve(Network.Explorer));

        public SelectableTreatment AddTreatment() => _Treatments.GetAdd(new SelectableTreatment(this));

        public IReadOnlyCollection<SelectableTreatment> GetActiveTreatments()
        {
            var result = Treatments.ToHashSet();
            _ = result.Remove(DesignatedPassiveTreatment);
            return result;
        }

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (DesignatedPassiveTreatment == null)
            {
                results.Add(ValidationStatus.Error, "Designated passive treatment is unset.", this, nameof(DesignatedPassiveTreatment));
            }
            else if (DesignatedPassiveTreatment.Costs.Count > 0)
            {
                results.Add(ValidationStatus.Error, "Designated passive treatment has costs.", this, nameof(DesignatedPassiveTreatment));
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            if (NumberOfYearsOfTreatmentOutlook < 1)
            {
                results.Add(ValidationStatus.Error, "Number of years of treatment outlook is less than one.", this, nameof(NumberOfYearsOfTreatmentOutlook));
            }

            if (Treatments.Select(treatment => treatment.Name).Distinct().Count() < Treatments.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple selectable treatments have the same name.", this, nameof(Treatments));
            }

            if (CommittedProjects.Select(project => (project.Section, project.Year)).Distinct().Count() < CommittedProjects.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple projects are committed to the same section in the same year.", this, nameof(CommittedProjects));
            }

            return results;
        }

        public void Remove(SelectableTreatment treatment) => _Treatments.Remove(treatment);

        public void Remove(PerformanceCurve performanceCurve) => _PerformanceCurves.Remove(performanceCurve);

        private readonly List<PerformanceCurve> _PerformanceCurves = new List<PerformanceCurve>();

        private readonly List<SelectableTreatment> _Treatments = new List<SelectableTreatment>();
    }
}
