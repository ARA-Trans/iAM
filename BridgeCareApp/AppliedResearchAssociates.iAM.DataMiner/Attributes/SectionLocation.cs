﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SectionLocation : Location
    {
        public SectionLocation(string uniqueIdentifier) => UniqueIdentifier = uniqueIdentifier;

        public string UniqueIdentifier { get; }
    }
}
