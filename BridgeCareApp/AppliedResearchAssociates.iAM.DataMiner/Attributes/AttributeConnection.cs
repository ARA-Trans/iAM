using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class AttributeConnection
    {
        public SQLConnection SQLConnection {get; set;}

        public AttributeConnection() { }
        public AttributeConnection(SQLConnection sQLConnection)
        {
            SQLConnection = sQLConnection;
        }
    }
}
