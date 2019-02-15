using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class SimulationYearsModel
    {
        public int SimulationID { get; set; }
        public List<int> Years { get; set; }
    }
}