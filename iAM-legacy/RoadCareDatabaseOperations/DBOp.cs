namespace RoadCareDatabaseOperations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using DatabaseManager;
    using DataObjects;
    using Simulation;

    public static class DBOp
    {
        private static readonly TimeSpan LockTimeout = new TimeSpan( 1, 0, 0 );

        private static String m_provider = "MSSQL";

        public static String Provider
        {
            set { m_provider = value; }
        }

        #region Generic SQL Statements
        /// <summary>
        /// Calls the DBMgr ExecuteNonQuery operation to execute an INSERT operation on the database.
        /// </summary>
        /// <param name="strTableName">Name of the table to insert the data.</param>
        /// <param name="dictFieldsToValues">A hash of insert fields and value pairs</param>
        /// <returns>The number of rows inserted.</returns>
        public static int Insert(String strTableName, Dictionary<String, String> dictFieldsToValues)
        {
            int iRowsInserted = 0;
            String strInsert = "INSERT INTO " + strTableName;
            String strFieldList = "(";
            String strValueList = " VALUES(";
            foreach (String key in dictFieldsToValues.Keys)
            {
                if (!String.IsNullOrEmpty(dictFieldsToValues[key]))
                {
                    String strValue = dictFieldsToValues[key].ToString();
                    strFieldList += key + ",";
                    strValueList += "'" + strValue + "',";
                }
            }
            if (strFieldList.Contains(','))
            {
                strFieldList = strFieldList.Substring(0, strFieldList.Length - 1) + ")";
                strValueList = strValueList.Substring(0, strValueList.Length - 1) + ")";
                strInsert += strFieldList + strValueList;
                try
                {
                    iRowsInserted = DBMgr.ExecuteNonQuery(strInsert);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            return iRowsInserted;
        }

        /// <summary>
        /// Creates an insert string for use in a transaction to be performed by the DBMgr
        /// </summary>
        /// <param name="strTableName">The table to perform the insert on.</param>
        /// <param name="dictFieldsToValues">The hash of field to values pairs</param>
        /// <returns>A string to be used in a DBMgr transaction command.</returns>
        public static String TransactionInsert(String strTableName, Dictionary<String, String> dictFieldsToValues)
        {
            String strInsert = "INSERT INTO " + strTableName;
            String strFieldList = "(";
            String strValueList = " VALUES(";
            foreach (String key in dictFieldsToValues.Keys)
            {
                if (dictFieldsToValues[key] != null)
                {
                    String strValue = dictFieldsToValues[key].ToString();
                    strFieldList += key + ",";
                    strValueList += strValue + ",";
                }
            }
            if (strFieldList.Contains(','))
            {
                strFieldList = strFieldList.Substring(0, strFieldList.Length - 1) + ")";
                strValueList = strValueList.Substring(0, strValueList.Length - 1) + ")";
                strInsert += strFieldList + strValueList;
            }
            else
            {
                strInsert = "";
            }
            return strInsert;
        }
        #endregion

        #region Asset SQL Statements
        /// <summary>
        /// Given an asset table name, and a dictionary hash of field keys to value pairs, updates the asset table with the values in the dictionary.
        /// Calls InsertIntoAssetChangeLog to perform the necessary ChangeLog entry.
        /// </summary>
        /// <param name="fieldName">Name of the column type being modified</param>
        /// <param name="oldValue">The value before the user entered value in the field.</param>
        /// <param name="strAssetName">The name of the asset being modified</param>
        /// <param name="strGeoID">The ID of the asset being modified</param>
        /// <param name="value">The new user entered value.</param>
        public static void UpdateAssetTable(String strAssetName, String strGeoID, String fieldName, String value, String oldValue, String strWorkActivity)
        {
            String strUpdate = "UPDATE " + strAssetName + " SET " + fieldName + " = " + "'" + value + "' WHERE GEO_ID = " + strGeoID;
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
                InsertIntoAssetChangeLog(strAssetName, strGeoID, fieldName, oldValue, null, null, strWorkActivity);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Inserts a new row into the asset_CHANGELOG table.
        /// </summary>
        /// <param name="strAssetName">Name of the table_CHANGELOG to insert into</param>
        /// <param name="strGeoID">The ID of the asset</param>
        /// <param name="strAssetAttribute">The field name of the asset being modified</param>
        /// <param name="strAssetValue">The value of the field being modified</param>
        private static void InsertIntoAssetChangeLog(String strAssetName, String strGeoID, String strAssetAttribute, String strAssetValue, String strUserID, String strWorkActivityID, String strWorkActivity)
        {
            Dictionary<String, String> dictFieldToValues = new Dictionary<String, String>();
            dictFieldToValues.Add("GEO_ID", strGeoID);
            dictFieldToValues.Add("FIELD", strAssetAttribute);
            dictFieldToValues.Add("VALUE", strAssetValue);
            dictFieldToValues.Add("USER_ID", strUserID);
            dictFieldToValues.Add("WORKACTIVITY", strWorkActivity);
            dictFieldToValues.Add("WORKACTIVITY_ID", strWorkActivityID);
            dictFieldToValues.Add("DATE_MODIFIED", DateTime.Now.ToString());
            try
            {
                DBOp.Insert(strAssetName + "_CHANGELOG", dictFieldToValues);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Runs a select on a selected assets CHANGELOG table.  The select returns all fields from the CHANGELOG table where the assets GEO_ID matches
        /// in the CHANGELOG table
        /// </summary>
        /// <returns>The results of the query.</returns>
        /// 
        public static DataSet QueryAssetHistory(String strAssetName, String strGeoID)
        {
            DataSet dsToReturn = null;
            String strQuery = "SELECT ID_, GEO_ID, FIELD, VALUE, USER_ID, WORKACTIVITY, WORKACTIVITY_ID, DATE_MODIFIED FROM " + strAssetName +
                                          "_CHANGELOG WHERE GEO_ID = '" + strGeoID + "' ORDER BY DATE_MODIFIED DESC";
            try
            {
                dsToReturn = DBMgr.ExecuteQuery(strQuery);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dsToReturn;
        }

        /// Created By: CBB - 8/25/2008
        /// <summary>
        /// Returns a list of asset names from the ASSETS table.
        /// </summary>
        /// <returns></returns>
        public static List<String> GetRawAssetNames()
        {
            List<String> assetNames = new List<String>();
            try
            {
                DataSet ds = DBMgr.ExecuteQuery("SELECT ASSET FROM ASSETS");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    assetNames.Add(dr["ASSET"].ToString());
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return assetNames;
        }

        public static List<String> GetAssetAttributes(String assetName, ConnectionParameters cp)
        {
            List<string> assetColumnNames = DBMgr.GetTableColumns(assetName, cp);
            assetColumnNames.Remove("GEO_ID");
            return assetColumnNames;
        }

        /// <summary>
        /// Gets the asset changelog for the given strGeoID, after the given date.
        /// </summary>
        /// <param name="strAssetName">Asset table name</param>
        /// <param name="strGeoID">Asset ID</param>
        /// <param name="strDate">Earliest asset action date</param>
        /// <returns>DataSet with all asset changes before and including the given date.</returns>
        public static DataSet QueryAssetHistory(String strAssetName, String strGeoID, String strDate)
        {
            DataSet dsToReturn = null;
            String strQuery = "SELECT ID_, GEO_ID, FIELD, VALUE, USER_ID, WORKACTIVITY, WORKACTIVITY_ID, DATE_MODIFIED FROM " + strAssetName +
                                          "_CHANGELOG WHERE GEO_ID = '" + strGeoID + "' AND DATE_MODIFIED > '" + strDate + "' ORDER BY DATE_MODIFIED DESC";
            try
            {
                dsToReturn = DBMgr.ExecuteQuery(strQuery);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dsToReturn;
        }

        public static DataSet QueryAttributeGroupNames()
        {
            DataSet dsReturn = null;
            String strSelect;

            strSelect = "SELECT DISTINCT GROUPING FROM ATTRIBUTES_ ORDER BY 1";			
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryAttributeByGroup()
        {
            DataSet dsReturn = null;
            String strSelect;

            strSelect = "SELECT GROUPING, ATTRIBUTE_ FROM ATTRIBUTES_ ORDER BY GROUPING, ATTRIBUTE_";
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryReport(string strNetworkID, string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years FROM Report_" + strNetworkID
                    + "_" + strSimulationID + " Rpt INNER JOIN Section_" + strNetworkID + " Sec ON Rpt.SectionID = Sec.SectionID "
                    + "ORDER BY Facility, Section, Years";
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryPageHeader(string strReportName)
        {
            DataSet dsReturn = null;
            
            string reportID = DBMgr.ExecuteQuery("SELECT reportID FROM REPORTS WHERE reportName = '" + strReportName + "'").Tables[0].Rows[0].ItemArray[0].ToString();
            string strSelect = "SELECT * FROM PageHeaders, Reports WHERE reportName='" + strReportName + "' AND phReportID = " + reportID + " ORDER BY phLineNum";
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryDeficients(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM Deficients WHERE Simulationid = " + strSimulationID;

            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryTargets(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM Targets WHERE Simulationid = " + strSimulationID;

            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryPriorityFund(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT Budget, Funding, pf.Priorityid FROM PRIORITYFUND pf JOIN PRIORITY p on p.Priorityid = pf.priorityid WHERE Simulationid = " + strSimulationID;

            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryConsequences(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT Attribute_, Change_,Criteria,Treatmentid FROM Consequences WHERE Treatmentid IN (SELECT Treatmentid FROM Treatments WHERE Simulationid =" + strSimulationID + ")";
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryCosts(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT Cost_, Unit,Criteria,Treatmentid FROM Costs WHERE Treatmentid IN (SELECT Treatmentid FROM Treatments WHERE Simulationid =" + strSimulationID + ")";
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryFeasibility(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT Criteria,Treatmentid FROM Feasibility WHERE Treatmentid IN (SELECT Treatmentid FROM Treatments WHERE Simulationid =" + strSimulationID + ")";
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryTreatments(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM Treatments WHERE SimulationID=" + strSimulationID;
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryPerformance(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM Performance WHERE SimulationID=" + strSimulationID;
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryBudgetYears(string strSimulationID)
        {
            DataSet dsReturn = null;
            String strSelect;

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    strSelect = "SELECT DISTINCT [Year_] FROM YearlyInvestment WHERE simulationID = " + strSimulationID + " ORDER BY [Year_]";
                    break;
                case "ORACLE":
                    strSelect = "SELECT DISTINCT Year_ FROM YearlyInvestment WHERE simulationID = " + strSimulationID + " ORDER BY Year_";
                    break;
                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for QueryBudgetYears()");
                //break;
            }
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        //added on 12/15/2015 by Aditya 
        //need simulation analysis years and most recent data collection year for MDSHA All Sections Report
        public static DataSet QueryBudgetYearsinclmostrecent(string strSimulationID)
        {
            DataSet dsReturn = null;
            String strSelect;

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    strSelect = "SELECT DISTINCT [Year_] FROM YearlyInvestment WHERE simulationID = " + strSimulationID + " UNION SELECT MIN([Year_])-1 FROM YearlyInvestment WHERE simulationID = " + strSimulationID + " ORDER BY [Year_]";
                    break;
                case "ORACLE":
                    strSelect = "SELECT DISTINCT Year_ FROM YearlyInvestment WHERE simulationID = " + strSimulationID + " UNION SELECT MIN(Year_)-1 FROM YearlyInvestment WHERE simulationID = " + strSimulationID + " ORDER BY Year_";
                    break;
                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for QueryBudgetYears()");
                //break;
            }
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryYearlyInvestment(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM YearlyInvestment WHERE SIMULATIONID=" + strSimulationID;
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryInvestments(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM INVESTMENTS WHERE SIMULATIONID=" + strSimulationID;
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QueryPriority(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM Priority WHERE simulationID = " + strSimulationID;
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

        public static DataSet QuerySimulations(string strSimulationID)
        {
            DataSet dsReturn = null;
            string strSelect = "SELECT * FROM Simulations WHERE simulationID = " + strSimulationID;
            try
            {
                dsReturn = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsReturn;
        }

	    public static DataSet QuerySimulationResult(string strNetworkID, string strSimulationID)
	    {
	        DataSet dsReturn = null;
	        string strSelect = "SELECT * FROM SIMULATION_" + strNetworkID + "_" + strSimulationID;
	        try
	        {
	            dsReturn = DBMgr.ExecuteQuery(strSelect);
	        }
	        catch (Exception ex)
	        {
	            throw ex;
	        }
	        return dsReturn;
	    }

        public static DataSet GetNetworkDesc(String networkID)
        {
            try
            {
                String query = "SELECT NETWORK_NAME, DESCRIPTION FROM NETWORKS WHERE NETWORKID = '" + networkID + "'";
                DataSet ds = DBMgr.ExecuteQuery(query);
                String networkName = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                return ds;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public static String GetNetworkName(String networkID)
        {
            try
            {
                String query = "SELECT NETWORK_NAME FROM NETWORKS WHERE NETWORKID = '" + networkID + "'";
                DataSet ds = DBMgr.ExecuteQuery(query);
                String networkName = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                return networkName;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public static bool IsAtEndOfNetwork(String networkID, String sectionID)
        {
            bool bEndOfNetwork = false;
            try
            {
                float fMaxEndStation;
                String query = "SELECT FACILITY, END_STATION FROM SECTION_" + networkID + " WHERE SECTIONID = '" + sectionID + "'";
                DataSet currentSectionSet = DBMgr.ExecuteQuery(query);
                String facilityName = currentSectionSet.Tables[0].Rows[0]["FACILITY"].ToString();
                //if (currentSectionSet.Tables[0].Rows[0]["END_STATION"] == -1)
                //{
                //    //this is a test
                //}
                float fEndStation = float.Parse(currentSectionSet.Tables[0].Rows[0]["END_STATION"].ToString());
                
                query = "SELECT MAX(END_STATION) FROM SECTION_" + networkID + " WHERE FACILITY = '" + facilityName + "'";
                try
                {
                    DataSet maxEndStation = DBMgr.ExecuteQuery(query);
                    fMaxEndStation = float.Parse(maxEndStation.Tables[0].Rows[0].ItemArray[0].ToString());
                    bEndOfNetwork = (fMaxEndStation == fEndStation);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return bEndOfNetwork;
        }

        // FIXME: Will this return false when the table exists and is empty?
        // See http://stackoverflow.com/q/167576/402749 and similar.
        public static bool IsTableInDatabase(String tableName)
        {
            try
            {
                DBMgr.ExecuteQuery("SELECT TOP(1) * FROM " + tableName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<String> GetAttributeNames()
        {
            String strSelect = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ ORDER BY ATTRIBUTE_";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            List<String> attributeNames = new List<String>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                attributeNames.Add(row["ATTRIBUTE_"].ToString());
            }
            return attributeNames;
        }

        #endregion

        /// <summary>
        /// Returns the connection parameters with a given name ID if it exists.  If it does not exist, then we return null
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        //public static ConnectionParameters GetConnectionParameters(string connName)
        //{
        //    string query = "SELECT CONNECTION_ID, CONNECTION_NAME, SERVER, PROVIDER, NATIVE_, DATABASE_NAME, SERVICE_NAME, SID, PORT, USERID, PASSWORD, INTEGRATED_SEC, VIEW_STATEMENT, IDENTIFIER FROM CONNECTION_PARAMETERS WHERE CONNECTION_NAME = '" + connName + "'";
        //    DataSet ds = DBMgr.ExecuteQuery(query);
        //    if(ds.Tables[0].Rows.Count != 0)
        //    {
        //        DataRow dr = ds.Tables[0].Rows[0];
        //        string connID = dr["CONNECTION_ID"].ToString();
        //        string server = dr["SERVER"].ToString();
        //        string provider = dr["PROVIDER"].ToString();
        //        string databaseName = dr["DATABASE_NAME"].ToString();
        //        string serviceName = dr["SERVICE_NAME"].ToString();
        //        string sid = dr["SID"].ToString();
        //        string port = dr["PORT"].ToString();
        //        string userid = dr["USERID"].ToString();
        //        string password = dr["PASSWORD"].ToString();
        //        bool integratedSec = Convert.ToBoolean(dr["INTEGRATED_SEC"]);
        //        string viewStatement = dr["VIEW_STATEMENT"].ToString();
        //        string identifier = dr["IDENTIFIER"].ToString();
        //        bool native = Convert.ToBoolean(dr["NATIVE_"]);

        //        return (new ConnectionParameters(port, sid, serviceName, userid, password, integratedSec, server, databaseName, connName, viewStatement, identifier, connID, provider, native));
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// Returns a list of unique values in a String Attribute.  Primarily used for Jurisdiction (i.e. County, District, Maintenance Yard, etc.)
        /// </summary>
        /// <param name="strNetworkID">NetworkID to summarize</param>
        /// <param name="strAttribute">Attribute field.  String field only</param>
        /// <returns>List of unique values in Attribute</returns>
        public static List<String> GetJurisdiction(String strNetworkID, String strAttribute)
        {
            List<String> listJurisdiction = new List<String>();
            String strSegment = "SEGMENT_" + strNetworkID + "_NS0";

            String strSelect = "SELECT DISTINCT " + strAttribute + " FROM " + strSegment;

            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strValue = row[strAttribute].ToString();
                    listJurisdiction.Add(strValue);
                }
            }
            catch //(Exception exception)
            {
                
            }
            return listJurisdiction;
        }

        /// <summary>
        /// Returns a list of unique values of Treatment in a given simulation.
        /// </summary>
        /// <param name="strNetworkID">NetworkID to summarize</param>
        /// <param name="strAttribute">Attribute field.  String field only</param>
        /// <returns>List of unique values in Attribute</returns>
        public static List<String> GetTreatments(String strNetworkID, String strSimulationID)
        {
            List<String> listTreatment = new List<String>();
            String strReport = "REPORT_" + strNetworkID + "_" + strSimulationID;
            String strSelect = "SELECT DISTINCT TREATMENT FROM " + strReport;
            
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strValue = row["TREATMENT"].ToString();
                    listTreatment.Add(strValue);
                }
            }
            catch //(Exception exception)
            {

            }
            return listTreatment;
        }

        /// <summary>
        /// Retrieves requested summary of data for a given attribute, sorted by jurisdiction and filtered.
        /// </summary>
        /// <param name="strNetworkID">NetworkID to summarize</param>
        /// <param name="SimulationID">SimulationID to summarize.  Enter empty string to summarize Network only (no simulation)</param>
        /// <param name="strMethod">Method for summary Sum,Average, Minimum or Maximum (SUM,AVG,MIN,MAX allowed entries)</param>
        /// <param name="strYear">Year for attribute summary. Required</param>
        /// <param name="strAttribute">Attribute to summarize. Required.</param>
        /// <param name="strJurisdiction">Jurisidictional attribute - must be string field. Empty field does not break out summary by juridiction.</param>
        /// <param name="strCriteria">Criteria to filter possible values (from SEGMENTED_ table). Empty field applies no filter criteria.</param></param>
        /// <returns></returns>
        public static Hashtable GetAttributeSummary(String strNetwork, String SimulationID, String strMethod, String strYear, String strAttribute, String strJurisdiction, String strCriteria)
        {
            Hashtable hashJurisdictionSummary = new Hashtable();


            return hashJurisdictionSummary;
        }

        /// <summary>
        /// Retrieves the cost total for a given budget, per Jurisdiction,  for a given year, filtered.
        /// </summary>
        /// <param name="strNetworkID">NetworkID to summarize</param>
        /// <param name="strSimulationID">SimulationID to summarize</param>
        /// <param name="strBudget">Budget to calculate total for.  Empty string provides summary of all budgets</param>
        /// <param name="strYear">Year for which budget totals are desired.  Empty string provides summary for all years</param>
        /// <param name="strJurisdiction">Jurisidictional attribute - must be string field. Empty field does not break out solution by juridiction.</param>
        /// <param name="strCriteria">Criteria to filter possible values (from SEGMENTED_ table). Empty field applies no filter criteria.</param>
        /// <returns>A hashtable of jurisdiction (District 1, 2, 3) and total budget. If strAttribute(jurisdiction) blank key is "All"</returns>
        public static Hashtable GetBudgetTotals(String strNetworkID, String strSimulationID, String strYear, String strBudget, String strJurisdiction, String strCriteria)
        {
            Hashtable hashJurisdictionBudget = new Hashtable();
            if (strNetworkID == "") return hashJurisdictionBudget;
            if (strSimulationID == "") return hashJurisdictionBudget;

            String strSegment = "SEGMENT_" + strNetworkID + "_NS0";
            String strReport = "REPORT_" + strNetworkID + "_" + strSimulationID;

            String strWhere = "";
            String strYearWhere = "";
            String strBudgetWhere = "";
            String strSelect = "";
            if (strYear != "") strYearWhere = strReport + ".YEARS='" + strYear + "'";
            if (strBudget != "") strBudgetWhere = strReport + ".BUDGET='" + strBudget + "'";

            if (strJurisdiction == "")
            {
                //strSelect = "SELECT SUM(" + strReport + ".COST) FROM " + strReport + " INNER JOIN " + strSegment + " ON " + strReport + ".SECTIONID=" + strSegment + ".SECTIONID ";
                strSelect = "SELECT SUM(" + strReport + ".COST_) " + BuildFromStatementWithReport(strNetworkID, strSimulationID);

                if (strYearWhere != "" || strBudgetWhere != "" || strCriteria != "")
                {
                    strWhere = "WHERE ";
                    strWhere += strBudgetWhere;

                    if (strWhere != "WHERE " && strYearWhere != "")
                    {
                        strWhere += " AND " + strYearWhere;
                    }
                    else if (strWhere == "WHERE " && strYearWhere != "")
                    {

                        strWhere += strYearWhere;
                    }


                    if (strWhere != "WHERE " && strCriteria != "")
                    {
                        strWhere += " AND " + strCriteria;
                    }
                    else if (strWhere == "WHERE " && strCriteria != "")
                    {

                        strWhere += strCriteria;
                    }


                    strSelect += strWhere;
                }

                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    if (ds.Tables[0].Rows[0].ItemArray[0] != null)
                    {
                        String strCost = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        if (strCost == "") strCost = "0";
                        hashJurisdictionBudget.Add("All", strCost);
                    }
                    else
                    {
                        hashJurisdictionBudget.Add("All", "0");
                    }
                }
                catch //(Exception exception)
                {
                    
                }
            }
            else//Break into jurisdictions
            {
                List<String> listJuridiction = GetJurisdiction(strNetworkID, strJurisdiction);

                foreach (String strValue in listJuridiction)
                {
                    strSelect = "SELECT SUM(" + strReport + ".COST_) FROM " + strReport + " INNER JOIN " + strSegment + " ON " + strReport + ".SECTIONID=" + strSegment + ".SECTIONID ";

                    strWhere = "WHERE " + strSegment + "." + strJurisdiction + "='" + strValue + "' ";


                    if (strBudgetWhere != "")
                    {
                        strWhere += " AND " + strBudgetWhere;
                    }
                    if (strYearWhere != "")
                    {
                        strWhere += " AND " + strYearWhere;
                    }

                    if (strCriteria != "")
                    {
                        strWhere += " AND " + strCriteria;
                    }

                    strSelect += strWhere;


                    try
                    {
                        DataSet ds = DBMgr.ExecuteQuery(strSelect);
                        if (ds.Tables[0].Rows[0].ItemArray[0] != null)
                        {
                            String strCost = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                            if (strCost == "") strCost = "0";
                            hashJurisdictionBudget.Add(strValue, strCost);
                        }
                        else
                        {
                            hashJurisdictionBudget.Add(strValue, "0");
                        }
                    }
                    catch //(Exception exception)
                    {
                        hashJurisdictionBudget.Add(strValue, "0");
                    }
                }
            }

            return hashJurisdictionBudget;

        }

        /// <summary>
        /// Retrieves the cost total for a given treatment, per Jurisdiction,  for a given year, filtered.
        /// </summary>
        /// <param name="strNetworkID">NetworkID to summarize</param>
        /// <param name="strSimulationID">SimulationID to summarize</param>
        /// <param name="strYear">Year for which budget totals are desired.  Empty string provides summary for all years</param>
        /// <param name="strCriteria">Criteria to filter possible values (from SEGMENTED_ table). Empty field applies no filter criteria.</param>
        /// <returns>A hashtable of jurisdiction (District 1, 2, 3) and total budget. If strAttribute(jurisdiction) blank key is "All"</returns>
        public static Hashtable GetBudgetPerTreatment(String strNetworkID, String strSimulationID, String strYear, String strCriteria)
        {
            Hashtable hashTreatmentBudget = new Hashtable();
            if (strNetworkID == "") return hashTreatmentBudget;
            if (strSimulationID == "") return hashTreatmentBudget;

            String strSegment = "SEGMENT_" + strNetworkID + "_NS0";
            String strReport = "REPORT_" + strNetworkID + "_" + strSimulationID;

            String strWhere = "";
            String strYearWhere = "";
            String strSelect = "";
            if (strYear != "") strYearWhere = strReport + ".YEARS='" + strYear + "'";


            List<String> listTreatment = GetTreatments(strNetworkID, strSimulationID);

            foreach (String strValue in listTreatment)
            {
                strSelect = "SELECT SUM(" + strReport + ".COST_) FROM " + strReport;
                strWhere = " WHERE TREATMENT ='" + strValue + "' ";

                if (strYearWhere != "")
                {
                    strWhere += " AND " + strYearWhere;
                }

                if (strCriteria != "")
                {
                    strWhere += " AND " + strCriteria;
                }

                strSelect += strWhere;


                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    if (ds.Tables[0].Rows[0].ItemArray[0] != null)
                    {
                        String strCost = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        if (strCost == "") strCost = "0";
                        hashTreatmentBudget.Add(strValue, strCost);
                    }
                    else
                    {
                        hashTreatmentBudget.Add(strValue, "0");
                    }
                }
                catch// (Exception exception)
                {
                    hashTreatmentBudget.Add(strValue, "0");
                }
            }

            return hashTreatmentBudget;

        }

        /// <summary>
        /// Calculates a summary value for the input attribute (i.e. rutting)- For NUMBER attributes
        /// </summary>
        /// <param name="strNetworkID">NetworkID of variable to summarize</param>
        /// <param name="strSimulationID">SimulationID of variable to summarize.  Empty string uses network SEGMENT_ table</param>
        /// <param name="strAttribute">Attribute to summarize</param>
        /// <param name="strYear">Year to summarize.  Empty if most recent</param>
        /// <param name="strMethod">AVG (average) MIN (minimum) MAX (maximum) SUM (summation)</param>
        /// <param name="strCriteria">Filter criteria.</param>
        /// <returns></returns>
        public static String GetConditionSummary(String strNetworkID, String strSimulationID, String strAttribute,String strYear, String strMethod, String strCriteria) 
        {
            String strSummary = "";
            String strSegment = "SEGMENT_" + strNetworkID + "_NS0";
            String strSection = "SECTION_" + strNetworkID;
            String strSimulation = "SIMULATION_" + strNetworkID + "_" + strSimulationID;
            String strReport = "REPORT_" + strNetworkID + "_" + strSimulationID;
            String strFrom = BuildFromStatementWithReport(strNetworkID, strSimulationID);

            List<string> columnNames = columnNames = DBMgr.GetTableColumns( strSegment );




            String strTable = "";
            if (strSimulationID != "")
            {
                strTable = strSimulation;
            }
            else
            {
                strTable = strSegment;
            }
            if(strYear != "")
            {
                strYear = "_" + strYear;
            }

            if(columnNames.Contains(strAttribute + strYear))
            {
                strTable = strSegment;
            }



            String strSelect;
            if(strMethod != "AVG")
            {
                    strSelect = "SELECT " + strMethod + "(" + strTable + "." + strAttribute + strYear + ")" + strFrom + " WHERE " + strAttribute + strYear + " IS NOT NULL ";
            }
            else
            {
                    strSelect = "SELECT SUM((" + strTable + "." + strAttribute + strYear + ")*" + strSection+ ".AREA)/SUM(" + strSection + ".AREA)" + strFrom + " WHERE " + strAttribute + strYear + " IS NOT NULL ";
            }

            String strWhere = "";
            if (strCriteria != "")
            {
                strWhere = "AND " + strCriteria;
                strSelect += strWhere;
            }

            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strSummary = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                }
            }
            catch
            {
                strSummary = "NaN";
            }
            return strSummary;
        }

        public static Hashtable GetPercentagePerStringAttribute(String strNetworkID, String strSimulationID, String strAttribute, String strYear, String strMethod, String strCriteria, bool useLaneMiles, out List<String> listAttributes)
        {
            Hashtable hashAttributePercentage = new Hashtable();
            String strSegment = "SEGMENT_" + strNetworkID + "_NS0";
            String strSection = "SECTION_" + strNetworkID;
            String strSimulation = "SIMULATION_" + strNetworkID + "_" + strSimulationID;
            String strReport = "REPORT_" + strNetworkID + "_" + strSimulationID;
            String strSelect;
            DataSet ds;

            String strFrom = BuildFromStatementWithReport(strNetworkID, strSimulationID);
            listAttributes = new List<String>();

            if (strYear != "")
            {
                strYear = "_" + strYear;
            }

            String strTotalArea = "";

            if (strMethod == "PERCENTAGE")
            {
                //Get the total area for the given criteria
                if (useLaneMiles) strSelect = "SELECT SUM(" + strSegment + ".LANE_MILES) " + strFrom;
                else strSelect = "SELECT SUM(" + strReport + ".AREA) " + strFrom;
                if (strCriteria != "")
                {
                    strSelect += " WHERE " + strCriteria;
                }

                try
                {
                    ds = DBMgr.ExecuteQuery(strSelect);
                    strTotalArea = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                }
                catch
                {
                    return hashAttributePercentage;
                }
            }

            // Retrieve distinct
            strSelect = "SELECT DISTINCT " + strAttribute + strYear + strFrom;
            if (strCriteria != "")
            {
                strSelect += " WHERE " + strCriteria;
            }
            strSelect += " ORDER BY " + strAttribute + strYear;




            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch
            {
                return hashAttributePercentage;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strValue = row[strAttribute + strYear].ToString();
                listAttributes.Add(strValue);
                //Get the total area for the given criteria
                if (useLaneMiles) strSelect = "SELECT SUM(" + strSegment + ".LANE_MILES) " + strFrom + " WHERE " + strAttribute + strYear + "='" + strValue + "' ";
                else strSelect = "SELECT SUM(" + strReport + ".AREA) " + strFrom + " WHERE " + strAttribute + strYear + "='" + strValue + "' ";
                if (strCriteria != "")
                {
                    strSelect += " AND " + strCriteria;
                }

                try
                {
                    DataSet dsAttribute = DBMgr.ExecuteQuery(strSelect);
                    String strArea = dsAttribute.Tables[0].Rows[0].ItemArray[0].ToString();
                    if (strArea == "" && strValue == "")
                    {
                        listAttributes.Remove("");
                        continue;
                    }

                    if (strMethod == "AREA")
                    {
                        hashAttributePercentage.Add(strValue, strArea);
                    }
                    else
                    {
                        double dPercentage = (double.Parse(strArea) / double.Parse(strTotalArea)) * 100;
                        hashAttributePercentage.Add(strValue, dPercentage.ToString());
                    }
                }
                catch
                {
                    hashAttributePercentage.Add(strValue, "0");
                }
            }

            //Get the total area for the given criteria
            if (useLaneMiles) strSelect = "SELECT SUM(" + strSegment + ".LANE_MILES) " + strFrom + " WHERE " + strAttribute + strYear + " IS NULL ";
            else strSelect = "SELECT SUM(" + strReport + ".AREA) " + strFrom + " WHERE " + strAttribute + strYear + " IS NULL ";
            if (strCriteria != "")
            {
                strSelect += " AND " + strCriteria;
            }

            try
            {
                DataSet dsAttribute = DBMgr.ExecuteQuery(strSelect);
                String strArea = dsAttribute.Tables[0].Rows[0].ItemArray[0].ToString();
                if (strArea != "") listAttributes.Add("NULL");

                if (strMethod == "AREA")
                {
                    hashAttributePercentage.Add("NULL", strArea);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(strArea)) strArea = "0";
                    if (String.IsNullOrWhiteSpace(strTotalArea)) strTotalArea = "0";
                    double dPercentage = (double.Parse(strArea) / double.Parse(strTotalArea)) * 100;
                    hashAttributePercentage.Add("NULL", dPercentage.ToString());
                }
            }
            catch
            {
                hashAttributePercentage.Add("NULL", "0");
            }

            return hashAttributePercentage;
        }

        public static Hashtable GetPercentagePerStringAttribute(String strNetworkID, String strSimulationID, String strAttribute, String strYear, String strMethod, String strCriteria, String strFormula, bool useLaneMiles,out List<String> listAttributes)
        {
            Hashtable hashAttributePercentage = new Hashtable();
            String strSegment = "SEGMENT_" + strNetworkID + "_NS0";
            String strSection = "SECTION_" + strNetworkID;
            String strSimulation = "SIMULATION_" + strNetworkID + "_" + strSimulationID;
            string strReport = "REPORT_" + strNetworkID + "_" + strSimulationID;
            String strSelect;
            DataSet ds;

            String strFrom = BuildFromStatementWithReport(strNetworkID, strSimulationID);

            listAttributes = new List<String>();

            if (strYear != "")
            {
                strYear = "_" + strYear;
            }

            String strTotalArea = "";

            if (strMethod == "PERCENTAGE")
            {
                //Get the total area for the given criteria
                if(!useLaneMiles)strSelect = "SELECT SUM(" + strReport + ".AREA) " + strFrom;
                else strSelect = "SELECT SUM(" + strSegment + ".LANE_MILES) "  + strFrom;
                if (strCriteria != "")
                {
                    strSelect += " WHERE " + strCriteria;
                }

                try
                {
                    ds = DBMgr.ExecuteQuery(strSelect);
                    strTotalArea = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                }
                catch
                {
                    return hashAttributePercentage;
                }
            }

            // Retrieve distinct
            strSelect = "SELECT DISTINCT " + strAttribute + strYear + strFrom;
            if (strCriteria != "")
            {
                strSelect += " WHERE " + strCriteria;
            }
            strSelect += " ORDER BY " + strAttribute + strYear;




            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch
            {
                return hashAttributePercentage;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strValue = row[strAttribute + strYear].ToString();

                listAttributes.Add(strValue);
                //Get the total (area * strFormula) for the given criteria.  e.g. Lane_Mile Years
                if(useLaneMiles) strSelect = "SELECT SUM(" + strSegment + ".LANE_MILES * " + strFormula + ") " + strFrom + " WHERE " + strAttribute + strYear + "='" + strValue + "' ";
                else strSelect = "SELECT SUM(" + strReport + ".AREA * " + strFormula + ") " + strFrom + " WHERE " + strAttribute + strYear + "='" + strValue + "' ";
                
                if (strCriteria != "")
                {
                    strSelect += " AND " + strCriteria;
                }

                try
                {
                    DataSet dsAttribute = DBMgr.ExecuteQuery(strSelect);
                    String strArea = dsAttribute.Tables[0].Rows[0].ItemArray[0].ToString();
                    if (strArea == "" && strValue == "")
                    {
                        listAttributes.Remove("");
                        continue;
                    }

                    if (strMethod == "AREA")
                    {
                        hashAttributePercentage.Add(strValue, strArea);
                    }
                    else
                    {
                        double dPercentage = (double.Parse(strArea) / double.Parse(strTotalArea)) * 100;
                        hashAttributePercentage.Add(strValue, dPercentage.ToString());
                    }
                }
                catch
                {
                    hashAttributePercentage.Add(strValue, "0");
                }
            }



            //Get the total area for the given criteria


            if(useLaneMiles) strSelect = "SELECT SUM(" + strSegment + ".LANE_MILES) " + strFrom + " WHERE " + strAttribute + strYear + " IS NULL ";
            strSelect = "SELECT SUM(" + strReport + ".AREA) " + strFrom + " WHERE " + strAttribute + strYear + " IS NULL ";
            if (strCriteria != "")
            {
                strSelect += " AND " + strCriteria;
            }

            try
            {
                DataSet dsAttribute = DBMgr.ExecuteQuery(strSelect);
                String strArea = dsAttribute.Tables[0].Rows[0].ItemArray[0].ToString();
                if (strArea != "") listAttributes.Add("NULL");

                if (strMethod == "AREA")
                {
                    hashAttributePercentage.Add("NULL", strArea);
                }
                else
                {
                    double dPercentage = (double.Parse(strArea) / double.Parse(strTotalArea)) * 100;
                    hashAttributePercentage.Add("NULL", dPercentage.ToString());
                }
            }
            catch
            {
                hashAttributePercentage.Add("NULL", "0");
            }

            return hashAttributePercentage;
        }

        /// <summary>
        /// Builds inner join from statement for Network level queries
        /// </summary>
        /// <param name="strNetworkID">NetworkID of network to query</param>
        /// <returns>Full inner join of section and segment tables</returns>
        public static String BuildFromStatement(String strNetworkID)
        {
            String strFrom = " FROM SECTION_" + strNetworkID + " INNER JOIN SEGMENT_" + strNetworkID + "_NS0 ON SECTION_" + strNetworkID + ".SECTIONID=SEGMENT_" + strNetworkID + "_NS0.SECTIONID";
            return strFrom;
        }
        
        /// <summary>
        /// Builds inner join from statement for Network + Simulation level queries
        /// </summary>
        /// <param name="strNetworkID">NetworkID of network to query</param>
        /// <param name="strNetworkID">SimulationID of network to query. Empty string if searching only network</param>
        /// <returns>Full inner join of section, segment, simulation and report tables</returns>
        public static String BuildFromStatement(String strNetworkID, String strSimulationID, bool bShowAll)
        {
            String strFrom = "";
            if (strSimulationID == "")
            {
                return BuildFromStatement(strNetworkID);
            }
            else
            {
                String sSection = "SECTION_" + strNetworkID;
                String sSegment = "SEGMENT_" + strNetworkID + "_NS0";
                String sSimulation = "SIMULATION_" + strNetworkID + "_" + strSimulationID + "_0";
                String sReport = "REPORT_" + strNetworkID + "_" + strSimulationID;

                if (bShowAll)
                {
                    strFrom = " FROM " + sSection + " INNER JOIN " + sSegment + " ON " + sSection + ".SECTIONID=" + sSegment + ".SECTIONID INNER JOIN " + sSimulation + " ON " + sSection + ".SECTIONID=" + sSimulation + ".SECTIONID "; //INNER JOIN " + sReport + " ON " + sSection + ".SECTIONID=" + sReport + ".SECTIONID";
                }
                else
                {
                    //strFrom = " FROM " + sSection + " SAMPLE(10) INNER JOIN " + sSegment + " ON " + sSection + ".SECTIONID=" + sSegment + ".SECTIONID INNER JOIN " + sSimulation + " ON " + sSection + ".SECTIONID=" + sSimulation + ".SECTIONID "; //INNER JOIN " + sReport + " ON " + sSection + ".SECTIONID=" + sReport + ".SECTIONID";
                    strFrom = " FROM " + sSection + " INNER JOIN " + sSegment + " ON " + sSection + ".SECTIONID=" + sSegment + ".SECTIONID INNER JOIN " + sSimulation + " ON " + sSection + ".SECTIONID=" + sSimulation + ".SECTIONID "; //INNER JOIN " + sReport + " ON " + sSection + ".SECTIONID=" + sReport + ".SECTIONID";
                }
            }

            return strFrom;

        }

        public static String BuildFromStatementWithReport(String strNetworkID, String strSimulationID)
        {
            String strFrom = "";
            if (strSimulationID == "")
            {
                return BuildFromStatement(strNetworkID);
            }
            else
            {
                String sSection = "SECTION_" + strNetworkID;
                String sSegment = "SEGMENT_" + strNetworkID + "_NS0";
                String sSimulation = "SIMULATION_" + strNetworkID + "_" + strSimulationID;
                String sReport = "REPORT_" + strNetworkID + "_" + strSimulationID;

                strFrom = " FROM " + sSection + " INNER JOIN " + sSegment + " ON " + sSection + ".SECTIONID=" + sSegment + ".SECTIONID INNER JOIN " + sSimulation + " ON " + sSection + ".SECTIONID=" + sSimulation + ".SECTIONID INNER JOIN " + sReport + " ON " + sSection + ".SECTIONID=" + sReport + ".SECTIONID ";
            }

            return strFrom;
        }

        public static String GetNetworkIDFromName( String strNetworkName )
        {
            String query = "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME = '" + strNetworkName + "'";
            return DBMgr.ExecuteQuery( query ).Tables[0].Rows[0]["NETWORKID"].ToString();
        }
        
        public static DataSet GetDistinctAttributeYears(String attributeName, ConnectionParameters cp)
        {
            String query;
            DataSet distinctAttributeYears;
            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    query = "Select DISTINCT year(DATE_) AS Years From " + attributeName + " ORDER BY Years";
                    distinctAttributeYears = DBMgr.ExecuteQuery(query, cp);
                    break;
                case "ORACLE":
                    query = "Select DISTINCT TO_CHAR(DATE_, 'YYYY') AS Years From " + attributeName + " ORDER BY Years";
                    distinctAttributeYears = DBMgr.ExecuteQuery(query, cp);
                    break;
                default:
                    throw new NotImplementedException( "TODO: Create ANSI implementation for GetDistinctAttributeYears()" );
                    //break;
            }
            return distinctAttributeYears;
        }

        public static DataSet GetValidFacilities(ConnectionParameters cp)
        {
            String query;
            DataSet nonNullFacilities;
            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    query = "Select DISTINCT FACILITY FROM NETWORK_DEFINITION WHERE FACILITY <> '' ORDER BY FACILITY";
                    break;
                case "ORACLE":
                    query = "Select DISTINCT FACILITY FROM NETWORK_DEFINITION WHERE FACILITY LIKE '_%' ORDER BY FACILITY";
                    break;
                default:
                    query = "Select DISTINCT FACILITY FROM NETWORK_DEFINITION WHERE FACILITY <> '' ORDER BY FACILITY";
                    break;
            }
            nonNullFacilities = DBMgr.ExecuteQuery(query, cp);
            return nonNullFacilities;
        }

        public static DataSet GetValidRoutes(ConnectionParameters cp)
        {
            String query;
            DataSet nonNullFacilities;
            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    query = "Select DISTINCT ROUTES FROM NETWORK_DEFINITION WHERE ROUTES <> '' ORDER BY ROUTES";
                    break;
                case "ORACLE":
                    query = "Select DISTINCT ROUTES FROM NETWORK_DEFINITION WHERE ROUTES LIKE '_%' ORDER BY ROUTES";
                    break;
                default:
                    query = "Select DISTINCT ROUTES FROM NETWORK_DEFINITION WHERE ROUTES <> '' ORDER BY ROUTES";
                    break;
            }
            nonNullFacilities = DBMgr.ExecuteQuery(query, cp);
            return nonNullFacilities;
        }

        public static int GetReportID(string reportName)
        {
            return DBMgr.ExecuteScalar( "SELECT REPORTID FROM REPORTS WHERE REPORTNAME = '" + reportName + "'" );
        }

        public static string GetReportName(int reportID)
        {
            return DBMgr.ExecuteQuery("SELECT REPORTNAME FROM REPORTS WHERE REPORTID = " + reportID).Tables[0].Rows[0]["REPORTNAME"].ToString();
        }

        public static string GetReportType(int reportID)
        {
            return DBMgr.ExecuteQuery("SELECT ReportType FROM REPORTS WHERE REPORTID = " + reportID).Tables[0].Rows[0]["ReportType"].ToString();
        }

        public static List<string> GetNetworkReportNames()
        {
            List<string> networkReportNames = new List<string>();
            DataSet networkReports = DBMgr.ExecuteQuery("SELECT REPORTNAME FROM REPORTS WHERE ReportType = 'NETWORK'");
            if( networkReports.Tables.Count > 0 )
            {
                foreach( DataRow networkReportName in networkReports.Tables[0].Rows )
                {
                    networkReportNames.Add(networkReportName["REPORTNAME"].ToString());
                }
            }
            return networkReportNames;
        }

        public static List<string> GetSimulationReportNames()
        {
            List<string> networkReportNames = new List<string>();
            DataSet networkReports = DBMgr.ExecuteQuery("SELECT REPORTNAME FROM REPORTS WHERE ReportType = 'SIMULATION'");
            foreach (DataRow networkReportName in networkReports.Tables[0].Rows)
            {
                networkReportNames.Add(networkReportName["REPORTNAME"].ToString());
            }
            return networkReportNames;
        }

        public static List<string> GetAssetInventoryReportNames()
        {
            List<string> assetReportNames = new List<string>();
            DataSet assetReports = DBMgr.ExecuteQuery("SELECT REPORTNAME FROM REPORTS WHERE ReportType = 'ASSET'");
            foreach (DataRow assetReport in assetReports.Tables[0].Rows)
            {
                assetReportNames.Add(assetReport["REPORTNAME"].ToString());
            }
            return assetReportNames;
        }

        public static List<string> GetAttributeInventoryReportNames()
        {
            List<string> attributeReportNames = new List<string>();
            DataSet attributeReports = DBMgr.ExecuteQuery("SELECT REPORTNAME FROM REPORTS WHERE ReportType = 'ASSET'");
            foreach (DataRow attributeReport in attributeReports.Tables[0].Rows)
            {
                attributeReportNames.Add(attributeReport["REPORTNAME"].ToString());
            }
            return attributeReportNames;
        }

        public static bool IsAttributeCalculated(string attribute)
        {
            string query = "SELECT * FROM ATTRIBUTES_ WHERE ATTRIBUTE_ = '" + attribute + "'";
            DataSet attributeData = DBMgr.ExecuteQuery(query);
            AttributeObject attributeToTest;
            if (attributeData.Tables[0].Rows.Count == 1)
            {
                attributeToTest = new AttributeObject(attributeData.Tables[0].Rows[0]);
            }
            else
            {
                throw new ArgumentException("Invalid attribute specification for IsAttributeCalculated().");
            }
            return attributeToTest.Calculated;
        }

        public static string GetAdvancedFilterSelectStatement(bool IsLinear, string networkID, string facility, string advancedSearchText, string property, string value)
        {
            string select = "";
            if (IsLinear)
            {
                select = "SELECT FACILITY,SECTION,BEGIN_STATION,END_STATION,DIRECTION";
            }
            else
            {
                select = "SELECT FACILITY,SECTION,SECTION_" + networkID + ".SECTIONID";
            }

            //Then at a minumum
            select += " FROM SECTION_" + networkID;

            // Now for each table that is attached we add
            select += " INNER JOIN SEGMENT_" + networkID + "_NS0 ON SECTION_" + networkID + ".SECTIONID=SEGMENT_" + networkID + "_NS0.SECTIONID";

            String strWhere = "";
            String strSearch = advancedSearchText;
            strSearch = strSearch.Trim();

            if (facility != "All")
            {
                strWhere = " WHERE FACILITY='" + facility + "'";
                select += strWhere;
            }
            if (strSearch != "")
            {
                select += " AND ";
                select += "(" + strSearch + ")";
            }
            if (property != "")
            {
                if (value != "All")
                {
                    select += " AND ";
                    select += "(" + property.Trim() + " = '" + value.Trim() +"')";
                }
            }
            
            return select;
        }

        public static BindingSource CreateBoundTable(string query)
        {
            BindingSource binding = new BindingSource();
            DataAdapter dataAdapter = new DataAdapter(query);

            // Populate a new data table and bind it to the BindingSource.
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);

            binding.DataSource = table;
            return binding;
        }

        public static DataSet GetCurrentNetworkLockData( string networkID )
        {
            DateTime lastTime = DateTime.Now - LockTimeout;
            DataSet lockSet = null;

            switch( DBMgr.NativeConnectionParameters.Provider )
            {
                case "MSSQL":
                    //with networkread and simulationread, we can collapse the readonly locking of the network
                    //and the full locking of the simulation into one row
                    //lockSet = DBMgr.ExecuteQuery( "SELECT NETWORKID, SIMULATIONID, LOCK_START, USERID, LOCKID, NETWORKREAD, SIMULATIONREAD FROM MULTIUSER_LOCK WHERE SIMULATIONID IS NULL AND NETWORKID = '" + networkID + "' AND LOCK_START >= '" + lastTime.ToString() + "' ORDER BY LOCK_START DESC" );
                    lockSet = DBMgr.ExecuteQuery( "SELECT NETWORKID, SIMULATIONID, LOCK_START, USERID, LOCKID, NETWORKREAD, SIMULATIONREAD FROM MULTIUSER_LOCK WHERE NETWORKID = '" + networkID + "' AND LOCK_START >= (CAST('" + lastTime.ToString() + "' as datetime)) ORDER BY LOCK_START DESC" );
                    break;
                case "ORACLE":
                    //lockSet = DBMgr.ExecuteQuery( "SELECT NETWORKID, SIMULATIONID, LOCK_START, USERID, LOCKID, NETWORKREAD, SIMULATIONREAD FROM MULTIUSER_LOCK WHERE SIMULATIONID IS NULL AND NETWORKID = '" + networkID + "' AND LOCK_START >= TO_DATE(" + lastTime.ToString( "yyyyMMddHHmm" ) + ", 'YYYYMMDDHH24MI') ORDER BY LOCK_START DESC" );
                    lockSet = DBMgr.ExecuteQuery( "SELECT NETWORKID, SIMULATIONID, LOCK_START, USERID, LOCKID, NETWORKREAD, SIMULATIONREAD FROM MULTIUSER_LOCK WHERE NETWORKID = '" + networkID + "' AND LOCK_START >= TO_DATE(" + lastTime.ToString( "yyyyMMddHHmm" ) + ", 'YYYYMMDDHH24MI') ORDER BY LOCK_START DESC" );
                    break;
                default:
                    throw new NotImplementedException( "TODO: Implement ANSI version of GetCurrentNetworkLockData() " );
            }

            return lockSet;
        }

        public static DataSet GetCurrentSimulationLockData( string networkID, string simulationID )
        {
            DateTime lastTime = DateTime.Now - LockTimeout;
            DataSet lockSet = null;

            switch( DBMgr.NativeConnectionParameters.Provider )
            {
                case "MSSQL":
                    lockSet = DBMgr.ExecuteQuery( "SELECT NETWORKID, SIMULATIONID, LOCK_START, USERID, LOCKID, NETWORKREAD, SIMULATIONREAD FROM MULTIUSER_LOCK WHERE NETWORKID = '" + networkID + "' AND SIMULATIONID = " + simulationID + " AND LOCK_START >= convert( datetime, '" + lastTime.ToString( "yyyy-MM-dd HH:mm:ss" ) + "', 120) ORDER BY LOCK_START DESC" );
                    break;
                case "ORACLE":
                    lockSet = DBMgr.ExecuteQuery( "SELECT NETWORKID, SIMULATIONID, LOCK_START, USERID, LOCKID, NETWORKREAD, SIMULATIONREAD FROM MULTIUSER_LOCK WHERE NETWORKID = '" + networkID + "' AND SIMULATIONID = " + simulationID + " AND LOCK_START >= TO_DATE(" + lastTime.ToString( "yyyyMMddHHmm" ) + ", 'YYYYMMDDHH24MI') ORDER BY LOCK_START DESC" );
                    break;
                default:
                    throw new NotImplementedException( "TODO: Implement ANSI version of GetCurrentSimulationLockData() " );
            }

            return lockSet;
        }

        public static void AddNetworkLock( string networkID, string userID, bool readable )
        {
            int lockID = -1;
            switch( DBMgr.NativeConnectionParameters.Provider )
            {
                case "MSSQL":
                    lockID = DBMgr.ExecuteScalar( "SELECT MAX(LOCKID) FROM MULTIUSER_LOCK" ) + 1;
                    if( DBMgr.ExecuteNonQuery( "INSERT INTO MULTIUSER_LOCK (LOCKID, NETWORKID, LOCK_START, USERID, NETWORKREAD)  VALUES ('" + lockID.ToString() + "', '" + networkID + "', '" + DateTime.Now.ToString() + "', '" + userID + "', '"+ (readable ? "1" : "0") +"')" ) < 1 )
                    {
                        throw new Exception( "ERROR creating network lock for networkID [" + networkID + "] / userID [" + userID + "]" );
                    }
                    break;
                case "ORACLE":
                    lockID = DBMgr.ExecuteScalar( "SELECT MAX(LOCKID) FROM MULTIUSER_LOCK" ) + 1;
                    if( DBMgr.ExecuteNonQuery( "INSERT INTO MULTIUSER_LOCK (LOCKID, NETWORKID, LOCK_START, USERID, NETWORKREAD)  VALUES ('" + lockID.ToString() + "', '" + networkID + "', TO_DATE('" + DateTime.Now.ToString( "yyyyMMddHHmm" ) + "', 'YYYYMMDDHH24MI'), '" + userID + "', '" + (readable ? "1" : "0") + "')" ) < 1 )
                    {
                        throw new Exception( "ERROR creating network lock for networkID [" + networkID + "] / userID [" + userID + "]" );
                    }
                    break;
                default:
                    throw new NotImplementedException( "TODO: Implement ANSI version of AddNetworkLock() " );
            }
        }

        public static void AddSimulationLock( string networkID, string simulationID, string userID, bool readable )
        {
            int lockID = -1;
            switch( DBMgr.NativeConnectionParameters.Provider )
            {
                case "MSSQL":
                    lockID = DBMgr.ExecuteScalar( "SELECT MAX(LOCKID) FROM MULTIUSER_LOCK" ) + 1;
                    if( DBMgr.ExecuteNonQuery( "INSERT INTO MULTIUSER_LOCK (LOCKID, NETWORKID, SIMULATIONID, LOCK_START, USERID, NETWORKREAD, SIMULATIONREAD)  VALUES ('" + lockID.ToString() + "', '" + networkID + "','" + simulationID + "', convert( datetime, '" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) + "', 120), '" + userID + "', '1', '" + ( readable ? "1" : "0" ) + "')" ) < 1 )
                    {
                        throw new Exception( "ERROR creating simulation lock for networkID [" + networkID + "] / simulationID [" + simulationID + "] / userID [" + userID + "]" );
                    }
                    break;
                case "ORACLE":
                    lockID = DBMgr.ExecuteScalar( "SELECT MAX(LOCKID) FROM MULTIUSER_LOCK" ) + 1;
                    if( DBMgr.ExecuteNonQuery( "INSERT INTO MULTIUSER_LOCK (LOCKID, NETWORKID, SIMULATIONID, LOCK_START, USERID, NETWORKREAD, SIMULATIONREAD)  VALUES ('" + lockID.ToString() + "', '" + networkID + "','" + simulationID + "', TO_DATE(" + DateTime.Now.ToString( "yyyyMMddHHmm" ) + ", 'YYYYMMDDHH24MI'), '" + userID + "', '1', '" + ( readable ? "1" : "0" ) + "')" ) < 1 )
                    {
                        throw new Exception( "ERROR creating simulation lock for networkID [" + networkID + "] / simulationID [" + simulationID + "] / userID [" + userID + "]" );
                    }
                    break;
                default:
                    throw new NotImplementedException( "TODO: Implement ANSI version of AddSimulationLock() " );
            }
        }

        public static void RemoveLock( string lockID )
        {
            switch( DBMgr.NativeConnectionParameters.Provider )
            {
                case "MSSQL":
                case "ORACLE":
                    if( DBMgr.ExecuteNonQuery( "DELETE FROM MULTIUSER_LOCK WHERE LOCKID = '" + lockID + "'" ) < 1 )
                    {
                        throw new Exception( "ERROR deleting network lock for LOCKID [" + lockID + "]" );
                    }

                    break;
                default:
                    throw new NotImplementedException( "TODO: Implement Non-Oracle version of RemoveLock() " );
            }
        }

        public static List<CompoundTreatment> GetCompoundTreatments()
        {
            List<CompoundTreatment> toReturn = new List<CompoundTreatment>();
            string query = "SELECT COMPOUND_TREATMENT_NAME FROM COMPOUND_TREATMENTS";
            DataSet ds = DBMgr.ExecuteQuery(query);
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                CompoundTreatment toAdd = new CompoundTreatment(row["COMPOUND_TREATMENT_NAME"].ToString());
                toReturn.Add(toAdd);
            }
            return toReturn;
        }

        public static void RenameSimulation(string newName, string simulationID)
        {
            string sqlUpdate = "UPDATE SIMULATIONS SET SIMULATION='" + newName + "' WHERE SIMULATIONID='" + simulationID + "'";
            DBMgr.ExecuteNonQuery(sqlUpdate);
        }

        public static void RenameNetwork(string newName, string networkID)
        {
            string sqlUpdate = "UPDATE NETWORKS SET NETWORK_NAME='" + newName + "' WHERE NETWORKID='" + networkID + "'";
            DBMgr.ExecuteNonQuery(sqlUpdate);
        }

        /// <summary>
        /// Sets network specific area for a given network.
        /// </summary>
        /// <param name="networkID">NetworkID of the network</param>
        /// <param name="areaEquation">The</param>
        public static void UpdateNetworkSpecificArea(string networkID, string areaEquation)
        {
            string sqlUpdate = "UPDATE NETWORKS SET NETWORK_AREA='" + areaEquation + "' WHERE NETWORKID='" + networkID + "'";
            DBMgr.ExecuteNonQuery(sqlUpdate);
        }

        /// <summary>
        /// Get network specific area.
        /// </summary>
        /// <param name="networkID">NetworkID of network</param>
        /// <returns>The area specific network equation</returns>
        public static string GetNetworkSpecificArea(string networkID)
        {
            string areaEquation = "";
            string query = "SELECT NETWORK_AREA FROM NETWORKS WHERE NETWORKID='" + networkID + "'";
            DataSet ds = DBMgr.ExecuteQuery(query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["NETWORK_AREA"] != DBNull.Value)
                {
                    areaEquation = row["NETWORK_AREA"].ToString();
                }
            }
            return areaEquation;
        }


        /// <summary>
        /// Modify RoadCare schema to allow multiple budgets per treatment.
        /// </summary>
        public static void UpdateRoadCareForMultipleBudgetsPerTreatment()
        {
            DataSet ds =  DBMgr.GetTableColumnsWithTypes("TREATMENTS");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if(row["column_name"].ToString().ToUpper() == "BUDGET")
                {
                    string length = row["Character_maximum_length"].ToString();
                    int nLength = Convert.ToInt32(length);
                    if (nLength < 513)
                    {
                        if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                        {
                            DBMgr.ExecuteNonQuery("ALTER TABLE TREATMENTS MODIFY BUDGET varchar2(512 CHAR)");
                        }
                        if (DBMgr.NativeConnectionParameters.Provider == "MSSQL")
                        {
                            DBMgr.ExecuteNonQuery("ALTER TABLE TREATMENTS ALTER COLUMN BUDGET [varchar](512) NULL");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Modify Roadcare schema to allow for equations as a result of consequences.
        /// </summary>
        public static void UpdateRoadCareForEquationConsequences()
        {
            DataSet ds = DBMgr.GetTableColumnsWithTypes("CONSEQUENCES");
            bool isEquation = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "EQUATION")
                {
                    isEquation = true;
                }
            }
            if (!isEquation)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONSEQUENCES ADD EQUATION varchar2(4000) NULL");
                }
                else
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONSEQUENCES ADD EQUATION [varchar](4000) NULL");
                }
            }
        }

        /// <summary>
        /// Modify the RoadCare schema to store piecewise performance curves
        /// </summary>
        public static void UpdateRoadCareForPiecewisePerformance()
        {
            DataSet ds = DBMgr.GetTableColumnsWithTypes("PERFORMANCE");
            bool isPieceWise = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "PIECEWISE")
                {
                    isPieceWise = true;
                }
            }
            if (!isPieceWise)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                    DBMgr.ExecuteNonQuery("ALTER TABLE PERFORMANCE ADD PIECEWISE NUMBER(1) NULL");
                else
                    DBMgr.ExecuteNonQuery("ALTER TABLE PERFORMANCE ADD PIECEWISE bit NULL");
            }

        }


        /// <summary>
        /// Modify the RoadCare schema to store network specific areas.
        /// </summary>
        public static void UpdateRoadCareForNetworkSpecificArea()
        {
            DataSet ds = DBMgr.GetTableColumnsWithTypes("NETWORKS");
            bool isNetworkArea = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "NETWORK_AREA")
                {
                    isNetworkArea = true;
                }
            }
            if (!isNetworkArea)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE NETWORKS ADD NETWORK_AREA varchar2(4000) NULL");
                }
                else
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE NETWORKS ADD NETWORK_AREA [varchar](4000) NULL");
                }
            }
        }

        public static void UpdateRoadCareForFunctionEquationTables()
        {
            DataSet ds = DBMgr.GetTableColumnsWithTypes("PERFORMANCE");
            bool isPerformaceFunctionEquation = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "ISFUNCTION")
                {
                    isPerformaceFunctionEquation = true;
                }
            }

            if (!isPerformaceFunctionEquation)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE PERFORMANCE ADD ISFUNCTION NUMBER(1) NULL");
                }
                else
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE PERFORMANCE ADD ISFUNCTION bit NULL");
                }
            }


            ds = DBMgr.GetTableColumnsWithTypes("CONSEQUENCES");
            bool isConsequencesFunctionEquation = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "ISFUNCTION")
                {
                    isConsequencesFunctionEquation = true;
                }
            }

            if (!isConsequencesFunctionEquation)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONSEQUENCES ADD ISFUNCTION NUMBER(1) NULL");
                }
                else
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONSEQUENCES ADD ISFUNCTION bit NULL");
                }
            }


            ds = DBMgr.GetTableColumnsWithTypes("COSTS");
            bool isCostFunctionEquation = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "ISFUNCTION")
                {
                    isCostFunctionEquation = true;
                }
            }

            if (!isCostFunctionEquation)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE COSTS ADD ISFUNCTION NUMBER(1) NULL");
                }
                else
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE COSTS ADD ISFUNCTION bit NULL");
                }
            }


            ds = DBMgr.GetTableColumnsWithTypes("ATTRIBUTES_CALCULATED");
            bool isCalculatedFunctionEquation = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "ISFUNCTION")
                {
                    isCalculatedFunctionEquation = true;
                }
            }

            if (!isCalculatedFunctionEquation)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE ATTRIBUTES_CALCULATED ADD ISFUNCTION NUMBER(1) NULL");
                }
                else
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE ATTRIBUTES_CALCULATED ADD ISFUNCTION bit NULL");
                }
            }




        }


        /// <summary>
        /// Update Roadcare schema for new features.
        /// </summary>
        public static void UpdateTables()
        {
            //Decide to just concatenate Budget under treatment
            UpdateRoadCareForMultipleBudgetsPerTreatment();
            UpdateRoadCareForEquationConsequences();
            UpdateRoadCareForPiecewisePerformance();
            UpdateRoadCareForNetworkSpecificArea();
            UpdateRoadCareForFunctionEquationTables();//  Adds the necessary columns to tables with equations for allowing use of complete functions for equations.
            UpdateRoadCareForConditionalRsl();
            UpdateRoadCareForCumulativeCost();
            UpdateRoadCareForUseAcrossBudget();
            UpdateRoadCareForRemainingLifeLimits();
            UpdateRoadcareForBudgetCriteria();
            UpdateRoadcareForSupercedes();
            UpdateRoadCareForScheduled();
            UpdateRoadCareForSplitTreatment();
            UpdateRoadCareForSplitTreatmentLimit();

        }

        private static void UpdateRoadCareForSplitTreatmentLimit()
        {
            if (!DBMgr.CheckIfTableExists("SPLIT_TREATMENT_LIMIT"))
            {
                DBMgr.ExecuteNonQuery("CREATE TABLE [dbo].[SPLIT_TREATMENT_LIMIT]([SPLIT_TREATMENT_LIMIT_ID][int] IDENTITY(1, 1) NOT NULL,[SPLIT_TREATMENT_ID][int] NOT NULL,[RANK][int] NULL,[AMOUNT][float] NULL,[PERCENTAGE][varchar](max) NULL,CONSTRAINT[PK_SPLIT_TREATMENT_LIMIT] PRIMARY KEY CLUSTERED"+
                        "([SPLIT_TREATMENT_LIMIT_ID] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SPLIT_TREATMENT_LIMIT]  WITH CHECK ADD  CONSTRAINT [FK_SPLIT_TREATMENT_LIMIT_SPLIT_TREATMENT] FOREIGN KEY([SPLIT_TREATMENT_ID])REFERENCES[dbo].[SPLIT_TREATMENT]([SPLIT_TREATMENT_ID]) ON DELETE CASCADE");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SPLIT_TREATMENT_LIMIT] CHECK CONSTRAINT [FK_SPLIT_TREATMENT_LIMIT_SPLIT_TREATMENT]");
            }
        }

        private static void UpdateRoadCareForSplitTreatment()
        {
            if (!DBMgr.CheckIfTableExists("SPLIT_TREATMENT"))
            {
                DBMgr.ExecuteNonQuery("CREATE TABLE [dbo].[SPLIT_TREATMENT]([SIMULATIONID][int] NOT NULL,[SPLIT_TREATMENT_ID][int] IDENTITY(1, 1) NOT NULL,[DESCRIPTION][varchar](50) NULL,[CRITERIA][varchar](max) NULL, CONSTRAINT[PK_SPLIT_TREATMENT] PRIMARY KEY CLUSTERED" +
                    "([SPLIT_TREATMENT_ID] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SPLIT_TREATMENT]  WITH CHECK ADD  CONSTRAINT [FK_SPLIT_TREATMENT_SIMULATIONS] FOREIGN KEY([SIMULATIONID]) REFERENCES[dbo].[SIMULATIONS]([SIMULATIONID])");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SPLIT_TREATMENT] CHECK CONSTRAINT [FK_SPLIT_TREATMENT_SIMULATIONS]");

            }
        }

        private static void UpdateRoadCareForScheduled()
        {
            if (!DBMgr.CheckIfTableExists("SCHEDULED"))
            {
                DBMgr.ExecuteNonQuery("CREATE TABLE [dbo].[SCHEDULED]([SCHEDULEDID][int] IDENTITY(1, 1) NOT NULL,[TREATMENTID][int] NOT NULL,[SCHEDULEDYEAR][int] NOT NULL,[SCHEDULEDTREATMENTID][int] NOT NULL,"+
                        "CONSTRAINT[PK_SCHEDULED] PRIMARY KEY CLUSTERED([SCHEDULEDID] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SCHEDULED]  WITH CHECK ADD  CONSTRAINT [FK_SCHEDULED_TREATMENTS] FOREIGN KEY([TREATMENTID])REFERENCES[dbo].[TREATMENTS]([TREATMENTID]) ON DELETE CASCADE");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SCHEDULED] CHECK CONSTRAINT [FK_SCHEDULED_TREATMENTS]");

            }
        }

        private static void UpdateRoadcareForSupercedes()
        {
            if (!DBMgr.CheckIfTableExists("SUPERSEDES"))
            {
                DBMgr.ExecuteNonQuery("CREATE TABLE [dbo].[SUPERSEDES]([SUPERSEDE_ID][int] IDENTITY(1, 1) NOT NULL,[TREATMENT_ID][int] NULL,[SUPERSEDE_TREATMENT_ID][int] NULL,[CRITERIA][varchar](max) NULL," +
                                    "CONSTRAINT[PK_SUPERSEDES] PRIMARY KEY CLUSTERED([SUPERSEDE_ID] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SUPERSEDES]  WITH CHECK ADD  CONSTRAINT [FK_SUPERSEDES_TREATMENTS] FOREIGN KEY([TREATMENT_ID])REFERENCES[dbo].[TREATMENTS]([TREATMENTID])ON DELETE CASCADE");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SUPERSEDES] CHECK CONSTRAINT [FK_SUPERSEDES_TREATMENTS]");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SUPERSEDES]  WITH CHECK ADD  CONSTRAINT [FK_SUPERSEDES_TREATMENTS_COMPONENT] FOREIGN KEY([SUPERSEDE_TREATMENT_ID]) REFERENCES[dbo].[TREATMENTS]([TREATMENTID])");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[SUPERSEDES] CHECK CONSTRAINT [FK_SUPERSEDES_TREATMENTS_COMPONENT]");
            }
        }

        private static void UpdateRoadcareForBudgetCriteria()
        {
            if (!DBMgr.CheckIfTableExists("BUDGET_CRITERIA"))
            {
                DBMgr.ExecuteNonQuery("CREATE TABLE [dbo].[BUDGET_CRITERIA]([BUDGET_CRITERIA_ID][int] IDENTITY(1, 1) NOT NULL,[SIMULATIONID][int] NOT NULL,[BUDGET_NAME][varchar](50) NOT NULL,[CRITERIA][varchar](max) NULL,"+
                                    "CONSTRAINT[BUDGETCRITERIAPK] PRIMARY KEY CLUSTERED([BUDGET_CRITERIA_ID] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[BUDGET_CRITERIA]  WITH CHECK ADD  CONSTRAINT [FK_BUDGETCRITERIA_SIMULATIONS] FOREIGN KEY([SIMULATIONID])REFERENCES[dbo].[SIMULATIONS]([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[BUDGET_CRITERIA] CHECK CONSTRAINT [FK_BUDGETCRITERIA_SIMULATIONS]");
            }
        }

        private static void UpdateRoadCareForRemainingLifeLimits()
        {
            if (!DBMgr.CheckIfTableExists("REMAINING_LIFE_LIMITS"))
            {
                DBMgr.ExecuteNonQuery("CREATE TABLE [dbo].[REMAINING_LIFE_LIMITS]([REMAINING_LIFE_ID][int] IDENTITY(1, 1) NOT NULL,[SIMULATION_ID][int] NOT NULL,[ATTRIBUTE_][varchar](50) NOT NULL," +
                                       "[REMAINING_LIFE_LIMIT][float] NULL,[CRITERIA][varchar](max) NULL, CONSTRAINT[PK_REMAINING_LIFE_LIMITS] PRIMARY KEY CLUSTERED([REMAINING_LIFE_ID] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]" +
                                        ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");


                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[REMAINING_LIFE_LIMITS]  WITH CHECK ADD  CONSTRAINT [FK_REMAINING_LIFE_LIMITS_ATTRIBUTES_] FOREIGN KEY([ATTRIBUTE_])REFERENCES[dbo].[ATTRIBUTES_]([ATTRIBUTE_])ON UPDATE CASCADE ON DELETE CASCADE");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[REMAINING_LIFE_LIMITS] CHECK CONSTRAINT [FK_REMAINING_LIFE_LIMITS_ATTRIBUTES_]");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[REMAINING_LIFE_LIMITS]  WITH CHECK ADD  CONSTRAINT [FK_REMAINING_LIFE_LIMITS_REMAINING_LIFE_LIMITS] FOREIGN KEY([SIMULATION_ID]) REFERENCES[dbo].[SIMULATIONS]([SIMULATIONID]) ON DELETE CASCADE");
                DBMgr.ExecuteNonQuery("ALTER TABLE [dbo].[REMAINING_LIFE_LIMITS] CHECK CONSTRAINT [FK_REMAINING_LIFE_LIMITS_REMAINING_LIFE_LIMITS]");
            }
        }

        private static void UpdateRoadCareForCumulativeCost()
	    {
	        var ds = DBMgr.GetTableColumnsWithTypes("SIMULATIONS");
	        bool isCumulativeCost = false;
	        foreach (DataRow row in ds.Tables[0].Rows)
	        {
	            if (row["column_name"].ToString().ToUpper() == "USE_CUMULATIVE_COST")
	            {
	                isCumulativeCost = true;
	            }
	        }
	        if (!isCumulativeCost)
	        {
	            if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
	            {
	                DBMgr.ExecuteNonQuery("ALTER TABLE SIMULATIONS ADD USE_CUMULATIVE_COST NUMBER(1) NULL");
	            }
	            else
	            {
	                DBMgr.ExecuteNonQuery("ALTER TABLE SIMULATIONS ADD USE_CUMULATIVE_COST BIT NULL");
	            }
	        }
        }

        private static void UpdateRoadCareForUseAcrossBudget()
        {
            var ds = DBMgr.GetTableColumnsWithTypes("SIMULATIONS");
            bool isCumulativeCost = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "USE_ACROSS_BUDGET")
                {
                    isCumulativeCost = true;
                }
            }
            if (!isCumulativeCost)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE SIMULATIONS ADD USE_ACROSS_BUDGET NUMBER(1) NULL");
                }
                else
                {
                    DBMgr.ExecuteNonQuery("ALTER TABLE SIMULATIONS ADD USE_ACROSS_BUDGET BIT NULL");
                }
            }
        }


        private static void UpdateRoadCareForConditionalRsl()
        {
            if (!DBMgr.IsTableInDatabase("CONDITIONAL_RSL"))
            {
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        DBMgr.ExecuteNonQuery("CREATE TABLE [CONDITIONAL_RSL]( ATTRIBUTE [varchar](50) NOT NULL, [BIN] [INT] NULL, [VALUE][FLOAT] NOT NULL) ");
                        break;
                    case "ORACLE":
                        DBMgr.ExecuteNonQuery("CREATE TABLE \"CONDITIONAL_RSL\"   ( \"ATTRIBUTE\" VARCHAR2(256 BYTE) NOT NULL ENABLE,  \"BIN\" NUMBER NOT NULL ENABLE,  \"VALUE\" NUMBER NOT NULL ENABLE )");
                        break;
                    default:
                        throw new NotImplementedException("TODO: Implement ANSI version of CheckMultiUserTable()");
                }
            }


            DataSet ds = DBMgr.GetTableColumnsWithTypes("SIMULATIONS");
            bool isConditionalRSL = false;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "USE_CONDITIONAL_RSL")
                {
                    isConditionalRSL = true;
                }
            }
            if (!isConditionalRSL)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                    DBMgr.ExecuteNonQuery("ALTER TABLE SIMULATIONS ADD USE_CONDITIONAL_RSL NUMBER(1) NULL");
                else
                    DBMgr.ExecuteNonQuery("ALTER TABLE SIMULATIONS ADD USE_CONDITIONAL_RSL bit NULL");
            }

            DataSet dsCriteria = DBMgr.GetTableColumnsWithTypes("CONDITIONAL_RSL");
            bool isCriteria = false;
            foreach (DataRow row in dsCriteria.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "CRITERIA")
                {
                    isCriteria = true;
                }
            }

            if (!isCriteria)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONDITIONAL_RSL ADD  \"CRITERIA\" VARCHAR2(1024 BYTE)");
                else
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONDITIONAL_RSL ADD CRITERIA [varchar](1024) NULL");      
            }


            DataSet dsID = DBMgr.GetTableColumnsWithTypes("CONDITIONAL_RSL");
            bool isID = false;
            foreach (DataRow row in dsCriteria.Tables[0].Rows)
            {
                if (row["column_name"].ToString().ToUpper() == "ID")
                {
                    isID = true;
                }
            }

            if (!isID)
            {
                if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONDITIONAL_RSL ADD  \"ID\" NUMBER");
                else
                    DBMgr.ExecuteNonQuery("ALTER TABLE CONDITIONAL_RSL ADD ID [INT] NULL");
            }

        }
        

        public static void AddToConnectionParameters(ConnectionParameters cpToInsert, string connectionName)
        {
            // The oracle database doesnt support the bool datatype, gotta convert to numbers.
            int numNative = 0;
            if(cpToInsert.IsNative)
            {
                numNative = 1;
            }
            int intSec = 0;
            if(cpToInsert.IsIntegratedSecurity)
            {
                intSec = 1;
            }
            string insert = "INSERT INTO CONNECTION_PARAMETERS (PROVIDER, CONNECTION_ID, SERVER, NATIVE, DATABASE_NAME, SERVICE_NAME, SID, PORT, USERID, PASSWORD, INTEGRATED_SEC, VIEW_STATEMENT, IDENTIFIER, CONNECTION_NAME) VALUES ('"
                + cpToInsert.Provider + "', "
                + "-1,'"
                + cpToInsert.Server + "', '"
                + numNative + "', '"
                + cpToInsert.Database + "', '"
                + cpToInsert.NetworkAlias + "', '"
                + cpToInsert.SID + "', '"
                + cpToInsert.Port + "', '"
                + cpToInsert.UserName + "', '"
                + cpToInsert.Password + "', '"
                + intSec + "', '"
                + cpToInsert.ViewStatement + "', '"
                + cpToInsert.Identifier + "', '"
                + cpToInsert.ConnectionName + "')";
            DBMgr.ExecuteNonQuery(insert);
        }
        
        public static List<string> GetRemoteNetworkDefinitionConnectionParameterNames()
        {
            string query = "SELECT CONNECTION_NAME FROM CONNECTION_PARAMETERS ORDER BY CONNECTION_NAME";
            List<string> toReturn = new List<string>();
            DataSet ds = DBMgr.ExecuteQuery(query);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                toReturn.Add(dr.ItemArray[0].ToString());
            }
            return toReturn;
        }


        public static List<Performance> GetPerformanceEquations(String simulationID)
        {
            List<Performance> performances = new List<Performance>();
            String strSelect = "SELECT PERFORMANCEID,ATTRIBUTE_,EQUATIONNAME,EQUATION,CRITERIA,SHIFT,PIECEWISE,ISFUNCTION FROM PERFORMANCE WHERE SIMULATIONID='" + simulationID + "'";
            DataSet ds = new DataSet();
            ds = DBMgr.ExecuteQuery(strSelect);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Performance performance = new Performance();
                String strAttribute = dr[1].ToString();
                performance.PerformanceID = dr["PERFORMANCEID"].ToString();
                if (dr["PIECEWISE"] != DBNull.Value) performance.IsPiecewise= Convert.ToBoolean(dr["PIECEWISE"]);
                if (dr["ISFUNCTION"] != DBNull.Value) performance.IsFunction = Convert.ToBoolean(dr["ISFUNCTION"]);
                if (dr["SHIFT"] != DBNull.Value) performance.IsShift = Convert.ToBoolean(dr["SHIFT"]);
                if (dr["CRITERIA"] != DBNull.Value) performance.Criteria = dr["CRITERIA"].ToString().Replace("|", "'");
                if (dr["ATTRIBUTE_"] != DBNull.Value) performance.Attribute = dr["ATTRIBUTE_"].ToString();
                if (dr["EQUATION"] != DBNull.Value) performance.Equation = dr["EQUATION"].ToString();
                if (dr["EQUATIONNAME"] != DBNull.Value) performance.Name = dr["EQUATIONNAME"].ToString();
                performances.Add(performance);
            }
            return performances;
        }

        public static void DeletePerformanceEquations(List<string> performanceIDs)
        {

            foreach (String strID in performanceIDs)
            {
                String strDelete = "DELETE FROM PERFORMANCE WHERE PERFORMANCEID='" + strID + "'";
                DBMgr.ExecuteNonQuery(strDelete);
            }
        }

    }
}
