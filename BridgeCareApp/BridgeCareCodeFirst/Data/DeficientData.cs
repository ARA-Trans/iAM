using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Data
{
    public class DeficientData : IDeficient
    {
        private readonly BridgeCareContext db;
        private readonly DeficientOrTarget getDeficients;
        private readonly CellAddress address;

        public DeficientData(BridgeCareContext context, DeficientOrTarget deficients, CellAddress cell)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            getDeficients = deficients ?? throw new ArgumentNullException(nameof(deficients));
            address = cell ?? throw new ArgumentNullException(nameof(cell));
        }
        public GetDeficients GetDeficient(SimulationResult data, int[] totalYears)
        {
            IQueryable<DeficientResult> deficients = null;
            GetDeficients Results = null;

            var select =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientResult>(select).AsQueryable();

                deficients = rawDeficientList.Where(_ => _.IsDeficient == true);

                var targetAndYear = getDeficients.DeficientTargetList(deficients);
                Results = GetDeficientInformation(data, targetAndYear, totalYears);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Target");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }
            return Results;
        }

        public GetDeficients GetDeficientInformation(SimulationResult data, Hashtable YearsIDValues, int[] totalYears)
        {
            var deficientTableData = db.Deficient.AsNoTracking().Where(_ => _.SimulationID == data.SimulationId);
            var totalYearCount = totalYears.Count();
            DataTable DeficientTable = new DataTable();
            DeficientTable.Columns.Add("Attribute");
            DeficientTable.Columns.Add("Group");
            var deficientList = new Dictionary<int, List<int>>();
            for (int i = 0; i < totalYearCount; i++)
            {
                DeficientTable.Columns.Add(totalYears[i].ToString());
            }
            var increment = 2;
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
                        address.Cells.Add((increment, column));
                    }
                }
                DeficientTable.Rows.Add(newDataRow);
                deficientList.Add(increment, deficients);
                increment++;
            }
            var forDeficient = new GetDeficients
            {
                Deficients = DeficientTable,
                DeficientColorFill = deficientList,
                Address = address
            };
            return forDeficient;
        }
    }
}