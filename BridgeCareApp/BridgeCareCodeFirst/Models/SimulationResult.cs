using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class SimulationResult
    {
        public int SimulationId { get; set; }
        public string SimulationName { get; set; }
        public string NetworkName { get; set; }
        public int NetworkId { get; set; }
    }
}