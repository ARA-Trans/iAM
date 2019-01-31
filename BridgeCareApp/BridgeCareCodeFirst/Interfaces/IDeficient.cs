using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BridgeCare.Interfaces
{
    public interface IDeficient
    {
        GetDeficients GetDeficient(SimulationResult data, int[] totalYears);
        GetDeficients GetDeficientInformation(SimulationResult data, Hashtable YearsIDValues, int[] totalYears);
    }
}