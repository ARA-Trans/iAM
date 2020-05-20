using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Network : IValidator
    {
        public Explorer Explorer { get; }

        public string Name { get; set; }

        public ICollection<SectionHistory> SectionHistories { get; } = new CollectionWithoutNulls<SectionHistory>();

        public IReadOnlyCollection<Simulation> Simulations => _Simulations;

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                if (Simulations.Select(simulation => simulation.Name).Distinct().Count() < Simulations.Count)
                {
                    results.Add(ValidationStatus.Error.Describe("Multiple simulations have the same name."));
                }

                return results;
            }
        }

        public Simulation AddSimulation()
        {
            var simulation = new Simulation(this);
            _Simulations.Add(simulation);
            return simulation;
        }

        public bool RemoveSimulation(Simulation simulation) => _Simulations.Remove(simulation);

        internal Network(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        private readonly List<Simulation> _Simulations = new List<Simulation>();
    }
}
