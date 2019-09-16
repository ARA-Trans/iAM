using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.DTOs;
using System.Data.SqlClient;
using Utility.ExceptionHandling;
using Microsoft.SqlServer.Types;

namespace DataAccessLayer
{
    public static class SelectScenario
    {
        public static List<SimulationStore> GetSimulations()
        {
            List<SimulationStore> simulations = new List<SimulationStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string select = "SELECT SIMULATIONID FROM " + DB.TablePrefix + "SIMULATIONS";
                    SqlCommand cmd = new SqlCommand(select, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string simulationID = dr["SIMULATIONID"].ToString();
                        simulations.Add(GetSimulationStore(simulationID));
                    }
                }
                catch
                {
                }
            }
            return simulations;

        }

        public static List<AssetTypeStore> GetAssetTypesWithSimulations()
        {
            List<AssetTypeStore> assets = new List<AssetTypeStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string select = "SELECT DISTINCT ASSET_TYPE  FROM " + DB.TablePrefix + "SIMULATIONS";
                SqlCommand cmd = new SqlCommand(select, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    AssetTypeStore asset = new AssetTypeStore();
                    asset.AssetName = dr["ASSET_TYPE"].ToString();
                    assets.Add(asset);
                }
            }

            List<AssetTypeStore> omsAssets = OMS.GetAssetTypes();
            foreach (AssetTypeStore asset in assets)
            {
                AssetTypeStore omsAsset = omsAssets.Find(delegate(AssetTypeStore ats) { return ats.AssetName == asset.AssetName; });
                asset.OID = omsAsset.OID;
            }
            return assets;
        }

        public static SimulationStore GetSimulationStore(string simulationID)
        {
            SimulationStore simulation = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string select = "SELECT * FROM " + DB.TablePrefix + "SIMULATIONS WHERE SIMULATIONID=@simulationID";
                    SqlCommand cmd = new SqlCommand(select, connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        simulation = new SimulationStore();
                        if (dr["SIMULATIONID"] != DBNull.Value) simulation.SimulationID = dr["SIMULATIONID"].ToString();
                        if (dr["SIMULATION"] != DBNull.Value) simulation.SimulationName = dr["SIMULATION"].ToString();
                        if (dr["ASSET_TYPE"] != DBNull.Value) simulation.Asset = dr["ASSET_TYPE"].ToString();
                        if (dr["DATECREATED"] != DBNull.Value) simulation.LastRunDate = dr["DATECREATED"].ToString();
                        if (dr["USERNAME"] != DBNull.Value) simulation.LastAuthor = dr["USERNAME"].ToString();
                        if (dr["RUN_TIME"] != DBNull.Value) simulation.EstimatedRunTime = dr["RUN_TIME"].ToString();
                        if (dr["JURISDICTION"] != DBNull.Value) simulation.Jurisdiction = dr["JURISDICTION"].ToString();
                        if (dr["USE_TARGET"] != DBNull.Value) simulation.UseTarget = Convert.ToBoolean(dr["USE_TARGET"]);
                        if (dr["TARGET_OCI"] != DBNull.Value) simulation.TargetOCI = dr["TARGET_OCI"].ToString();
                        if (dr["COMMENTS"] != DBNull.Value) simulation.ScopeDescription = dr["COMMENTS"].ToString();
                        if (dr["SIMULATION_AREA"] != DBNull.Value) simulation.SimulationArea = dr["SIMULATION_AREA"].ToString();
                        if (dr["ANALYSIS"] != DBNull.Value) simulation.AnalysisType = dr["ANALYSIS"].ToString();
                        if (dr["BUDGET_CONSTRAINT"] != DBNull.Value) simulation.BudgetConstraint = dr["BUDGET_CONSTRAINT"].ToString();
                    }
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                    simulation = null;
                }
            }
            if (simulation != null)
            {
                GetSimulationInvestment(simulation);
                FillSimulationStoreResults(simulation);
            }
            List<AssetTypeStore> correspondingAssetTypes = OMS.GetAssetTypes();
            if (correspondingAssetTypes != null)
            {
                AssetTypeStore toFind = correspondingAssetTypes.Find(delegate(AssetTypeStore ats)
                {
                    return ats.AssetName == simulation.Asset;
                });
                if (toFind != null)
                {
                    simulation.AssetTypeOID = toFind.OID;
                }
            }
            
            return simulation;
        }

        public static void GetSimulationInvestment(SimulationStore simulation)
        {
            InvestmentStore investment = GetInvestment(simulation.SimulationID);
            simulation.Years = investment.NumberYear;
            simulation.StartDate = investment.StartDate;
            simulation.CategoryBudgets = GetCategoryBudgets(simulation.SimulationID, investment);
            simulation.YearBudgets = GetYearBudgets(simulation.SimulationID);
            simulation.BudgetName = investment.BudgetName;
            simulation.InflationRate = investment.InflationRate;

            string[] budgets = investment.BudgetOrder.Split('|');
            simulation.BudgetOrder = new List<string>();
            foreach (string budget in budgets)
            {
                simulation.BudgetOrder.Add(budget);
            }
        }

        private static void FillSimulationStoreResults(SimulationStore simulation)
        {
            simulation.NetworkConditions = GetNetworkCondition(simulation.SimulationID);
            simulation.TotalAssets = OMS.GetNumberAsset(simulation.SimulationID);
            simulation.CurrentOCI = OMS.GetCurrentOCI(simulation.Asset, simulation.ScopeDescription);
        }

        /// <summary>
        /// Retrieves the current Investment store for a given SimulationID and Asset.
        /// </summary>
        /// <param name="simulationID">Asset is required </param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static InvestmentStore GetInvestment(string simulationID)
        {
            InvestmentStore investment = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string select = "SELECT * FROM " + DB.TablePrefix + "INVESTMENTS WHERE SIMULATIONID=@simulationID";
                    SqlCommand cmd = new SqlCommand(select, connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        investment = new InvestmentStore();
                        if (dr["FIRSTYEAR"] != DBNull.Value) investment.FirstYear = Convert.ToInt32(dr["FIRSTYEAR"]);
                        if (dr["NUMBERYEARS"] != DBNull.Value) investment.NumberYear = Convert.ToInt32(dr["NUMBERYEARS"]);
                        if (dr["INFLATIONRATE"] != DBNull.Value) investment.InflationRate = Convert.ToDouble(dr["INFLATIONRATE"]);
                        if (dr["DISCOUNTRATE"] != DBNull.Value) investment.DiscountRate = Convert.ToDouble(dr["DISCOUNTRATE"]);
                        if (dr["SIMULATION_START_DATE"] != DBNull.Value) investment.StartDate = Convert.ToDateTime(dr["SIMULATION_START_DATE"]);
                        if (dr["BUDGET_NAME"] != DBNull.Value) investment.BudgetName = dr["BUDGET_NAME"].ToString();
                        if (dr["BUDGETORDER"] != DBNull.Value) investment.BudgetOrder = Convert.ToString(dr["BUDGETORDER"]);
                    }
                }
                catch (Exception ex)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
                    investment = null;
                }
            }
            return investment;
        }

        public static List<CategoryBudgetStore> GetCategoryBudgets(string simulationID, InvestmentStore investment)
        {
            List<CategoryBudgetStore> listCategoryBudget = new List<CategoryBudgetStore>();

            foreach (string budget in investment.Budgets)
            {
                CategoryBudgetStore categoryBudget = new CategoryBudgetStore();
                categoryBudget.Key = budget;
                listCategoryBudget.Add(categoryBudget);
            }
            // Now retrieve the amount actually spent.
            FillResultCategoryBudget(listCategoryBudget, simulationID);
            return listCategoryBudget;
        }


        public static List<YearBudgetStore> GetYearBudgets(string simulationID)
        {
            List<YearBudgetStore> listYearBudget = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {

                connection.Open();
                string select = "SELECT YEAR_,SUM(AMOUNT)AS AMOUNT FROM cgDE_YEARLYINVESTMENT WHERE SIMULATIONID='" + simulationID + "' GROUP BY YEAR_ ORDER BY YEAR_";
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                SqlDataReader dr = cmd.ExecuteReader();
                listYearBudget = new List<YearBudgetStore>();
                while (dr.Read())
                {
                    YearBudgetStore yearBudget = new YearBudgetStore();
                    if (dr["YEAR_"] != DBNull.Value) yearBudget.Key = Convert.ToString(dr["YEAR_"]);
                    if (dr["AMOUNT"] != DBNull.Value) yearBudget.Target = Convert.ToString(dr["AMOUNT"]);
                    listYearBudget.Add(yearBudget);
                }
                dr.Close();
                // Now retrieve the amount actually spent (from report table).
                FillResultYearBudget(listYearBudget, simulationID);
            }
            return listYearBudget;
        }


        public static string GetNetworkID(string simulationID)
        {
            string networkID = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {

                connection.Open();
                string select = "SELECT NETWORKID FROM " + DB.TablePrefix + "SIMULATIONS WHERE SIMULATIONID=@simulationID";
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                networkID = cmd.ExecuteScalar().ToString();
            }
            return networkID;
        }


        public static void FillResultYearBudget(List<YearBudgetStore> listYearBudget, string simulationID)
        {
            string networkID = GetNetworkID(simulationID);
            if (networkID == null) return;//There is some kind of error.

            string reportTable = DB.TablePrefix + "REPORT_" + networkID + "_" + simulationID;
            if (DB.CheckIfTableExists(reportTable) == 0) return; //Usually occurs because project has not run.

            foreach (YearBudgetStore yearBudget in listYearBudget)
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {

                    connection.Open();
                    string select = "SELECT SUM(COST_) FROM " + reportTable + " WHERE YEARS='" + yearBudget.Key + "'";
                    SqlCommand cmd = new SqlCommand(select, connection);
                    yearBudget.Spent = cmd.ExecuteScalar().ToString();
                }
            }
        }

        public static void FillResultCategoryBudget(List<CategoryBudgetStore> listCategoryBudget, string simulationID)
        {
            string networkID = GetNetworkID(simulationID);
            if (networkID == null) return;//There is some kind of error.

            string reportTable = DB.TablePrefix + "REPORT_" + networkID + "_" + simulationID;
            double spentSum = 0;
            if (DB.CheckIfTableExists(reportTable) == 0)//Get Budgeted
            {
                foreach (CategoryBudgetStore categoryBudget in listCategoryBudget)
                {
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {

                        connection.Open();
                        string select = "SELECT SUM(AMOUNT) FROM " + DB.TablePrefix + "YEARLYINVESTMENT WHERE BUDGETNAME='" + categoryBudget.Key + "' AND SIMULATIONID='" + simulationID + "'";
                        SqlCommand cmd = new SqlCommand(select, connection);
                        double budgetSpent = (double)cmd.ExecuteScalar();
                        spentSum += budgetSpent;
                        categoryBudget.Spent = budgetSpent.ToString();
                    }
                }
            }
            else //Get result
            {

                foreach (CategoryBudgetStore categoryBudget in listCategoryBudget)
                {
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {

                        connection.Open();
                        string select = "SELECT SUM(COST_) FROM " + reportTable + " WHERE BUDGET='" + categoryBudget.Key + "'";
                        SqlCommand cmd = new SqlCommand(select, connection);
                        object o = cmd.ExecuteScalar();
                        if (o != DBNull.Value)
                        {

                            double budgetSpent = Convert.ToDouble(o);
                            spentSum += budgetSpent;
                            categoryBudget.Spent = budgetSpent.ToString();
                        }
                        else
                        {
                            categoryBudget.Spent = "0";
                        }
                    }
                }
            }
            if (spentSum > 0)
            {
                foreach (CategoryBudgetStore categoryBudget in listCategoryBudget)
                {
                    double budgetSpent = double.Parse(categoryBudget.Spent);
                    double percentage = (budgetSpent / spentSum) * 100;
                    categoryBudget.Percentage = percentage.ToString("f1");
                }
            }
        }

        public static List<NetworkConditionStore> GetNetworkCondition(string simulationID)
        {
            List<NetworkConditionStore> listConditions = null;
            string networkID = GetNetworkID(simulationID);
            if (networkID != null)
            {
                string targetTable = DB.TablePrefix + "TARGET_" + networkID + "_" + simulationID;
                List<string> targetIDs = GetTargetID(simulationID);
                string deficientID = GetDeficientID(simulationID);

                if (targetIDs.Count > 0 && DB.CheckIfTableExists(targetTable) == 1)
                {
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        connection.Open();
                        string select = "SELECT TARGETID,YEARS, TARGETMET FROM " + targetTable + " ORDER BY YEARS";
                        SqlCommand cmd = new SqlCommand(select, connection);
                        SqlDataReader dr = cmd.ExecuteReader();
                        listConditions = new List<NetworkConditionStore>();
                        while (dr.Read())
                        {
                            string id = dr["TARGETID"].ToString();
                            if (targetIDs.Contains(id))
                            {
                                NetworkConditionStore networkCondition = new NetworkConditionStore();
                                if (dr["YEARS"] != DBNull.Value) networkCondition.Year = dr["YEARS"].ToString();
                                if (dr["TARGETMET"] != DBNull.Value) networkCondition.Average = dr["TARGETMET"].ToString();
                                listConditions.Add(networkCondition);
                            }
                        }
                        dr.Close();

                        //select = "SELECT YEARS, TARGETMET FROM " + targetTable + " WHERE TARGETID=@deficientID ORDER BY YEARS";
                        //cmd = new SqlCommand(select, connection);
                        //cmd.Parameters.Add(new SqlParameter("deficientID", deficientID));
                        //dr = cmd.ExecuteReader();
                        //while (dr.Read())
                        //{
                        //    string year = null;
                        //    string deficient = null;
                        //    if (dr["YEARS"] != DBNull.Value) year = dr["YEARS"].ToString();
                        //    if (dr["TARGETMET"] != DBNull.Value) deficient = dr["TARGETMET"].ToString();

                        //    NetworkConditionStore networkCondition = listConditions.Find(delegate(NetworkConditionStore ncs) { return ncs.Year == year; });

                        //    if (networkCondition != null)
                        //    {
                        //        double value = Convert.ToDouble(deficient) * 100;
                        //        networkCondition.Deficient = value.ToString();
                        //    }
                        //}
                        //dr.Close();
                    }
                }
            }
            return listConditions;
        }

        public static List<string> GetTargetID(string simulationID)
        {
            List<string> targetIDs = new List<string>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {

                connection.Open();
                string select = "SELECT ID_ FROM " + DB.TablePrefix + "TARGETS WHERE SIMULATIONID=@simulationID AND ATTRIBUTE_='OverallConditionIndex' AND TARGETNAME='OCI TARGET' ORDER BY YEARS";
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    string targetID = dr["ID_"].ToString();
                    targetIDs.Add(targetID);
                }
            }
            return targetIDs;
        }

        public static string GetDeficientID(string simulationID)
        {
            string deficientID = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string select = "SELECT ID_ FROM " + DB.TablePrefix + "DEFICIENTS WHERE SIMULATIONID=@simulationID AND ATTRIBUTE_='OverallConditionIndex' AND DEFICIENTNAME='OCI Deficient'";
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    deficientID = dr["ID_"].ToString();
                }
            }
            return deficientID;
        }


        public static List<PriorityStore> GetPriorities(string simulationID)
        {
            List<PriorityStore> priorities = new List<PriorityStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string select = "SELECT F.PRIORITYID, P.PRIORITYLEVEL, P.CRITERIA, P.YEARS, F.BUDGET, F.FUNDING FROM " + DB.TablePrefix + "PRIORITY P, " + DB.TablePrefix + "PRIORITYFUND F WHERE P.PRIORITYID = F.PRIORITYID AND P.SIMULATIONID='" + simulationID + "'";
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                SqlDataReader dr = cmd.ExecuteReader();

                PriorityStore priority = null;
                if (dr.Read())
                {
                    string priorityID = dr["PRIORITYID"].ToString();
                    int priorityLevel = Convert.ToInt32(dr["PRIORITYLEVEL"]);
                    string criteria = null;
                    string years = null;

                    if (dr["CRITERIA"] != DBNull.Value) criteria = dr["CRITERIA"].ToString();
                    if (dr["YEARS"] != DBNull.Value) years = dr["YEARS"].ToString();

                    if (priority == null || priority.PriorityID != priorityID)
                    {
                        priority = new PriorityStore(priorityID, priorityLevel, criteria, years, new List<PriorityFundStore>());
                        priorities.Add(priority);
                    }

                    priority.PriorityFunds.Add(new PriorityFundStore(dr["BUDGET"].ToString(), Convert.ToDouble(dr["FUNDING"])));
                }
            }
            return priorities;
        }

        public static List<ActivityStore> GetActivities(string simulationID, bool isIncludeOCI)
        {
            string assetType = GetAssetType(simulationID);
            List<AttributeStore> attributes = OMS.GetAssetAttributes(assetType);

            List<ActivityStore> activities = new List<ActivityStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM " + DB.TablePrefix + "TREATMENTS WHERE SIMULATIONID='" + simulationID + "'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    try
                    {
                        ActivityStore activity = new ActivityStore(assetType);

                        if (dr["TREATMENT"] != DBNull.Value) activity.Activity = dr["TREATMENT"].ToString();
                        if (dr["TREATMENTID"] != DBNull.Value) activity.ActivityID = dr["TREATMENTID"].ToString();
                        if (dr["BUDGET"] != DBNull.Value)
                        {
                            string[] budgets = dr["BUDGET"].ToString().Split('|');
                            activity.Budgets = new List<string>();
                            foreach (string budget in budgets)
                            {
                                activity.Budgets.Add(budget);
                            }
                        }
                        if (dr["OMS_IS_SELECTED"] != DBNull.Value) activity.IsSelected = Convert.ToBoolean(dr["OMS_IS_SELECTED"]);
                        if (dr["OMS_IS_EXCLUSIVE"] != DBNull.Value) activity.IsExclusive = Convert.ToBoolean(dr["OMS_IS_EXCLUSIVE"]);
                        if (dr["OMS_IS_REPEAT"] != DBNull.Value) activity.IsRepeatActivity = Convert.ToBoolean(dr["OMS_IS_REPEAT"]);
                        if (dr["OMS_REPEAT_START"] != DBNull.Value) activity.RepeatStart = Convert.ToInt32(dr["OMS_REPEAT_START"]);
                        if (dr["OMS_REPEAT_INTERVAL"] != DBNull.Value) activity.RepeatInterval = Convert.ToInt32(dr["OMS_REPEAT_INTERVAL"]);
                        if (dr["BEFORESAME"] != DBNull.Value) activity.YearSame = Convert.ToInt32(dr["BEFORESAME"]);
                        if (dr["OMS_OID"] != DBNull.Value) activity.OmsOID = dr["OMS_OID"].ToString();
                        
                        string criteria = GetActivityFeasibility(activity.ActivityID);
                        activity.RawCriteria = criteria;


                        string cost = GetActivityCost(activity.ActivityID);
                        activity.RawCost = cost;


                        if (cost != null)
                        {
                            string[] values = cost.Split('*');
                            if (values.Length > 0)
                            {
                                activity.Cost = values[0];
                            }
                            if (values.Length > 1)
                            {
                                string parsed = SimulationComponents.FindAttribute(values[1], 0);
                                //Converts OmsObjectUserIDHierarchy to display name                               
                                AttributeStore attribute = attributes.Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == parsed; });
                                if(attribute != null)
                                {
                                    activity.CostField = "[" + attribute.OmsHierarchy + "]";
                                }
                            }
                        }
                        if (isIncludeOCI)
                        {
                            List<ConsequenceStore> listConsequence = GetConsequences(activity.ActivityID);
                            activity.ParseCriteria(criteria, listConsequence);
                        }
                        activities.Add(activity);
                    }
                    catch
                    {

                    }
                }
            }
            return activities;
        }
        
        
        
        private static List<ConsequenceStore> GetConsequences(string treatmentID)
        {
            List<ConsequenceStore> consequences = new List<ConsequenceStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ATTRIBUTE_,CHANGE_ FROM " + DB.TablePrefix + "CONSEQUENCES WHERE TREATMENTID='" + treatmentID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ConsequenceStore consequence = new ConsequenceStore();
                        if (dr["ATTRIBUTE_"] != DBNull.Value) consequence.Attribute = dr["ATTRIBUTE_"].ToString();
                        if (dr["CHANGE_"] != DBNull.Value) consequence.Change = dr["CHANGE_"].ToString();
                        consequences.Add(consequence);
                    }
                }
                catch
                {
                    consequences = null;
                }
            }


            return consequences;
        }

        public static string GetActivityFeasibility(string treatmentID)
        {
            string criteria = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT CRITERIA FROM " + DB.TablePrefix + "FEASIBILITY WHERE TREATMENTID='" + treatmentID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["CRITERIA"] != DBNull.Value) criteria = dr["CRITERIA"].ToString();
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    criteria = null;
                }
            }
            return criteria;
        }


        public static string GetActivityCost(string treatmentID)
        {
            string cost = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COST_ FROM " + DB.TablePrefix + "COSTS WHERE TREATMENTID='" + treatmentID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["COST_"] != DBNull.Value) cost = dr["COST_"].ToString();
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    cost = null;
                }
            }
            return cost;
        }


        public static string GetAssetType(string simulationID)
        {
            string assetType = null;
            if (simulationID != null)
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT ASSET_TYPE FROM " + DB.TablePrefix + "SIMULATIONS WHERE SIMULATIONID='" + simulationID + "'";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            if (dr["ASSET_TYPE"] != DBNull.Value) assetType = dr["ASSET_TYPE"].ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        DataAccessExceptionHandler.HandleException(e, false);
                        assetType = null;
                    }
                }
            }
            return assetType;
        }

        public static List<YearBudgetAmountStore> GetBudgetsByYear(string simulationID)
        {
            List<YearBudgetAmountStore> yearBudgets = new List<YearBudgetAmountStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT YEAR_,BUDGETNAME,AMOUNT FROM " + DB.TablePrefix + "YEARLYINVESTMENT WHERE SIMULATIONID='" + simulationID + "' ORDER BY BUDGETNAME, YEAR_";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        YearBudgetAmountStore yearBudget = new YearBudgetAmountStore();
                        if (dr["YEAR_"] != DBNull.Value) yearBudget.Year = Convert.ToInt32(dr["YEAR_"]);
                        if (dr["BUDGETNAME"] != DBNull.Value) yearBudget.Budget = dr["BUDGETNAME"].ToString();
                        if (dr["AMOUNT"] != DBNull.Value) yearBudget.Amount = Convert.ToDouble(dr["AMOUNT"]);
                        yearBudgets.Add(yearBudget);
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    yearBudgets = null;
                }
            }
            return yearBudgets;
        }

        public static List<TargetStore> GetTargets(string simulationID)
        {
            List<TargetStore> targets = new List<TargetStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_,TARGETMEAN,TARGETNAME,CRITERIA FROM " + DB.TablePrefix + "TARGETS WHERE SIMULATIONID='" + simulationID + "' AND IS_OMS_PRIORITY='1'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        TargetStore target = new TargetStore();
                        if (dr["ID_"] != DBNull.Value) target.ID = dr["ID_"].ToString();
                        if (dr["TARGETMEAN"] != DBNull.Value) target.TargetOCI = Convert.ToDouble(dr["TARGETMEAN"]);
                        if (dr["TARGETNAME"] != DBNull.Value) target.Name = dr["TARGETNAME"].ToString();
                        if (dr["CRITERIA"] != DBNull.Value) target.Criteria = dr["CRITERIA"].ToString();
                        targets.Add(target);
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    targets = null;
                }

            }
            return targets;
        }



        public static List<TargetStore> GetEditSimulationTargets(string simulationID)
        {
            List<TargetStore> targets = new List<TargetStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_,TARGETMEAN,YEARS FROM " + DB.TablePrefix + "TARGETS WHERE SIMULATIONID='" + simulationID + "' AND IS_OMS_PRIORITY='0' ORDER BY YEARS";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        TargetStore target = new TargetStore();
                        if (dr["ID_"] != DBNull.Value) target.ID = dr["ID_"].ToString();
                        if (dr["TARGETMEAN"] != DBNull.Value) target.TargetOCI = Convert.ToDouble(dr["TARGETMEAN"]);
                        if (dr["YEARS"] != DBNull.Value) target.Year = dr["YEARS"].ToString();
                        targets.Add(target);
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    targets = null;
                }

            }
            return targets;
        }


        public static List<TargetStore> GetTargetsForCopy(string simulationID)
        {
            List<TargetStore> targets = new List<TargetStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT TARGETMEAN,YEARS,TARGETNAME,CRITERIA,IS_OMS_PRIORITY FROM " + DB.TablePrefix + "TARGETS WHERE SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        TargetStore target = new TargetStore();
                        if (dr["TARGETMEAN"] != DBNull.Value) target.TargetOCI = Convert.ToDouble(dr["TARGETMEAN"]);
                        if (dr["YEARS"] != DBNull.Value) target.Year = dr["YEARS"].ToString();
                        if (dr["TARGETNAME"] != DBNull.Value) target.Name = dr["TARGETNAME"].ToString();
                        if (dr["CRITERIA"] != DBNull.Value) target.Criteria = dr["CRITERIA"].ToString();
                        if (dr["IS_OMS_PRIORITY"] != DBNull.Value) target.IsOMSPriority = dr["IS_OMS_PRIORITY"].ToString();
                        targets.Add(target);
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    targets = null;
                }

            }
            return targets;
        }


        public static List<string> GetResultOIDs(string simulationID, string orderbyField)
        {
            List<string> resultOIDs = new List<string>();
            string firstYear = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT FIRSTYEAR FROM " + DB.TablePrefix + "INVESTMENTS WHERE SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        firstYear = dr["FIRSTYEAR"].ToString();
                    }
                    dr.Close();

                    string order_field = orderbyField + "_" + firstYear;
                    query = "SELECT SECTIONID FROM " + DB.TablePrefix + "SIMULATION_1_" + simulationID + " ORDER BY " + order_field;
                    cmd = new SqlCommand(query, connection);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        resultOIDs.Add(dr["SECTIONID"].ToString());
                    }
                    dr.Close();
                }
                catch
                {
                    resultOIDs = null;
                }
            }
            return resultOIDs;
        }

        /// <summary>
        /// Gets all of the Activity Results
        /// </summary>
        /// <param name="sessionID"></param>
        public static int GetActivityResults(string sessionID, WorkPlanFilter filter)
        {

            SessionStore session = OMS.GetWorkPlanSessionFromSessionID(sessionID);
            string simulationID = session.Simulation.SimulationID;
            List<string> scenarioBudgets = OMS.GetWorkPlanBudgets(sessionID);
            List<string> scenarioActivities = OMS.GetWorkPlanActivities(sessionID);
            session.OIDs = new List<string>();
            session.EncodedActivities = new List<string>();
            Dictionary<string, List<ActivityResultStore>> feasibles = GetFeasibleActivities(simulationID, null);
            bool filterMet = false;
            int rowsFilterMet = 0;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM " + DB.TablePrefix + "REPORT_1_" + simulationID + " ORDER BY SECTIONID,YEARS";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    string previousSectionID = "";
                    int previousYear = 0;
                    List<byte> activities = null;
                    List<ActivityResultStore> decodedActivities = null;
                    double area = 0;// Stores area of previous section for feasible
                    byte[] activityArray = null;
                    while (dr.Read())
                    {
                        string sectionID = dr["SECTIONID"].ToString();
                        ActivityResultStore activity = new ActivityResultStore();
                        activity.OID = sectionID;
                        if (dr["TREATMENT"] != DBNull.Value) activity.ActivityName = dr["TREATMENT"].ToString();
                        if (dr["YEARS"] != DBNull.Value) activity.Year = Convert.ToInt32(dr["YEARS"]);

                        if (sectionID != previousSectionID)
                        {
                            if (activities != null)
                            {
                                //Add feasibles that were left over from previous year
                                //Remove from feasible if activity exist
                                if (feasibles.ContainsKey(previousSectionID))
                                {
                                    List<ActivityResultStore> feasible = feasibles[previousSectionID];
                                    foreach (ActivityResultStore feasibleActivity in feasible)
                                    {
                                        //If any activity meets filter row is written
                                        if (!filterMet) filterMet = IsFilterMet(activity, filter);

                                        activityArray = feasibleActivity.EncodeActivity(scenarioActivities, scenarioBudgets, session.Simulation.StartDate.Year);
                                        foreach (byte b in activityArray)
                                        {
                                            activities.Add(b);
                                        }
                                        decodedActivities.Add(feasibleActivity);
                                    }
                                    //Remove feasibles just added to result
                                    feasible.RemoveAll(delegate(ActivityResultStore a) { return a.Year < activity.Year; });
                                    //Remove feasible current year because a treatment of this name was committed.
                                    feasible.RemoveAll(delegate(ActivityResultStore a) { return a.ActivityName == activity.ActivityName && a.Year == activity.Year; });
                                }

                                if (!filter.UseFilter ||  filterMet)
                                {
                                    //Convert activities to Base64 string
                                    string base64Activities = Convert.ToBase64String(activities.ToArray());
                                    session.EncodedActivities.Add(base64Activities);
                                    session.OIDs.Add(previousSectionID);
                                    rowsFilterMet++;
                                }
                                filterMet = false;
                            }
                            activities = new List<byte>();
                            decodedActivities = new List<ActivityResultStore>();
                            area = 0;

                        }
                        string change = "";

                        if (dr["COST_"] != DBNull.Value) activity.Cost = Convert.ToDouble(dr["COST_"]);
                        if (dr["BUDGET"] != DBNull.Value) activity.Budget = dr["BUDGET"].ToString();
                        if (dr["RESULT_TYPE"] != DBNull.Value) activity.ResultType = Convert.ToInt32(dr["RESULT_TYPE"]);
                        if (dr["CHANGEHASH"] != DBNull.Value) change = dr["CHANGEHASH"].ToString();
                        if (dr["AREA"] != DBNull.Value) area = Convert.ToDouble(dr["AREA"]);
                        if (dr["COMMITORDER"] != DBNull.Value) activity.CommitOrder = Convert.ToInt32(dr["COMMITORDER"]);
                        string[] attributes = change.Split('\n');
                        foreach (string attribute in attributes)
                        {
                            string[] values = attribute.Split('\t');
                            if (values[0] == "OverallConditionIndex")
                            {
                                activity.OCIChange = (int) Math.Round(Convert.ToDouble(values[1]));
                                activity.OCIChange += 100;// Add 100 to allow -100 to +100 OCI
                            }
                        }
                        
                        //If any activity meets filter row is written
                        if (!filterMet)
                        {
                            filterMet = IsFilterMet(activity, filter);
                        }
                        //Remove from feasible if activity exist
                        if (feasibles.ContainsKey(sectionID))
                        {
                            List<ActivityResultStore> feasible = feasibles[sectionID];
                            foreach (ActivityResultStore feasibleActivity in feasible)
                            {
                                //Feasible before activities.
                                if (feasibleActivity.Year < activity.Year)
                                {
                                    if (!filterMet) filterMet = IsFilterMet(activity, filter);

                                    activityArray = feasibleActivity.EncodeActivity(scenarioActivities, scenarioBudgets, session.Simulation.StartDate.Year);
                                    foreach (byte b in activityArray)
                                    {
                                        activities.Add(b);
                                    }
                                    decodedActivities.Add(feasibleActivity);
                                }
                            }
                            //Remove feasibles just added to result
                            feasible.RemoveAll(delegate(ActivityResultStore a) { return a.Year < activity.Year; });
                            //Remove feasible current year because a treatment of this name was committed.
                            feasible.RemoveAll(delegate(ActivityResultStore a) { return a.ActivityName == activity.ActivityName && a.Year == activity.Year; });
                        }


                        activityArray = activity.EncodeActivity(scenarioActivities, scenarioBudgets, session.Simulation.StartDate.Year);
                        foreach (byte b in activityArray)
                        {
                            activities.Add(b);
                        }
                        decodedActivities.Add(activity);
                        previousSectionID = activity.OID;
                        previousYear = activity.Year;
                    }
      
                    
                    
                    
                    //Add last feasibles for last asset.
                    if (activities != null) activities = new List<byte>();
                    if (feasibles.ContainsKey(previousSectionID))
                    {
                        List<ActivityResultStore> feasible = feasibles[previousSectionID];
                        foreach (ActivityResultStore feasibleActivity in feasible)
                        {
                            //If any activity meets filter row is written
                            if (!filterMet) filterMet = IsFilterMet(feasibleActivity, filter);

                            activityArray = feasibleActivity.EncodeActivity(scenarioActivities, scenarioBudgets, session.Simulation.StartDate.Year);
                            foreach (byte b in activityArray)
                            {
                                activities.Add(b);
                            }
                            decodedActivities.Add(feasibleActivity);
                        }
                        feasible.Clear();
                    }

                    //Convert activities to Base64 string
                    if (!filter.UseFilter || filterMet)
                    {
                        string base64ActivitiesFinal = Convert.ToBase64String(activities.ToArray());
                        session.EncodedActivities.Add(base64ActivitiesFinal);
                        session.OIDs.Add(previousSectionID);
                        rowsFilterMet++;
                    }
                    
                }
                catch(Exception e)
                {

                }
            
                if(filter.UseFilter)
                {
                    int numberYear = session.Simulation.Years;
                    int startYear = session.Simulation.StartDate.Year;
                    for (int i = 0; i < session.OIDs.Count; i++)
                    {
                        string oid = session.OIDs[i];
                        byte[] yearOCI = new byte[numberYear];
                        for (int year = startYear; year < startYear + numberYear; year++)
                        {
                            string overallConditionIndex = session.OIDResults[oid]["OverallConditionIndex"][year.ToString()];
                            int oci = (int)Math.Round(Convert.ToDouble(overallConditionIndex));
                            yearOCI[year - startYear] = (byte)oci;
                        }
                        string oci64 = Convert.ToBase64String(yearOCI);
                        session.EncodedActivities[i] = oci64 + ":" + session.EncodedActivities[i];
                    }
                }
            }
            return rowsFilterMet;
        }

        internal static bool IsFilterMet(ActivityResultStore activity, WorkPlanFilter filter)
        {
            if (!filter.UseFilter) return true;  //If not using filter always return true;  

            if(filter.Status > -1 && filter.Status != activity.ResultType)
            {
                return false;
            }
            else if(filter.Status == 0 && activity.ActivityName == "No Treatment")
            {
                return false;
            }

            if(filter.Budget != null && filter.Budget != activity.Budget)
            {
                return false;
            }

            if(filter.Year != null && filter.Year != activity.Year.ToString())
            {
                return false;
            }

            if(filter.Activity != null && filter.Activity != activity.ActivityName)
            {
                return false;
            }

            if(filter.MinimumCost > double.MinValue && filter.MinimumCost > activity.Cost)
            {
                return false;
            }

            if(filter.MaximumCost < double.MaxValue && filter.MaximumCost < activity.Cost)
            {
                return false;
            }

            if(filter.MinimumImpact > double.MinValue && filter.MinimumImpact > activity.OCIChange-100)
            {
                return false;
            }

            if(filter.MaximumImpact < double.MaxValue && filter.MaximumImpact < activity.OCIChange-100)
            {
                return false;
            }


            return true;
        }

        public static string GetActivityResults(string sessionID, string sectionID)
        {

            SessionStore session = OMS.GetWorkPlanSessionFromSessionID(sessionID);
            string simulationID = session.Simulation.SimulationID;
            List<string> scenarioBudgets = OMS.GetWorkPlanBudgets(sessionID);
            List<string> scenarioActivities = OMS.GetWorkPlanActivities(sessionID);

            //Modify this to just get feasible for a given section.
            Dictionary<string, List<ActivityResultStore>> feasibles = GetFeasibleActivities(simulationID, sectionID);
            string base64Activities = null;

            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM " + DB.TablePrefix + "REPORT_1_" + simulationID + " WHERE SECTIONID='" + sectionID + "' ORDER BY SECTIONID,YEARS";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    
                    List<byte> activities = null;
                    List<ActivityResultStore> decodedActivities = null;
                    double area = 0;// Stores area of previous section for feasible
                    byte[] activityArray = null;
                    activities = new List<byte>();
                    decodedActivities = new List<ActivityResultStore>();
                    area = 0;
                    int previousYear = 0;
                    string previousSectionID = "";

                    while (dr.Read())
                    {
                        string change = "";
                        ActivityResultStore activity = new ActivityResultStore();
                        activity.OID = sectionID;
                        if (dr["TREATMENT"] != DBNull.Value) activity.ActivityName = dr["TREATMENT"].ToString();
                        if (dr["YEARS"] != DBNull.Value) activity.Year = Convert.ToInt32(dr["YEARS"]);
                        if (dr["COST_"] != DBNull.Value) activity.Cost = Convert.ToDouble(dr["COST_"]);
                        if (dr["BUDGET"] != DBNull.Value) activity.Budget = dr["BUDGET"].ToString();
                        if (dr["RESULT_TYPE"] != DBNull.Value) activity.ResultType = Convert.ToInt32(dr["RESULT_TYPE"]);
                        if (dr["COMMITORDER"] != DBNull.Value) activity.CommitOrder = Convert.ToInt32(dr["COMMITORDER"]);
                        if (dr["CHANGEHASH"] != DBNull.Value) change = dr["CHANGEHASH"].ToString();
                        if (dr["AREA"] != DBNull.Value) area = Convert.ToDouble(dr["AREA"]);
                        string[] attributes = change.Split('\n');
                        foreach (string attribute in attributes)
                        {
                            string[] values = attribute.Split('\t');
                            if (values[0] == "OverallConditionIndex")
                            {
                                activity.OCIChange = (int)Math.Round(Convert.ToDouble(values[1]));
                                activity.OCIChange += 100;// Add 100 to allow -100 to +100 OCI
                            }
                        }

                        //Remove from feasible if activity exist
                        if(feasibles.ContainsKey(sectionID))
                        {
                            List<ActivityResultStore> feasible = feasibles[sectionID];
                            foreach (ActivityResultStore feasibleActivity in feasible)
                            {
                                //Feasible before activities.
                                if(feasibleActivity.Year < activity.Year)
                                {
                                    activityArray = feasibleActivity.EncodeActivity(scenarioActivities, scenarioBudgets, session.Simulation.StartDate.Year);
                                    foreach (byte b in activityArray)
                                    {
                                        activities.Add(b);
                                    }
                                    decodedActivities.Add(feasibleActivity);
                                }
                            }
                            //Remove feasibles just added to result
                            feasible.RemoveAll(delegate(ActivityResultStore a) { return a.Year < activity.Year; });
                            //Remove feasible current year because a treatment of this name was committed.
                            feasible.RemoveAll(delegate(ActivityResultStore a) { return a.ActivityName == activity.ActivityName && a.Year == activity.Year; });
                        }



                        activityArray = activity.EncodeActivity(scenarioActivities, scenarioBudgets, session.Simulation.StartDate.Year);
                        foreach (byte b in activityArray)
                        {
                            activities.Add(b);
                        }
                        decodedActivities.Add(activity);
                        previousSectionID = activity.OID.ToString();
                        previousYear = activity.Year;
                    }
                    dr.Close();//Now get the feasible treatments from the benefit cost table.

                    //Add remain feasibles (all others have been added and removed from this list.
                    if (feasibles.ContainsKey(previousSectionID))
                    {
                        foreach(ActivityResultStore feasibleActivity in feasibles[previousSectionID])
                        {
                            activityArray = feasibleActivity.EncodeActivity(scenarioActivities, scenarioBudgets, session.Simulation.StartDate.Year);
                            foreach (byte b in activityArray)
                            {
                                activities.Add(b);
                            }
                            decodedActivities.Add(feasibleActivity);
                        }
                    }
 
                    if (activities != null)
                    {
                        
                        //Convert activities to Base64 string
                        base64Activities = Convert.ToBase64String(activities.ToArray());

                        //Need to find index of sectionID;
                        int index = session.OIDs.IndexOf(sectionID);
                        int numberYear = session.Simulation.Years;
                        int startYear = session.Simulation.StartDate.Year;

                        byte[] yearOCI = new byte[numberYear];
                        List<double> ocis = GetOCI(simulationID, sectionID, numberYear, startYear);
                        int i = 0;
                        foreach(double oci in ocis)
                        {
                            int noci = (int)oci;
                            yearOCI[i] = (byte)noci;
                            i++;
                        }
                        string oci64 = Convert.ToBase64String(yearOCI);
                        if(index > -1)
                        {
                            base64Activities = oci64 + ":" + base64Activities;
                            session.EncodedActivities[index] =  base64Activities; 
                        }
                    }
                }
                catch
                {
                }
            }
            return base64Activities;
        }

        //Gets OCIs from Simulation table for a given sectionID
        private static List<double> GetOCI(string simulationID, string sectionID, int numberYear, int startYear)
        {
            List<double> ocis = new List<double>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM " + DB.TablePrefix + "SIMULATION_1_" + simulationID + " WHERE SECTIONID='" + sectionID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    
                    if (dr.Read())
                    {
                        for(int i = startYear; i < startYear+numberYear; i++)
                        {
                            string oci = "OVERALLCONDITIONINDEX_" + i.ToString();
                            if(dr[oci] != DBNull.Value)
                            {
                                ocis.Add(Convert.ToDouble(dr[oci]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return ocis;
        }




        private static Dictionary<string, List<ActivityResultStore>> GetFeasibleActivities(string simulationID, string sectionID)
        {
            Dictionary<string, List<ActivityResultStore>> feasibles = new Dictionary<string, List<ActivityResultStore>>();
            string benefitTable = DB.TablePrefix + "BENEFITCOST_1_" + simulationID;

            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    List<ActivityResultStore> activities = null;
                    connection.Open();
                    string where = "";
                    if (sectionID != null)
                    {
                        where = " WHERE SECTIONID='" + sectionID + "'";
                    }
                    string query = "SELECT DISTINCT SECTIONID, YEARS, TREATMENT, COST_, BUDGET FROM " + benefitTable + where + " ORDER BY SECTIONID,YEARS";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    string previousSectionID = "";
                    while (dr.Read())
                    {
                        sectionID = dr["SECTIONID"].ToString();
                        if (sectionID != previousSectionID)
                        {
                            if (activities != null)
                            {
                                feasibles.Add(previousSectionID, activities);
                            }
                            activities = new List<ActivityResultStore>();
                        }

                        ActivityResultStore activity = new ActivityResultStore();
                        activity.OID = sectionID;
                        if (dr["TREATMENT"] != DBNull.Value) activity.ActivityName = dr["TREATMENT"].ToString();
                        if (dr["YEARS"] != DBNull.Value) activity.Year = Convert.ToInt32(dr["YEARS"]);
                        if (dr["COST_"] != DBNull.Value) activity.Cost = Convert.ToDouble(dr["COST_"]);
                        if (dr["BUDGET"] != DBNull.Value) activity.Budget = dr["BUDGET"].ToString();
                        activity.CommitOrder = 0;
                        activity.ResultType = 2;
                        activity.OCIChange = 100;
                        previousSectionID = sectionID;
                        activities.Add(activity);
                    }
                    if (activities != null)
                    {
                        feasibles.Add(previousSectionID, activities);
                    }
                }
                catch
                {
                }
            }
            return feasibles;
        }

        public static List<string> GetTreatmentsPerYear(string simulationID)
        {
            List<string> treatmentsPeryear = new List<string>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT YEARS,COUNT(*) AS ACTIVITY_COUNT FROM " + DB.TablePrefix + "REPORT_1_" + simulationID + " WHERE TREATMENT<>'No Treatment' GROUP BY YEARS ORDER BY YEARS";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        treatmentsPeryear.Add(dr["ACTIVITY_COUNT"].ToString());
                    }
                }
                catch
                {
                    treatmentsPeryear = null;
                }
            }
            return treatmentsPeryear;
        }

        public static List<AssetLocationStore> GetAssetLocation(string simulationID)
        {
            List<AssetLocationStore> assetLocations = new List<AssetLocationStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string query = "SELECT OID, ATTRIBUTE_,ASSET_VALUE FROM " +  DB.TablePrefix + "OMS_ASSETS WHERE (ATTRIBUTE_='cgShape' OR ATTRIBUTE_='ID' OR ATTRIBUTE_='STREET') AND SIMULATIONID='" + simulationID + "'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                AssetLocationStore location = null;
                
                while (dr.Read())
                {
                    string OID = dr["OID"].ToString();
                    if(location == null || location.OID != OID)
                    {
                        if(location != null)
                        {
                            //Add location to list
                            assetLocations.Add(location);
                        }
                        location = new AssetLocationStore();
                        location.OID = OID;
                    }

                    string attribute = dr["ATTRIBUTE_"].ToString();
                    string value = null;
                    if(dr["ASSET_VALUE"] != DBNull.Value) value = dr["ASSET_VALUE"].ToString();

                    switch (attribute.ToUpper())
                    {
                        case "ID":
                            location.ID = value;
                            break;
                        case "STREET":
                            location.Name = value;
                            break;
                        case "CGSHAPE":
                            if(value != null)
                            {
                                SqlGeometry geo = dr["ASSET_VALUE"] as SqlGeometry;
                                location.Shape = geo.STAsText().ToString();
                            }
                            break;
                    }
                }
                if (location != null)//After reading last locatuib
                {
                    //Add location to list
                    assetLocations.Add(location);
                }
            }
            return assetLocations;
        }


        public static List<string> GetBudgetsForActivity(string activityID)
        {
            List<string> budgets = new List<string>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string query = "SELECT BUDGET FROM " + DB.TablePrefix + "TREATMENTS WHERE TREATMENTID=@activityID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.Add(new SqlParameter("activityID", activityID));
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["BUDGET"] != DBNull.Value)
                    {
                        budgets = dr["BUDGET"].ToString().Split('|').ToList();
                    }
                }
            }
            return budgets;
        }


        public static Dictionary<string,  Dictionary<string, Dictionary<string,string>>> GetSimulationResults(string simulationID)
        {
            Dictionary<string,  Dictionary<string, Dictionary<string,string>>> oidResults = new Dictionary<string,  Dictionary<string, Dictionary<string,string>>>();

            string networkID = null;
            string simulationVariables = null;
            int startYear = 0;
            int numberYear = 0;


            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string query = "SELECT NETWORKID, SIMULATION_VARIABLES,FIRSTYEAR,NUMBERYEARS FROM  " + DB.TablePrefix + "SIMULATIONS S INNER JOIN " + DB.TablePrefix + "INVESTMENTS I ON S.SIMULATIONID=I.SIMULATIONID WHERE S.SIMULATIONID='" + simulationID + "'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["NETWORKID"] != DBNull.Value) networkID = dr["NETWORKID"].ToString();
                    if (dr["SIMULATION_VARIABLES"] != DBNull.Value) simulationVariables = dr["SIMULATION_VARIABLES"].ToString();
                    if (dr["NUMBERYEARS"] != DBNull.Value) numberYear = Convert.ToInt32(dr["NUMBERYEARS"]);
                    if (dr["FIRSTYEAR"] != DBNull.Value) startYear = Convert.ToInt32(dr["FIRSTYEAR"]);
                }
            }

            if (networkID != null && simulationVariables != null)
            {
 
                List<string> attributes = simulationVariables.Split('\t').ToList();
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM " + DB.TablePrefix + "SIMULATION_" + networkID + "_" + simulationID;
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string sectionID = dr["SECTIONID"].ToString();
                        Dictionary<string, Dictionary<string,string>> section = new Dictionary<string,Dictionary<string,string>>();
                        oidResults.Add(sectionID, section);

                        foreach(string attribute in attributes)
                        {
                            Dictionary<string,string> yearValues = new Dictionary<string,string>();
                            section.Add(attribute,yearValues);
                            
                            for(int year = startYear; year < startYear + numberYear; year++)
                            {
                                string value = null;
                                string column = attribute + "_" + year.ToString();
                                if(dr[column] != DBNull.Value) value = dr[column].ToString();
                                yearValues.Add(year.ToString(), value);//Value can be null here.
                            }
                        }
                    }
                }
            }
            return oidResults;
        }


        public static List<CommittedStore> GetCommittedForCopy(string simulationID)
        {
            List<CommittedStore> committeds = new List<CommittedStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string query = "SELECT SECTIONID, YEARS, TREATMENTNAME, YEARSAME, YEARANY, BUDGET, COST_, OMS_IS_REPEAT, OMS_IS_EXCLUSIVE, OMS_IS_NOT_ALLOWED FROM  " + DB.TablePrefix + "COMMITTED_ WHERE SIMULATIONID='" + simulationID + "'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CommittedStore committed = new CommittedStore();
                    if (dr["SECTIONID"] != DBNull.Value) committed.SectionID = dr["SECTIONID"].ToString();
                    if (dr["YEARS"] != DBNull.Value) committed.Years = Convert.ToInt32(dr["YEARS"]);
                    if (dr["TREATMENTNAME"] != DBNull.Value) committed.TreatmentName = dr["TREATMENTNAME"].ToString();
                    if (dr["YEARSAME"] != DBNull.Value) committed.YearsAny = Convert.ToInt32(dr["YEARSAME"]);
                    if (dr["YEARANY"] != DBNull.Value) committed.YearsSame = Convert.ToInt32(dr["YEARANY"]);
                    if (dr["YEARANY"] != DBNull.Value) committed.Budget = dr["BUDGET"].ToString();
                    if (dr["COST_"] != DBNull.Value) committed.Cost = (float)Convert.ToDouble(dr["COST_"]);
                    if (dr["OMS_IS_REPEAT"] != DBNull.Value) committed.OMSIsRepeat =  Convert.ToBoolean(dr["OMS_IS_REPEAT"]);
                    if (dr["OMS_IS_EXCLUSIVE"] != DBNull.Value) committed.OMSIsExclusive = Convert.ToBoolean(dr["OMS_IS_EXCLUSIVE"]);
                    if (dr["OMS_IS_NOT_ALLOWED"] != DBNull.Value) committed.OMSIsNotAllowed = Convert.ToBoolean(dr["OMS_IS_NOT_ALLOWED"]);
                    committeds.Add(committed);
                }
            }
            return committeds;
        }


        public static List<TargetResultStore> GetYearlyTargets(string simulationID)
        {
            List<TargetResultStore> targets = new List<TargetResultStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM " + DB.TablePrefix + "TARGET_1_" + simulationID + " WHERE YEARS IS NOT NULL";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TargetResultStore target = new TargetResultStore();
                    target.ID = Convert.ToInt32(dr["ID_"]);
                    target.TargetID = Convert.ToInt32(dr["TARGETID"]);
                    if (dr["YEARS"] != DBNull.Value) target.Years = dr["YEARS"].ToString();
                    if (dr["TARGETMET"] != DBNull.Value) target.TargetMet = Convert.ToDouble(dr["TARGETMET"]);
                    if (dr["AREA"] != DBNull.Value) target.Area = Convert.ToDouble(dr["AREA"]);
                    if (dr["ISDEFICIENT"] != DBNull.Value) target.IsDeficient = Convert.ToBoolean(dr["ISDEFICIENT"]);
                    targets.Add(target);
                }
            }
            return targets;
        }

        public static List<ActivityResultStore> GetRecommendedActivities(string simulationID)
        {
            List<ActivityResultStore> activities = new List<ActivityResultStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM " + DB.TablePrefix + "REPORT_1_" + simulationID + " WHERE RESULT_TYPE=0 ORDER BY SECTIONID,YEARS ";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ActivityResultStore activity = new ActivityResultStore();
                        activity.OID = dr["SECTIONID"].ToString();
                        activity.Year = Convert.ToInt32(dr["YEARS"]);
                        activity.ActivityName = dr["TREATMENT"].ToString();

                        if (activity.ActivityName != "No Treatment")
                        {
                            activities.Add(activity);
                        }
                    }
                }
                catch
                {

                }
            }
            return activities;
        }

        public static List<string> GetSimulationCSV(string simulationID)
        {
            List<string> assets = new List<string>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM " + DB.TablePrefix + "SIMULATION_1_" + simulationID + " ORDER BY SECTIONID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    string row = "";
                    for(int i = 0; i < dr.FieldCount; i++)
                    {
                        if(row.Length > 0) row += ",";
                        row += dr.GetName(i);
                    }
                    assets.Add(row.ToString());

                    while (dr.Read())
                    {
                        row = "";
                        bool addPlus = false;
                        for(int i = 0; i < dr.FieldCount; i++)
                        {
                            if(addPlus) row += ",";
                            if(dr.GetValue(i) != DBNull.Value) row += dr.GetValue(i).ToString();
                            addPlus = true;
                        }
                        assets.Add(row);
                    }
                }
                catch
                {
                    assets.Clear();
                }
            }
            return assets;
        }


        public static List<string> GetWorkPlanCSV(string simulationID)
        {
            List<string> activities = new List<string>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT S.ID_0, R.TREATMENT, R.YEARS AS YEAR, R.BUDGET,R.COST_, R.BC_RATIO, R.COMMITORDER,R.CHANGEHASH AS PRIORITY FROM " + DB.TablePrefix + "REPORT_1_" + simulationID + " R INNER JOIN "+ DB.TablePrefix + "SIMULATION_1_" + simulationID + " S ON S.SECTIONID=R.SECTIONID WHERE R.TREATMENT<> 'No Treatment' ORDER BY YEARS,R.SECTIONID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string row = "";
                        bool addPlus = false;
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            if (addPlus) row += ",";
                            if (dr.GetValue(i) != DBNull.Value)
                            {
                                if (i == 7)
                                {
                                    string[] attributes = dr.GetValue(i).ToString().Split('\n');
                                    foreach (string attribute in attributes)
                                    {
                                        string[] values = attribute.Split('\t');
                                        if (values[0] == "OverallConditionIndex")
                                        {
                                            int ociChange = (int)Math.Round(Convert.ToDouble(values[1]));
                                            row += ociChange.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    row += dr.GetValue(i).ToString();
                                }
                            }
                            addPlus = true;
                        }
                        activities.Add(row);
                    }
                }
                catch(Exception e)
                {
                    activities.Clear();
                }
            }
            return activities;
        }

    }
}
