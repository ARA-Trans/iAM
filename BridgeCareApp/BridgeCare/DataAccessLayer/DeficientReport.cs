using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class DeficientReport : IDeficientReport
    {
        private readonly BridgeCareContext db;
        private readonly TargetsMet deficients;
        private readonly CellAddress address;

        public DeficientReport(BridgeCareContext context, TargetsMet deficient, CellAddress cell)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            deficients = deficient ?? throw new ArgumentNullException(nameof(deficient));
            address = cell ?? throw new ArgumentNullException(nameof(cell));
        }

        public DeficientResult GetData(SimulationModel data, int[] totalYears)
        {
            // Deficient and DeficientResults are models. Deficient gets data
            // from the database. DeficientResult gets the processed data
            IQueryable<DeficientModel> deficients = null;
            DeficientResult result = null;

            var select =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientModel>(select).AsQueryable();

                deficients = rawDeficientList.Where(_ => _.IsDeficient == true);

                var targetAndYear = this.deficients.GetData(deficients);
                result = GetDeficientInformation(data, targetAndYear, totalYears);
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
            return result;
        }

        private DeficientResult GetDeficientInformation(SimulationModel data, Hashtable yearsIDValues, int[] totalYears)
        {
            var deficientTableData = db.Deficient.AsNoTracking().Where(_ => _.SimulationID == data.SimulationId);
            var totalYearCount = totalYears.Count();
            DataTable DeficientTable = new DataTable();
            DeficientTable.Columns.Add("Attribute");
            DeficientTable.Columns.Add("Group");
            for (int i = 0; i < totalYearCount; i++)
            {
                DeficientTable.Columns.Add(totalYears[i].ToString());
            }
            FillData(deficientTableData, yearsIDValues, DeficientTable, totalYears);
            var forDeficient = new DeficientResult
            {
                Deficients = DeficientTable,
                Address = address
            };
            return forDeficient;
        }

        private void FillData(IQueryable<Deficients> deficientTableData, Hashtable yearsIDValues, DataTable DeficientTable, int[] totalYears)
        {
            var increment = 2;
            foreach (var item in deficientTableData)
            {
                Hashtable yearValues = new Hashtable();
                yearValues = (Hashtable)yearsIDValues[item.Id_];

                DataRow newDataRow = DeficientTable.NewRow();
                newDataRow["Attribute"] = item.Attribute_;
                newDataRow["Group"] = item.DeficientName;

                if (yearValues.Count <= 0)
                {
                    continue;
                }
                foreach (int key in yearValues.Keys)
                {
                    int column = key - totalYears[0] + 2;
                    var value = (double)yearValues[key] * 100;
                    newDataRow[column] = value + "%/" + item.PercentDeficient + "% (" + item.Deficient + ")";

                    if (value > item.PercentDeficient)
                    {
                        address.Cells.Add((increment, column));
                    }
                }
                DeficientTable.Rows.Add(newDataRow);
                increment++;
            }
        }
    }
}