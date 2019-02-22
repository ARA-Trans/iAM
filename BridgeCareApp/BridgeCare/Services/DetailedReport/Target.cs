﻿using BridgeCare.Interfaces;
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
    public class Target
    {
        private readonly ITarget targets;
        public Target(ITarget target)
        {
            targets = target ?? throw new ArgumentNullException(nameof(target));
        }
        internal void Fill(ExcelWorksheet targetReport, SimulationModel data, int[] totalYears)
        {
            var targetResults = targets.GetTarget(data, totalYears);

            if(targetResults.Targets.Rows.Count > 0)
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