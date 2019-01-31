using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Services
{
    public class FillDetailedReport
    {
        private Dictionary<bool, Action<ConditionalData, ExcelWorksheet>> ExcelValues = new Dictionary<bool, Action<ConditionalData, ExcelWorksheet>>();
        private readonly IDetailedReport detailedReport;
        private readonly FillWorkSheet fillWorkSheet;

        public FillDetailedReport(IDetailedReport yearlyReport, FillWorkSheet sheet)
        {
            detailedReport = yearlyReport ?? throw new ArgumentNullException(nameof(yearlyReport));
            fillWorkSheet = sheet ?? throw new ArgumentNullException(nameof(sheet));

            ExcelValues.Add(true, fillWorkSheet.OnCommittedTrue);
            ExcelValues.Add(false, fillWorkSheet.OnCommittedFalse);
        }
        public void FillYearlyData(ExcelWorksheet worksheet, int[] totalYears, SimulationResult data, BridgeCareContext dbContext)
        {
            var totalYearsCount = totalYears.Count();
            var rawQueryForData = detailedReport.GetDataForReport(data, dbContext);

            List<string> headers = new List<string>
            {
                "Facility",
                "Section"
            };
            for (int i = 0; i < totalYearsCount; i++)
            {
                headers.Add(totalYears[i].ToString());
            }
            for (int x = 0; x < headers.Count; x++)
            {
                worksheet.Cells[1, x + 1].Value = headers[x];
            }
            int rowsToColumns = 0, columnNumber = 2, rowNumber = 2;

            var conditionalData = new ConditionalData();
            foreach (var newData in rawQueryForData)
            {
                if (rowsToColumns == 0)
                {
                    columnNumber = 2;
                    worksheet.Cells[rowNumber, columnNumber - 1].Value = newData.Facility;
                    worksheet.Cells[rowNumber, columnNumber].Value = newData.Section;
                }
                conditionalData.Treatment = newData.Treatment;
                conditionalData.IsCommitted = newData.IsCommitted;
                conditionalData.NumberTreatment = newData.NumberTreatment;
                conditionalData.RowNumber = rowNumber;
                conditionalData.ColumnNumber = columnNumber;

                ExcelValues[conditionalData.IsCommitted].Invoke(conditionalData, worksheet);

                columnNumber++;
                if (rowsToColumns + 1 >= totalYearsCount)
                {
                    rowsToColumns = 0;
                    rowNumber++;
                }
                else
                {
                    rowsToColumns++;
                }
            }
            worksheet.Cells.AutoFitColumns();
        }
    }
}