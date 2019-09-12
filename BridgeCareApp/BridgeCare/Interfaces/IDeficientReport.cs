using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficientReport
    {
        DeficientResult GetData(SimulationModel data, int[] totalYears);
    }
}