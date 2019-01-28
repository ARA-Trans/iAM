using BridgeCareCodeFirst.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Models
{
    public class DeficientData
    {
        public DeficientOrTargetTab GetDeficient(SimulationResult data, int[] totalYears)
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

                deficientOrTargetList = rawDeficientList.Where(_ => _.IsDeficient == true);

                var dataList = new DeficientOrTarget();
                var targetAndYear = dataList.DeficientTargetList(deficientOrTargetList);
                var deficient = new DeficientData();
                Results = deficient.DeficientInfo(data, targetAndYear, totalYears);
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

                var listDeficient = new List<int>();

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