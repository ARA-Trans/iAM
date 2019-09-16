using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DatabaseManager;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.OleDb;
using RoadCareGlobalOperations;

namespace RoadCare3
{
	public static class SimulationManager
	{
		static String m_strCopyAnalysisSimulationID;
		static String m_strCopyInvestmentSimulationID;
		static String m_strCopyPerformanceSimulationID;
		static String m_strCopyTreatmentSimulationID;
		static String m_strCopyCommittedSimulationID;

		public static void CloneAnalysis()
		{
		}

		public static void CloneInvestment()
		{
		}

		public static void ClonePerformance()
		{
		}

		public static void CloneCommitted()
		{
		}

		public static void CloneTreatments()
		{
		}


		public static void CopyInvestmentID( String strSimulationIDToCopy )
		{
			m_strCopyInvestmentSimulationID = strSimulationIDToCopy;
		}

		public static void CopyPerformanceID( String strSimulationIDToCopy )
		{
			m_strCopyPerformanceSimulationID = strSimulationIDToCopy;
		}

		public static void CopyTreatmentID( String strSimulationIDToCopy )
		{
			m_strCopyTreatmentSimulationID = strSimulationIDToCopy;
		}

		public static void CopyAnalysisID( String strSimulationIDToCopy )
		{
			m_strCopyAnalysisSimulationID = strSimulationIDToCopy;
		}

        public static void CopyCommittedID( String strSimulationIDToCopy)
        {
            m_strCopyCommittedSimulationID = strSimulationIDToCopy;
        }
       
		public static void PasteAnalysis( String strNewSimulation )
		{
			if( String.IsNullOrEmpty( m_strCopyAnalysisSimulationID ) )
				return;
			List<string> listInserts = new List<string>();

			String strSelect = "SELECT * FROM SIMULATIONS WHERE SIMULATIONID = " + m_strCopyAnalysisSimulationID;

			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strSelect );
				if( ds.Tables[0].Rows.Count != 1 )
					return;

				DataRow dr = ds.Tables[0].Rows[0];


				String strComments = dr["COMMENTS"].ToString();
				String strDateCreated = dr["DATE_CREATED"].ToString();
				String strDateLastRun = dr["DATE_LAST_RUN"].ToString();
				String strCreatorID = dr["CREATOR_ID"].ToString();
				String strUserName = dr["USERNAME"].ToString();
				String strPermission = dr["PERMISSIONS"].ToString();
				String strJurisdiction = dr["JURISDICTION"].ToString();
				String strAnalysis = dr["ANALYSIS"].ToString();
				String strBudgetConstraint = dr["BUDGET_CONSTRAINT"].ToString();
				String strWeighting = dr["WEIGHTING"].ToString();
				String strBenefitVariable = dr["BENEFIT_VARIABLE"].ToString();
				String strBenefitLimit = dr["BENEFIT_LIMIT"].ToString();
				String strCommittedStart = dr["COMMITTED_START"].ToString();
				String strCommittedPeriod = dr["COMMITTED_PERIOD"].ToString();
				String strSimulationVariable = dr["SIMULATION_VARIABLES"].ToString();

				String strUpdate = "UPDATE SIMULATIONS SET ";

				if( strComments != "" )
					strUpdate += "COMMENTS" + "='" + strComments + "',";
				if( strDateCreated != "" )
				{
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							strUpdate += "DATE_CREATED" + "='" + strDateCreated + "',";
							break;
						case "ORACLE":
							strUpdate += "DATE_CREATED=to_date('" + DateTime.Parse( strDateCreated ).ToString( "MM/dd/yyyy" ) + "','MM/DD/YYYY'),";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI version of PasteAnalysis()" );
							//break;
					}
				}
				if( strDateLastRun != "" )
					strUpdate += "DATE_LAST_RUN" + "='" + strDateLastRun + "',";
				if( strCreatorID != "" )
					strUpdate += "CREATOR_ID" + "='" + strCreatorID + "',";
				if( strUserName != "" )
					strUpdate += "USERNAME" + "='" + strUserName + "',";
				if( strPermission != "" )
					strUpdate += "PERMISSIONS" + "='" + strPermission + "',";
				if( strJurisdiction != "" )
					strUpdate += "JURISDICTION" + "='" + strJurisdiction + "',";
				if( strAnalysis != "" )
					strUpdate += "ANALYSIS" + "='" + strAnalysis + "',";
				if( strBudgetConstraint != "" )
					strUpdate += "BUDGET_CONSTRAINT" + "='" + strBudgetConstraint + "',";
				if( strWeighting != "" )
					strUpdate += "WEIGHTING" + "='" + strWeighting + "',";
				if( strBenefitVariable != "" )
					strUpdate += "BENEFIT_VARIABLE" + "='" + strBenefitVariable + "',";
				if( strBenefitLimit != "" )
					strUpdate += "BENEFIT_LIMIT" + "='" + strBenefitLimit + "',";
				if( strBenefitLimit != "" )
                    strUpdate += "COMMITTED_START" + "='" + strCommittedStart + "',";
				if( strCommittedPeriod != "" )
					strUpdate += "COMMITTED_PERIOD" + "='" + strCommittedPeriod + "',";
				if( strSimulationVariable != "" )
					strUpdate += "SIMULATION_VARIABLES" + "='" + strSimulationVariable + "',";


				if( strUpdate.Substring( strUpdate.Length - 1 ) == "," )
				{
					strUpdate = strUpdate.Substring( 0, strUpdate.Length - 1 );
					strUpdate += " WHERE SIMULATIONID=" + strNewSimulation;
					listInserts.Add( strUpdate );
				}

			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Copying ANALYSIS information." + exception.Message );
				return;

			}


			//Get TARGETS
			String strDelete = "DELETE FROM TARGETS WHERE SIMULATIONID = " + strNewSimulation;
			listInserts.Add( strDelete );


			strSelect = "SELECT * FROM TARGETS WHERE SIMULATIONID = " + m_strCopyAnalysisSimulationID;
			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strSelect );
				foreach( DataRow dr in ds.Tables[0].Rows )
				{
					String strAttribute = dr["ATTRIBUTE_"].ToString();
					String strYears = dr["YEARS"].ToString();
					String strTargetMean = dr["TARGETMEAN"].ToString();
					String strTargetName = dr["TARGETNAME"].ToString();
					String strCriteria = dr["CRITERIA"].ToString();

					String strInsert = "INSERT INTO TARGETS (SIMULATIONID,ATTRIBUTE_,";
					String strValue = "VALUES(" + strNewSimulation + ",'" + strAttribute + "',";

					if( !String.IsNullOrEmpty( strYears ) )
					{
						strInsert += "YEARS,";
						strValue += "'" + strYears + "',";
					}

					if( !String.IsNullOrEmpty( strTargetMean ) )
					{
						strInsert += "TARGETMEAN,";
						strValue += "'" + strTargetMean + "',";
					}


					if( !String.IsNullOrEmpty( strTargetName ) )
					{
						strInsert += "TARGETNAME,";
						strValue += "'" + strTargetName + "',";
					}

					if( !String.IsNullOrEmpty( strCriteria ) )
					{
						strInsert += "CRITERIA,";
						strValue += "'" + strCriteria + "',";
					}

					strInsert = strInsert.Substring( 0, strInsert.Length - 1 ) + ") ";
					strValue = strValue.Substring( 0, strValue.Length - 1 ) + ")";

					strInsert += strValue;

					listInserts.Add( strInsert );
				}
			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Pasting TARGETS.  Operation aborted." + exception.Message );
				return;
			}




			//Get DEFICIENT
			strDelete = "DELETE FROM DEFICIENTS WHERE SIMULATIONID = " + strNewSimulation;
			listInserts.Add( strDelete );


			strSelect = "SELECT * FROM DEFICIENTS WHERE SIMULATIONID = " + m_strCopyAnalysisSimulationID;
			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strSelect );
				foreach( DataRow dr in ds.Tables[0].Rows )
				{
					String strAttribute = dr["ATTRIBUTE_"].ToString();
					String strDeficientName = dr["DEFICIENTNAME"].ToString();
					String strDeficient = dr["DEFICIENT"].ToString();
					String strPercentDeficient = dr["PERCENTDEFICIENT"].ToString();
					String strCriteria = dr["CRITERIA"].ToString();

					String strInsert = "INSERT INTO DEFICIENTS (SIMULATIONID,ATTRIBUTE_,";
					String strValue = "VALUES(" + strNewSimulation + ",'" + strAttribute + "',";

					if( !String.IsNullOrEmpty( strDeficientName ) )
					{
						strInsert += "DEFICIENTNAME,";
						strValue += "'" + strDeficientName + "',";
					}

					if( !String.IsNullOrEmpty( strDeficient ) )
					{
						strInsert += "DEFICIENT,";
						strValue += "'" + strDeficient + "',";
					}


					if( !String.IsNullOrEmpty( strPercentDeficient ) )
					{
						strInsert += "PERCENTDEFICIENT,";
						strValue += "'" + strPercentDeficient + "',";
					}

					if( !String.IsNullOrEmpty( strCriteria ) )
					{
						strInsert += "CRITERIA,";
						strValue += "'" + strCriteria + "',";
					}

					strInsert = strInsert.Substring( 0, strInsert.Length - 1 ) + ") ";
					strValue = strValue.Substring( 0, strValue.Length - 1 ) + ")";

					strInsert += strValue;

					listInserts.Add( strInsert );
				}
			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Pasting DEFICIENT.  Operation aborted." + exception.Message );
				return;
			}


			strDelete = "DELETE FROM PRIORITY WHERE SIMULATIONID = " + strNewSimulation;
			listInserts.Add( strDelete );

			//Run as much of this that can be batch in one try.
			try
			{
				DBMgr.ExecuteBatchNonQuery( listInserts );
			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Copying ANALYSIS. Operation aborted." + exception.Message );
			}


			//Now get PRIORITY and PRIORITYFUND

			strSelect = "SELECT * FROM PRIORITY WHERE SIMULATIONID=" + m_strCopyAnalysisSimulationID;

			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strSelect );
				foreach( DataRow dr in ds.Tables[0].Rows )
				{
					String strPriorityID = dr["PRIORITYID"].ToString();
					String strPriorityLevel = dr["PRIORITYLEVEL"].ToString();
					String strCriteria = dr["CRITERIA"].ToString();
                    string years = null;

                    if (dr["YEARS"] != DBNull.Value)
                    {
                        years = dr["YEARS"].ToString();
                    }




					String strInsert;
                    if (years == null)
                    {
                        if (String.IsNullOrEmpty(strCriteria))
                        {
                            strInsert = "INSERT INTO PRIORITY (SIMULATIONID,PRIORITYLEVEL) VALUES(" + strNewSimulation + "," + strPriorityLevel + ")";
                        }
                        else
                        {
                            strInsert = "INSERT INTO PRIORITY (SIMULATIONID,PRIORITYLEVEL,CRITERIA) VALUES(" + strNewSimulation + "," + strPriorityLevel + ",'" + strCriteria + "')";
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(strCriteria))
                        {
                            strInsert = "INSERT INTO PRIORITY (SIMULATIONID,PRIORITYLEVEL,YEARS) VALUES(" + strNewSimulation + "," + strPriorityLevel + ",'" + years + "')";
                        }
                        else
                        {
                            strInsert = "INSERT INTO PRIORITY (SIMULATIONID,PRIORITYLEVEL,CRITERIA,YEARS) VALUES(" + strNewSimulation + "," + strPriorityLevel + ",'" + strCriteria + "','" + years + "')";
                        }
                    }

					DBMgr.ExecuteNonQuery( strInsert );
					String strIdentity;
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							strIdentity = "SELECT IDENT_CURRENT ('PRIORITY') FROM PRIORITY";
							break;
						case "ORACLE":
							//strIdentity = "SELECT PRIORITY_PRIORITYID_SEQ.CURRVAL FROM DUAL";
							//strIdentity = "SELECT SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'PRIORITY_PRIORITYID_SEQ'";
							strIdentity = "SELECT MAX(PRIORITYID) FROM PRIORITY";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
							//break;
					}

					DataSet dsIdentity = DBMgr.ExecuteQuery( strIdentity );
					strIdentity = dsIdentity.Tables[0].Rows[0].ItemArray[0].ToString();



                  
                    
                    
                    
                    strSelect = "SELECT * FROM PRIORITYFUND WHERE PRIORITYID =" + strPriorityID;
					DataSet dsPriorityFund = DBMgr.ExecuteQuery( strSelect );
					foreach( DataRow drPriorityFund in dsPriorityFund.Tables[0].Rows )
					{
						String strBudget = drPriorityFund["BUDGET"].ToString();
						String strFunding = drPriorityFund["FUNDING"].ToString();

						strInsert = "INSERT INTO PRIORITYFUND (PRIORITYID,BUDGET,FUNDING) VALUES(" + strIdentity + ",'" + strBudget + "'," + strFunding + ")";
						DBMgr.ExecuteNonQuery( strInsert );
					}
				}

			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Failure in copying Priority Levels and Funding.  Rest of Analysis copied correctly." + exception.Message );
			}

            try
            {
                var delete = "DELETE FROM REMAINING_LIFE_LIMITS WHERE SIMULATION_ID='" + strNewSimulation + "'";
                DBMgr.ExecuteNonQuery(delete);

            }
            catch (Exception e)
            {
                Global.WriteOutput("Error: Failure in copying Remaining Life Limits. " + e.Message);
            }


            try
            {
                DataSet datasetRemainingLife =
                    DBMgr.ExecuteQuery("SELECT * FROM REMAINING_LIFE_LIMITS WHERE SIMULATION_ID ='" +
                                       m_strCopyAnalysisSimulationID + "'");
                foreach (DataRow dr in datasetRemainingLife.Tables[0].Rows)
                {
                    var remainingLifeLimit = dr["REMAINING_LIFE_LIMIT"].ToString();
                    var attribute = dr["ATTRIBUTE_"].ToString();
                    var criteria = "";
                    if (dr["CRITERIA"] != DBNull.Value) criteria = dr["CRITERIA"].ToString();

                    var insert =
                        "INSERT INTO REMAINING_LIFE_LIMITS (SIMULATION_ID,ATTRIBUTE_,REMAINING_LIFE_LIMIT,CRITERIA) VALUES ('"
                        + strNewSimulation + "','" + attribute + "','" + remainingLifeLimit + "','" +
                        criteria + "')";

                    DBMgr.ExecuteNonQuery(insert);

                }
            }
            catch(Exception e)
            {
                Global.WriteOutput("Error: Failure in copying Remaining Life Limits. " + e.Message);
            }
        }

        public static void PasteInvestment( String strNewSimulationID )
		{
			if( String.IsNullOrEmpty( m_strCopyInvestmentSimulationID ) )
				return;

			List<string> listInserts = new List<string>();

			//Delete existing SIMULATIONID FROM 
			var deleteInvestments  = "DELETE FROM INVESTMENTS WHERE SIMULATIONID=" + strNewSimulationID;
			listInserts.Add(deleteInvestments);

			var deleteYearlyInvestment = "DELETE FROM YEARLYINVESTMENT WHERE SIMULATIONID=" + strNewSimulationID;
			listInserts.Add(deleteYearlyInvestment);

            var delete = "DELETE FROM BUDGET_CRITERIA WHERE SIMULATIONID=" + strNewSimulationID;
            listInserts.Add(delete);




			String strSelect = "SELECT * FROM INVESTMENTS WHERE SIMULATIONID=" + m_strCopyInvestmentSimulationID;

			String strSimulationID;
			String strFirstYear;
			String strNumberYear;
			String strInflation;
			String strDiscount;
			String strBudgetOrder;
            String strDescription;

			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strSelect );
				if( ds.Tables[0].Rows.Count == 1 )
				{
					DataRow dr = ds.Tables[0].Rows[0];
					strSimulationID = dr["SIMULATIONID"].ToString();
					strFirstYear = dr["FIRSTYEAR"].ToString();
					strNumberYear = dr["NUMBERYEARS"].ToString();
					strInflation = dr["INFLATIONRATE"].ToString();
					strDiscount = dr["DISCOUNTRATE"].ToString();
					strBudgetOrder = dr["BUDGETORDER"].ToString();
                    strDescription = dr["DESCRIPTION"].ToString();

                    String strInsert = "INSERT INTO INVESTMENTS (SIMULATIONID,FIRSTYEAR,NUMBERYEARS,INFLATIONRATE,DISCOUNTRATE,BUDGETORDER,DESCRIPTION) VALUES (" + strNewSimulationID + "," + strFirstYear + "," + strNumberYear + "," + strInflation + "," + strDiscount + ",'" + strBudgetOrder + ",'" + strDescription + "')";
					listInserts.Add( strInsert );
				}
				else
				{
					return;
				}
			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Retrieving existing Investment Information." + exception.Message );
				return;
			}


			strSelect = "SELECT * FROM YEARLYINVESTMENT  WHERE SIMULATIONID=" + m_strCopyInvestmentSimulationID;

			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strSelect );
				foreach( DataRow dr in ds.Tables[0].Rows )
				{
					String strYear = dr["YEAR_"].ToString();
					String strBudget = dr["BUDGETNAME"].ToString();
					String strAmount = dr["AMOUNT"].ToString();

					String strInsert = "INSERT INTO YEARLYINVESTMENT (SIMULATIONID,YEAR_,BUDGETNAME,AMOUNT) VALUES (" + strNewSimulationID + "," + strYear + ",'" + strBudget + "'," + strAmount + ")";
					listInserts.Add( strInsert );
				}
			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Retrieving existing YearlyInvestment Information." + exception.Message );
				return;
			}




            var selectBudgetCriteria = "SELECT * FROM BUDGET_CRITERIA  WHERE SIMULATIONID=" + m_strCopyInvestmentSimulationID;

            try
            {
                DataSet ds = DBMgr.ExecuteQuery(selectBudgetCriteria);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var budgetName = dr["BUDGET_NAME"].ToString();
                    var criteria = dr["CRITERIA"].ToString();
    
                    String strInsert = "INSERT INTO BUDGET_CRITERIA (SIMULATIONID,BUDGET_NAME,CRITERIA) VALUES ('" + strNewSimulationID + "','" + budgetName + "','" + criteria + "')";
                    listInserts.Add(strInsert);
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Retrieving existing YearlyInvestment Information." + exception.Message);
                return;
            }





            try
            {
				DBMgr.ExecuteBatchNonQuery( listInserts );
                Global.WriteOutput("Investment and Budget Criteria successfully copied.");
			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Copying Investments from one simulation to another" + exception.Message );
			}


		}

		public static void PastePerformance( String strNewSimulationID )
		{

			if( String.IsNullOrEmpty( m_strCopyPerformanceSimulationID ) )
				return;

			List<string> listInserts = new List<string>();

			//Delete existing SIMULATIONID FROM 
			String strDelete = "DELETE FROM PERFORMANCE WHERE SIMULATIONID=" + strNewSimulationID;
			listInserts.Add( strDelete );



            String strSelect = "SELECT * FROM PERFORMANCE WHERE SIMULATIONID=" + m_strCopyPerformanceSimulationID;
			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strSelect );
				foreach( DataRow dr in ds.Tables[0].Rows )
				{
					String strAttribute = dr["ATTRIBUTE_"].ToString();
					String strEquationName = dr["EQUATIONNAME"].ToString();
					String strCritera = dr["CRITERIA"].ToString();
					String strEquations = dr["EQUATION"].ToString();
					String strShift = dr["SHIFT"].ToString();
                    string strPiecewise = dr["PIECEWISE"].ToString();
                    string isFunction = "0";
                    bool bIsFunction = false;
                    if (dr["ISFUNCTION"] != DBNull.Value) bIsFunction = Convert.ToBoolean(dr["ISFUNCTION"]);
                    if (bIsFunction) isFunction = "1";
    				String strInsert = "";
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
                            strInsert = "INSERT INTO PERFORMANCE (SIMULATIONID,ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,SHIFT,PIECEWISE,ISFUNCTION)VALUES(" + strNewSimulationID + ",'" + strAttribute + "','" + strEquationName + "','" + strCritera + "','" + strEquations + "','" + strShift + "','" + strPiecewise + "','" + isFunction + "')";
							break;
						case "ORACLE":
                            strInsert = "INSERT INTO PERFORMANCE (SIMULATIONID,ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,SHIFT,PIECEWISE,ISFUNCTION)VALUES(" + strNewSimulationID + ",'" + strAttribute + "','" + strEquationName + "','" + strCritera + "','" + strEquations + "','" + strShift + "','" + strPiecewise + "','" + isFunction + "')";
							break;
						default:
							throw new NotImplementedException( "TODO: Implement ANSI version of PastePerformance()" );
							//break;
					}
                    listInserts.Add(strInsert);
				}


			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Copy PERFORMANCE table." + exception.Message );
				return;
			}

            try
            {
                DBMgr.ExecuteBatchNonQuery(listInserts);
                Global.WriteOutput("Performance equations successfully copied.");
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Copying Performance equations from one simulation to another" + exception.Message);
            }
        }

		public static void PasteTreatment( String strNewSimulationID )
		{

			if( String.IsNullOrEmpty( m_strCopyTreatmentSimulationID ) )
				return;

			//Delete existing SIMULATIONID from TREATMENT
			String strDelete = "DELETE FROM TREATMENTS WHERE SIMULATIONID=" + strNewSimulationID;

			try
			{
				DBMgr.ExecuteNonQuery( strDelete );
			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Overwriting existig treatment." + exception.Message );
				return;
			}


            var copyFromTreatmentHash = new Hashtable();
			String strSelect = "SELECT * FROM TREATMENTS WHERE SIMULATIONID=" + m_strCopyTreatmentSimulationID;
			DataSet ds = DBMgr.ExecuteQuery( strSelect );
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String strTreatmentID = dr["TREATMENTID"].ToString();
                String strTreatment = dr["TREATMENT"].ToString();
                String strBeforeAny = dr["BEFOREANY"].ToString();
                String strBeforeSame = dr["BEFORESAME"].ToString();
                String strBudget = dr["BUDGET"].ToString();
                String strDescription = dr["DESCRIPTION"].ToString();


                Global.WriteOutput("Pasting treatment [" + strTreatment + "]");

                String strInsert;
                if (strBudget.Trim() == "")
                {
                    strInsert =
                        "INSERT INTO TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,DESCRIPTION)VALUES(" +
                        strNewSimulationID + ",'" + strTreatment + "'," + strBeforeAny + "," + strBeforeSame + ",'" +
                        strDescription + "')";
                }
                else
                {
                    strInsert =
                        "INSERT INTO TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,BUDGET,DESCRIPTION)VALUES(" +
                        strNewSimulationID + ",'" + strTreatment + "'," + strBeforeAny + "," + strBeforeSame + ",'" +
                        strBudget + "','" + strDescription + "')";
                }

                DBMgr.ExecuteNonQuery(strInsert);
                String strIdentity;
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        strIdentity = "SELECT IDENT_CURRENT ('TREATMENTS') FROM TREATMENTS";
                        break;
                    case "ORACLE":
                        //strIdentity = "SELECT TREATMENTS_TREATMENTID_SEQ.CURRVAL FROM DUAL";
                        //strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE + 1 FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'TREATMENTS_TREATMENTID_SEQ'";
                        strIdentity = "SELECT MAX(TREATMENTID) FROM TREATMENTS";
                        break;
                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
                }

                DataSet dsIdentity = DBMgr.ExecuteQuery(strIdentity);
                strIdentity = dsIdentity.Tables[0].Rows[0].ItemArray[0].ToString();

                copyFromTreatmentHash.Add(strTreatmentID, strIdentity);


                Global.WriteOutput("Pasting Feasibility");

                DataSet dsFeasibility =
                    DBMgr.ExecuteQuery("SELECT * FROM FEASIBILITY WHERE TREATMENTID=" + strTreatmentID);
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        SQLCopyFeasibility(strIdentity, dsFeasibility);
                        break;
                    case "ORACLE":
                        ORACLECopyFeasibility(strIdentity, dsFeasibility);
                        break;
                    default:
                        throw new NotImplementedException("TODO: implement ANSICopyConsequences()");
                    //break;
                }

                Global.WriteOutput("Pasting Costs");

                DataSet dsCosts = DBMgr.ExecuteQuery("SELECT * FROM COSTS WHERE TREATMENTID = " + strTreatmentID);
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        SQLCopyCosts(strIdentity, dsCosts);
                        break;
                    case "ORACLE":
                        ORACLECopyCosts(strIdentity, dsCosts);
                        break;
                    default:
                        throw new NotImplementedException("TODO: implement ANSICopyConsequences()");
                    //break;
                }


                Global.WriteOutput("Pasting Consequences");

                DataSet dsConsequences =
                    DBMgr.ExecuteQuery("SELECT * FROM CONSEQUENCES WHERE TREATMENTID = " + strTreatmentID);
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        SQLCopyConsequences(strIdentity, dsConsequences);
                        break;
                    case "ORACLE":
                        ORACLECopyConsequences(strIdentity, dsConsequences);
                        break;
                    default:
                        throw new NotImplementedException("TODO: implement ANSICopyConsequences()");
                    //break;
                }
            }

            Global.WriteOutput("Pasting Scheduled and Supersedes for all treatments");
            foreach (string key in copyFromTreatmentHash.Keys)
            { 



                DataSet dataSetScheduled = DBMgr.ExecuteQuery("SELECT * FROM SCHEDULED WHERE TREATMENTID = " + key);
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        SQLCopyScheduled(key, dataSetScheduled,copyFromTreatmentHash);
                        break;
                    case "ORACLE":
                        ORACLECopyScheduled(key, dataSetScheduled);
                        break;
                    default:
                        throw new NotImplementedException("TODO: implement ANSICopyConsequences()");
                    //break;
                }


                DataSet dataSetSupersedes = DBMgr.ExecuteQuery("SELECT * FROM SUPERSEDES WHERE TREATMENT_ID = " + key);
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        SQLCopySupersedes(key, dataSetSupersedes,copyFromTreatmentHash);
                        break;
                    case "ORACLE":
                        ORACLECopySupersedes(key, dataSetSupersedes);
                        break;
                    default:
                        throw new NotImplementedException("TODO: implement ANSICopyConsequences()");
                    //break;
                }
            }
		}

        private static void ORACLECopySupersedes(string strIdentity, DataSet dataSetSupersedes)
        {
            Global.WriteOutput("ERROR: Oracle Supersedes Copy is not implemented.");
        }

        private static void SQLCopySupersedes(string oldTreatmentId, DataSet dataSetSupersedes, Hashtable copyFromTreatmentHash)
        {
            foreach (DataRow row in dataSetSupersedes.Tables[0].Rows)
            {
                var criteria = "";
                var supersedeTreatmentId = row["SUPERSEDE_TREATMENT_ID"].ToString();



                if (row["CRITERIA"] != DBNull.Value) criteria = row["CRITERIA"].ToString();

                var insert = "INSERT INTO SUPERSEDES (TREATMENT_ID, SUPERSEDE_TREATMENT_ID, CRITERIA) VALUES ('" +
                             copyFromTreatmentHash[oldTreatmentId]  + "','" + copyFromTreatmentHash[supersedeTreatmentId] + "','" + criteria + "')";

                DBMgr.ExecuteNonQuery(insert);

            }
        }

        private static void ORACLECopyScheduled(string strIdentity, DataSet dataScheduled)
        {
            Global.WriteOutput("ERROR: Oracle Scheduled Copy is not implemented.");
        }

        private static void SQLCopyScheduled(string oldTreatmentId, DataSet dataScheduled, Hashtable copyFromTreatmentHash)
        {
            foreach (DataRow row in dataScheduled.Tables[0].Rows)
            {
                var scheduledTreatmentId = row["SCHEDULEDTREATMENTID"].ToString();
                var scheduledYear = row["SCHEDULEDYEAR"].ToString();

                var insert = "INSERT INTO SCHEDULED (TREATMENTID, SCHEDULEDTREATMENTID, SCHEDULEDYEAR) VALUES ('" +
                             copyFromTreatmentHash[oldTreatmentId] + "','" + copyFromTreatmentHash[scheduledTreatmentId] + "','" + scheduledYear + "')";

                DBMgr.ExecuteNonQuery(insert);

            }
        }

        private static void ORACLECopyCosts( string identity, DataSet costSet )
		{
			string insertStatement = "";

			foreach( DataRow costRow in costSet.Tables[0].Rows )
			{
				string criteriaEquation = costRow["CRITERIA"].ToString();
				string costEquation = costRow["COST_"].ToString();
				string valuesClause = "VALUES(" + identity;
				string fieldsClause = "(TREATMENTID";

                string isFunction = "0";
                Boolean bIsFunction = false;
                if (costRow["ISFUNCTION"] != DBNull.Value) bIsFunction = Convert.ToBoolean(costRow["ISFUNCTION"]);
                if (bIsFunction) isFunction = "1";


				if( !String.IsNullOrEmpty( costEquation ) )
				{
					fieldsClause += ",COST_";
					valuesClause += ",'" + costEquation + "'";
				}

                valuesClause += ",'" + isFunction + "'";
                fieldsClause += ",ISFUNCTION";

				fieldsClause += ",CRITERIA";
				valuesClause += ",'" + criteriaEquation + "'";

				fieldsClause += ")";
				valuesClause += ")";

				insertStatement = "INSERT INTO COSTS " + fieldsClause + valuesClause;
				DBMgr.ExecuteNonQuery( insertStatement);
			}
		}

		private static void SQLCopyCosts( string strIdentity, DataSet dsCosts )
		{
			String strInsert;
			foreach( DataRow drCosts in dsCosts.Tables[0].Rows )
			{

				String strCriteria = drCosts["CRITERIA"].ToString();
				String strCost = drCosts["COST_"].ToString();

				String strValues = "VALUES(" + strIdentity;
				String strFields = "(TREATMENTID";

                string isFunction = "0";
                Boolean bIsFunction = false;
                if (drCosts["ISFUNCTION"] != DBNull.Value) bIsFunction = Convert.ToBoolean(drCosts["ISFUNCTION"]);
                if (bIsFunction) isFunction = "1";


				if( !String.IsNullOrEmpty( strCost ) )
				{
					strValues += ",'" + strCost + "'";
					strFields += ",COST_";
				}

                strValues += ",'" + isFunction + "'";
                strFields += ",ISFUNCTION";

				strValues += ",'" + strCriteria + "'";
				strFields += ",CRITERIA";

				strValues += ")";
				strFields += ")";

				strInsert = "INSERT INTO COSTS " + strFields + strValues;
				SqlCommand command = new SqlCommand( strInsert, DBMgr.NativeConnectionParameters.SqlConnection );
				command.ExecuteNonQuery();
			}
		}

		private static void ORACLECopyFeasibility( string identity, DataSet feaSet )
		{
			string insertStatement = "";
			foreach( DataRow feaRow in feaSet.Tables[0].Rows )
			{
				string criteriaEquation = feaRow["CRITERIA"].ToString();
    			insertStatement = "INSERT INTO FEASIBILITY (TREATMENTID, CRITERIA) VALUES(" + identity + ",'"+ criteriaEquation + "')";
				DBMgr.ExecuteNonQuery( insertStatement );
			}
		}

		private static void SQLCopyFeasibility( string strIdentity, DataSet dsFeasibility )
		{
			String strInsert;
			foreach( DataRow drFeasibility in dsFeasibility.Tables[0].Rows )
			{
				String strCriteria = drFeasibility["CRITERIA"].ToString();
				strInsert = "INSERT INTO FEASIBILITY (TREATMENTID, CRITERIA) VALUES(" + strIdentity + ",'" + strCriteria + "')";
					DBMgr.ExecuteNonQuery( strInsert );
			}
		}

		private static void SQLCopyConsequences( string strIdentity, DataSet dsConsequences )
		{
			foreach( DataRow drConsequences in dsConsequences.Tables[0].Rows )
			{

				String strCriteria = drConsequences["CRITERIA"].ToString();
				String strAttribute = drConsequences["ATTRIBUTE_"].ToString();
				String strChange = drConsequences["CHANGE_"].ToString();
                String strEquation = drConsequences["EQUATION"].ToString();

                string isFunction = "0";
                Boolean bIsFunction = false;
                if (drConsequences["ISFUNCTION"] != DBNull.Value)
                {
                    bIsFunction = Convert.ToBoolean(drConsequences["ISFUNCTION"]);
                }
                if (bIsFunction)
                {
                    isFunction = "1";
                }

				String strValues = "VALUES(" + strIdentity;
				String strFields = "(TREATMENTID";

				if( strAttribute != "" )
				{
					strValues += ",'" + strAttribute + "'";
					strFields += ",ATTRIBUTE_";
				}

				if( strChange != "" )
				{
					strValues += ",'" + strChange + "'";
					strFields += ",CHANGE_";
				}

                if (!string.IsNullOrWhiteSpace(strEquation))
                {
                    strValues += ",'" + strEquation + "'";
                    strFields += ",EQUATION";
                }

                strValues += ",'" + isFunction + "'";
                strFields += ",ISFUNCTION";

                strValues += ",'" + strCriteria + "'";
				strFields += ",CRITERIA";
                
                strValues += ")";
				strFields += ")";

                string strInsert = "INSERT INTO CONSEQUENCES " + strFields + strValues;
				SqlCommand command = new SqlCommand( strInsert, DBMgr.NativeConnectionParameters.SqlConnection );
				command.ExecuteNonQuery();
			}
		}

		private static void ORACLECopyConsequences( string identity, DataSet conSet )
		{
			foreach( DataRow conRow in conSet.Tables[0].Rows )
			{
				string criteriaFormula = conRow["CRITERIA"].ToString();
				string attribute = conRow["ATTRIBUTE_"].ToString();
				string change = conRow["CHANGE_"].ToString();
                string strEquation = conRow["EQUATION"].ToString();

                string isFunction = "0";
                Boolean bIsFunction = false;
                if (conRow["ISFUNCTION"] != DBNull.Value) bIsFunction = Convert.ToBoolean(conRow["ISFUNCTION"]);
                if (bIsFunction) isFunction = "1";
                
                string valueClause = "VALUES(" + identity;
				string fieldClause = "(TREATMENTID";

				if( attribute != "" )
				{
					valueClause += ",'" + attribute + "'";
					fieldClause += ",ATTRIBUTE_";
				}

				if( change != "" )
				{
					valueClause += ",'" + change + "'";
					fieldClause += ",CHANGE_";
				}

                if (!string.IsNullOrWhiteSpace(strEquation))
                {
                    valueClause += ",'" + strEquation + "'";
                    fieldClause += ",EQUATION";
                }

                valueClause += ",'" + isFunction + "'";
                fieldClause += ",ISFUNCTION";

				valueClause += ",'" + criteriaFormula + "'";
				fieldClause += ",CRITERIA";

				valueClause += ")";
				fieldClause += ")";

				string insertStatement = "INSERT INTO CONSEQUENCES " + fieldClause + valueClause;
				try
				{
					DBMgr.ExecuteNonQuery( insertStatement);
				}
				catch( Exception ex )
				{
					throw ex;
				}
			}
		}


        public static void PasteCommitted(String simulationID)
        {
            if (String.IsNullOrEmpty(m_strCopyTreatmentSimulationID)) return;

            //Delete existing SIMULATIONID from TREATMENT
            String strDelete = "DELETE FROM COMMITTED_ WHERE SIMULATIONID=" + simulationID;

            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Overwriting existing committed projects." + exception.Message);
                return;
            }



            String strSelect = "SELECT * FROM COMMITTED_ WHERE SIMULATIONID=" + m_strCopyCommittedSimulationID;
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                int sectionID = Convert.ToInt32(dr["SECTIONID"]);
                int years = Convert.ToInt32(dr["YEARS"]);
                String treatmentName = dr["TREATMENTNAME"].ToString();
                int yearSame = Convert.ToInt32(dr["YEARSAME"]);
                int yearAny = Convert.ToInt32( dr["YEARANY"]);
                String budget = dr["BUDGET"].ToString();
                double cost = Convert.ToDouble(dr["COST_"]);
                int commitID = Convert.ToInt32(dr["COMMITID"]);

                String insert = "INSERT INTO COMMITTED_ (SIMULATIONID,SECTIONID,YEARS,TREATMENTNAME,YEARSAME,YEARANY, BUDGET,COST_) VALUES('" +
                    simulationID.ToString() + "','" +
                    sectionID.ToString() + "','" +
                    years.ToString() + "','" +
                    treatmentName + "','" +
                    yearSame.ToString() + "','" +
                    yearAny.ToString() + "','" +
                    budget + "','" +
                    cost.ToString() + "')";

                try
                {
                    DBMgr.ExecuteNonQuery(insert);
                }
                catch (Exception e)
                {
                    Global.WriteOutput("Error: copying committed projects." + e.Message);
                    break;
                }



                String strIdentity;
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        strIdentity = "SELECT TOP 1 IDENT_CURRENT ('COMMITTED_') FROM COMMITTED_";
                        break;
                    case "ORACLE":
                        //strIdentity = "SELECT TREATMENTS_TREATMENTID_SEQ.CURRVAL FROM DUAL";
                        //strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE + 1 FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'TREATMENTS_TREATMENTID_SEQ'";
                        strIdentity = "SELECT MAX(COMMITID) FROM COMMITTED_";
                        break;
                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
                }


           		DataSet dsIdentity = DBMgr.ExecuteQuery( strIdentity );
				strIdentity = dsIdentity.Tables[0].Rows[0].ItemArray[0].ToString();
                    
                strSelect = "SELECT * FROM COMMIT_CONSEQUENCES WHERE COMMITID =" + commitID;
				DataSet dsCommitConsequences = DBMgr.ExecuteQuery( strSelect );
                foreach (DataRow drCommitConsequences in dsCommitConsequences.Tables[0].Rows)
                {
                    String attribute = drCommitConsequences["ATTRIBUTE_"].ToString();
                    String change = drCommitConsequences["CHANGE_"].ToString();

                    insert = "INSERT INTO COMMIT_CONSEQUENCES (COMMITID,ATTRIBUTE_,CHANGE_) VALUES('" + strIdentity + "','" + attribute + "','" + change + "')";
                    try
                    {
                        DBMgr.ExecuteNonQuery(insert);
                    }
                    catch (Exception e)
                    {
                        Global.WriteOutput("Error: copying committed projects." + e.Message);
                        break;
                    }

                }
            }

            Global.WriteOutput("Cloning committed projects complete!");
        }
	}
}
