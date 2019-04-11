using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ICommonSummaryReportData
    {
        SimulationYearsModel GetSimulationYearsData(int simulationId);        
    }
}