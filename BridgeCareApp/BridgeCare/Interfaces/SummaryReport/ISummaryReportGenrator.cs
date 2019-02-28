using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISummaryReportGenerator
    {
        byte[] GenerateExcelReport(SimulationModel simulationModel);
    }
}