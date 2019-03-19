using System;
using System.Collections.Generic;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class BridgeWorkSummary
    {
        private readonly CostBudgetsWorkSummary costBudgetsWorkSummary;
        private readonly BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary;
        private readonly BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary;

        public BridgeWorkSummary(CostBudgetsWorkSummary costBudgetsWorkSummary, BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary, BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary)
        {
            this.costBudgetsWorkSummary = costBudgetsWorkSummary ?? throw new ArgumentNullException(nameof(costBudgetsWorkSummary));
            this.bridgesCulvertsWorkSummary = bridgesCulvertsWorkSummary ?? throw new ArgumentNullException(nameof(bridgesCulvertsWorkSummary));
            this.bridgeRateDeckAreaWorkSummary = bridgeRateDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(bridgeRateDeckAreaWorkSummary));
        }

        public void Fill(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, BridgeCareContext dbContext)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            bridgesCulvertsWorkSummary.FillBridgesCulvertsWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            bridgeRateDeckAreaWorkSummary.FillBridgeRateDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();
        }        
    }
}