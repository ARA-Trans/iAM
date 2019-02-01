using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.Linq;

namespace BridgeCare.Services
{
    public class FillDeficients
    {
        private readonly IDeficient deficientResult;

        public FillDeficients(IDeficient deficient)
        {
            deficientResult = deficient ?? throw new ArgumentNullException(nameof(deficient));
        }

        public void FillDeficient(ExcelWorksheet deficientReport, SimulationResult data, int[] totalYears)
        {
            var totalYearsCount = totalYears.Count();
            var deficientResults = deficientResult.GetDeficient(data, totalYears);

            deficientReport.Cells[2, 3, deficientResults.Deficients.Rows.Count + 1, deficientResults.Deficients.Columns.Count]
                        .Style.Fill.PatternType = ExcelFillStyle.Solid;
            deficientReport.Cells[2, 3, deficientResults.Deficients.Rows.Count + 1, deficientResults.Deficients.Columns.Count]
                        .Style.Fill.BackgroundColor.SetColor(Color.LightCoral);

            foreach (var (row, column) in deficientResults.Address.Cells)
            {
                if (column > totalYearsCount + 2)
                {
                    deficientReport.Cells[row, column + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }
            }

            deficientReport.Cells.LoadFromDataTable(deficientResults.Deficients, true);
            deficientReport.Cells.AutoFitColumns();
        }
    }
}