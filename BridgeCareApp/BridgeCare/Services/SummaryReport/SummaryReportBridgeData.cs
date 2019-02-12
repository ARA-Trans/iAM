using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport
{
    public class SummaryReportBridgeData
    {
        public void Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears, BridgeCareContext dbContext)
        {
            throw new NotImplementedException();
        }
    }
}