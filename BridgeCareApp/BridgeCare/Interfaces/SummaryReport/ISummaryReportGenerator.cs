using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISummaryReportGenerator
    {
        byte[] GenerateExcelReport(SimulationModel simulationModel);
    }
}
