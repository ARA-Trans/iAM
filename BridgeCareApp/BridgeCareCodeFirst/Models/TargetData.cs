using BridgeCare;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static BridgeCare.Models.DeficientData;

namespace BridgeCareCodeFirst.Models
{
    public class TargetData
    {
        public DeficientOrTargetTab GetTarget(SimulationResult data, int[] totalYears)
        {
            var db = new BridgeCareContext();
            IQueryable<DeficientResult> deficientOrTargetList = null;
            DeficientOrTargetTab Results = null;

            string getReport =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientResult>(getReport).AsQueryable();

                deficientOrTargetList = rawDeficientList.Where(_ => _.IsDeficient == false);

                var dataList = new DeficientOrTarget();
                var targetAndYear = dataList.DeficientTargetList(deficientOrTargetList);
                var targetInfo = new TargetData();
                Results = targetInfo.TargetInfo(data, targetAndYear, totalYears);
            }
            catch (SqlException ex)
            {
                db.Dispose();
                if (ex.Number == -2 || ex.Number == 11)
                {
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    throw new InvalidOperationException("Target table does not exist in the database");
                }
            }
            catch (OutOfMemoryException)
            {
                db.Dispose();
                throw new OutOfMemoryException("The server is out of memory. Please try after some time");
            }
            catch (Exception ex)
            {
                db.Dispose();
                Console.WriteLine(ex);
            }
            db.Dispose();
            return Results;
        }
        public DeficientOrTargetTab TargetInfo(SimulationResult data, Hashtable YearsIDValues, int[] totalYears)
        {
            var db = new BridgeCareContext();
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
            var dataForTargetTab = new DeficientOrTargetTab
            {
                DeficientOrTarget = targetTable,
                CoralData = excelFillCoral,
                DeficientOrGreen = excelFillGreen
            };
            db.Dispose();
            return dataForTargetTab;
        }
    }
}