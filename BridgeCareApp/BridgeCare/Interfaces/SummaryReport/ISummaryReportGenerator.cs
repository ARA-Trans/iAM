using BridgeCare.Models;

namespace BridgeCare.Interfaces.SummaryReport
{
    public interface ISummaryReportGenerator
    {
        byte[] GenerateExcelReport(SimulationModel data);
    }
}
