using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class TextAttribute : AttributeDatum<string>
    {
        public TextAttribute(Attribute attribute, string value, Location location) : base(attribute, value, location)
        {
        }
    }
}
