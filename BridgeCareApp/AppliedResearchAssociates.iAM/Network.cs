using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Network : IValidator
    {
        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
                }

                if (Simulations.Select(simulation => simulation.Name).Distinct().Count() < Simulations.Count)
                {
                    results.Add(ValidationStatus.Error, "Multiple simulations have the same name.", this, nameof(Simulations));
                }

                return results;
            }
        }

        public Explorer Explorer { get; }

        public string Name { get; set; }

        // [TODO] This member is basically the "hook" for integration with whatever code is handling segmentation-aggregation.
        public ICollection<SectionHistory> SectionHistories { get; } = new SetWithoutNulls<SectionHistory>();

        public IReadOnlyCollection<Simulation> Simulations => _Simulations;

        public ValidatorBag Subvalidators => new ValidatorBag { Simulations };

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
