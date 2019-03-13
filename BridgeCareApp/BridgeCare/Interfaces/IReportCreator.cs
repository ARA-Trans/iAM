using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IReportCreator
    {
        byte[] CreateExcelReport(SimulationModel data);
    }
}