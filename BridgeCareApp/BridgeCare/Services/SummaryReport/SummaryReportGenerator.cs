using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Properties;
using BridgeCare.Services.SummaryReport.Charts;
using BridgeCare.Services.SummaryReport.Charts.PostedCountByBPN;
using BridgeCare.Services.SummaryReport.PoorDeckAreaByBPN;
using BridgeCare.Services.SummaryReport.ShortNameGlossary;
using BridgeCare.Services.SummaryReport.WorkSummaryByBudget;
using Hangfire;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        private readonly BridgeWorkSummaryByBudget bridgeWorkSummaryByBudget;
        private readonly BridgeWorkSummaryCharts bridgeWorkSummaryCharts;
        private readonly SummaryReportGlossary summaryReportGlossary;
        private readonly NonNHSConditionBridgeCount nonNHSconditionBridgeCount;
        private readonly NonNHSConditionDeckArea nonNHSConditionDeckArea;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SummaryReportGenerator));

        public SummaryReportGenerator(ICommonSummaryReportData commonSummaryReportData, SummaryReportBridgeData summaryReportBridgeData,
            BridgeWorkSummary bridgeWorkSummary, ConditionBridgeCount conditionBridgeCount, ConditionDeckArea conditionDeckArea, PoorBridgeCount poorBridgeCount,
            PoorBridgeDeckArea poorBridgeDeckArea, NHSConditionChart nhsConditionBridgeCount, SummaryReportParameters summaryReportParameters,
            BridgeWorkSummaryByBudget workSummaryByBudget, BridgeWorkSummaryCharts bridgeWorkSummaryCharts,
            SummaryReportGlossary summaryReportGlossary, NonNHSConditionBridgeCount nonNHSconditionBridgeCount, NonNHSConditionDeckArea nonNHSConditionDeckArea)
        {
            this.summaryReportBridgeData = summaryReportBridgeData ?? throw new ArgumentNullException(nameof(summaryReportBridgeData));
            this.commonSummaryReportData = commonSummaryReportData ?? throw new ArgumentNullException(nameof(commonSummaryReportData));
            this.bridgeWorkSummary = bridgeWorkSummary ?? throw new ArgumentNullException(nameof(bridgeWorkSummary));
            this.conditionBridgeCount = conditionBridgeCount ?? throw new ArgumentNullException(nameof(conditionBridgeCount));
            this.conditionDeckArea = conditionDeckArea ?? throw new ArgumentNullException(nameof(conditionDeckArea));
            this.poorBridgeCount = poorBridgeCount ?? throw new ArgumentNullException(nameof(poorBridgeCount));
            this.poorBridgeDeckArea = poorBridgeDeckArea ?? throw new ArgumentNullException(nameof(poorBridgeDeckArea));
            nhsConditionChart = nhsConditionBridgeCount ?? throw new ArgumentNullException(nameof(nhsConditionBridgeCount));
            this.summaryReportParameters = summaryReportParameters ?? throw new ArgumentNullException(nameof(summaryReportParameters));
            bridgeWorkSummaryByBudget = workSummaryByBudget ?? throw new ArgumentNullException(nameof(workSummaryByBudget));
            this.bridgeWorkSummaryCharts = bridgeWorkSummaryCharts ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCharts));
            this.summaryReportGlossary = summaryReportGlossary ?? throw new ArgumentNullException(nameof(summaryReportGlossary));
            this.nonNHSconditionBridgeCount = nonNHSconditionBridgeCount ?? throw new ArgumentNullException(nameof(nonNHSconditionBridgeCount));
            this.nonNHSConditionDeckArea = nonNHSConditionDeckArea ?? throw new ArgumentNullException(nameof(nonNHSConditionDeckArea));
        }

        /// <summary>
        /// Generate Bridge Summary Report for given simulation details.
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public void GenerateExcelReport(SimulationModel simulationModel)
        {
            // Get data
            var simulationId = simulationModel.simulationId;
            var simulationYearsModel = commonSummaryReportData.GetSimulationYearsData((int)simulationId);
            var simulationYears = simulationYearsModel.Years;
            simulationYears.Sort();
            var simulationYearsCount = simulationYears.Count;            
            var dbContext = new BridgeCareContext();
            
            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("SummaryReport.xlsx")))
            {
#if DEBUG
                var mongoConnection = Settings.Default.MongoDBDevConnectionString;
#else
                var mongoConnection = Settings.Default.MongoDBProdConnectionString;
#endif
                var client = new MongoClient(mongoConnection);
                var MongoDatabase = client.GetDatabase("BridgeCare");
                var simulations = MongoDatabase.GetCollection<SimulationModel>("scenarios");

                var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Begin summary report generation");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Simulation parameters TAB
                var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");
                summaryReportParameters.Fill(parametersWorksheet, simulationModel, simulationYearsCount);

                // Bridge Data tab
                var bridgeDataModels = new List<BridgeDataModel>();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
                var workSummaryModel = summaryReportBridgeData.Fill(worksheet, simulationModel, simulationYears, dbContext);

                // Simulation ShortName TAB
                var shortNameWorksheet = excelPackage.Workbook.Worksheets.Add("ShortName");
                summaryReportGlossary.Fill(shortNameWorksheet);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - Bridge data TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Bridge Work Summary tab
                var bridgeWorkSummaryWorkSheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
                var chartRowsModel = bridgeWorkSummary.Fill(bridgeWorkSummaryWorkSheet, workSummaryModel.SimulationDataModels, workSummaryModel.BridgeDataModels, simulationYears, dbContext, simulationModel, workSummaryModel.Treatments);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - work summary TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Bridge work summary by Budget TAB 
                var summaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary By Budget");
                bridgeWorkSummaryByBudget.Fill(summaryByBudgetWorksheet, simulationModel, simulationYears);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - Work Summary By Budget");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // NHS Condition Bridge Cnt tab
                worksheet = excelPackage.Workbook.Worksheets.Add("NHS Condition Bridge Cnt");
                nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.NHSBridgeCountPercentSectionYearsRow, Properties.Resources.NHSConditionByBridgeCountLLCC, simulationYearsCount);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - NHS Condition TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // NHS Condition DA tab
                worksheet = excelPackage.Workbook.Worksheets.Add("NHS Condition DA");
                nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.NHSBridgeDeckAreaPercentSectionYearsRow, Properties.Resources.NHSConditionByDeckAreaLLCC, simulationYearsCount);

                // Non-NHS Condition Bridge Count
                worksheet = excelPackage.Workbook.Worksheets.Add("Non-NHS Condition Bridge Cnt");
                nonNHSconditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.NonNHSBridgeCountPercentSectionYearsRow, simulationYearsCount);

                // Non-NHS Condition DA
                worksheet = excelPackage.Workbook.Worksheets.Add("Non-NHS Condition DA");
                nonNHSConditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.NonNHSDeckAreaPercentSectionYearsRow, simulationYearsCount);

                // Condition Bridge Cnt tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Combined Condition Bridge Cnt");
                conditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalBridgeCountPercentYearsRow, simulationYearsCount);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - condition bridge count TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Condition DA tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Combined Condition DA");
                conditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalDeckAreaPercentYearsRow, simulationYearsCount);

                // Poor Bridge Cnt tab 
                worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge Cnt");
                poorBridgeCount.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalPoorBridgesCountSectionYearsRow, simulationYearsCount);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - Poor Bridge count TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Poor Bridge DA tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge DA");
                poorBridgeDeckArea.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - Poor Bridge DA TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Poor Bridge DA By BPN TAB
                worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge DA By BPN");
                bridgeWorkSummaryCharts.FillPoorDeckAreaByBPN(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalPoorDeckAreaByBPNSectionYearsRow, simulationYearsCount);

                // Posted By BPN Bridge Count TAB
                //worksheet = excelPackage.Workbook.Worksheets.Add("Posted By BPN Bridge Count");
                //bridgeWorkSummaryCharts.FillPostedBridgeCountByBPN(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalBridgePostedCountByBPNYearsRow, simulationYearsCount);

                // Posted By BPN Bridge DA
                //worksheet = excelPackage.Workbook.Worksheets.Add("Posted By BPN Bridge DA");
                //bridgeWorkSummaryCharts.FillPostedBridgeDeckAreaByBPN(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalPostedBridgeDeckAreaByBPNYearsRow, simulationYearsCount);

                // Closed By BPN Bridge count
                //worksheet = excelPackage.Workbook.Worksheets.Add("Closed By BPN Bridge count");
                //bridgeWorkSummaryCharts.FillClosedBridgeCountByBPN(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalClosedBridgeCountByBPNYearsRow, simulationYearsCount);

                // Closed By BPN Bridge Deck Area
                //worksheet = excelPackage.Workbook.Worksheets.Add("Closed By BPN Bridge DA");
                //bridgeWorkSummaryCharts.FillClosedBridgeDeckAreaByBPN(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalClosedBridgeDeckAreaByBPNYearsRow, simulationYearsCount);

                // Combiled posted and closed
                //worksheet = excelPackage.Workbook.Worksheets.Add("Combined Posted and Closed");
                //bridgeWorkSummaryCharts.FillCombinedPostedAndClosedByBPN(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalPostedAndClosedByBPNYearsRow, simulationYearsCount);

                // Cash Needed DA By BPN
                //worksheet = excelPackage.Workbook.Worksheets.Add("Cash Needed DA By BPN");
                //bridgeWorkSummaryCharts.FillCashNeededDeckAreaByBPN(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalCashNeededByBPNYearsRow, simulationYearsCount);

                var folderPathForSimulation = $"DownloadedReports\\{simulationModel.simulationId}";
                string relativeFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForSimulation);
                Directory.CreateDirectory(relativeFolderPath);
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForSimulation, "SummaryReport.xlsx");
                byte[] bin = excelPackage.GetAsByteArray();
                File.WriteAllBytes(filePath, bin);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Summary report has been generated");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);
            }
        }

        public byte[] DownloadExcelReport(SimulationModel simulationModel)
        {
            var folderPathForSimulation = $"DownloadedReports\\{simulationModel.simulationId}";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForSimulation, "SummaryReport.xlsx");
            if (File.Exists(filePath))
            {
                byte[] summaryReportData = File.ReadAllBytes(filePath);
                return summaryReportData;
            }
            log.Error($"Summary report is not available in the path {filePath}");
            //return Encoding.ASCII.GetBytes($"Summary report is not available in the path {filePath}");
            throw new FileNotFoundException($"Summary report is not available in the path {filePath}", "SummaryReport.xlsx");
        }
    }
}
