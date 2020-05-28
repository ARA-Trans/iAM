using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Network : IValidator
    {
        public Network(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

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

                var facilities = Facilities.ToArray();
                if (facilities.Select(facility => facility.Name).Distinct().Count() < facilities.Length)
                {
                    results.Add(ValidationStatus.Error, "Multiple facilities have the same name.", this, nameof(Facilities));
                }

                if (Sections.Count == 0)
                {
                    results.Add(ValidationStatus.Error, "There are no sections.", this, nameof(Sections));
                }
                else if (Sections.Select(section => (section.Facility, section.Name)).Distinct().Count() < Sections.Count)
                {
                    results.Add(ValidationStatus.Error, "At least one facility has multiple sections that have the same name.", this, nameof(Sections));
                }

                return results;
            }
        }

        public Explorer Explorer { get; }

        public IEnumerable<Facility> Facilities => Sections.Select(section => section.Facility).Distinct();

        public string Name { get; set; }

        public ICollection<Section> Sections { get; } = new SetWithoutNulls<Section>();

        public IReadOnlyCollection<Simulation> Simulations => _Simulations;

        public ValidatorBag Subvalidators => new ValidatorBag { Sections, Simulations };

        public Simulation AddSimulation()
        {
            var simulation = new Simulation(this);
            _Simulations.Add(simulation);
            return simulation;
        }

        public bool RemoveSimulation(Simulation simulation) => _Simulations.Remove(simulation);

        private readonly List<Simulation> _Simulations = new List<Simulation>();
    }
}
