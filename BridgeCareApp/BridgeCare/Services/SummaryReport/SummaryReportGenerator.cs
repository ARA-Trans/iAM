using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Properties;
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

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SummaryReportGenerator));

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
        public void GenerateExcelReport(SimulationModel simulationModel)
        {
            // Get data
            var simulationId = simulationModel.simulationId;
           var simulationYearsModel = commonSummaryReportData.GetSimulationYearsData((int)simulationId);
            var simulationYears = simulationYearsModel.Years;
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
                    .Set(s => s.status, "Begin report generation");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Simulation parameters TAB
                var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");
                summaryReportParameters.Fill(parametersWorksheet, simulationModel);

                // Bridge Data tab
                var bridgeDataModels = new List<BridgeDataModel>();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
                var workSummaryModel = summaryReportBridgeData.Fill(worksheet, simulationModel, simulationYears, dbContext);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - Bridge data TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Bridge Work Summary tab
                var bridgeWorkSummaryWorkSheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
                var chartRowsModel = bridgeWorkSummary.Fill(bridgeWorkSummaryWorkSheet, workSummaryModel.SimulationDataModels, workSummaryModel.BridgeDataModels, simulationYears, dbContext, (int)simulationId, workSummaryModel.Treatments);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - work summary TAB");
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

                // Condition Bridge Cnt tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Condition Bridge Cnt");
                conditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalBridgeCountSectionYearsRow, simulationYearsCount);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report generation - condition bridge count TAB");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);

                // Condition DA tab
                worksheet = excelPackage.Workbook.Worksheets.Add("Condition DA");
                conditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorkSheet, chartRowsModel.TotalDeckAreaSectionYearsRow, simulationYearsCount);

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

                var folderPath = "DownloadedReports";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "SummaryReport.xlsx");
                byte[] bin = excelPackage.GetAsByteArray();
                File.WriteAllBytes(filePath, bin);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Report has been generated");
                simulations.UpdateOne(s => s.simulationId == simulationId, updateStatus);
            }
        }

        public byte[] DownloadExcelReport(SimulationModel simulationModel)
        {
            var folderPath = "DownloadedReports";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "SummaryReport.xlsx");
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
