using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Explorer : IValidator
    {
        public Explorer() => AgeAttribute = AddNumberAttribute("AGE");

        public NumberAttribute AgeAttribute { get; }

        public IEnumerable<Attribute> AllAttributes
        {
            get
            {
                var result = CalculatedFields.Concat<Attribute>(NumberAttributes).Concat(TextAttributes);

                if (AgeAttribute != null)
                {
                    result = result.Prepend(AgeAttribute);
                }

                return result;
            }
        }

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

        public IEnumerable<INumericAttribute> NumericAttributes => CalculatedFields.Concat<INumericAttribute>(NumberAttributes);

        public ValidatorBag Subvalidators => new ValidatorBag { AgeAttribute, CalculatedFields, Networks, NumberAttributes };

        public IReadOnlyCollection<TextAttribute> TextAttributes => _TextAttributes;

        public CalculatedField AddCalculatedField(string name) => Add(name, new CalculatedField(name, this), _CalculatedFields, CalculateEvaluateParameterType.Number);

        public Network AddNetwork() => _Networks.GetAdd(new Network(this));

        public NumberAttribute AddNumberAttribute(string name) => Add(name, new NumberAttribute(name), _NumberAttributes, CalculateEvaluateParameterType.Number);

        public TextAttribute AddTextAttribute(string name) => Add(name, new TextAttribute(name), _TextAttributes, CalculateEvaluateParameterType.Text);

        public void Remove(CalculatedField attribute) => Remove(attribute, _CalculatedFields);

        public void Remove(Network network) => _ = _Networks.Remove(network);

        public void Remove(NumberAttribute attribute) => Remove(attribute, _NumberAttributes);

        public void Remove(TextAttribute attribute) => Remove(attribute, _TextAttributes);

        internal CalculateEvaluateCompiler Compiler { get; } = new CalculateEvaluateCompiler();

        private readonly List<CalculatedField> _CalculatedFields = new List<CalculatedField>();

        private readonly List<Network> _Networks = new List<Network>();

        private readonly List<NumberAttribute> _NumberAttributes = new List<NumberAttribute>();

        private readonly List<TextAttribute> _TextAttributes = new List<TextAttribute>();

        private T Add<T>(string name, T attribute, ICollection<T> attributes, CalculateEvaluateParameterType parameterType) where T : Attribute
        {
            if (AllAttributes.Any(a => a.Name == name))
            {
                throw new ArgumentException("Name is already taken by another attribute.", nameof(name));
            }

            Compiler.ParameterTypes.Add(attribute.Name, parameterType);
            attributes.Add(attribute);
            return attribute;
        }

        private void Remove<T>(T attribute, ICollection<T> attributes) where T : Attribute
        {
            if (attribute != AgeAttribute)
            {
                if (!Compiler.ParameterTypes.Remove(attribute.Name))
                {
                    throw new InvalidOperationException("Failed to remove parameter from compiler.");
                }

                _ = attributes.Remove(attribute);
            }
        }
    }
}
