using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;

namespace BridgeCare.DataAccessLayer.SummaryReport
{
    public class BridgeDataDAL : IBridgeData
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BridgeDataDAL));
        /// <summary>
        /// Fetches bridge data using a list of br keys
        /// </summary>
        /// <param name="brKeys">br keys list</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>BridgeDataModel list</returns>        
        public List<BridgeDataModel> GetBridgeData(List<int> brKeys, SimulationModel model, BridgeCareContext db)
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
            var selectSectionStatement = "SELECT SECTIONID, FACILITY, SECTION " + " FROM SECTION_" + simulationModel.networkId + " Rpt WITH(NOLOCK) Order By FACILITY ASC";
            try
            {
                rawQueryForSectionData = dbContext.Database.SqlQuery<Section>(selectSectionStatement).AsQueryable();
            }
            catch (SqlException ex)
            {
                log.Error(ex.Message);
                HandleException.SqlError(ex, "Section_");
            }
            catch (OutOfMemoryException ex)
            {
                log.Error(ex.Message);
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

            var foundColumns = new List<string>();

            var selectAvailableColumns =
                $"select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='SIMULATION_{networkId}_{simulationId}_0'" +
                $" AND COLUMN_NAME IN ('{string.Join("','", requiredColumns)}');";

            using (var connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(selectAvailableColumns, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foundColumns.Add($"{reader[0]}");
                        }
                    }
                }
            }

            return requiredColumns.Except(foundColumns).ToList();
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

            var selectSimulationStatement = $"SELECT SECTIONID, {Properties.Resources.DeckSeeded}0, {Properties.Resources.SupSeeded}0, {Properties.Resources.SubSeeded}0, {Properties.Resources.CulvSeeded}0, " +
                                            $"{Properties.Resources.DeckDurationN}0, {Properties.Resources.SupDurationN}0, {Properties.Resources.SubDurationN}0, {Properties.Resources.CulvDurationN}0, " +
                                            dynamicColumns + $" FROM SIMULATION_{simulationModel.networkId}_{simulationModel.simulationId}_0 WITH (NOLOCK);";

            using (var connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(selectSimulationStatement, connection))
                {
                    command.CommandTimeout = 180;
                    using (var dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(simulationDataTable);
                    }
                }
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
            var listOfBudgets = dbContext.CriteriaDrivenBudgets.Where(y => y.SIMULATIONID == simulationModel.simulationId)
                .Select(cri => "'" + cri.BUDGET_NAME + "'").ToList();

            var budgets = string.Join(",", listOfBudgets);
            var selectReportStatement = $"SELECT SECTIONID, TREATMENT, COST_, YEARS FROM REPORT_{simulationModel.networkId}_{simulationModel.simulationId} " +
                                        $"WITH (NOLOCK) WHERE BUDGET IN (" + budgets + ") AND YEARS IN (" + years + ")";

            rawQueryForReportData = dbContext.Database.SqlQuery<ReportProjectCost>(selectReportStatement).AsQueryable();

            return rawQueryForReportData;
        }

        public List<string> GetTreatments(int simulationId, BridgeCareContext db)
        {
            var treatments = db.Treatments.Where(t => t.SIMULATIONID == simulationId).Select(t => t.TREATMENT).ToList();
            return treatments;
        }
        public List<string> GetBudgets(int simulationId, BridgeCareContext db)
        {
            var budgets = db.CriteriaDrivenBudgets.Where(t => t.SIMULATIONID == simulationId).Select(cri => cri.BUDGET_NAME).ToList();
            return budgets;
        }

        #region private methods
        private string GetDynamicColumns(List<int> simulationYears)
        {
            var dynamicColumns = "";
            foreach (var year in simulationYears)
            {
                if (dynamicColumns != "")
                    dynamicColumns += ", ";

                dynamicColumns += $"{Properties.Resources.DeckSeeded}{year}, {Properties.Resources.SupSeeded}{year}, {Properties.Resources.SubSeeded}{year}, {Properties.Resources.CulvSeeded}{year}, " +
                                 $"{Properties.Resources.DeckDurationN}{year}, {Properties.Resources.SupDurationN}{year}, {Properties.Resources.SubDurationN}{year}, {Properties.Resources.CulvDurationN}{year}";
            }

            return dynamicColumns;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>")]
        private BridgeDataModel CreateBridgeDataModel(PennDotBridgeData penndotBridgeDataRow, PennDotReportAData pennDotReportADataRow, SdRisk sdRiskRow)
        {
            bool adtTotalHasValue = Int32.TryParse(pennDotReportADataRow.ADTTOTAL, out int adtTotal);
            bool isADTOverTenThousand = adtTotalHasValue ? adtTotal > 10000 : false;
            return new BridgeDataModel
            {
                BRKey = penndotBridgeDataRow.BRKEY,
                BridgeFamily = penndotBridgeDataRow.BRIDGE_FAMILY_ID,
                Age = penndotBridgeDataRow.CONDITION_BASED_AGE,
                BridgeCulvert = penndotBridgeDataRow.BridgeCulvert,

                BridgeID = pennDotReportADataRow.BRIDGE_ID,
                District = pennDotReportADataRow.DISTRICT,
                DeckArea = pennDotReportADataRow.DECK_AREA,
                BPN = pennDotReportADataRow.BUS_PLAN_NETWORK,
                FunctionalClass = pennDotReportADataRow.FUNC_CLASS,
                NHS = pennDotReportADataRow.NHS_IND == "1" ? "Y" : "N",
                YearBuilt = pennDotReportADataRow.YEAR_BUILT,
                StructureLength = pennDotReportADataRow.StructureLength,
                PlanningPartner = pennDotReportADataRow.PlanningPartner,
                StructureType = pennDotReportADataRow.StructureType,
                Posted = pennDotReportADataRow.Posted.ToLower() == "posted" ? "Y" : "N",
                AdtTotal = pennDotReportADataRow.ADTTOTAL,
                P3 = pennDotReportADataRow.P3,
                ParallelBridge = pennDotReportADataRow.ParallelBridge,

                ADTOverTenThousand = isADTOverTenThousand ? "Y" : "N",
                RiskScore = Convert.ToDouble(sdRiskRow.SD_RISK)
            };
        }

        public List<BudgetsPerBRKey> GetBudgetsPerBRKey(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            var budgetsPerBrKey = new List<BudgetsPerBRKey>();
            var selectBugetForBrKey = $"select SECTION_13.SECTIONID, SECTION_13.FACILITY as BRKey, SECTION_13.SECTION as BridgeId, BUDGET, YEARS, ISCOMMITTED, TREATMENT " +
                $"from SECTION_{simulationModel.networkId} " +
                $"INNER JOIN Report_{simulationModel.networkId}_{simulationModel.simulationId} " +
                $"on SECTION_{simulationModel.networkId}.SECTIONID = Report_{simulationModel.networkId}_{simulationModel.simulationId}.SECTIONID " +
                $" WHERE BUDGET IS NOT NULL OR ISCOMMITTED != 0 Order By BRKey ASC";

            try
            {
                budgetsPerBrKey = dbContext.Database.SqlQuery<BudgetsPerBRKey>(selectBugetForBrKey).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return budgetsPerBrKey;
        }
        #endregion
    }
}
