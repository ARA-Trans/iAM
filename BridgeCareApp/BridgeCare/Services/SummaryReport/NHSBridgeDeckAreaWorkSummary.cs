using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class NHSBridgeDeckAreaWorkSummary
    {
        internal void FillNHSBridgeDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ChartRowsModel chartRowsModel)
        {
            // TODO add new rows to chartRowsModel
            throw new NotImplementedException();
        }
    }
}