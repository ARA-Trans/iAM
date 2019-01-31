using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BridgeCare.Data
{
    public class GetTargetCellsData
    {
        private readonly BridgeCareContext db;
        private readonly ExcelFillCoral excelFillCoral;

        public GetTargetCellsData(BridgeCareContext context, ExcelFillCoral coral)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            excelFillCoral = coral ?? throw new ArgumentNullException(nameof(coral));
        }
        public Target GetCells(Hashtable YearsIDValues, int[] totalYears, Hashtable idTargets)
        {
            var attributeData = db.Attributes.AsNoTracking();
            var attributeOrder = new Dictionary<string, bool>();

            foreach (var attribute in attributeData)
            {
                attributeOrder.Add(attribute.Attribute_, attribute.Ascending);
            }

            var totalYearCount = totalYears.Count();
            DataTable targetTable = new DataTable();
            targetTable.Columns.Add("Attribute");
            targetTable.Columns.Add("Group");
            for (int i = 0; i < totalYearCount; i++)
            {
                targetTable.Columns.Add(totalYears[i].ToString());
            }

            //var excelFillCoral = new Dictionary<int, List<int>>();
            var excelFillGreen = new Dictionary<int, List<int>>();
            int increment = 2;
            foreach (var key in YearsIDValues.Keys)
            {
                Hashtable yearHashValue = new Hashtable();
                yearHashValue = (Hashtable)YearsIDValues[key];
                TargetParameters target = (TargetParameters)idTargets[key];
                var columns = new List<int>();

                DataRow newDataRow = targetTable.NewRow();
                newDataRow["Attribute"] = target.Attribute;
                newDataRow["Group"] = target.Name;
                excelFillCoral.Row.Add(increment);
                //excelFillCoral.Add(increment, new List<int>());
                excelFillGreen.Add(increment, new List<int>());

                int column = 2;
                foreach (int year in yearHashValue.Keys)
                {
                    double targetMetValue = (double)yearHashValue[year];
                    newDataRow[column] = targetMetValue + "/" + target.TargetMean;
                    if (attributeOrder.ContainsKey(target.Attribute) && !attributeOrder[target.Attribute])
                    {
                        columns.Add(column);
                        //excelFillCoral[increment].Add(column);
                        if (targetMetValue < target.TargetMean)
                        {
                            excelFillGreen[increment].Add(column);
                            //excelFillCoral[increment].Remove(column);
                        }
                    }
                    else
                    {
                        columns.Add(column);
                        //excelFillCoral[increment].Add(column);
                        if (targetMetValue > target.TargetMean)
                        {
                            excelFillGreen[increment].Add(column);
                            //excelFillCoral[increment].Remove(column);
                            columns.Remove(column);
                        }
                    }
                    column++;
                }
                excelFillCoral.CoralColumns.Add(columns);
                targetTable.Rows.Add(newDataRow);
                increment++;
            }
            var dataForTarget = new Target
            {
                Targets = targetTable,
                CoralColorFill = excelFillCoral,
                GreenColorFill = excelFillGreen
            };
            return dataForTarget;
        }
    }
}