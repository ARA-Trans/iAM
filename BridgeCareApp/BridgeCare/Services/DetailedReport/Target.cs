using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;

namespace BridgeCare.Services
{
    public class Target
    {
        private readonly ITarget repo;
        private readonly BridgeCareContext db;

        public Target(ITarget repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        internal void Fill(ExcelWorksheet targetReport, SimulationModel model, int[] totalYears)
        {
            var targetResults = repo.GetTarget(model, totalYears, db);

            if (targetResults.Targets.Rows.Count > 0)
            {
                targetReport.Cells[2, 3, targetResults.Targets.Rows.Count + 1, targetResults.Targets.Columns.Count]
                        .Style.Fill.PatternType = ExcelFillStyle.Solid;
                targetReport.Cells[2, 3, targetResults.Targets.Rows.Count + 1, targetResults.Targets.Columns.Count]
                            .Style.Fill.BackgroundColor.SetColor(Color.LightCoral);
            }

            foreach (var (row, column) in targetResults.Address.Cells)
            {
                targetReport.Cells[row, column + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }
            targetReport.Cells.LoadFromDataTable(targetResults.Targets, true);
            targetReport.Cells.AutoFitColumns();
        }
    }
}