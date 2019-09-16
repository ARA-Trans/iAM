using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DataAccessLayer.DTOs;
using Utility.ExceptionHandling;

namespace DataAccessLayer
{
    public static class UpdateScenario
    {
        public static int UpdateInvestments(string simulationID, string field, string value)
        {
            int rows = 0;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE " + DB.TablePrefix + "INVESTMENTS SET @field=@value WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.Parameters.Add(new SqlParameter("field", field));
                    cmd.Parameters.Add(new SqlParameter("value", value));
                    rows =  cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
            return rows;
        }

        /// <summary>
        /// Updates TREATMENTS table from changes to the interface.
        /// </summary>
        /// <param name="activityID">Treatment ID</param>
        /// <param name="property">The JSON property to change</param>
        /// <param name="value">The new value</param>
        /// <returns>True if successful</returns>
        public static bool UpdateActivity(string activityID, string property, string value)
        {
            bool isSuccessful = true;
            string budgets = "";
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string update = null;
                switch (property)
                {
                    case "IsSelected":
                        update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET OMS_IS_SELECTED=@value WHERE TREATMENTID=@activityID";
                        break;
                    case "IsRepeatActivity":
                        update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET OMS_IS_REPEAT=@value WHERE TREATMENTID=@activityID";
                        break;
                    case "StartYear":
                        update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET OMS_REPEAT_START=@value WHERE TREATMENTID=@activityID";
                        break;
                    case "RepeatYear":
                        update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET OMS_REPEAT_INTERVAL=@value WHERE TREATMENTID=@activityID";
                        break;
                    case "IsExclusive":
                        update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET OMS_IS_EXCLUSIVE=@value WHERE TREATMENTID=@activityID";
                        break;
                    case "Criteria":
                        if(value == "null")
                        {
                            update = "UPDATE " + DB.TablePrefix + "FEASIBILITY SET CRITERIA=NULL WHERE TREATMENTID=@activityID";
                        }
                        else
                        { 
                            update = "UPDATE " + DB.TablePrefix + "FEASIBILITY SET CRITERIA=@value WHERE TREATMENTID=@activityID";
                        }
                        break;
                    case "YearSame":
                        update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET BEFORESAME=@value WHERE TREATMENTID=@activityID";
                        break;
                    case "Cost":
                        value = ConvertDisplayCostToOMS(activityID, value);
                        update = "UPDATE " + DB.TablePrefix + "COSTS SET COST_=@value WHERE TREATMENTID=@activityID";
                        break;

                    case "DeleteBudget":
                        List<string> removeBudgets = SelectScenario.GetBudgetsForActivity(activityID);
                        removeBudgets.Remove(value);
                        foreach (string budget in removeBudgets)
                        {
                            if (budgets.Length != 0) budgets += "|";
                            budgets += budget;
                        }
                        value = budgets;
                        update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET BUDGET=@value WHERE TREATMENTID=@activityID";
                        break;
                    case "AddBudget":
                        List<string> addBudgets = SelectScenario.GetBudgetsForActivity(activityID);
                        if (!addBudgets.Contains(value))
                        {
                            addBudgets.Add(value);
                            foreach (string budget in addBudgets)
                            {
                                if (budgets.Length != 0) budgets += "|";
                                budgets += budget;
                            }
                            value = budgets;
                            update = "UPDATE " + DB.TablePrefix + "TREATMENTS SET BUDGET=@value WHERE TREATMENTID=@activityID";
                        }
                        break;
                }

                if (update != null)
                {
                    try
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(update, connection);
                        cmd.Parameters.Add(new SqlParameter("activityID", activityID));
                        cmd.Parameters.Add(new SqlParameter("value", value));
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                        isSuccessful = false;
                    }
                }
            }
            return isSuccessful;
        }

        public static string ConvertDisplayCostToOMS(string activityID, string criteria)
        {
            string simulationID = GetSimulationIDFromActivityID(activityID);
            string assetName = SelectScenario.GetAssetType(simulationID);
            string displayAttribute = SimulationComponents.FindAttribute(criteria, 0);
            if(displayAttribute != null)
            {
                List<AttributeStore> attributes = OMS.GetAssetAttributes(assetName);
                AttributeStore attribute = attributes.Find(delegate(AttributeStore a) { return a.OmsHierarchy == displayAttribute; });
                criteria = criteria.Replace("[" + displayAttribute + "]", "[" + attribute.OmsObjectUserIDHierarchy + "]");
            }
            return criteria;
        }




        public static bool UpdateActivityConditionIndex(string activityID, string conditionIndex, string property, string value)
        {
            string simulationID = GetSimulationIDFromActivityID(activityID);
            string assetType = SelectScenario.GetAssetType(simulationID);
            bool isSuccessful = true;

            if (property == "Impact")
            {
                UpdateImpact(assetType, activityID, conditionIndex, value);
            }
            //WE NEED TO HAVE INTERFACE RETURN ENTIRE CRITERIA.
            //else if (property == "MinimumIndex")  
            //{
            //    string existingCriteria = SelectScenario.GetActivityFeasibility(activityID);
            //}
            //else if (property == "MaximumIndex")
            //{

            //}
            return isSuccessful;
        }

        private static void UpdateImpact(string assetType, string activityID, string conditionIndex, string value)
        {
            OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(assetType);
            OMSConditionIndexStore omsConditionIndex = oci.ConditionIndexes.Find(delegate(OMSConditionIndexStore ci) { return ci.ConditionCategory == conditionIndex; });
            if (omsConditionIndex != null)
            {
                string attribute = omsConditionIndex.AttributeDE;
                if (string.IsNullOrWhiteSpace(value))//The user has deleted an impact.
                {
                    DeleteScenario.DeleteImpact(activityID, attribute);
                }
                else
                {
                    bool isExists = false;
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "SELECT CONSEQUENCEID FROM " + DB.TablePrefix + "CONSEQUENCES WHERE TREATMENTID='" + activityID + "' AND ATTRIBUTE_='" + attribute + "'";
                            SqlCommand cmd = new SqlCommand(query, connection);
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.Read())
                            {
                                isExists = true;
                            }
                        }
                        catch (Exception e)
                        {
                            DataAccessExceptionHandler.HandleException(e, false);
                            return;
                        }
                    }

                    if (isExists)
                    {
                        //Update impact
                        UpdateImpact(activityID, attribute, value);
                    }
                    else //Insert impact
                    {
                        InsertImpact(activityID, attribute, value);
                    }
                }
            }
        }

        private static void UpdateImpact(string activityID, string attribute, string value)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string update = null;
                update = "UPDATE " + DB.TablePrefix + "CONSEQUENCES SET CHANGE_=@value WHERE TREATMENTID=@activityID AND ATTRIBUTE_='" + attribute + "'";
                if (update != null)
                {
                    try
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(update, connection);
                        cmd.Parameters.Add(new SqlParameter("activityID", activityID));
                        cmd.Parameters.Add(new SqlParameter("value", value));
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                    }
                }
            }
        }

        public static void InsertImpact(string activityID, string attribute, string value)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_) VALUES (@activityID,'" + attribute + "',@value)";
                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.Parameters.Add(new SqlParameter("activityID", activityID));
                    cmd.Parameters.Add(new SqlParameter("value", value));

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }

        }

        public static string GetSimulationIDFromActivityID(string activityID)
        {
            string simulationID = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT SIMULATIONID FROM " + DB.TablePrefix + "TREATMENTS WHERE TREATMENTID='" + activityID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["SIMULATIONID"] != DBNull.Value) simulationID = dr["SIMULATIONID"].ToString();
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    simulationID = null;
                }
            }
            return simulationID;
        }

        public static void UpdateEditScenario(string simulationID, string property, string value)
        {
            if(property == "BudgetCategories")
            {
 //               UpdateBudgetCategories(simulationID, value);
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    string update = null;
                    switch (property)
                    {
                        case "Asset":
                            update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET ASSET_TYPE=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "SimulationName":
                            update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET SIMULATION=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "ScopeName":
                            update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET COMMENTS=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "ScopeFilter":
                            value = value.Replace("_cgDEDate", "#");
                            if (value == "null") 
                                update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET JURISDICTION=NULL WHERE SIMULATIONID=@simulationID";
                            else
                                update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET JURISDICTION=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "AnalysisType":
                            update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET ANALYSIS=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "NumberYears":
                            update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET NUMBERYEARS=@value WHERE SIMULATIONID=@simulationID";
                            UpdateYearlyTargetNumberYears(int.Parse(value), simulationID);
                            UpdateYearlyInvestmentNumberYears(int.Parse(value), simulationID);
                            break;
                        case "BudgetName":
                            update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET BUDGET_NAME=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "InflationRate":
                            update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET INFLATIONRATE=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "StartDate":
                            UpdateEditScenario(simulationID,"FirstYear",Convert.ToDateTime(value).Year.ToString());
                            update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET SIMULATION_START_DATE=@value WHERE SIMULATIONID=@simulationID";
                            break;
                        case "FirstYear":
                            update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET FIRSTYEAR=@value WHERE SIMULATIONID=@simulationID";
                            UpdateYearlyInvestmentStartYear(int.Parse(value), simulationID);
                            UpdateYearlyTargetStartYear(int.Parse(value), simulationID);
                            break;
                        case "AddBudget":
                            InvestmentStore investment = SelectScenario.GetInvestment(simulationID);
                            string budgetOrder = "";
                            foreach (string budget in investment.Budgets)
                            {
                                if (budgetOrder.Length > 0) budgetOrder += "|";
                                budgetOrder += budget;
                            }
                            if (budgetOrder.Length > 0) budgetOrder += "|";
                            budgetOrder += value;
                            value = budgetOrder;
                            update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET BUDGETORDER=@value WHERE SIMULATIONID=@simulationID";
                            UpdateYearlyInvestmentBudgetOrder(budgetOrder, simulationID);
                            break;

                        case "DeleteBudget":
                            InvestmentStore investmentDelete = SelectScenario.GetInvestment(simulationID);
                            string budgetOrderDelete = "";
                            foreach (string budget in investmentDelete.Budgets)
                            {
                                if (budget != value)
                                {
                                    if (budgetOrderDelete.Length > 0) budgetOrderDelete += "|";
                                    budgetOrderDelete += budget;
                                }
                            }
                            
                            value = budgetOrderDelete;
                            update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET BUDGETORDER=@value WHERE SIMULATIONID=@simulationID";
                            UpdateYearlyInvestmentBudgetOrder(budgetOrderDelete, simulationID);
                            break;

                        case "Optimization":
                            update = "UPDATE " + DB.TablePrefix + "SIMULATIONS SET BUDGET_CONSTRAINT=@value WHERE SIMULATIONID=@simulationID";
                            break;
                    }

                    if (update != null)
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(update, connection);
                            cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                            cmd.Parameters.Add(new SqlParameter("value", value));
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                        }
                    }
                }
            }
            return;
        }

        private static void UpdateYearlyTargetNumberYears(int numberYears, string simulationID)
        {
            List<TargetStore> targets = SelectScenario.GetEditSimulationTargets(simulationID);
            if (numberYears == 0) return;
            int startYear = int.Parse(targets[0].Year);
            if (numberYears > targets.Count)
            {
                int currentLastYear = int.Parse(targets[targets.Count - 1].Year) + 1;
                while (currentLastYear < startYear + numberYears)
                {
                    CreateScenario.InsertSimulationOCITargets(simulationID, currentLastYear.ToString());
                    currentLastYear++;
                }
  
            }
            else if (numberYears < targets.Count)
            {
                for (int i = numberYears; i < targets.Count; i++)
                {
                    if (i >= numberYears)
                    {
                        DeleteScenario.DeleteTarget(targets[i].ID);
                    }
                }
            }
        }

        private static void UpdateYearlyInvestmentNumberYears(int numberYears, string simulationID)
        {
            List<string> years = GetInvestmentYears(simulationID);
            List<string> budgets = GetInvestmentBudgets(simulationID);
            if (numberYears == 0) return;
            if (budgets.Count == 0) return;

            int startYear = int.Parse(years[0]);

            if (numberYears > years.Count) //Add new years.
            {
                int currentLastYear = int.Parse(years[years.Count - 1]) + 1;
                while (currentLastYear < startYear + numberYears)
                {
                    foreach (string budget in budgets)
                    {
                        UpdateYearlyBudget(simulationID, currentLastYear, budget, 0);
                    }
                    currentLastYear++;
                }

            }
            else if(numberYears < years.Count) //
            {
                for (int i = numberYears; i < years.Count; i++)
                {
                    if (i >= numberYears)
                    {
                        DeleteScenario.DeleteYearlyInvestment(simulationID, int.Parse(years[i]));
                    }
               }
            }
        }


        private static void UpdateYearlyTargetStartYear(int startYear, string simulationID)
        {
            List<TargetStore> targets = SelectScenario.GetEditSimulationTargets(simulationID);
            int numberYears = targets.Count;
            if (numberYears == 0) return;
            // If startYear earlier than current, add new years with 0 amounts, delete excess years.
            if (startYear < int.Parse(targets[0].Year))
            {
                for (int i = 0; i < numberYears; i++)
                {
                    int year = startYear + i;
                    TargetStore target = targets.Find(delegate (TargetStore ts) { return ts.Year == year.ToString();});
                    if(target == null)
                    {
                        CreateScenario.InsertSimulationOCITargets(simulationID, year.ToString());
                        
                    }
                }
                for (int index = 0; index < targets.Count; index++)
                {
                    if (int.Parse(targets[index].Year) > startYear + numberYears)
                    {
                        DeleteScenario.DeleteTarget(targets[index].ID);
                    }
                }
            }
            else if (startYear > int.Parse(targets[0].Year)) // If startYear later than current, delete earlier.  Add later.
            {
                for (int index = 0; index < targets.Count; index++)
                {
                    if (int.Parse(targets[index].Year) < startYear)
                    {
                        DeleteScenario.DeleteTarget(targets[index].ID);
                    }
                }

                for (int i = 0; i < numberYears; i++)
                {
                    int year = i + startYear;
                    TargetStore target = targets.Find(delegate(TargetStore ts) { return ts.Year == year.ToString(); });
                    if (target == null)
                    {
                        CreateScenario.InsertSimulationOCITargets(simulationID, year.ToString());
                    }
                }
            }
        }

        private static void UpdateYearlyInvestmentStartYear(int startYear, string simulationID)
        {
            List<string> years = GetInvestmentYears(simulationID);
            List<string> budgets = GetInvestmentBudgets(simulationID);
            int numberYears = years.Count;
            

            if (numberYears == 0) return;
            if (budgets.Count == 0) return;

            // If startYear earlier than current, add new years with 0 amounts, delete excess years.
            if (startYear < int.Parse(years[0]))
            {
                for (int i = 0; i < numberYears; i++)
                {
                    int year = startYear + i;
                    if(!years.Contains(year.ToString()))
                    {
                        foreach (string budget in budgets)
                        {
                            UpdateYearlyBudget(simulationID, startYear + i, budget, 0);
                        }
                    }
                }

                for (int index = 0; index < years.Count; index++)
                {
                    if (int.Parse(years[index]) > startYear + numberYears)
                    {
                        DeleteScenario.DeleteYearlyInvestment(simulationID, int.Parse(years[index]));
                    }
                }
            }
            else if (startYear > int.Parse(years[0])) // If startYear later than current, delete earlier.  Add later.
            {
                for (int index = 0; index < years.Count; index++)
                {
                    if (int.Parse(years[index]) < startYear)
                    {
                        DeleteScenario.DeleteYearlyInvestment(simulationID, int.Parse(years[index]));
                    }
                }
                for (int i = 0; i < numberYears; i++)
                {
                    int year = i + startYear;
                    if(!years.Contains(year.ToString()))
                    {
                        foreach (string budget in budgets)
                        {
                            UpdateYearlyBudget(simulationID, year, budget, 0);
                        }
                    }
                }
            }
        }




        //Update on change of budget order.
        private static void UpdateYearlyInvestmentBudgetOrder(string budgetOrder, string simulationID)
        {
            if (budgetOrder != null)
            {
                string[] values = budgetOrder.Split('|');
                if(values.Length > 0)
                {
                    //Remove budgets in YearlyInvestment not in new BudgetOrder
                    string delete = "DELETE FROM " + DB.TablePrefix + "YEARLYINVESTMENT WHERE SIMULATIONID=@simulationID AND NOT(";
                    foreach (string value in values)
                    {
                        if (delete.Contains("BUDGETNAME")) delete += " OR "; 
                        delete += " BUDGETNAME='" + value + "' ";
                    }
                    delete += ")";
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(delete, connection);
                        cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                        cmd.ExecuteNonQuery();
                    }

                    List<string> listYears = GetInvestmentYears(simulationID);
                    List<string> listBudgets = GetInvestmentBudgets(simulationID);

                    foreach (string value in values)//From budget order
                    {
                        if (listBudgets.Contains(value)) continue;
                        foreach (string year in listYears)
                        {
                            UpdateYearlyBudget(simulationID, int.Parse(year), value, 0);
                        }
                    }
                }
            }
        }

        private static List<string> GetInvestmentYears(string simulationID)
        {
            List<string> listYears = new List<string>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string select = "SELECT DISTINCT YEAR_ FROM cgDE_YEARLYINVESTMENT WHERE SIMULATIONID=@simulationID ORDER BY YEAR_";
                connection.Open();
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                SqlDataReader dr = cmd.ExecuteReader();
                listYears = new List<string>();
                while (dr.Read())
                {
                    listYears.Add(dr["YEAR_"].ToString());
                }
            }
            return listYears;
        }


        private static List<string> GetInvestmentBudgets(string simulationID)
        {
            List<string> listBudgets = new List<string>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string select = "SELECT DISTINCT BUDGETNAME FROM cgDE_YEARLYINVESTMENT WHERE SIMULATIONID=@simulationID";
                connection.Open();
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    listBudgets.Add(dr["BUDGETNAME"].ToString());
                }
            }
            return listBudgets;
        }


        public static void UpdateYearlyBudget(string simulationID, int year, string budget, double amount)
        {
            string yearID = GetYearID(simulationID, year, budget);
            if (yearID == null) //Insert
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    string insert = "INSERT INTO " + DB.TablePrefix + "YEARLYINVESTMENT " +
                      "(SIMULATIONID, YEAR_,BUDGETNAME,AMOUNT)" +
                        "VALUES(@simulationID,'" + year.ToString() + "',@budget,'" + amount.ToString() + "')";
                    if (insert != null)
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(insert, connection);
                            cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                            cmd.Parameters.Add(new SqlParameter("budget", budget));
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                        }
                    }
                }
            }
            else//Updates
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    string update = "UPDATE " + DB.TablePrefix + "YEARLYINVESTMENT SET AMOUNT='" + amount.ToString() + "' WHERE YEARID='" + yearID + "'";
                    if (update != null)
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(update, connection);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                        }
                    }
                }
            }
        }


        private static string GetYearID(string simulationID, int year, string budgetName)
        {
            string yearID = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT YEARID FROM " + DB.TablePrefix + "YEARLYINVESTMENT WHERE SIMULATIONID='" + simulationID + "' AND YEAR_='" + year.ToString() + "' AND BUDGETNAME='" + budgetName + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["YEARID"] != DBNull.Value) yearID = dr["YEARID"].ToString();
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    yearID = null;
                }
            }
            return yearID;
        }



        public static bool UpdateTarget(string targetID, string property, string value)
        {
            bool isSuccess = true;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string update = null;
                switch (property)
                {
                    case "TargetMean":
                        update = "UPDATE " + DB.TablePrefix + "TARGETS SET TARGETMEAN=@value WHERE ID_=@targetID";
                        break;
                    case "Name":
                        update = "UPDATE " + DB.TablePrefix + "TARGETS SET TARGETNAME=@value WHERE ID_=@targetID";
                        break;
                    case "Criteria":
                        if (value == "null")
                        {
                            update = "UPDATE " + DB.TablePrefix + "TARGETS SET CRITERIA=NULL WHERE ID_=@targetID";
                        }
                        else
                        {
                            update = "UPDATE " + DB.TablePrefix + "TARGETS SET CRITERIA=@value WHERE ID_=@targetID";
                        }
                        break;
                }

                if (update != null)
                {
                    try
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(update, connection);
                        cmd.Parameters.Add(new SqlParameter("targetID", targetID));
                        cmd.Parameters.Add(new SqlParameter("value", value));
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                        isSuccess = false;
                    }
                }
            }
            return isSuccess;
        }


        public static void UpdateBudgetCategory(string simulationID, string oldBudget, string newBudget)
        {
            InvestmentStore investment = SelectScenario.GetInvestment(simulationID);
            string budgetOrder = "";
            foreach (string budget in investment.Budgets)
            {
                if (budgetOrder.Length > 0) budgetOrder += "|";
                if (budget == oldBudget)
                {
                    budgetOrder += newBudget;
                }
                else
                {
                    budgetOrder += budget;
                }
            }
            // Update budget order
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string update = "UPDATE " + DB.TablePrefix + "INVESTMENTS SET BUDGETORDER=@budgetOrder WHERE SIMULATIONID=@simulationID";
                connection.Open();
                SqlCommand cmd = new SqlCommand(update, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                cmd.Parameters.Add(new SqlParameter("budgetOrder", budgetOrder));
                cmd.ExecuteNonQuery();
            }


            //Updating yearly budget.
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                string update = "UPDATE " + DB.TablePrefix + "YEARLYINVESTMENT SET BUDGETNAME=@newBudget WHERE SIMULATIONID=@simulationID AND BUDGETNAME=@oldBudget";
                connection.Open();
                SqlCommand cmd = new SqlCommand(update, connection);
                cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                cmd.Parameters.Add(new SqlParameter("newBudget", newBudget));
                cmd.Parameters.Add(new SqlParameter("oldBudget", oldBudget));
                cmd.ExecuteNonQuery();
            }


            //Need to change all committed projects.

        }
    }
}
