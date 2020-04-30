using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Explorer
    {
        public List<CalculatedField> CalculatedFields { get; }

        public List<Network> Networks { get; }

        public List<NumberAttribute> NumberAttributes { get; }

        public List<TextAttribute> TextAttributes { get; }
    }
}
