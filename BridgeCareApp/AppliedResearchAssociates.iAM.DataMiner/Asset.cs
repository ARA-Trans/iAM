using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    /// <summary>
    /// Type of asset being represented by a collection of attributes
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// Unique identifier for asset type
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Plain name for asset type 
        /// </summary>
        /// <example>Pavement</example>
        public string Name { get; }

        /// <summary>
        /// Defines the time interval used by a given asset
        /// </summary>
        public TimeInterval TimeInterval { get; set; }

    }
}
