using BridgeCare.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using static BridgeCare.Services.DetailedReportRepository;

namespace BridgeCare.Services
{
    public class FillWorkSheet
    {
        public readonly Action<ConditionalData, ExcelWorksheet> OnCommittedFalse = (conditionalData, worksheet) =>
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

        public readonly Action<ConditionalData, ExcelWorksheet> OnCommittedTrue = (conditionalData, worksheet) =>
        {
            int rowNumber = conditionalData.RowNumber;
            int columnNumber = conditionalData.ColumnNumber;
            worksheet.Cells[rowNumber, columnNumber + 1].Value = conditionalData.Treatment;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.BackgroundColor.SetColor(Color.Red);
        };
    }
}