using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Explorer : IValidator
    {
        public IReadOnlyCollection<CalculatedField> CalculatedFields => _CalculatedFields;

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Networks.Select(network => network.Name).Distinct().Count() < Networks.Count)
                {
                    results.Add(ValidationStatus.Error, "Multiple networks have the same name.", this, nameof(Networks));
                }

                return results;
            }
        }

        public IReadOnlyCollection<Network> Networks => _Networks;

        public IReadOnlyCollection<NumberAttribute> NumberAttributes => _NumberAttributes;

        public ValidatorBag Subvalidators => new ValidatorBag { CalculatedFields, Networks, NumberAttributes, TextAttributes };

        public IReadOnlyCollection<TextAttribute> TextAttributes => _TextAttributes;

        public bool AddAttribute(string name, out CalculatedField attribute)
        {
            attribute = new CalculatedField(this);
            return AddAttribute(name, ref attribute, _CalculatedFields, CalculateEvaluateParameterType.Number);
        }

        public bool AddAttribute(string name, out NumberAttribute attribute)
        {
            attribute = new NumberAttribute(this);
            return AddAttribute(name, ref attribute, _NumberAttributes, CalculateEvaluateParameterType.Number);
        }

        public bool AddAttribute(string name, out TextAttribute attribute)
        {
            attribute = new TextAttribute(this);
            return AddAttribute(name, ref attribute, _TextAttributes, CalculateEvaluateParameterType.Text);
        }

        public Network AddNetwork()
        {
            var network = new Network(this);
            _Networks.Add(network);
            return network;
        }

        public bool RemoveAttribute(CalculatedField attribute) => RemoveAttribute(attribute, _CalculatedFields);

        public bool RemoveAttribute(NumberAttribute attribute) => RemoveAttribute(attribute, _NumberAttributes);

        public bool RemoveAttribute(TextAttribute attribute) => RemoveAttribute(attribute, _TextAttributes);

        public bool RemoveNetwork(Network network) => _Networks.Remove(network);

        internal IEnumerable<Attribute> AllAttributes => CalculatedFields.Concat<Attribute>(NumberAttributes).Concat(TextAttributes);

        internal CalculateEvaluateCompiler Compiler { get; } = new CalculateEvaluateCompiler();

        internal void RemoveParameterType(string parameterName)
        {
            if (!Compiler.ParameterTypes.Remove(parameterName))
            {
                throw new InvalidOperationException("Failed to remove parameter from compiler.");
            }
        }

        private readonly List<CalculatedField> _CalculatedFields = new List<CalculatedField>();

        private readonly List<Network> _Networks = new List<Network>();

        private readonly List<NumberAttribute> _NumberAttributes = new List<NumberAttribute>();

        private readonly List<TextAttribute> _TextAttributes = new List<TextAttribute>();

        private bool AddAttribute<T>(string name, ref T attribute, ICollection<T> attributes, CalculateEvaluateParameterType parameterType) where T : Attribute
        {
            if (!attribute.UpdateName(name))
            {
                attribute = default;
                return false;
            }

            Compiler.ParameterTypes.Add(attribute.Name, parameterType);
            attributes.Add(attribute);
            return true;
        }

        private bool RemoveAttribute<T>(T attribute, ICollection<T> attributes) where T : Attribute
        {
            if (!attributes.Remove(attribute))
            {
                return false;
            }

            RemoveParameterType(attribute.Name);
            return true;
        }
    }
}
