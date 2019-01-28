using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Models
{
    public class DeficientOrTargetData
    {
        private Dictionary<bool, Func<SimulationResult, Hashtable, int[], DeficientOrTargetTab>> GetInfo = 
            new Dictionary<bool, Func<SimulationResult, Hashtable, int[], DeficientOrTargetTab>>();

        public DeficientOrTargetData()
        {
            GetInfo.Add(true, DeficientInfo);
            GetInfo.Add(false, TargetInfo);
        }

        public DeficientOrTargetTab GetDeficientOrTarget(SimulationResult data, int[] totalYears, bool isDeficient)
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
                Hashtable keyValuePairs = new Hashtable();
                Hashtable MetTarget;

                deficientOrTargetList = rawDeficientList.Where(_ => _.IsDeficient == true);
                if (isDeficient == false)
                {
                    deficientOrTargetList = rawDeficientList.Where(_ => _.IsDeficient == false);
                }

                foreach (var item in deficientOrTargetList)
                {
                    if (keyValuePairs.ContainsKey(item.TargetID))
                    {
                        MetTarget = (Hashtable)keyValuePairs[item.TargetID];
                    }
                    else
                    {
                        MetTarget = new Hashtable();
                        keyValuePairs.Add(item.TargetID, MetTarget);
                    }
                    MetTarget.Add(item.Years, item.TargetMet);
                }
                Results = GetInfo[isDeficient].Invoke(data, keyValuePairs, totalYears);
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

        public DeficientOrTargetTab DeficientInfo(SimulationResult data, Hashtable YearsIDValues, int[] totalYears)
        {
            BridgeCareContext db = new BridgeCareContext();
            var deficientTableData = db.Deficient.AsNoTracking().Where(_ => _.SimulationID == data.SimulationId);
            var totalYearCount = totalYears.Count();
            DataTable DeficientTable = new DataTable();
            DeficientTable.Columns.Add("Attribute");
            DeficientTable.Columns.Add("Group");
            Dictionary<int, List<int>> deficientList = new Dictionary<int, List<int>>();
            for (int i = 0; i < totalYearCount; i++)
            {
                DeficientTable.Columns.Add(totalYears[i].ToString());
            }
            var increment = 0;
            foreach (var item in deficientTableData)
            {
                Hashtable yearValues = new Hashtable();
                yearValues = (Hashtable)YearsIDValues[item.Id_];

                DataRow newDataRow = DeficientTable.NewRow();
                newDataRow["Attribute"] = item.Attribute_;
                newDataRow["Group"] = item.DeficientName;

                List<int> listDeficient = new List<int>();

                foreach (int key in yearValues.Keys)
                {
                    int column = key - totalYears[0] + 2;
                    var value = (double)yearValues[key] * 100;
                    newDataRow[column] = value + "%/" + item.PercentDeficient + "% (" + item.Deficient + ")";

                    if (value > item.PercentDeficient)
                    {
                        listDeficient.Add(column);
                    }
                }
                DeficientTable.Rows.Add(newDataRow);
                deficientList.Add(increment, listDeficient);
                increment++;
            }
            DeficientOrTargetTab dataForDeficientTAB = new DeficientOrTargetTab();
            dataForDeficientTAB.DeficientOrTarget = DeficientTable;
            dataForDeficientTAB.DeficientOrGreen = deficientList;

            db.Dispose();
            return dataForDeficientTAB;
        }

        public DeficientOrTargetTab TargetInfo(SimulationResult data, Hashtable YearsIDValues, int[] totalYears)
        {
            BridgeCareContext db = new BridgeCareContext();
            var targetData = db.Target.AsNoTracking().Where(_ => _.SimulationID == data.SimulationId);
            var attributeData = db.Attributes.AsNoTracking();
            Dictionary<string, bool> attributeOrder = new Dictionary<string, bool>();

            // [TODO] : Ask Chad - are Attributes in Attributes_ table unique or not.
            foreach (var attribute in attributeData)
            {
                attributeOrder.Add(attribute.Attribute_, attribute.Ascending);
            }

            List<string> listRows = new List<string>();
            Hashtable idTargets = new Hashtable();

            foreach (var item in targetData)
            {
                TargetParameters target = new TargetParameters();
                target.Id = item.Id_;
                target.Attribute = item.Attribute_;
                target.TargetMean = item.TargetMean;
                target.Name = item.TargetName;
                target.Criteria = item.Criteria;
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

            Dictionary<int, List<int>> excelFillCoral = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> excelFillGreen = new Dictionary<int, List<int>>();
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
            DeficientOrTargetTab dataForTargetTAB = new DeficientOrTargetTab
            {
                DeficientOrTarget = targetTable,
                CoralData = excelFillCoral,
                DeficientOrGreen = excelFillGreen
            };
            db.Dispose();
            return dataForTargetTAB;
        }

        public class DeficientOrTargetTab
        {
            public DataTable DeficientOrTarget { get; set; } = new DataTable();
            public Dictionary<int, List<int>> DeficientOrGreen = new Dictionary<int, List<int>>();
            public Dictionary<int, List<int>> CoralData = new Dictionary<int, List<int>>();
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
    }
}