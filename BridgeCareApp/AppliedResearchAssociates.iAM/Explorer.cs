using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Explorer
    {
        public List<CalculatedField> CalculatedFields { get; }

        public List<Network> Networks { get; }

        public IReadOnlyCollection<NumberAttribute> NumberAttributes => _NumberAttributes;

        public IReadOnlyCollection<TextAttribute> TextAttributes => _TextAttributes;

        internal IReadOnlyDictionary<string, Attribute> AttributesByName => _AttributesByName;

        internal CalculateEvaluateCompiler Compiler { get; } = new CalculateEvaluateCompiler();

        private readonly Dictionary<string, Attribute> _AttributesByName = new Dictionary<string, Attribute>();

        private readonly List<NumberAttribute> _NumberAttributes = new List<NumberAttribute>();

        private readonly List<TextAttribute> _TextAttributes = new List<TextAttribute>();
    }
}
