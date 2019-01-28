using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using static BridgeCare.Models.BudgetReportData;

namespace BridgeCare.Services
{
    public class DetailedReportRepository : IDetailedReport
    {
        private Dictionary<bool, Action<ConditionalData, ExcelWorksheet>> ExcelValues = new Dictionary<bool, Action<ConditionalData, ExcelWorksheet>>();

        private readonly IBudgetReportData budgetReportData;
        public DetailedReportRepository(IBudgetReportData budgetReportData)
        {
            this.budgetReportData = budgetReportData ?? throw new ArgumentNullException(nameof(budgetReportData));

            ExcelValues.Add(true, OnCommittedTrue);
            ExcelValues.Add(false, OnCommittedFalse);
        }

        private readonly Action<ConditionalData, ExcelWorksheet> OnCommittedFalse = (conditionalData, worksheet) =>
        {
            if (conditionalData.NumberTreatment == 0)
            {
                return; // Guard clause
            }
            int rowNumber = conditionalData.RowNumber;
            int columnNumber = conditionalData.ColumnNumber;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            if (conditionalData.Treatment == "No Treatment")
            {
                worksheet.Cells[rowNumber, columnNumber + 1].Value = "-";
                worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
            }
            else
            {
                worksheet.Cells[rowNumber, columnNumber + 1].Value = conditionalData.Treatment;
                worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }
        };

        private readonly Action<ConditionalData, ExcelWorksheet> OnCommittedTrue = (conditionalData, worksheet) =>
        {
            int rowNumber = conditionalData.RowNumber;
            int columnNumber = conditionalData.ColumnNumber;
            worksheet.Cells[rowNumber, columnNumber + 1].Value = conditionalData.Treatment;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[rowNumber, columnNumber + 1].Style.Fill.BackgroundColor.SetColor(Color.Red);
        };

        public byte[] CreateExcelReport(SimulationResult data)
        {
            var detailedReportData = new DetailedReport();
            // Getting data from the database
            var yearlyInvestment = detailedReportData.GetYearsData(data);

            // Fetching years data here instead of in the 'GetYearsData' method, 
            // because result of 'GetYearsData' method is used in another function
            List<int> listOfYears = new List<int>();
            foreach (var allYears in yearlyInvestment)
            {
                listOfYears.Add(allYears.Year);
            }
            var totalYears = listOfYears.Distinct().ToArray();
            var budgetTypes = budgetReportData.InvestmentData(data);
            var budgetAndCost = budgetReportData.GetBudgetReportData(data, budgetTypes);
            var budgetReportTable = budgetAndCost.BudgetForYear;
            var db = new BridgeCareContext();
            var rawQueryForData = detailedReportData.GetDataForReport(data, db);

            var totalYearsCount = totalYears.Count();

            List<string> headers = new List<string>
            {
                "Facility",
                "Section"
            };
            for (int i = 0; i < totalYearsCount; i++)
            {
                headers.Add(totalYears[i].ToString());
            }

            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("New.xlsx")))
            {
                var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
                //create a WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Detailed report");

                for (int x = 0; x < headers.Count; x++)
                {
                    worksheet.Cells[1, x + 1].Value = headers[x];
                }
                int rowsToColumns = 0, columnNumber = 2, rowNumber = 2;
                ConditionalData conditionalData = new ConditionalData();
                foreach (var newData in rawQueryForData)
                {
                    if (rowsToColumns == 0)
                    {
                        columnNumber = 2;
                        worksheet.Cells[rowNumber, columnNumber - 1].Value = newData.Facility;
                        worksheet.Cells[rowNumber, columnNumber].Value = newData.Section;
                    }
                    conditionalData.Treatment = newData.Treatment;
                    conditionalData.IsCommitted = newData.IsCommitted;
                    conditionalData.NumberTreatment = newData.NumberTreatment;
                    conditionalData.RowNumber = rowNumber;
                    conditionalData.ColumnNumber = columnNumber;

                    ExcelValues[conditionalData.IsCommitted].Invoke(conditionalData, worksheet);

                    columnNumber++;
                    if (rowsToColumns + 1 >= totalYearsCount)
                    {
                        rowsToColumns = 0;
                        rowNumber++;
                    }
                    else
                    {
                        rowsToColumns++;
                    }
                }
                db.Dispose();
                worksheet.Cells.AutoFitColumns();

                // Adding new Excel TAB for Budget results
                ExcelWorksheet budgetReport = excelPackage.Workbook.Worksheets.Add("Budget results");
                budgetReport.Cells["A1"].Value = "Budget";

                // Setting up columns of datatable
                DataTable viewTable = new DataTable();
                viewTable.Columns.Add();
                viewTable.Columns.Add();
                DataRow totalViewRow = viewTable.NewRow();
                DataRow totalTargetRow = viewTable.NewRow();
                DataRow totalSpentRow = viewTable.NewRow();
                for (int i = 0; i < totalYearsCount; i++)
                {
                    budgetReport.Cells[1, i + 3].Value = totalYears[i];
                    viewTable.Columns.Add($"{i + 2}", typeof(double));
                    totalViewRow[i + 2] = 0;
                    totalTargetRow[i + 2] = 0;
                    totalSpentRow[i + 2] = 0;
                }
                totalViewRow[0] = "Total";
                totalViewRow[1] = "View";
                totalTargetRow[0] = "Total";
                totalTargetRow[1] = "Target";
                totalSpentRow[0] = "Total";
                totalSpentRow[1] = "Spent";

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
                viewTable.Rows.Add(totalViewRow);
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

                // Adding new Excel TAB for Deficient results
                ExcelWorksheet deficientReport = excelPackage.Workbook.Worksheets.Add("Deficient results");

                DeficientOrTargetData deficientResultData = new DeficientOrTargetData();
                var deficientTABResult = deficientResultData.GetDeficientOrTarget(data, totalYears, true);

                int increment = 2;
                foreach (var result in deficientTABResult.DeficientOrGreen)
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

                deficientReport.Cells.LoadFromDataTable(deficientTABResult.DeficientOrTarget, true);
                deficientReport.Cells.AutoFitColumns();

                // Adding new Excel TAB for Target Results
                ExcelWorksheet targetReport = excelPackage.Workbook.Worksheets.Add("Target results");

                var targetTABResult = deficientResultData.GetDeficientOrTarget(data, totalYears, false);

                foreach (var coral in targetTABResult.CoralData)
                {
                    foreach (var col in coral.Value)
                    {
                        targetReport.Cells[coral.Key, col + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        targetReport.Cells[coral.Key, col + 1].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);
                    }
                }
                foreach (var green in targetTABResult.DeficientOrGreen)
                {
                    foreach (var col in green.Value)
                    {
                        targetReport.Cells[green.Key, col + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        targetReport.Cells[green.Key, col + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                    }
                }
                targetReport.Cells.LoadFromDataTable(targetTABResult.DeficientOrTarget, true);
                deficientReport.Cells.AutoFitColumns();

                return excelPackage.GetAsByteArray();
            }
        }

        public class ConditionalData
        {
            public string Treatment { get; set; }
            public bool IsCommitted { get; set; }
            public int NumberTreatment { get; set; }
            public int RowNumber { get; set; }
            public int ColumnNumber { get; set; }
        }
    }
}