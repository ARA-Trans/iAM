using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCare.Services.SummaryReport
{
    public class SummaryReportParameters
    {
        internal void Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears)
        {
            worksheet.Cells["A1:B1"].Merge = true;
            worksheet.Cells["A1:B1"].Value = "Simulation Name";
            worksheet.Cells["C1:J1"].Merge = true;
            worksheet.Cells["C1:J1"].Value = simulationModel.SimulationName;
            worksheet.Cells["A1:B1"].Style.Font.Bold = true;

            worksheet.Cells["F6:H6"].Merge = true;
            worksheet.Cells["F8:G8"].Merge = true;
            worksheet.Cells["F10:G10"].Merge = true;
            worksheet.Cells["F12:G12"].Merge = true;

            worksheet.Cells["F6:H6"].Value = "Investment:";
            worksheet.Cells["F8:G8"].Value = "Start Year:";
            worksheet.Cells["F10:G10"].Value = "Analysis Period:";
            worksheet.Cells["F12:G12"].Value = "Inflation Rate:";
            worksheet.Cells["F8:G8"].Style.Font.Bold = true;

            worksheet.Cells["L6:O6"].Merge = true;
            worksheet.Cells["L8:M8"].Merge = true;
            worksheet.Cells["L10:M10"].Merge = true;
            worksheet.Cells["L12:M12"].Merge = true;
            worksheet.Cells["L14:M14"].Merge = true;

            worksheet.Cells["N8:O8"].Merge = true;

            worksheet.Cells["L6:O6"].Value = "Analysis:";
            worksheet.Cells["L8:M8"].Value = "Optimization:";
            worksheet.Cells["L10:M10"].Value = "Budget:";
            worksheet.Cells["L12:M12"].Value = "Weighting:";
            worksheet.Cells["L14:M14"].Value = "Benefit:";

            worksheet.Cells["L8:M8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["L8:M8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["L8:M8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["L8:M8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            var optimization = worksheet.DataValidations.AddListValidation("N8:O8");
            optimization.Formula.Values.Add("Test1");
            optimization.Formula.Values.Add("Test2");
            optimization.AllowBlank = false;
            worksheet.Cells["N8:O8"].Value = "Test1";
        }
    }
}
