using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer.SummaryReport
{
    public class BridgeDataDAL : IBridgeData
    {
        /// <summary>
        /// Fetches bridge data using a list of br keys
        /// </summary>
        /// <param name="brKeys">br keys list</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>BridgeDataModel list</returns>        
        public List<BridgeDataModel> GetBridgeData(List<int> brKeys, BridgeCareContext db)
        {
            var bridgeDataModels = new List<BridgeDataModel>();

            var penndotBridgeData = db.PennDotBridgeData.Where(p => brKeys.Contains(p.BRKEY)).ToList();

            var pennDotReportAData = db.PennDotReportAData.Where(p => brKeys.Contains(p.BRKEY)).ToList();

            var sdRisk = db.SdRisks.Where(s => brKeys.Contains(s.BRKEY)).ToList();

            brKeys = brKeys.OrderBy(b => b).ToList();

            foreach (var BRKey in brKeys)
            {
                var penndotBridgeDataRow = penndotBridgeData.Where(b => b.BRKEY == BRKey).FirstOrDefault();

                var pennDotReportADataRow = pennDotReportAData.Where(p => p.BRKEY == BRKey).FirstOrDefault();

                var sdRiskRow = sdRisk.Where(s => s.BRKEY == BRKey).FirstOrDefault();

                bridgeDataModels.Add(CreateBridgeDataModel(penndotBridgeDataRow, pennDotReportADataRow, sdRiskRow));
            }

            return bridgeDataModels;
        }

        /// <summary>
        /// Get Section_x dynamic table data, x = Network Id
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="dbContext"></param>
        /// <returns>IQueryable<Section></returns>
        public IQueryable<Section> GetSectionData(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            IQueryable<Section> rawQueryForSectionData = null;

            // FACILITY is BRKEY, SECTION is BRIDGE_ID
            var selectSectionStatement = "SELECT SECTIONID, FACILITY, SECTION " + " FROM SECTION_" + simulationModel.NetworkId + " Rpt WITH(NOLOCK) Order By FACILITY ASC";
            try
            {
                rawQueryForSectionData = dbContext.Database.SqlQuery<Section>(selectSectionStatement).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Section_" + simulationModel.SimulationId);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return rawQueryForSectionData;
        }

        public List<string> GetSummaryReportMissingAttributes(int simulationId, int networkId, BridgeCareContext db)
        {
            var simulationYears = db.YearlyInvestments.Where(y => y.SIMULATIONID == simulationId)
                .Select(y => y.YEAR_).Distinct().ToList();
            var dynamicColumns = GetDynamicColumns(simulationYears);

            var requiredColumns = new List<string>()
            {
                $"{Properties.Resources.DeckSeeded}0",
                $"{Properties.Resources.SupSeeded}0",
                $"{Properties.Resources.SubSeeded}0",
                $"{Properties.Resources.CulvSeeded}0",
                $"{Properties.Resources.DeckDurationN}0",
                $"{Properties.Resources.SupDurationN}0",
                $"{Properties.Resources.SubDurationN}0",
                $"{Properties.Resources.CulvDurationN}0"
            };

            if (dynamicColumns.Length > 0)
            {
                requiredColumns.AddRange(dynamicColumns.Replace(" ", "").Split(','));
            }

            var selectAvailableColumns =
                $"select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='SIMULATION_{networkId}_{simulationId}'";

            var informationSchemaDataTable = new DataTable();
            var connection = new SqlConnection(db.Database.Connection.ConnectionString);
            using (var cmd = new SqlCommand(selectAvailableColumns, connection))
            {
                cmd.CommandTimeout = 180;
                var dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(informationSchemaDataTable);
                dataAdapter.Dispose();
            }

            var foundColumns = informationSchemaDataTable.Columns
                .Cast<DataColumn>()
                .Select(dt => dt.ColumnName)
                .ToList();

            var missingAttributes = new List<string>();
            requiredColumns.ForEach(requiredCol =>
            {
                if (!foundColumns.Contains(requiredCol))
                {
                    missingAttributes.Add(requiredCol);
                }
            });

            return missingAttributes;
        }

        /// <summary>
        /// Get Simulation_x_y dynamic table data, x = Newtwork Id, y = Simulation Id
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="dbContext"></param>
        /// <param name="simulationYears"></param>
        /// <returns>Datatable for run time selected columns</returns>
        public DataTable GetSimulationData(SimulationModel simulationModel, BridgeCareContext dbContext, List<int> simulationYears)
        {

            var simulationDataTable = new DataTable();
            var dynamicColumns = GetDynamicColumns(simulationYears);

            var selectSimulationStatement = "SELECT SECTIONID, " + Properties.Resources.DeckSeeded + "0, " + Properties.Resources.SupSeeded + "0, " + Properties.Resources.SubSeeded + "0, " + Properties.Resources.CulvSeeded + "0, " + Properties.Resources.DeckDurationN + "0, " + Properties.Resources.SupDurationN + "0, " + Properties.Resources.SubDurationN + "0, " + Properties.Resources.CulvDurationN + "0" + dynamicColumns + " FROM SIMULATION_" + simulationModel.NetworkId + "_" + simulationModel.SimulationId + "  WITH (NOLOCK)";
            try
            {
                var connection = new SqlConnection(dbContext.Database.Connection.ConnectionString);
                using (var cmd = new SqlCommand(selectSimulationStatement, connection))
                {
                    cmd.CommandTimeout = 180;
                    var dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(simulationDataTable);
                    dataAdapter.Dispose();
                }
            }
            catch (SqlException ex)
            {
                var table = "Simulation_" + simulationModel.NetworkId + "_" + simulationModel.SimulationId;
                if (ex.Number == 207)
                {
                    throw new InvalidOperationException($"{table} table does not have all the required simulation variables in the database to run summary report.");
                }
                HandleException.SqlError(ex, table);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return simulationDataTable;
        }

        /// <summary>
        /// Get Project, Cost related data from dynamic table Report_x_y, x = Network Id, y = Simulation Id
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="dbContext"></param>
        /// <param name="simulationYears"></param>
        /// <returns></returns>
        public IQueryable<ReportProjectCost> GetReportData(SimulationModel simulationModel, BridgeCareContext dbContext, List<int> simulationYears)
        {            
            IQueryable<ReportProjectCost> rawQueryForReportData = null;
            var years = string.Join(",", simulationYears);
            var selectReportStatement = "SELECT SECTIONID, TREATMENT, COST_, YEARS " + " FROM REPORT_" + simulationModel.NetworkId + "_" + simulationModel.SimulationId + " WITH(NOLOCK) WHERE BUDGET = 'actual_spent' AND YEARS IN (" + years + ")";
            try
            {
                rawQueryForReportData = dbContext.Database.SqlQuery<ReportProjectCost>(selectReportStatement).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Report_" + simulationModel.NetworkId + "_" + simulationModel.SimulationId);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return rawQueryForReportData;
        }

        #region private methods
        private string GetDynamicColumns(List<int> simulationYears)
        {
            var dynamicColumns = "";
            foreach (var year in simulationYears)
            {
                dynamicColumns = Properties.Resources.DeckSeeded + year + ", " + Properties.Resources.SupSeeded + year + ", " + Properties.Resources.SubSeeded + year + ", " + Properties.Resources.CulvSeeded + year + ", " + Properties.Resources.DeckDurationN + year + ", " + Properties.Resources.SupDurationN + year + ", " + Properties.Resources.SubDurationN + year + ", " + Properties.Resources.CulvDurationN + year;
            }

            return dynamicColumns;
        }

        private BridgeDataModel CreateBridgeDataModel(PennDotBridgeData penndotBridgeDataRow, PennDotReportAData pennDotReportADataRow, SdRisk sdRiskRow)
        {           
            return new BridgeDataModel
            {
                BRKey = penndotBridgeDataRow.BRKEY,
                BridgeFamily = penndotBridgeDataRow.BRIDGE_FAMILY_ID,
                Age = penndotBridgeDataRow.CONDITION_BASED_AGE,
                BridgeID = pennDotReportADataRow.BRIDGE_ID,
                District = pennDotReportADataRow.DISTRICT,
                DeckArea = pennDotReportADataRow.DECK_AREA,
                BPN = pennDotReportADataRow.BUS_PLAN_NETWORK,
                FunctionalClass = pennDotReportADataRow.FUNC_CLASS,
                NHS = pennDotReportADataRow.NHS_IND == "1" ? "Y" : "N",
                YearBuilt = pennDotReportADataRow.YEAR_BUILT,
                ADTOverTenThousand = Convert.ToInt32(pennDotReportADataRow.ADTTOTAL) > 10000 ? "Y" : "N",
                RiskScore = Convert.ToDouble(sdRiskRow.SD_RISK)
            };
        }        
        #endregion
    }
}