using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace BridgeCare.Services
{
    public class Budget
    {
        private readonly IBudgetReport budget;

        public Budget(IBudgetReport report)
        {
            budget = report ?? throw new ArgumentNullException(nameof(report));
        }

        public void Fill(ExcelWorksheet budgetReport, int[] totalYears, SimulationModel data, List<YearlyDataModel> yearlyInvestment)
        {
            var budgetTypes = budget.InvestmentData(data);
            var budgetAndCost = budget.GetData(data, budgetTypes);
            var budgetReportTable = budgetAndCost.BudgetForYear;

            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";

            var totalYearsCount = totalYears.Count();
            budgetReport.Cells["A1"].Value = "Budget";

            // Setting up columns of datatable
            DataTable viewTable = new DataTable();
            viewTable.Columns.Add();
            viewTable.Columns.Add();
            DataRow totalViewRow = viewTable.NewRow();
            for (int i = 0; i < totalYearsCount; i++)
            {
                budgetReport.Cells[1, i + 3].Value = totalYears[i];
                viewTable.Columns.Add($"{i + 2}", typeof(double));
                totalViewRow[i + 2] = 0;
            }
            totalViewRow[0] = "Total";
            totalViewRow[1] = "View";

            foreach (string budget in budgetReportTable.Keys)
            {
                Hashtable yearAndView = new Hashtable();
                // Creating row for view
                DataRow viewRow = viewTable.NewRow();
                yearAndView = (Hashtable)budgetReportTable[budget];
                viewRow[0] = budget;
                viewRow[1] = "View";
                for (int j = 0; j < totalYearsCount; j++)
                {
                    viewRow[j + 2] = 0;
                    if (yearAndView.Contains(totalYears[j]))
                    {
                        var viewData = (double)yearAndView[totalYears[j]];
                        viewRow[j + 2] = viewData;
                        totalViewRow[j + 2] = (double)totalViewRow[j + 2] + viewData;
                    }
                }
                viewTable.Rows.Add(viewRow);
            }
            viewTable.Rows.Add(totalViewRow);

            budgetReport.Cells[2, 1].LoadFromDataTable(viewTable, false);
            var lastRecord = budgetReport.Dimension.End;
            budgetReport.Cells[2, 3, lastRecord.Row, totalYearsCount + 2].Style.Numberformat.Format = currencyFormat;
            budgetReport.Cells[2, 1, lastRecord.Row, totalYearsCount + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            budgetReport.Cells[2, 1, lastRecord.Row, totalYearsCount + 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            FillTargetAndSpent(budgetReport, totalYears, budgetAndCost, budgetTypes, yearlyInvestment);
        }

        private void FillTargetAndSpent(ExcelWorksheet budgetReport, int[] totalYears, YearlyBudgetAndCost budgetAndCost,
            string[] budgetTypes, List<YearlyDataModel> yearlyInvestment)
        {
            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            var totalYearsCount = totalYears.Count();
            DataTable viewTable = new DataTable();
            viewTable.Columns.Add();
            viewTable.Columns.Add();
            DataRow totalTargetRow = viewTable.NewRow();
            DataRow totalSpentRow = viewTable.NewRow();

            var lastRow = budgetReport.Dimension.End.Row;

            for (int i = 0; i < totalYearsCount; i++)
            {
                viewTable.Columns.Add($"{i + 2}", typeof(double));
                totalTargetRow[i + 2] = 0;
                totalSpentRow[i + 2] = 0;
            }

            totalTargetRow[0] = "Total";
            totalTargetRow[1] = "Target";
            totalSpentRow[0] = "Total";
            totalSpentRow[1] = "Spent";
            foreach (var budget in budgetTypes)
            {
                // creating row for target
                DataRow targetRow = viewTable.NewRow();
                targetRow[0] = budget;
                targetRow[1] = "Target";
                var amounts = yearlyInvestment.Where(_ => _.BudgetName == budget).Select(p => p.Amount);
                int count = 2;
                foreach (var amount in amounts)
                {
                    targetRow[count] = amount;
                    totalTargetRow[count] = amount + (double)totalTargetRow[count];
                    count++;
                }
                budgetReport.InsertRow(lastRow + 1, 1);
                viewTable.Rows.Add(targetRow);
                budgetReport.Cells[lastRow + 1, 1].LoadFromDataTable(viewTable, false);
                budgetReport.Cells[lastRow + 1, 1, lastRow + 1, totalYearsCount + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                budgetReport.Cells[lastRow + 1, 1, lastRow + 1, totalYearsCount + 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                // creating row for spend amount
                DataRow spentRow = viewTable.NewRow();
                spentRow[0] = budget;
                spentRow[1] = "Spent";
                var budgetOvershoot = new List<int>();
                for (int i = 0; i < totalYearsCount; i++)
                {
                    var sumOfCosts = budgetAndCost.CostDetails.Where(_ => _.Years == totalYears[i] && _.Budget == budget)
                         .Select(p => p.Cost).Sum();
                    var target = targetRow[i + 2] != null ? (double)targetRow[i + 2] : 0;
                    if (sumOfCosts > target)
                    {
                        budgetOvershoot.Add(i);
                    }
                    spentRow[i + 2] = sumOfCosts;
                    totalSpentRow[i + 2] = sumOfCosts + (double)totalSpentRow[i + 2];
                }
                budgetReport.InsertRow(lastRow + 1, 1);
                viewTable.Rows.Remove(targetRow);
                viewTable.Rows.Add(spentRow);
                budgetReport.Cells[lastRow + 1, 1].LoadFromDataTable(viewTable, false);

                void setColor(int overShoot)
                {
                    budgetReport.Cells[lastRow + 1, overShoot + 3].Style.Font.Color.SetColor(Color.Red);
                }

                budgetOvershoot.ForEach(setColor);
                viewTable.Rows.Remove(spentRow);
            }
            budgetReport.InsertRow(lastRow + 1, 2);

            viewTable.Rows.Add(totalTargetRow);
            viewTable.Rows.Add(totalSpentRow);
            budgetReport.Cells[lastRow + 1, 1].LoadFromDataTable(viewTable, false);
            budgetReport.Cells[lastRow + 1, 1, lastRow + 2, totalYearsCount + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            budgetReport.Cells[lastRow + 1, 1, lastRow + 2, totalYearsCount + 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            for (int i = 0; i < totalYearsCount; i++)
            {
                if ((double)totalSpentRow.ItemArray[i + 2] > (double)totalTargetRow.ItemArray[i + 2])
                {
                    budgetReport.Cells[lastRow + 2, i + 3].Style.Font.Color.SetColor(Color.Red);
                }
            }
            budgetReport.Cells[lastRow + 1, 3, budgetReport.Dimension.End.Row, totalYearsCount + 2].Style.Numberformat.Format = currencyFormat;

            budgetReport.Cells.AutoFitColumns();
        }
    }
}