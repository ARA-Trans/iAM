using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport.ShortNameGlossary
{
    public class SummaryReportGlossary
    {
        private readonly ExcelHelper excelHelper;
        public SummaryReportGlossary(ExcelHelper excelHelper)
        {
            this.excelHelper = excelHelper;
        }

        internal void Fill(ExcelWorksheet worksheet)
        {
            var initialRow = 1;
            worksheet.Cells["A1"].Value = "Bridge Care Work Type";
            worksheet.Cells["B1"].Value = "Short Bridge Care Work Type";
            excelHelper.ApplyStyle(worksheet.Cells["A1:B1"]);

            var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();
            var row = 2;
            var column = 1;

            foreach (var treatment in abbreviatedTreatmentNames)
            {
                worksheet.Cells[row, column++].Value = treatment.Key;
                worksheet.Cells[row, column].Value = treatment.Value;
                column = 1;
                row++;
            }
            excelHelper.ApplyBorder(worksheet.Cells[initialRow, 1, row, 2]);
            row += 2;

            worksheet.Cells[row, 1].Value = "Color Key";
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            row += 2;

            worksheet.Cells[row, 1].Value = "Work Done Columns";
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            row++;

            worksheet.Cells[row, 1].Value = "Bridge being worked on has a parallel bridge - Project came from BAMS";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 204, 255));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Black);
            row++;

            worksheet.Cells[row, 1].Value = "Bridge being worked on has a parallel bridge - project is being cash flowed";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 204, 255));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(255, 0, 0));
            row++;

            worksheet.Cells[row, 1].Value = "Bridge being worked on has a parallel bridge - project came from MPMS";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 204, 255));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.White);
            row++;

            worksheet.Cells[row, 1].Value = "Bridge project is being cashed flowed";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 255, 0));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Red);
            row++;

            worksheet.Cells[row, 1].Value = "MPMS Project selected for consecutive years";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(255, 153, 0));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.White);
            row += 2;

            worksheet.Cells[row, 1].Value = "Details Colums";
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            row++;

            worksheet.Cells[row, 1].Value = "Project is being cash flowed";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 255, 0));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Black);
            row++;

            worksheet.Cells[row, 1].Value = "P3 Bridge where minimum condition is less than 5";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(255, 255, 0));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Black);
            row++;

            worksheet.Cells[row, 1].Value = "Min Condition is less than or equal to 3.5";
            excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            excelHelper.MergeCells(worksheet, row, 1, row, 2);
            excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(112, 48, 160));
            excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.White);

            row += 3;

            worksheet.Cells[row, 1].Value = "Example: ";

            worksheet.Cells[row, 2].Value = "2021";
            worksheet.Cells[row, 3].Value = "2022";
            worksheet.Cells[row, 4].Value = "2023";
            row++;

            worksheet.Cells[row, 2].Value = "Brdg_Repl";
            excelHelper.ApplyColor(worksheet.Cells[row, 2], Color.FromArgb(0, 204, 255));
            excelHelper.SetTextColor(worksheet.Cells[row, 2], Color.FromArgb(255, 0, 0));

            worksheet.Cells[row, 3].Value = "--";
            excelHelper.ApplyColor(worksheet.Cells[row, 3], Color.FromArgb(0, 255, 0));
            excelHelper.SetTextColor(worksheet.Cells[row, 3], Color.Red);

            worksheet.Cells[row, 4].Value = "--";
            excelHelper.ApplyColor(worksheet.Cells[row, 4], Color.FromArgb(0, 255, 0));
            excelHelper.SetTextColor(worksheet.Cells[row, 4], Color.Red);
            row++;
            excelHelper.ApplyBorder(worksheet.Cells[row - 1, 2, row - 1, 4]);

            worksheet.Cells.AutoFitColumns(70);
            worksheet.Cells[row, 2].Value = "(Bridge being replaced also has a parallel bridge.  Bridge replacement is cash flowed over 3 years.)";
        }
    }
}
