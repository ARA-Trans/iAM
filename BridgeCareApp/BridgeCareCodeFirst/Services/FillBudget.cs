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
using System.Web;

namespace BridgeCare.Services
{
    public class FillBudget
    {
        private readonly IBudget budget;
        public FillBudget(IBudget report)
        {
            budget = report ?? throw new ArgumentNullException(nameof(report));
        }
        public void FillTotalView(ExcelWorksheet budgetReport, int[] totalYears, SimulationResult data, List<YearlyData> yearlyInvestment)
        {
            var budgetTypes = budget.InvestmentData(data);
            var budgetAndCost = budget.GetBudgetReportData(data, budgetTypes);
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
                budgetReport.InsertRow(2, 1);
                viewTable.Rows.Add(viewRow);
                budgetReport.Cells[2, 3, 2, totalYearsCount + 2].Style.Numberformat.Format = currencyFormat;
                budgetReport.Cells[2, 1, 2, totalYearsCount + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                budgetReport.Cells[2, 1, 2, totalYearsCount + 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                budgetReport.Cells[2, 1].LoadFromDataTable(viewTable, false);
                viewTable.Rows.Remove(viewRow);
            }
            viewTable.Rows.Add(totalViewRow);
            FillTargetAndSpend(budgetReport, totalYears, budgetAndCost, budgetTypes, viewTable, yearlyInvestment);
        }

        public void FillTargetAndSpend(ExcelWorksheet budgetReport, int[] totalYears, CostAndBudgets budgetAndCost,
            string[] budgetTypes, DataTable viewTable, List<YearlyData> yearlyInvestment)
        {
            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            var totalYearsCount = totalYears.Count();
            DataRow totalTargetRow = viewTable.NewRow();
            DataRow totalSpentRow = viewTable.NewRow();

            for (int i = 0; i < totalYearsCount; i++)
            {
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
                budgetReport.InsertRow(2, 1);
                viewTable.Rows.Add(targetRow);
                budgetReport.Cells[2, 3, 2, totalYearsCount + 2].Style.Numberformat.Format = currencyFormat;
                budgetReport.Cells[2, 1, 2, totalYearsCount + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                budgetReport.Cells[2, 1, 2, totalYearsCount + 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                budgetReport.Cells[2, 1].LoadFromDataTable(viewTable, false);

                // creating row for spend amount
                DataRow spentRow = viewTable.NewRow();
                spentRow[0] = budget;
                spentRow[1] = "Spent";
                var listOverBudget = new List<int>();
                for (int i = 0; i < totalYearsCount; i++)
                {
                    var sumOfCosts = budgetAndCost.CostDetails.Where(_ => _.Years == totalYears[i] && _.Budget == budget)
                         .Select(p => p.Cost).Sum();
                    var target = targetRow[i + 2] != null ? (double)targetRow[i + 2] : 0;
                    if (sumOfCosts > target)
                    {
                        listOverBudget.Add(i);
                    }
                    spentRow[i + 2] = sumOfCosts;
                    totalSpentRow[i + 2] = sumOfCosts + (double)totalSpentRow[i + 2];
                }
                budgetReport.InsertRow(2, 1);
                viewTable.Rows.Remove(targetRow);
                viewTable.Rows.Add(spentRow);
                budgetReport.Cells[2, 3, 2, totalYearsCount + 2].Style.Numberformat.Format = currencyFormat;

                foreach (var overShoot in listOverBudget)
                {
                    budgetReport.Cells[4, overShoot + 3].Style.Font.Color.SetColor(Color.Red);
                }
                budgetReport.Cells[2, 1].LoadFromDataTable(viewTable, false);
                viewTable.Rows.Remove(spentRow);
            }
            budgetReport.InsertRow(2, 3);
            
            viewTable.Rows.Add(totalTargetRow);
            viewTable.Rows.Add(totalSpentRow);
            budgetReport.Cells[2, 1, 3, totalYearsCount + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            budgetReport.Cells[2, 1, 3, totalYearsCount + 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            for (int i = 0; i < totalYearsCount; i++)
            {
                if ((double)totalSpentRow.ItemArray[i + 2] > (double)totalTargetRow.ItemArray[i + 2])
                {
                    budgetReport.Cells[4, i + 3].Style.Font.Color.SetColor(Color.Red);
                }
            }
            budgetReport.Cells[2, 3, 4, totalYearsCount + 2].Style.Numberformat.Format = currencyFormat;
            budgetReport.Cells[2, 1].LoadFromDataTable(viewTable, false);
            budgetReport.Cells.AutoFitColumns();
        }
    }
}