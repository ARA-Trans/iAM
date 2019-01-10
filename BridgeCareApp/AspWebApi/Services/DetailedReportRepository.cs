using AspWebApi.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AspWebApi.Services
{
    public class DetailedReportRepository
    {
        private Dictionary<bool, Action<ConditionalData, ExcelWorksheet>> ExcelValues = new Dictionary<bool, Action<ConditionalData, ExcelWorksheet>>();

        public Action<ConditionalData, ExcelWorksheet> OnCommittedFalse = (conditionalData, worksheet) =>
        {
            if (conditionalData.NumberTreatment == 0)
            {
                return; // Guard clause
            }
            int rowNumber = conditionalData.RowNumber;
            int columnNumber = conditionalData.ColumnNumber;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            if (conditionalData.Treatment == "No Treatment")
            {
                worksheet.Cells[rowNumber, columnNumber + 1].Value = "-";
                worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
            }
            else
            {
                worksheet.Cells[rowNumber, columnNumber + 1].Value = conditionalData.Treatment;
                worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }
        };

        public Action<ConditionalData, ExcelWorksheet> OnCommittedTrue = (conditionalData, worksheet) =>
        {
            int rowNumber = conditionalData.RowNumber;
            int columnNumber = conditionalData.ColumnNumber;
            worksheet.Cells[rowNumber, columnNumber + 1].Value = conditionalData.Treatment;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.BackgroundColor.SetColor(Color.Red);
        };

        public DetailedReportRepository()
        {
            ExcelValues.Add(true, OnCommittedTrue);
            ExcelValues.Add(false, OnCommittedFalse);
        }

        public byte[] CreateExcelReport(ReportData data)
        {
            DetailedReportModel detailedReportModel = new DetailedReportModel();
            int[] totalYears = { };
            List<DetailedReportModel> dataForExcel = new List<DetailedReportModel>();

            // Getting data from the database
            var yearsForBudget = detailedReportModel.GetYearsData(data);
            totalYears = yearsForBudget.ToArray();

            dataForExcel = detailedReportModel.GetDetailedReportData(data);

            var totalYearsCount = totalYears.Count();

            List<string> headers = new List<string>();
            headers.Add("Facility");
            headers.Add("Section");
            for (int i = 0; i < totalYearsCount; i++)
            {
                headers.Add(totalYears[i].ToString());
            }

            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("New.xlsx")))
            {
                //create a WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                for (int x = 0; x < headers.Count; x++)
                {
                    worksheet.Cells[1, x + 1].Value = headers[x];
                }
                int rowsToColumns = 0, columnNumber = 2, rowNumber = 2;
                ConditionalData conditionalData = new ConditionalData();

                for (int j = 0; j < dataForExcel.Count; j++)
                {
                    if (rowsToColumns == 0)
                    {
                        columnNumber = 2;
                        worksheet.Cells[rowNumber, columnNumber - 1].Value = dataForExcel[j].Facility;
                        worksheet.Cells[rowNumber, columnNumber].Value = dataForExcel[j].Section;
                    }
                    conditionalData.Treatment = dataForExcel[j].Treatment;
                    conditionalData.IsCommitted = dataForExcel[j].IsCommitted;
                    conditionalData.NumberTreatment = dataForExcel[j].NumberTreatment;
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
                return excelPackage.GetAsByteArray();
            }
        }

        public class ConditionalData
        {
            public string Treatment { get; set; }
            public bool IsCommitted { get; set; }
            public int NumberTreatment { get; set; }
            public int RowNumber { get; set; }
            public int ColumnNumber { get; set; }
        }
    }
}