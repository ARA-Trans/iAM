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
        private readonly BridgeWorkSummary BridgeWorkSummary;

        public SummaryReportGenerator(ICommonSummaryReportData commonSummaryReportData, SummaryReportBridgeData summaryReportBridgeData, BridgeWorkSummary summaryReportBridgeWorkSummary)
        {
            this.summaryReportBridgeData = summaryReportBridgeData ?? throw new ArgumentNullException(nameof(summaryReportBridgeData));
            this.commonSummaryReportData = commonSummaryReportData ?? throw new ArgumentNullException(nameof(commonSummaryReportData));
            this.BridgeWorkSummary=summaryReportBridgeWorkSummary?? throw new ArgumentNullException(nameof(summaryReportBridgeWorkSummary));
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
                worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
                BridgeWorkSummary.Fill(worksheet, simulationDataModels, simulationYears, dbContext);                             
                
                return excelPackage.GetAsByteArray();
            }
        }
    }
}