using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

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

            int increment = 2;
            foreach (var result in deficientResults.DeficientColorFill)
            {
                for (int k = 2; k < totalYearsCount + 2; k++)
                {
                    if (result.Value.Contains(k))
                    {
                        deficientReport.Cells[increment, k + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        deficientReport.Cells[increment, k + 1].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);
                    }
                    else
                    {
                        deficientReport.Cells[increment, k + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        deficientReport.Cells[increment, k + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                    }
                }
                increment++;
            }

            deficientReport.Cells.LoadFromDataTable(deficientResults.Deficients, true);
            deficientReport.Cells.AutoFitColumns();
        }
    }
}