using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport.UnfundedRecommendation
{
    public class UnfundedRecommendations
    {
        private readonly ExcelHelper excelHelper;

        public UnfundedRecommendations(ExcelHelper excelHelper)
        {
            this.excelHelper = excelHelper;
        }

        internal void Fill(ExcelWorksheet unfundedRecommendationWorksheet, List<UnfundedRecommendationModel> unfundedRecommendations,
            List<BridgeDataModel> bridgeDataModels, List<int> simulationYears)
        {
            // Add excel headers to excel.
            var headers = GetHeaders();
            var currentCell = AddHeadersCells(unfundedRecommendationWorksheet, headers, simulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from top, left to right, and bottom set of data.
            using (var autoFilterCells = unfundedRecommendationWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }
            AddDynamicDataCells(unfundedRecommendationWorksheet, simulationYears, unfundedRecommendations, bridgeDataModels, currentCell);

            unfundedRecommendationWorksheet.Cells.AutoFitColumns();
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, List<int> simulationYears, List<UnfundedRecommendationModel> unfundedRecommendations,
            List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
        {
            currentCell.Row = 4; // Data starts here
            currentCell.Column = 1;
            var brKeys = unfundedRecommendations.Select(_ => _.BRKey).Distinct();

            foreach (var id in brKeys)
            {
                var perBRKeyData = unfundedRecommendations.Where(_ => _.BRKey == id).OrderBy(o => o.YEARS);
                var bridgeData = bridgeDataModels.Where(b => b.BRKey.ToString() == id).FirstOrDefault();
                if (!perBRKeyData.Select(p => p.Reason).Contains("Selected"))
                {
                    FillDataInWorkSheet(worksheet, currentCell, bridgeData);
                    foreach (var year in simulationYears)
                    {
                        if (!perBRKeyData.Where(y => y.YEARS == year).Any())
                        {
                            currentCell.Column++;
                            continue;
                        }
                        var yearlyData = perBRKeyData.Where(y => y.YEARS == year);
                        var minValue = yearlyData.Select(_ => _.TotalProjectCost).Min();
                        var selectedData = yearlyData.Where(t => t.TotalProjectCost == minValue).FirstOrDefault();

                        worksheet.Cells[currentCell.Row, currentCell.Column++].Value = selectedData.Treatment;
                    }
                }
                else
                {
                    var yearDataWithSelectedTreatment = perBRKeyData.Where(_ => _.Reason.Contains("Selected")).LastOrDefault();
                    if(!perBRKeyData.Any(_ => _.YEARS > yearDataWithSelectedTreatment.YEARS))
                    {
                        continue;
                    }
                    FillDataInWorkSheet(worksheet, currentCell, bridgeData);
                    foreach (var year in simulationYears)
                    {
                        if(year <= yearDataWithSelectedTreatment.YEARS)
                        {
                            currentCell.Column++;
                            continue;
                        }
                        if(!perBRKeyData.Where(y => y.YEARS == year).Any())
                        {
                            currentCell.Column++;
                            continue;
                        }
                        var yearlyData = perBRKeyData.Where(y => y.YEARS == year);
                        var minValue = yearlyData.Select(_ => _.TotalProjectCost).Min();
                        var selectedData = yearlyData.Where(t => t.TotalProjectCost == minValue).FirstOrDefault();

                        worksheet.Cells[currentCell.Row, currentCell.Column++].Value = selectedData.Treatment;

                    }
                }
                currentCell.Row++;
                currentCell.Column = 1;
            }
        }

        private void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, BridgeDataModel bridgeData)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            

            worksheet.Cells[row, column++].Value = bridgeData.BridgeID;
            worksheet.Cells[row, column++].Value = bridgeData.BRKey;
            worksheet.Cells[row, column++].Value = bridgeData.District;
            worksheet.Cells[row, column++].Value = bridgeData.BridgeCulvert;
            worksheet.Cells[row, column++].Value = bridgeData.DeckArea;
            worksheet.Cells[row, column++].Value = bridgeData.StructureLength;
            worksheet.Cells[row, column++].Value = bridgeData.PlanningPartner;
            worksheet.Cells[row, column++].Value = bridgeData.BridgeFamily;
            worksheet.Cells[row, column++].Value = bridgeData.NHS;
            worksheet.Cells[row, column++].Value = bridgeData.BPN;
            worksheet.Cells[row, column++].Value = bridgeData.StructureType;
            worksheet.Cells[row, column++].Value = bridgeData.FunctionalClass;
            worksheet.Cells[row, column++].Value = bridgeData.YearBuilt;
            worksheet.Cells[row, column++].Value = bridgeData.Age;
            worksheet.Cells[row, column++].Value = bridgeData.AdtTotal;
            worksheet.Cells[row, column++].Value = bridgeData.ADTOverTenThousand;
            worksheet.Cells[row, column++].Value = bridgeData.RiskScore;
            worksheet.Cells[row, column++].Value = bridgeData.P3 > 0 ? "Y" : "N";
            
            currentCell.Column = column;
        }

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers, List<int> simulationYears)
        {
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
            }
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            excelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);

            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Feasiable ";
            var column = currentCell.Column;
            var row = currentCell.Row;
            var initialColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = HeaderConstText + year + " Treatment";
                worksheet.Cells[row + 2, column].Value = year;
                excelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(244, 176, 132));
            }
            // Merge 2 rows for headers
            worksheet.Row(row).Height = 40;
            for (int cellColumn = 1; cellColumn < column + 1; cellColumn++)
            {
                excelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            excelHelper.ApplyBorder(worksheet.Cells[row, initialColumn, row + 1, worksheet.Dimension.Columns]);
            currentCell.Row = currentCell.Row + 2;
        }

        private List<string> GetHeaders()
        {
            return new List<string>
            {
                "BridgeID",
                "BRKey",
                "District",
                "Bridge (B/C)",
                "Deck Area",
                "Structure Length",
                "Planning Partner",
                "Bridge Family",
                "NHS",
                "BPN",
                "Struct Type",
                "Functional Class",
                "Year Built",
                "Age",
                "ADTT",
                "ADT Over 10,000",
                "Risk Score",
                "P3"
            };
        }
    }
}
