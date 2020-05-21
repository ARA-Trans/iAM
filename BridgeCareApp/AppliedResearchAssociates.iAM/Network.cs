using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Network : IValidator
    {
        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Name, "Name is blank."));
                }

                if (Simulations.Select(simulation => simulation.Name).Distinct().Count() < Simulations.Count)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Simulations, "Multiple simulations have the same name."));
                }

                return results;
            }
        }

        public Explorer Explorer { get; }

        public Box<string> Name { get; } = new Box<string>();

        public ICollection<SectionHistory> SectionHistories { get; } = new ListWithoutNulls<SectionHistory>();

        public IReadOnlyCollection<Simulation> Simulations => _Simulations;

        public ICollection<IValidator> Subvalidators
        {
            get
            {
                var validators = new List<IValidator>();
                validators.AddMany(Simulations);
                return validators;
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
