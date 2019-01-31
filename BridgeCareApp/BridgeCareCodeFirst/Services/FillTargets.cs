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
    public class FillTargets
    {
        private readonly ITarget targets;
        public FillTargets(ITarget target)
        {
            targets = target ?? throw new ArgumentNullException(nameof(target));
        }
        public void FillTarget(ExcelWorksheet targetReport, SimulationResult data, int[] totalYears)
        {
            var targetResults = targets.GetTarget(data, totalYears);
            // Stored the cells information per row in the ExcelFillCoral class
            // Therfore the complexity is n^3 (which is not good) while traversing.
            // [Note]... By using dictionary, it can be brought down to n^2.
            foreach (var coral in targetResults.CoralColorFill.Row)
            {
                foreach (var excelColumns in targetResults.CoralColorFill.CoralColumns)
                {
                    foreach (var col in excelColumns)
                    {
                        targetReport.Cells[coral, col + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        targetReport.Cells[coral, col + 1].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);
                    }
                }
            }
            foreach (var green in targetResults.GreenColorFill)
            {
                foreach (var col in green.Value)
                {
                    targetReport.Cells[green.Key, col + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    targetReport.Cells[green.Key, col + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }
            }
            targetReport.Cells.LoadFromDataTable(targetResults.Targets, true);
            targetReport.Cells.AutoFitColumns();
        }
    }
}