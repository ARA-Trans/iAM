using System.Collections;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Section : IValidator
    {
        public static string AreaIdentifier => "AREA";

        public double Area { get; set; }

        public string AreaUnit
        {
            get => _AreaUnit;
            set => _AreaUnit = value?.Trim() ?? "";
        }

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (double.IsNaN(Area))
                {
                    results.Add(ValidationStatus.Error, "Area is not a number.", this, nameof(Area));
                }
                else if (double.IsInfinity(Area))
                {
                    results.Add(ValidationStatus.Error, "Area is infinite.", this, nameof(Area));
                }
                else if (Area <= 0)
                {
                    results.Add(ValidationStatus.Error, "Area is less than or equal to zero.", this, nameof(Area));
                }

                if (Facility == null)
                {
                    results.Add(ValidationStatus.Warning, "Facility is unset.", this, nameof(Facility));
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
                }

                return results;
            }
        }

        public Facility Facility { get; set; }

        public IEnumerable<Attribute> HistoricalAttributes => HistoryPerAttribute.Keys;

        public string Name { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { Facility };

        public void ClearHistory() => HistoryPerAttribute.Clear();

        public IDictionary<int, T> GetAttributeHistory<T>(Attribute<T> attribute)
        {
            if (!HistoryPerAttribute.TryGetValue(attribute, out var history))
            {
                history = new Dictionary<int, T>();
                HistoryPerAttribute.Add(attribute, history);
            }

            return (Dictionary<int, T>)history;
        }

        public bool RemoveAttributeHistory(Attribute attribute) => HistoryPerAttribute.Remove(attribute);

        private readonly Dictionary<Attribute, IDictionary> HistoryPerAttribute = new Dictionary<Attribute, IDictionary>();

        private string _AreaUnit = "";
    }
}
