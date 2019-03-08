using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITarget
    {
        TargetModel GetTarget(SimulationModel data, int[] totalYears);
    }
}