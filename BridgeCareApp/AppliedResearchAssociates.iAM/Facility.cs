﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Facility : IValidator
    {
        public Facility(Network network) => Network = network ?? throw new ArgumentNullException(nameof(network));

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
                }

                if (Sections.Count == 0)
                {
                    results.Add(ValidationStatus.Error, "There are no sections.", this, nameof(Sections));
                }
                else if (Sections.Select(section => section.Name).Distinct().Count() < Sections.Count)
                {
                    results.Add(ValidationStatus.Error, "Multiple sections have the same name.", this, nameof(Sections));
                }

                return results;
            }
        }

        public string Name { get; set; }

        public Network Network { get; }

        public IReadOnlyCollection<Section> Sections => _Sections;

        public ValidatorBag Subvalidators => new ValidatorBag { Sections };

        public Section AddSection()
        {
            var section = new Section(this);
            _Sections.Add(section);
            return section;
        }

        public void ClearSections() => _Sections.Clear();

        private readonly List<Section> _Sections = new List<Section>();
    }
}
