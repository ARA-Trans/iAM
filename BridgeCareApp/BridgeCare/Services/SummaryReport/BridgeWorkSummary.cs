using System;
using System.Collections.Generic;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class BridgeWorkSummary
    {
        private readonly CostBudgetsWorkSummary CostBudgetsWorkSummary;
        private readonly BridgesCulvertsWorkSummary BridgesCulvertsWorkSummary;
        private readonly BridgeRateDeckAreaWorkSummary BridgeRateDeckAreaWorkSummary;

        public BridgeWorkSummary(CostBudgetsWorkSummary costBudgetsWorkSummary, BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary, BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary)
        {
            CostBudgetsWorkSummary = costBudgetsWorkSummary ?? throw new ArgumentNullException(nameof(costBudgetsWorkSummary));
            BridgesCulvertsWorkSummary = bridgesCulvertsWorkSummary ?? throw new ArgumentNullException(nameof(bridgesCulvertsWorkSummary));
            BridgeRateDeckAreaWorkSummary = bridgeRateDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(bridgeRateDeckAreaWorkSummary));
        }

        public void Fill(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, BridgeCareContext dbContext)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            CostBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            BridgesCulvertsWorkSummary.FillBridgesCulvertsWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            BridgeRateDeckAreaWorkSummary.FillBridgeRateDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();
        }        
    }
}