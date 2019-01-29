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
    public class DeficientData : IDeficientData
    {
        private readonly BridgeCareContext db;

        public DeficientData(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Deficient GetDeficient(SimulationResult data, int[] totalYears)
        {
            IQueryable<DeficientResult> deficients = null;
            Deficient Results = null;

            var select =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientResult>(select).AsQueryable();

                deficients = rawDeficientList.Where(_ => _.IsDeficient == true);

                var dataList = new DeficientOrTarget();
                var targetAndYear = dataList.DeficientTargetList(deficients);
                Results = GetDeficientInformation(data, targetAndYear, totalYears);
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
                ThrowError.GeneralError(ex);
            }
            return Results;
        }

        public Deficient GetDeficientInformation(SimulationResult data, Hashtable YearsIDValues, int[] totalYears)
        {
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

                var deficients = new List<int>();

                foreach (int key in yearValues.Keys)
                {
                    int column = key - totalYears[0] + 2;
                    var value = (double)yearValues[key] * 100;
                    newDataRow[column] = value + "%/" + item.PercentDeficient + "% (" + item.Deficient + ")";

                    if (value > item.PercentDeficient)
                    {
                        deficients.Add(column);
                    }
                }
                DeficientTable.Rows.Add(newDataRow);
                deficientList.Add(increment, deficients);
                increment++;
            }
            var forDeficient = new Deficient
            {
                Deficients = DeficientTable,
                DeficientColorFill = deficientList
            };

            return forDeficient;
        }
        public class Deficient
        {
            public DataTable Deficients { get; set; } = new DataTable();
            public Dictionary<int, List<int>> DeficientColorFill = new Dictionary<int, List<int>>();
        }
    }
}