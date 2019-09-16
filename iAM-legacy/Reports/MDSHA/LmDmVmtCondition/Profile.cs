using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Reports.MDSHA.LmDmVmtCondition
{
    using AttributeFunc = Func<double, Tuple<double, string>>;
    using ConditionFunc = Func<double, string>;

    /// <summary>
    ///     Encapsulates all data needed to store a single custom index
    ///     settings profile for this report type.
    /// </summary>
    public class Profile
    {
        public Profile()
        {
            this.ProfileName = string.Empty;

            this.NetworkId = string.Empty;
            this.NetworkName = string.Empty;
            this.SimulationId = string.Empty;
            this.SimulationName = string.Empty;

            this.AttributeIndexes = new List<AttributeIndex>();
            this.DerivedIndexes = new List<DerivedIndex>();

            this.DestinationFolder = string.Empty;

            this.AttributeFunctions = new Dictionary<string, AttributeFunc>();
        }

        public Profile(Profile that)
        {
            this.ProfileName = that.ProfileName;

            this.NetworkId = that.NetworkId;
            this.NetworkName = that.NetworkName;
            this.SimulationId = that.SimulationId;
            this.SimulationName = that.SimulationName;

            this.AttributeIndexes =
                that.AttributeIndexes.Select(i => i.ConcreteCopy()).ToList();

            this.DerivedIndexes =
                that.DerivedIndexes.Select(i => i.ConcreteCopy()).ToList();

            this.DestinationFolder = that.DestinationFolder;

            this.AttributeFunctions =
                new Dictionary<string, AttributeFunc>(that.AttributeFunctions);
        }

        public string ProfileName { get; set; }

        public string NetworkId { get; set; }

        public string NetworkName { get; set; }

        public string SimulationId { get; set; }

        public string SimulationName { get; set; }

        public List<AttributeIndex> AttributeIndexes { get; set; }

        public List<DerivedIndex> DerivedIndexes { get; set; }

        public string DestinationFolder { get; set; }

        [JsonIgnore]
        public IEnumerable<IIndex> AllIndexes
        {
            get
            {
                return
                    this.AttributeIndexes
                    .Concat<IIndex>(this.DerivedIndexes);
            }
        }

        [JsonIgnore]
        public Dictionary<string, AttributeFunc> AttributeFunctions { get; set; }
    }
}
