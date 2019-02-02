using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BridgeCare.Interfaces
{
    public interface IDeficientReport
    {
        DeficientResult GetData(SimulationModel data, int[] totalYears);
        DeficientResult GetDeficientInformation(SimulationModel data, Hashtable YearsIDValues, int[] totalYears);
    }
}