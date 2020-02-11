using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISummaryReportGenerator
    {
        void GenerateExcelReport(SimulationModel simulationModel);
    }
}
