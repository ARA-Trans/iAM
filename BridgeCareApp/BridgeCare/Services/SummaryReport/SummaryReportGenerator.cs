using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;

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
        private readonly ConditionDeckArea conditionDeckArea;
        private readonly PoorBridgeCount poorBridgeCount;
        private readonly PoorBridgeDeckArea poorBridgeDeckArea;
        private readonly NHSConditionChart nhsConditionChart;
        private readonly SummaryReportParameters summaryReportParameters;

        public SummaryReportGenerator(ICommonSummaryReportData commonSummaryReportData, SummaryReportBridgeData summaryReportBridgeData,
            BridgeWorkSummary bridgeWorkSummary, ConditionBridgeCount conditionBridgeCount, ConditionDeckArea conditionDeckArea, PoorBridgeCount poorBridgeCount,
            PoorBridgeDeckArea poorBridgeDeckArea, NHSConditionChart nhsConditionBridgeCount, SummaryReportParameters summaryReportParameters)
        {
            this.summaryReportBridgeData = summaryReportBridgeData ?? throw new ArgumentNullException(nameof(summaryReportBridgeData));
            this.commonSummaryReportData = commonSummaryReportData ?? throw new ArgumentNullException(nameof(commonSummaryReportData));
            this.bridgeWorkSummary = bridgeWorkSummary ?? throw new ArgumentNullException(nameof(bridgeWorkSummary));
            this.conditionBridgeCount = conditionBridgeCount ?? throw new ArgumentNullException(nameof(conditionBridgeCount));
            this.conditionDeckArea = conditionDeckArea ?? throw new ArgumentNullException(nameof(conditionDeckArea));
            this.poorBridgeCount = poorBridgeCount ?? throw new ArgumentNullException(nameof(poorBridgeCount));
            this.poorBridgeDeckArea = poorBridgeDeckArea ?? throw new ArgumentNullException(nameof(poorBridgeDeckArea));
            this.nhsConditionChart = nhsConditionBridgeCount ?? throw new ArgumentNullException(nameof(nhsConditionBridgeCount));
            this.summaryReportParameters = summaryReportParameters ?? throw new ArgumentException(nameof(summaryReportParameters));
        }

        /// <summary>
        /// Generate Bridge Summary Report for given simulation details.
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        public byte[] GenerateExcelReport(SimulationModel simulationModel)
        {   
            // Get data
            var simulationId = simulationModel.SimulationId;
            var simulationYearsModel = commonSummaryReportData.GetSimulationYearsData(simulationId);
            var simulationYears = simulationYearsModel.Years;
            var simulationYearsCount = simulationYears.Count;            
            var dbContext = new BridgeCareContext();
            
            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("SummaryReport.xlsx")))
            {
                // Simulation parameters TAB
                var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");
                summaryReportParameters.Fill(parametersWorksheet, simulationModel);

                // Bridge Data tab
                var bridgeDataModels = new List<BridgeDataModel>();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
                var workSummaryModel = summaryReportBridgeData.Fill(worksheet, simulationModel, simulationYears, dbContext);

                // Bridge Work Summary tab
                var bridgeWorkSummaryWorkSheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
                var chartRowsModel = bridgeWorkSummary.Fill(bridgeWorkSummaryWorkSheet, workSummaryModel.SimulationDataModels, workSummaryModel.BridgeDataModels, simulationYears, dbContext, simulationId, workSummaryModel.Treatments);

                // NHS Condition Bridge Cnt tab
                worksheet = excelPackage.Workbook.Worksheets.Add("NHS Condition Bridge Cnt");
                nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.NHSBridgeCountPercentSectionYearsRow, Properties.Resources.NHSConditionByBridgeCountLLCC, simulationYearsCount);

                // NHS Condition DA tab
                worksheet = excelPackage.Workbook.Worksheets.Add("NHS Condition DA");
                nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.NHSBridgeDeckAreaPercentSectionYearsRow, Properties.Resources.NHSConditionByDeckAreaLLCC, simulationYearsCount);

                // Condition Bridge Cnt tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Condition Bridge Cnt");
                conditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalBridgeCountSectionYearsRow, simulationYearsCount);

                // Condition DA tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Condition DA");
                conditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalDeckAreaSectionYearsRow, simulationYearsCount);

                // Poor Bridge Cnt tab 
                worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge Cnt");
                poorBridgeCount.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalPoorBridgesCountSectionYearsRow, simulationYearsCount);

                // Poor Bridge DA tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge DA");
                poorBridgeDeckArea.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);

                return excelPackage.GetAsByteArray();
            }
        }
    }
}
