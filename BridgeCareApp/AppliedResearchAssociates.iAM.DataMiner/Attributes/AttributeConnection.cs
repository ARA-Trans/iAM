using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class AttributeConnection
    {
        public abstract void Connect();

        public abstract (Location location, string value) GetNext();
    }
}
