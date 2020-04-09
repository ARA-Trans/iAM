using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport.BridgeData
{
    public class HighlightWorkDoneCells
    {
        private readonly ExcelHelper excelHelper;

        public HighlightWorkDoneCells(ExcelHelper excelHelper)
        {
            this.excelHelper = excelHelper;
        }
        internal void CheckConditions(int parallelBridge, string treatment,
            Dictionary<int, int> projectPickByYear, int year, int index, string project, ExcelWorksheet worksheet, int row, int column)
        {
            if (treatment.Length > 0 && project.ToLower() != "no treatment")
            {
                var range = worksheet.Cells[row, column];
                ParallelBridgeBAMs(parallelBridge, projectPickByYear[year], range);
                CashFlowedBridge(projectPickByYear[year], range);
                if (index != 1 && (projectPickByYear[year] == 1 && projectPickByYear[year - 1] == 1))
                {
                    var rangeWithPreviousColumn = worksheet.Cells[row, column - 1];
                    CommittedForConsecutiveYears(rangeWithPreviousColumn);
                    CommittedForConsecutiveYears(range);
                }
                ParallelBridgeMPMS(parallelBridge, projectPickByYear[year], range);
                ParallelBridgeCashFlow(parallelBridge, projectPickByYear[year], range);
            }
        }

        private void CommittedForConsecutiveYears(ExcelRange range)
        {
            excelHelper.ApplyColor(range, Color.FromArgb(255, 153, 0));
            excelHelper.SetTextColor(range, Color.White);
        }

        private void ParallelBridgeBAMs(int isParallel, int projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == 0)
            {
                excelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                excelHelper.SetTextColor(range, Color.Black);
            }
        }
        private void ParallelBridgeCashFlow(int isParallel, int projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == 2)
            {
                excelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                excelHelper.SetTextColor(range, Color.FromArgb(255, 0, 0));
                return;
            }
        }
        private void ParallelBridgeMPMS(int isParallel, int projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == 1)
            {
                excelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                excelHelper.SetTextColor(range, Color.White);
            }
        }
        private void CashFlowedBridge(int projectPickType, ExcelRange range)
        {
            if (projectPickType == 2)
            {
                excelHelper.ApplyColor(range, Color.FromArgb(0, 255, 0));
                excelHelper.SetTextColor(range, Color.Red);
            }
        }
    }
}
