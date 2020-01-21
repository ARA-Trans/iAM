using System;
using System.Collections.Generic;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class BridgeWorkSummary
    {
        private readonly CostBudgetsWorkSummary costBudgetsWorkSummary;
        private readonly BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary;
        private readonly BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary;
        private readonly IBridgeWorkSummaryData bridgeWorkSummaryData;
        private readonly NHSBridgeDeckAreaWorkSummary nhsBridgeDeckAreaWorkSummary;

        public BridgeWorkSummary(CostBudgetsWorkSummary costBudgetsWorkSummary, BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary, BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary, IBridgeWorkSummaryData bridgeWorkSummaryData, NHSBridgeDeckAreaWorkSummary nhsBridgeDeckAreaWorkSummary)
        {
            this.costBudgetsWorkSummary = costBudgetsWorkSummary ?? throw new ArgumentNullException(nameof(costBudgetsWorkSummary));
            this.bridgesCulvertsWorkSummary = bridgesCulvertsWorkSummary ?? throw new ArgumentNullException(nameof(bridgesCulvertsWorkSummary));
            this.bridgeRateDeckAreaWorkSummary = bridgeRateDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(bridgeRateDeckAreaWorkSummary));
            this.bridgeWorkSummaryData = bridgeWorkSummaryData ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryData));
            this.nhsBridgeDeckAreaWorkSummary = nhsBridgeDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(nhsBridgeDeckAreaWorkSummary));
        }

        /// <summary>
        /// Fill Work Summary report
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="simulationDataModels"></param>
        /// <param name="bridgeDataModels"></param>
        /// <param name="simulationYears"></param>
        /// <param name="dbContext"></param>
        /// <param name="simulationId"></param>        
        /// <returns>ChartRowsModel object for usage in other tab reports.</returns>
        public ChartRowsModel Fill(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, List<int> simulationYears, BridgeCareContext dbContext, int simulationId, List<string> treatments)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };
            var yearlyBudgetModels = bridgeWorkSummaryData.GetYearlyBudgetModels(simulationId, dbContext);

            costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels, yearlyBudgetModels, treatments);

            bridgesCulvertsWorkSummary.FillBridgesCulvertsWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels, treatments);

            var chartRowsModel = bridgeRateDeckAreaWorkSummary.FillBridgeRateDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            nhsBridgeDeckAreaWorkSummary.FillNHSBridgeDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();
            return chartRowsModel;
        }        
    }
}