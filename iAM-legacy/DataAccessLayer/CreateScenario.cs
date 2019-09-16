using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.DTOs;
using System.Data.SqlClient;
using System.IO;

namespace DataAccessLayer
{
    public static class CreateScenario
    {
        /// <summary>
        /// Creates a new Decision Engine Simulation and initializes it.
        /// </summary>
        /// <param name="username">Login username who created simulation.</param>
        /// <param name="asset">Asset for which scenario is being created</param>
        /// <returns>Returns the SimulationStore created so user interface can be populated.</returns>
        public static SimulationStore CreateNewSimulationStore(string username,string asset)
        {
            SimulationStore simulation = null;
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(asset))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        connection.Open();
                        string insert = "INSERT INTO " + DB.TablePrefix + "SIMULATIONS " +
                            "(SIMULATION,NETWORKID,DATECREATED,USERNAME,BENEFIT_VARIABLE,BENEFIT_LIMIT,ANALYSIS, BUDGET_CONSTRAINT,ASSET_TYPE,RUN_TIME,USE_TARGET,TARGET_OCI,SIMULATION_AREA)" +
                            "VALUES('New Simulation',1,'" + DateTime.Now.ToString() + "',@username,'OCI',0,'Benefit/Cost','Budget Target',@asset,'0','false','" + DB.DefaultTargetOCI.ToString() + "','1')";
                        SqlCommand cmd = new SqlCommand(insert, connection);

                        cmd.Parameters.Add(new SqlParameter("username", username));
                        cmd.Parameters.Add(new SqlParameter("asset", asset));
                        cmd.ExecuteNonQuery();
                        string table = DB.TablePrefix + "SIMULATIONS";
                        string selectIdentity = "SELECT IDENT_CURRENT ('" + table + "') FROM " + table;
                        cmd = new SqlCommand(selectIdentity, connection);
                        int identity = Convert.ToInt32(cmd.ExecuteScalar());
                        InsertNewInvestment(identity.ToString(), asset);
                        simulation = SelectScenario.GetSimulationStore(identity.ToString());
                        InsertOMSActivities(simulation);
                        InsertSimulationOCITargets(simulation);
                        LoadPerformanceCurvesToDecisionEngine(identity, asset);
                    }
                }
                catch (Exception ex)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);

                }
            }
            return simulation;
        }

        public static void LoadPerformanceCurvesToDecisionEngine(int simulationID, string asset)
        {
            CreateScenario.LoadPerformanceCurvesToDecisionEngine(simulationID, asset, false);
        }


        public static void LoadPerformanceCurvesToDecisionEngine(int simulationID, string asset, Boolean addAssetClause)
        {
            string assetTypesOID = OMS.GetAssetTypeOID(asset);
            if (assetTypesOID != null)
            {
                OMSAssetConditionIndexStore assetOCI = OMS.GetAssetConditionIndex(asset);
                string predictionGroupTable = assetOCI.ConditionCategoryTable.Replace("ConditionCategories", "PredictionGroups");
                string predictionPointsTable = predictionGroupTable.Replace("PredictionGroups", "PredictionPoints");
                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    connection.Open();
                    string select = "SELECT " + predictionGroupTable + "OID, PredictionGroup, Filter FROM " + predictionGroupTable + " WHERE Inactive = '0'";
                    SqlCommand cmd = new SqlCommand(select, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string predictionGroupOID = dr[predictionGroupTable + "OID"].ToString();
                        string predictionGroup = "";
                        if (dr["PredictionGroup"] != DBNull.Value) predictionGroup = dr["PredictionGroup"].ToString();
                        //OMS doesn't use .amount - the parser definitely does not.
                        string criteria = dr["Filter"].ToString().Replace(".amount","").Replace("[Type]","[Type1]");
                        if (addAssetClause)
                        {
                            criteria += " && ([AssetType] is equal to \"" + asset + "\")";
                        }
                        InsertPredictionPoints(simulationID,predictionGroupOID, predictionGroup, criteria, predictionGroupTable,predictionPointsTable);
                    }
                }
            }
        }


        private static void InsertPredictionPoints(int simulationID, string predictionGroupOID, string predictionGroup, string criteria, string predictionGroupTable, string predictionPointsTable)
        {
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                connection.Open();
                string select = "SELECT ConditionCategory, IndexValue, PredictionYear FROM " + predictionPointsTable + " WHERE " + predictionGroupTable + "OID='" + predictionGroupOID + "' ORDER BY ConditionCategory, PredictionYear";
                SqlCommand cmd = new SqlCommand(select, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                string previousConditionCategory = null;
                string conditionCategory = null;
                string indexValue = null;
                string predictionYear = null;
                string equation = null;

                while (dr.Read())
                {
                    try
                    {
                        conditionCategory = dr["ConditionCategory"].ToString();
                        indexValue = dr["IndexValue"].ToString();
                        predictionYear = dr["PredictionYear"].ToString();
                    }
                    catch
                    {
                        continue;
                    }

                    if (previousConditionCategory != conditionCategory)
                    {
                        if (previousConditionCategory != null)
                        {
                            InsertPredictionPoints(simulationID, "__" + previousConditionCategory.Replace(" ", "").Replace("/", ""), predictionGroup, criteria, equation);
                        }
                        equation = "";
                    }
                    equation += "(" + indexValue + "," + predictionYear + ")";
                    previousConditionCategory = conditionCategory;
                   
                }
                if (previousConditionCategory != null)
                {
                    InsertPredictionPoints(simulationID, "__" + previousConditionCategory.Replace(" ", "").Replace("/", ""), predictionGroup, criteria, equation);
                }
            }
        }


        private static void InsertPredictionPoints(int simulationID, string attribute, string equationName, string criteria, string equation)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string insert = "INSERT INTO " + DB.TablePrefix + "PERFORMANCE " +
                    "(SIMULATIONID,ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,PIECEWISE)" +
                    "VALUES('" + simulationID.ToString() + "','" + attribute.Replace(" ", "") +"','" + equationName + "','" + criteria.Replace('\\','|') + "','" + equation + "','1')";
                SqlCommand cmd = new SqlCommand(insert, connection);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertSimulationOCITargets(SimulationStore simulation)
        {
            int startYear = simulation.StartDate.Year;
            for(int i = 0; i < simulation.Years; i++)
            {
                int year = startYear + i;
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "TARGETS " +
                        "(SIMULATIONID,ATTRIBUTE_,YEARS,TARGETMEAN,TARGETNAME,IS_OMS_PRIORITY)" +
                        "VALUES('" + simulation.SimulationID + "','OverallConditionIndex','" + year.ToString() + "','75','OCI TARGET','0')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertSimulationOCITargets(string simulationID, string year)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string insert = "INSERT INTO " + DB.TablePrefix + "TARGETS " +
                    "(SIMULATIONID,ATTRIBUTE_,YEARS,TARGETMEAN,TARGETNAME,IS_OMS_PRIORITY)" +
                    "VALUES('" + simulationID + "','OverallConditionIndex','" + year + "','75','OCI TARGET','0')";
                SqlCommand cmd = new SqlCommand(insert, connection);
                cmd.ExecuteNonQuery();
            }
        }



        //Retreives OMS default files and adds them to the RoadCare database.
        private static void InsertOMSActivities(SimulationStore simulation)
        {
            List<OMSActivityStore> activities = OMS.GetActivities(simulation.Asset);
            foreach (OMSActivityStore activity in activities)
            {
                AddActivity(activity, simulation);
            }
            AddNoTreatment(simulation);
        }


        public static void InsertOMSActivities(String simulationID, List<String> assets)
        {
            foreach(String asset in assets)
            {
                List<OMSActivityStore> activities = OMS.GetActivities(asset);
                foreach (OMSActivityStore activity in activities)
                {
                    AddActivity(activity, simulationID, asset);
                }
            }
            AddNoTreatment(simulationID);
        }


        
        /// <summary>
        /// This adds initial input into the INVESTMENT table
        /// </summary>
        /// <param name="simulationID"></param>
        /// <param name="asset"></param>
        private static void InsertNewInvestment(string simulationID,string asset)
        {
            string budgetOrder = "Maintenance";
            int nextYear = DateTime.Now.AddYears(1).Year;
            DateTime startDate = new DateTime(nextYear, 1, 1);

            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string insert = "INSERT INTO " + DB.TablePrefix + "INVESTMENTS " +
                    "(SIMULATIONID,FIRSTYEAR,NUMBERYEARS,INFLATIONRATE,DISCOUNTRATE,BUDGETORDER,SIMULATION_START_DATE)" +
                    "VALUES('" + simulationID + "','" + nextYear.ToString() + "','" + DB.DefaultNumberYears.ToString()  +"','0','0','"+ budgetOrder +"','" + startDate.ToString() + "')";
                SqlCommand cmd = new SqlCommand(insert, connection);
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < DB.DefaultNumberYears; i++)
            {
                int year = nextYear + i;
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "YEARLYINVESTMENT " +
                        "(SIMULATIONID,YEAR_,BUDGETNAME,AMOUNT)" +
                        " VALUES('" + simulationID + "','" + year.ToString() + "','Maintenance','0')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
            }

        }


        public static void AddActivity(OMSActivityStore activity, SimulationStore simulation)
        {
            //Insert activity in treatments
            //Get Identity
            string treatmentID = null;
            int year = DateTime.Now.Year + 1;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string insert = "INSERT INTO " + DB.TablePrefix + "TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,OMS_REPEAT_START, OMS_REPEAT_INTERVAL, OMS_OID, OMS_IS_SELECTED,OMS_IS_EXCLUSIVE, OMS_IS_REPEAT)"
                + " VALUES ('" + simulation.SimulationID + "','" + activity.ActivityName + "','1','1','" + year + "','1','" + activity.ActivityOID + "','0','0','0')";
                try
                {

                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                    string selectIdentity = "SELECT IDENT_CURRENT ('" + DB.TablePrefix + "TREATMENTS') FROM " + DB.TablePrefix + "TREATMENTS";
                    cmd = new SqlCommand(selectIdentity, connection);
                    treatmentID = cmd.ExecuteScalar().ToString();

                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }

                try
                {
                    double  cost = 0;
                    if (activity.OmsCost.CostPerUnit != double.NaN)
                    {
                        cost = activity.OmsCost.CostPerUnit;
                        if (cost <= 0)
                        {
                            cost = 1;
                        }
                    }
                    else
                    {
                        cost = 1;
                    }
                    insert = "INSERT INTO " + DB.TablePrefix + "COSTS (TREATMENTID,COST_) VALUES ('" + treatmentID + "','" + cost.ToString() + "')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }

                OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(simulation.Asset);

                foreach (OMSConsequenceStore consequence in activity.ConsequenceList)
                {
                    try
                    {
                        OMSConditionIndexStore condition = oci.ConditionIndexes.Find(delegate(OMSConditionIndexStore ci) { return ci.ConditionCategory == consequence.ConditionCategory; });

                        if (condition != null)
                        {

                            insert = "INSERT INTO " + DB.TablePrefix + "CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_) VALUES ('" + treatmentID + "','__" + condition.AttributeDE + "','" + consequence.GetDecisionEngineConsequence() + "')";
                            SqlCommand cmd = new SqlCommand(insert, connection);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                    }
                }

                try
                {
                    string conditionIndexCriteria = GetConditionIndexCriteria(activity.ConsequenceList);
                    insert = "INSERT INTO " + DB.TablePrefix + "FEASIBILITY (TREATMENTID,CRITERIA) VALUES ('" + treatmentID + "','" + conditionIndexCriteria + "')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        public static void AddActivity(OMSActivityStore activity, String simulationID, String asset)
        {
            //Insert activity in treatments
            //Get Identity
            string treatmentID = null;
            int year = DateTime.Now.Year + 1;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string insert = "INSERT INTO " + DB.TablePrefix + "TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,OMS_REPEAT_START, OMS_REPEAT_INTERVAL, OMS_OID, OMS_IS_SELECTED,OMS_IS_EXCLUSIVE, OMS_IS_REPEAT)"
                + " VALUES ('" + simulationID + "','" + activity.ActivityName + "','1','1','" + year + "','1','" + activity.ActivityOID + "','0','0','0')";
                try
                {

                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                    string selectIdentity = "SELECT IDENT_CURRENT ('" + DB.TablePrefix + "TREATMENTS') FROM " + DB.TablePrefix + "TREATMENTS";
                    cmd = new SqlCommand(selectIdentity, connection);
                    treatmentID = cmd.ExecuteScalar().ToString();

                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }

                try
                {
                    double cost = 0;
                    if (activity.OmsCost.CostPerUnit != double.NaN)
                    {
                        cost = activity.OmsCost.CostPerUnit;
                        if (cost <= 0)
                        {
                            cost = 1;
                        }
                    }
                    else
                    {
                        cost = 1;
                    }
                    insert = "INSERT INTO " + DB.TablePrefix + "COSTS (TREATMENTID,COST_) VALUES ('" + treatmentID + "','" + cost.ToString() + "')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }

                OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(asset);

                foreach (OMSConsequenceStore consequence in activity.ConsequenceList)
                {
                    try
                    {
                        OMSConditionIndexStore condition = oci.ConditionIndexes.Find(delegate(OMSConditionIndexStore ci) { return ci.ConditionCategory == consequence.ConditionCategory; });

                        if (condition != null)
                        {

                            insert = "INSERT INTO " + DB.TablePrefix + "CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_) VALUES ('" + treatmentID + "','__" + condition.AttributeDE.Replace(" ","").Replace("/","") + "','" + consequence.GetDecisionEngineConsequence() + "')";
                            SqlCommand cmd = new SqlCommand(insert, connection);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                    }
                }

                try
                {
                    string conditionIndexCriteria = GetConditionIndexCriteria(activity.ConsequenceList);
                    
                    if(String.IsNullOrWhiteSpace(conditionIndexCriteria))
                    {
                        conditionIndexCriteria = "[AssetType]=|" + asset + "|";
                    }
                    else
                    {
                        conditionIndexCriteria = " AND [AssetType]=|" + asset + "|";
                    }
                    
                    
                    insert = "INSERT INTO " + DB.TablePrefix + "FEASIBILITY (TREATMENTID,CRITERIA) VALUES ('" + treatmentID + "','" + conditionIndexCriteria + "')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        private static string GetConditionIndexCriteria(List<OMSConsequenceStore> consequenceList)
        {
            string criteria = "";

            OMSConsequenceStore previousConsequence = null;

            foreach (OMSConsequenceStore consequence in consequenceList)
            {
                if (consequence != previousConsequence)
                {
                    criteria += "([" + "__" + consequence.ConditionCategory.Replace(" ", "").Replace("/", "") + "] >= 0) AND ([" + "__" + consequence.ConditionCategory.Replace(" ", "").Replace("/", "") + "] <= 100) AND ";
                }
                previousConsequence = consequence;
            }

            criteria += "([OverallConditionIndex] >= 0) AND ([OverallConditionIndex] <= 100)";
            return criteria;
        }


        public static void AddNoTreatment(SimulationStore simulation)
        {
            AddNoTreatment(simulation.SimulationID);
        }





        public static void AddNoTreatment(String  simulationID)
        {
            //Insert activity in treatments
            //Get Identity
            string treatmentID = null;
            int year = DateTime.Now.Year + 1;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string insert = "INSERT INTO " + DB.TablePrefix + "TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,OMS_REPEAT_START, OMS_REPEAT_INTERVAL, OMS_OID, OMS_IS_SELECTED,OMS_IS_EXCLUSIVE, OMS_IS_REPEAT)"
                + " VALUES ('" + simulationID + "','No Treatment','0','0','" + year.ToString()+ "','1','-1','0','0','0')";
                try
                {

                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                    string selectIdentity = "SELECT IDENT_CURRENT ('" + DB.TablePrefix + "TREATMENTS') FROM " + DB.TablePrefix + "TREATMENTS";
                    cmd = new SqlCommand(selectIdentity, connection);
                    treatmentID = cmd.ExecuteScalar().ToString();

                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }

                try
                {
                    insert = "INSERT INTO " + DB.TablePrefix + "FEASIBILITY (TREATMENTID,CRITERIA) VALUES ('" + treatmentID + "','')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }

                try
                {
                    insert = "INSERT INTO " + DB.TablePrefix + "COSTS (TREATMENTID,COST_) VALUES ('" + treatmentID + "','0')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }


                try
                {
                    insert = "INSERT INTO " + DB.TablePrefix + "CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_) VALUES ('" + treatmentID + "','AGE','+1')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }


        public static TargetStore AddNewTarget(string simulationID)
        {
            TargetStore target = new TargetStore();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {

                try
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "TARGETS (SIMULATIONID,TARGETNAME,ATTRIBUTE_,TARGETMEAN,IS_OMS_PRIORITY) VALUES ('" + simulationID + "','Untitled Priority','OverallConditionIndex','50','1')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();


                    string selectIdentity = "SELECT IDENT_CURRENT ('" + DB.TablePrefix + "TARGETS') FROM " + DB.TablePrefix + "TARGETS";
                    cmd = new SqlCommand(selectIdentity, connection);
                    target.ID = cmd.ExecuteScalar().ToString();
                    target.TargetOCI = 50;
                    target.Name = "Untitled Priority";
                }
                catch {
                    target = null;
                }
            }
            return target;
        }


        public static string CopySimulation(string simulationID)
        {
            string newSimulationID = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "SIMULATIONS (NETWORKID, SIMULATION, COMMENTS, DATECREATED, CREATORID, USERNAME, PERMISSIONS, JURISDICTION, ANALYSIS, BUDGET_CONSTRAINT,"
                    + "WEIGHTING,BENEFIT_VARIABLE,BENEFIT_LIMIT,COMMITTED_START,COMMITTED_PERIOD,SIMULATION_VARIABLES,RUN_TIME, ASSET_TYPE,USE_TARGET,TARGET_OCI,DEFICIENT,PERCENT_DEFICIENT, SIMULATION_AREA)"
                    + " SELECT NETWORKID, SIMULATION, COMMENTS, DATECREATED, CREATORID, USERNAME, PERMISSIONS, JURISDICTION, ANALYSIS, BUDGET_CONSTRAINT,"
                    + "WEIGHTING,BENEFIT_VARIABLE,BENEFIT_LIMIT,COMMITTED_START,COMMITTED_PERIOD,SIMULATION_VARIABLES,RUN_TIME, ASSET_TYPE,USE_TARGET,TARGET_OCI,DEFICIENT,PERCENT_DEFICIENT, SIMULATION_AREA FROM " + DB.TablePrefix + "SIMULATIONS WHERE SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();

                    string simulation = "";
                    string select = "SELECT SIMULATION FROM " + DB.TablePrefix + "SIMULATIONS WHERE SIMULATIONID='" + simulationID + "'";
                    cmd = new SqlCommand(select, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["SIMULATION"] != DBNull.Value) simulation = dr["SIMULATION"].ToString() + "(copy)";
                    }
                    dr.Close();

                    string selectIdentity = "SELECT IDENT_CURRENT ('" + DB.TablePrefix + "SIMULATIONS') FROM " + DB.TablePrefix + "SIMULATIONS";
                    cmd = new SqlCommand(selectIdentity, connection);
                    newSimulationID = cmd.ExecuteScalar().ToString();

                    string update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET SIMULATION='" + simulation + "', DATECREATED='" + DateTime.Now.ToString() + "' WHERE SIMULATIONID='" + newSimulationID + "'";
                    cmd = new SqlCommand(update, connection);
                    cmd.ExecuteNonQuery();

                    CopyInvestments(simulationID, newSimulationID);
                    CopyTargets(simulationID, newSimulationID);
                    CopyCommitted(simulationID, newSimulationID);
                    CopyTreatments(simulationID, newSimulationID);
                }
                catch
                {

                }
            }
            return newSimulationID;
        }

        private static void CopyTargets(string simulationID, string newSimulationID)
        {
            List<TargetStore> targets = SelectScenario.GetTargetsForCopy(simulationID);
            foreach (TargetStore target in targets)
            {
                string criteria = null;
                string year = null;
                if(target.Criteria == null)
                {
                    criteria = "NULL";
                }
                else
                {
                    criteria = "'" + target.Criteria + "'";
                }

                if(target.Year == null)
                {
                    year = "NULL";
                }
                else
                {
                    year = "'" + target.Year + "'";
                }

                if (target.IsOMSPriority == null)
                {
                    target.IsOMSPriority = "0";
                }



                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        string insert = "INSERT INTO " + DB.TablePrefix + "TARGETS (SIMULATIONID,ATTRIBUTE_,YEARS,TARGETMEAN,TARGETNAME,CRITERIA,IS_OMS_PRIORITY) VALUES" +
                             "('" + newSimulationID + "','OverallConditionIndex'," + year + ",'" + target.TargetOCI + "','" + target.Name + "'," + criteria + ",'" + target.IsOMSPriority + "')";

                        SqlCommand cmd = new SqlCommand(insert, connection);
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void CopyInvestments(string simulationID, string newSimulationID)
        {
            DateTime dateStart = DateTime.Now;
            InvestmentStore investment = SelectScenario.GetInvestment(simulationID);
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "INVESTMENTS (SIMULATIONID,FIRSTYEAR,NUMBERYEARS,INFLATIONRATE,DISCOUNTRATE,BUDGETORDER,SIMULATION_START_DATE) VALUES"
                        + "('" + newSimulationID + "','" + investment.FirstYear.ToString() + "','" + investment.NumberYear + "','" + investment.InflationRate.ToString() + "','" + investment.DiscountRate.ToString() + "','" + investment.BudgetOrder + "','" + investment.StartDate + "')";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();

                    List<YearBudgetAmountStore> yearBudgets = SelectScenario.GetBudgetsByYear(simulationID);

                    foreach (YearBudgetAmountStore yearBudget in yearBudgets)
                    {
                        insert = "INSERT INTO " + DB.TablePrefix + "YEARLYINVESTMENT (SIMULATIONID,YEAR_,BUDGETNAME,AMOUNT) VALUES ('" + newSimulationID + "','" + yearBudget.Year.ToString() + "','" + yearBudget.Budget + "','" + yearBudget.Amount + "')";
                        cmd = new SqlCommand(insert, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch
                {

                }
            }
        }

        private static void CopyCommitted(string simulationID, string newSimulationID)
        {
            List<CommittedStore> committeds = SelectScenario.GetCommittedForCopy(simulationID);
            String myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            myDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(myDocumentsFolder);
            string fileOut = myDocumentsFolder + "\\oms_committed_copy_" + newSimulationID + ".txt";
            TextWriter tw = new StreamWriter(fileOut);
            string lineOut = null;

            foreach (CommittedStore committed in committeds)
            {
                try
                {
                    lineOut = "\t" + newSimulationID + ","
                        + committed.SectionID.ToString() + ","
                        + committed.Years.ToString() + ","
                        + committed.YearsSame.ToString() + ","
                        + committed.YearsAny.ToString() + ","
                        + committed.Budget.ToString() + ","
                        + committed.Cost.ToString() + ",";
                    if (committed.OMSIsRepeat) lineOut += "1,";
                    else lineOut += "0,";

                    if (committed.OMSIsExclusive) lineOut += "1,";
                    else lineOut += "0,";

                    if (committed.OMSIsNotAllowed) lineOut += "1";
                    else lineOut += "0";
                    tw.WriteLine(lineOut);
                }
                catch (Exception ex)
                {
                    int test =  ex.Message.Length;
                }

                
            }
            tw.Close();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string password = ParsePassword(DB.ConnectionString);
                string username = ParseUserName(DB.ConnectionString);
                string datasource = connection.DataSource;
                string database = connection.Database;
                char delimiter = ',';
                string bcp = "\"" + database + ".dbo." + DB.TablePrefix + "COMMITTED_" + "\" IN \"" + fileOut + "\" /S " + datasource + " -t\"" + delimiter + "\" -c -q -U " + username + " -P " + password; ;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = "bcp";
                proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = bcp;
                proc.Start();
                proc.WaitForExit();
            }
        }


        private static void CopyTreatments(string simulationID, string newSimulationID)
        {
            List<ActivityStore> activities = SelectScenario.GetActivities(simulationID,true);
            foreach(ActivityStore activity in activities)
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    try
                    {
                        connection.Open();
                    
                        string insert = "INSERT INTO " + DB.TablePrefix + "TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,BUDGET,DESCRIPTION,OMS_IS_REPEAT,OMS_REPEAT_START,OMS_REPEAT_INTERVAL,OMS_OID,OMS_IS_SELECTED, OMS_IS_EXCLUSIVE) VALUES " +
                            "(@simulationID,@treatment,@beforeAny,@beforeSame,@budget,@description,@omsIsRepeat,@omsRepeatStart,@omsRepeatInterval,@omsOID,@omsIsSelected,@omsIsExclusive)";

                        SqlCommand cmd = new SqlCommand(insert, connection);
                        cmd.Parameters.Add(new SqlParameter("simulationID", newSimulationID));
                        cmd.Parameters.Add(new SqlParameter("treatment", activity.Activity));
                        cmd.Parameters.Add(new SqlParameter("beforeAny", 1));
                        cmd.Parameters.Add(new SqlParameter("beforeSame", activity.YearSame));
                        string concatenatedbudget = "";
                        if (activity.Budgets != null)
                        {
                            foreach (string budget in activity.Budgets)
                            {
                                if (concatenatedbudget.Length > 0) concatenatedbudget += "|";
                                concatenatedbudget += budget;
                            }
                            if (concatenatedbudget.Length > 0)
                            {
                                cmd.Parameters.Add(new SqlParameter("budget", concatenatedbudget));
                            }
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("budget", DBNull.Value));
                        }
                        
                        cmd.Parameters.Add(new SqlParameter("description", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("omsIsRepeat", activity.IsRepeatActivity));
                        cmd.Parameters.Add(new SqlParameter("omsRepeatStart", activity.RepeatStart));
                        cmd.Parameters.Add(new SqlParameter("omsRepeatInterval", activity.RepeatInterval));
                        cmd.Parameters.Add(new SqlParameter("omsOID", activity.OmsOID));
                        cmd.Parameters.Add(new SqlParameter("omsIsSelected", activity.IsSelected));
                        cmd.Parameters.Add(new SqlParameter("omsIsExclusive", activity.IsExclusive));
                        cmd.ExecuteNonQuery();
                        string selectIdentity = "SELECT IDENT_CURRENT ('" + DB.TablePrefix + "TREATMENTS') FROM " + DB.TablePrefix + "TREATMENTS";
                        cmd = new SqlCommand(selectIdentity, connection);
                        int newTreatmentID = Convert.ToInt32(cmd.ExecuteScalar());
                        InsertFeasibilityCostConsequence(newTreatmentID, activity);
                    }
                    catch(Exception e)
                    {

                    }
                }
            }
        }


        private static void InsertFeasibilityCostConsequence(int newTreatmentID, ActivityStore activity)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "FEASIBILITY (TREATMENTID,CRITERIA) VALUES(@treatmentID, @criteria)";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.Parameters.Add(new SqlParameter("treatmentID", newTreatmentID));
                    cmd.Parameters.Add(new SqlParameter("criteria", activity.RawCriteria));
                    cmd.ExecuteNonQuery();

                    insert = "INSERT INTO " + DB.TablePrefix + "COSTS (TREATMENTID,COST_) VALUES(@treatmentID, @cost)";
                    cmd = new SqlCommand(insert, connection);
                    cmd.Parameters.Add(new SqlParameter("treatmentID", newTreatmentID));
                    cmd.Parameters.Add(new SqlParameter("cost", activity.RawCost));
                    cmd.ExecuteNonQuery();

                    if (activity.ConditionIndexes != null && activity.ConditionIndexes.Count > 0)
                    {
                        foreach (ActivityConditionIndex ci in activity.ConditionIndexes)
                        {
                            if (ci.Impact != null)
                            {
                                insert = "INSERT INTO " + DB.TablePrefix + "CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_) VALUES(@treatmentID, @attribute,@change)";
                                cmd = new SqlCommand(insert, connection);
                                cmd.Parameters.Add(new SqlParameter("treatmentID", newTreatmentID));
                                cmd.Parameters.Add(new SqlParameter("attribute", "__" + ci.DEConditionIndex.Replace(" ", "").Replace("/", "")));
                                cmd.Parameters.Add(new SqlParameter("change", ci.Impact));
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        insert = "INSERT INTO " + DB.TablePrefix + "CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_) VALUES(@treatmentID, @attribute,@change)";
                        cmd = new SqlCommand(insert, connection);
                        cmd.Parameters.Add(new SqlParameter("treatmentID", newTreatmentID));
                        cmd.Parameters.Add(new SqlParameter("attribute", "AGE"));
                        cmd.Parameters.Add(new SqlParameter("change", "+1"));
                        cmd.ExecuteNonQuery();
                    }
                }
                catch(Exception e)
                {

                }
            }
        }





        private static string ParsePassword(string connectionString)
        {
            //Password = " + m_strPassword + ";";
            string workingConnectionString = TrimNonLiteralSpaces(connectionString);
            int startIndex = workingConnectionString.ToUpper().IndexOf("PASSWORD=", 0) + 9;
            int endIndex = workingConnectionString.IndexOf(';', startIndex) - 1;
            int length = endIndex - startIndex + 1;
            string password = workingConnectionString.Substring(startIndex, length).Trim();

            return password;

        }

        private static string ParseUserName(string connectionString)
        {
            string workingConnectionString = TrimNonLiteralSpaces(connectionString);
            int startIndex = workingConnectionString.ToUpper().IndexOf("USERID=", 0) + 7;
            int endIndex = workingConnectionString.IndexOf(';', startIndex) - 1;
            int length = endIndex - startIndex + 1;
            string userName = workingConnectionString.Substring(startIndex, length);

            return userName;
        }
        
        private static string TrimNonLiteralSpaces(string connectionString)
        {
            string trimmed = "";
            bool literalMode = false;
            for (int i = 0; i < connectionString.Length; ++i)
            {
                switch (connectionString[i])
                {
                    case '\"':
                        trimmed = trimmed + connectionString[i];
                        literalMode = !literalMode;
                        break;
                    case ' ':
                        if (literalMode)
                        {
                            trimmed = trimmed + connectionString[i];
                        }
                        break;
                    default:
                        trimmed = trimmed + connectionString[i];
                        break;
                }
            }

            return trimmed;
        }

    }

}
