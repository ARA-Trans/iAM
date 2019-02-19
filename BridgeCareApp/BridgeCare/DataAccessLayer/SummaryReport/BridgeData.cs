using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
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
        /// 
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public IQueryable<SectionDataModel> GetSectionData(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            IQueryable<SectionDataModel> rawQueryForSectionData = null;

            //FACILITY is BRKEY, SECTION is BRIDGE_ID
            var selectSectionStatement = "SELECT FACILITY, SECTION " + " FROM SECTION_" + simulationModel.NetworkId + " Rpt WITH(NOLOCK) Order By FACILITY ASC";
            try
            {
                rawQueryForSectionData = dbContext.Database.SqlQuery<SectionDataModel>(selectSectionStatement).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Section_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForSectionData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public IQueryable<Type> GetSimulationData(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            //TODO build SimulationDataModel in Caller class based on this result set, use years list to get idea of how many yrs data plus _0 columns.
            IQueryable<Type> rawQueryForSimulationData = null;

            var selectSimulationStatement = "SELECT * " + " FROM SIMULATION_" + simulationModel.NetworkId + "_" + simulationModel.SimulationId + "  WITH (NOLOCK)";
            try
            {
                rawQueryForSimulationData = dbContext.Database.SqlQuery<Type>(selectSimulationStatement).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Simulation_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForSimulationData;
        }

        #region private methods
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