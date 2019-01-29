using BridgeCare.ApplicationLog;
using BridgeCareCodeFirst.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Models
{
    public class TargetData : ITarget
    {
        private readonly BridgeCareContext db;

        public TargetData(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Target GetTarget(SimulationResult data, int[] totalYears)
        {
            IQueryable<DeficientResult> targets = null;
            Target results = null;

            var select =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientResult>(select).AsQueryable();

                targets = rawDeficientList.Where(_ => _.IsDeficient == false);

                var dataList = new DeficientOrTarget();
                var targetAndYear = dataList.DeficientTargetList(targets);
                results = GetTargetInformation(data, targetAndYear, totalYears);
            }
            catch (SqlException ex)
            {
                ThrowError.SqlError(ex, "Target");
            }
            catch (OutOfMemoryException ex)
            {
                ThrowError.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return results;
        }
        public Target GetTargetInformation(SimulationResult data, Hashtable YearsIDValues, int[] totalYears)
        {
            var targetData = db.Target.AsNoTracking().Where(_ => _.SimulationID == data.SimulationId);
            var attributeData = db.Attributes.AsNoTracking();
            var attributeOrder = new Dictionary<string, bool>();

            foreach (var attribute in attributeData)
            {
                attributeOrder.Add(attribute.Attribute_, attribute.Ascending);
            }

            var listRows = new List<string>();
            var idTargets = new Hashtable();

            foreach (var item in targetData)
            {
                var target = new TargetParameters
                {
                    Id = item.Id_,
                    Attribute = item.Attribute_,
                    TargetMean = item.TargetMean,
                    Name = item.TargetName,
                    Criteria = item.Criteria
                };
                var rowKey = target.Attribute + "|" + target.Name + "|" + target.Criteria;

                if (!listRows.Contains(rowKey))
                {
                    listRows.Add(rowKey);
                    target.Row = listRows.Count - 1;
                }
                else
                {
                    target.Row = listRows.IndexOf(rowKey);
                }
                idTargets.Add(target.Id, target);
            }

            var totalYearCount = totalYears.Count();

            DataTable targetTable = new DataTable();
            targetTable.Columns.Add("Attribute");
            targetTable.Columns.Add("Group");
            for (int i = 0; i < totalYearCount; i++)
            {
                targetTable.Columns.Add(totalYears[i].ToString());
            }

            var excelFillCoral = new Dictionary<int, List<int>>();
            var excelFillGreen = new Dictionary<int, List<int>>();
            int increment = 2;
            foreach (var key in YearsIDValues.Keys)
            {
                Hashtable yearHashValue = new Hashtable();
                yearHashValue = (Hashtable)YearsIDValues[key];
                TargetParameters target = (TargetParameters)idTargets[key];

                DataRow newDataRow = targetTable.NewRow();
                newDataRow["Attribute"] = target.Attribute;
                newDataRow["Group"] = target.Name;

                excelFillCoral.Add(increment, new List<int>());
                excelFillGreen.Add(increment, new List<int>());

                int column = 2;
                foreach (int year in yearHashValue.Keys)
                {
                    double targetMetValue = (double)yearHashValue[year];
                    newDataRow[column] = targetMetValue + "/" + target.TargetMean;

                    if (attributeOrder.ContainsKey(target.Attribute) && !attributeOrder[target.Attribute])
                    {
                        excelFillCoral[increment].Add(column);
                        if (targetMetValue < target.TargetMean)
                        {
                            excelFillGreen[increment].Add(column);
                            excelFillCoral[increment].Remove(column);
                        }
                    }
                    else
                    {
                        excelFillCoral[increment].Add(column);
                        if (targetMetValue > target.TargetMean)
                        {
                            excelFillGreen[increment].Add(column);
                            excelFillCoral[increment].Remove(column);
                        }
                    }
                    column++;
                }
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
        public class TargetParameters
        {
            public int Id;
            public string Attribute;
            public double TargetMean;
            public string Name;
            public string Criteria;
            public int Row;
        }
        public class Target
        {
            public DataTable Targets { get; set; } = new DataTable();
            public Dictionary<int, List<int>> GreenColorFill = new Dictionary<int, List<int>>();
            public Dictionary<int, List<int>> CoralColorFill = new Dictionary<int, List<int>>();
        }
    }
}