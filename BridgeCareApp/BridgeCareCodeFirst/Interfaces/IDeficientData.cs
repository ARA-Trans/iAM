using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static BridgeCare.Models.DeficientData;

namespace BridgeCareCodeFirst.Interfaces
{
    public interface IDeficientData
    {
        Deficient GetDeficient(SimulationResult data, int[] totalYears);
        Deficient GetDeficientInformation(SimulationResult data, Hashtable YearsIDValues, int[] totalYears);
    }
}