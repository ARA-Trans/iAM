using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface ITarget
    {
        TargetModel GetTarget(SimulationModel data, int[] totalYears);
        TargetModel GetTargetInformation(SimulationModel data, Hashtable YearsIDValues, int[] totalYears);
    }
}
