using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Services
{
    public class Detailed
    {
        private Dictionary<bool, Action<DetailReportModel, ExcelWorksheet>> ExcelValues = new Dictionary<bool, Action<DetailReportModel, ExcelWorksheet>>();
        private readonly IDetailedReport detailedReport;
        private readonly FillDetailedSheet fillWorkSheet;

        public Detailed(IDetailedReport yearlyReport, FillDetailedSheet sheet)
        {
            detailedReport = yearlyReport ?? throw new ArgumentNullException(nameof(yearlyReport));
            fillWorkSheet = sheet ?? throw new ArgumentNullException(nameof(sheet));

            ExcelValues.Add(true, fillWorkSheet.OnCommittedTrue);
            ExcelValues.Add(false, fillWorkSheet.OnCommittedFalse);
        }

        public void Fill(ExcelWorksheet worksheet, int[] totalYears, SimulationModel data, BridgeCareContext dbContext)
        {
            var totalYearsCount = totalYears.Count();
            var rawQueryForData = detailedReport.GetRawQuery(data, dbContext);

            var headers = new List<string>
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

            var conditionalData = new DetailReportModel();
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