using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class BridgeData : IBridgeData
    {
        /// <summary>
        /// Get and create BridgeDataModels from Constant tables for summary report. 
        /// </summary>
        /// <param name="BRKeys"></param>
        /// <param name="dbContext"></param>
        /// <returns>List of BridgeDataModel</returns>        
        public List<BridgeDataModel> GetBridgeData(List<int> BRKeys, BridgeCareContext dbContext)
        {
            var bridgeDataModels = new List<BridgeDataModel>();

            try
            {
                var penndotBridgeData = dbContext.PennDotBridgeData.Where(p => BRKeys.Contains(p.BRKEY)).ToList();
                var pennDotReportAData = dbContext.PennDotReportAData.Where(p => BRKeys.Contains(p.BRKEY)).ToList();
                var sdRisk = dbContext.SdRisks.Where(s => BRKeys.Contains(s.BRKEY)).ToList();
                //TODO ask why this is set in macro Func_Class='12' where BRKey=8356") Func_Class='16' where BRKey=45700") Func_Class='01' where BRKey=29077") Func_Class='06' where BRKey=55748")
                foreach (int BRKey in BRKeys)
                {
                    var penndotBridgeDataRow = penndotBridgeData.Where(b => b.BRKEY == BRKey).FirstOrDefault();
                    var pennDotReportADataRow = pennDotReportAData.Where(p => p.BRKEY == BRKey).FirstOrDefault();
                    var sdRiskRow = sdRisk.Where(s => s.BRKEY == BRKey).FirstOrDefault();
                    bridgeDataModels.Add(CreateBridgeDataModel(penndotBridgeDataRow, pennDotReportADataRow, sdRiskRow));
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "BridgeData");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return bridgeDataModels;
        }

        /// <summary>
        /// Get Section_x dynamic table data, x = Network Id
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="dbContext"></param>
        /// <returns>IQueryable<SectionDataModel></returns>
        public IQueryable<SectionDataModel> GetSectionData(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            IQueryable<SectionDataModel> rawQueryForSectionData = null;

            //FACILITY is BRKEY, SECTION is BRIDGE_ID
            var selectSectionStatement = "SELECT SECTIONID, FACILITY, SECTION " + " FROM SECTION_" + simulationModel.NetworkId + " Rpt WITH(NOLOCK) Order By FACILITY ASC";
            try
            {
                rawQueryForSectionData = dbContext.Database.SqlQuery<SectionDataModel>(selectSectionStatement).AsQueryable();
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

        /// <summary>
        /// Get Simulation_x_y dynamic table data, x = Newtwork Id, y = Simulation Id
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="dbContext"></param>
        /// <param name="simulationYears"></param>
        /// <returns>Datatable for run time selected columns</returns>
        public DataTable GetSimulationData(SimulationModel simulationModel, BridgeCareContext dbContext, List<int> simulationYears)
        {
            var dynamicColumns = string.Empty;
            var simulationDataTable = new DataTable();
            dynamicColumns = GetDynamicColumns(simulationYears, dynamicColumns);

            var selectSimulationStatement = "SELECT SECTIONID, DECK_SEEDED_0, SUP_SEEDED_0, SUB_SEEDED_0, CULV_SEEDED_0, DECK_DURATION_N_0, SUP_DURATION_N_0, SUB_DURATION_N_0, CULV_DURATION_N_0" + dynamicColumns + " FROM SIMULATION_" + simulationModel.NetworkId + "_" + simulationModel.SimulationId + "  WITH (NOLOCK)";
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
                HandleException.SqlError(ex, "Simulation_" + simulationModel.SimulationId + "_" + simulationModel.SimulationId);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return simulationDataTable;
        }

        #region private methods
        private static string GetDynamicColumns(List<int> simulationYears, string dynamicColumns)
        {
            foreach (var year in simulationYears)
            {
                dynamicColumns = dynamicColumns + ", DECK_SEEDED_" + year + ", SUP_SEEDED_" + year + ", SUB_SEEDED_" + year + ", CULV_SEEDED_" + year + ", DECK_DURATION_N_" + year + ", SUP_DURATION_N_" + year + ", SUB_DURATION_N_" + year + ", CULV_DURATION_N_" + year;
            }

            return dynamicColumns;
        }

        private static BridgeDataModel CreateBridgeDataModel(PennDotBridgeData penndotBridgeDataRow, PennDotReportAData pennDotReportADataRow, SdRisk sdRiskRow)
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