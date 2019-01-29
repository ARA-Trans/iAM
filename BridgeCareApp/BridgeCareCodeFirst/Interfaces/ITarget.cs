using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BridgeCare.Models.TargetData;

namespace BridgeCareCodeFirst.Interfaces
{
    public interface ITarget
    {
        Target GetTarget(SimulationResult data, int[] totalYears);
        Target GetTargetInformation(SimulationResult data, Hashtable YearsIDValues, int[] totalYears);
    }
}
