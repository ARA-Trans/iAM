using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using System;

namespace BridgeCare.Services.SummaryReport
{
    /// <summary>
    /// This class utilizes services classes for each tab to fill report data.
    /// </summary>
    public class SummaryReportGenerator : ISummaryReportGenerator
    {
        private readonly ICommonSummaryReportData commonSummaryReportData;
        private readonly SummaryReportBridgeData summaryReportBridgeData;        
        private readonly BridgeWorkSummary bridgeWorkSummary;
        private readonly ConditionBridgeCount conditionBridgeCount;

        public SummaryReportGenerator(ICommonSummaryReportData commonSummaryReportData, SummaryReportBridgeData summaryReportBridgeData, BridgeWorkSummary summaryReportBridgeWorkSummary, ConditionBridgeCount conditionBridgeCount)
        {
            this.summaryReportBridgeData = summaryReportBridgeData ?? throw new ArgumentNullException(nameof(summaryReportBridgeData));
            this.commonSummaryReportData = commonSummaryReportData ?? throw new ArgumentNullException(nameof(commonSummaryReportData));
            this.bridgeWorkSummary = summaryReportBridgeWorkSummary ?? throw new ArgumentNullException(nameof(summaryReportBridgeWorkSummary));
            this.conditionBridgeCount = conditionBridgeCount ?? throw new ArgumentNullException(nameof(conditionBridgeCount));
        }

        /// <summary>
        /// Generate Bridge Summary Report for given simulation details.
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        public byte[] GenerateExcelReport(SimulationModel simulationModel)
        {
            // TODO: This is temporary -- remove after UI is ready to invoke this report
            simulationModel = new SimulationModel { NetworkId = 13, SimulationId = 24 };

            // Get data
            var simulationYearsModel = commonSummaryReportData.GetSimulationYearsData(simulationModel.SimulationId);
            var simulationYears = simulationYearsModel.Years;
            var dbContext = new BridgeCareContext();

            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("SummaryReport.xlsx")))
            {
                // Bridge Data tab
                var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
                var simulationDataModels = summaryReportBridgeData.Fill(worksheet, simulationModel, simulationYears, dbContext);

                // Bridge Work Summary tab
                var bridgeWorkSummaryWorkSheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
                var chartRowsModel = bridgeWorkSummary.Fill(bridgeWorkSummaryWorkSheet, simulationDataModels, simulationYears, dbContext);

                // Condition Bridge Cnt tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Condition Bridge Cnt");
                conditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalBridgeCountSectionYearsRow, simulationYears.Count);
                
                
                return excelPackage.GetAsByteArray();
            }
        }
    }
}