using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using DatabaseManager;

namespace DynamicSegmentation
{
    public class DynamicSegmentation
    {
        String m_strServer = "";
        String m_strDataSource = "";
        String m_strUserID = "";
        String m_strPassword = "";
        String m_strNetworkID = "";
        List<String> m_listRoutes; //List of Route|Direction - To keep original order.
        Hashtable m_hashRoutes; //Key = Route|Direction   Object is list of ordered break points.
        Hashtable m_hashBreaks; //Key = Route|Direction   Object is hashtable of <float>milepost, <string>
        Hashtable m_hashLogic;  //Key = FamilyName  Object = strLogic
        Hashtable m_hashYears;  //Key = Attribute   Object = list years
        Hashtable m_hashNative; //Key = Attribute   Object = bool TRUE if native
        Hashtable m_hashBegin; //Key = Route|Direction  Object float begin
        Hashtable m_hashEnd;   //Key = Route|Direction  Object float end

		public DynamicSegmentation(String server, String database, String userID, String password, String provider, bool bIntegratedSecurity, string SID, string serviceName, string port)
        {
            m_strServer = server;
            m_strDataSource = database;
			m_strUserID = userID;
			m_strPassword = password;
			DBMgr.NativeConnectionParameters = new ConnectionParameters(port, SID, serviceName, userID, password, bIntegratedSecurity, server, database, "", "", "", "", provider, true);
        }
        public DynamicSegmentation()
        {
        }
    
        /// <summary>
        /// Performs dynamic segmentation of RoadCare (or attached) raw data for Linear referenced system.
        /// </summary>
        public void DoSegmentation(String strNetwork)
        {
            //Version 3.0 - Eliminate concept of overlap sections.  Just put in extra breaks.  Let rollup handle it.
            //Version 3.0 - Eliminate concepts of missing sections.  Missing data will be filled in OR NULLEd in roll up.

            
            
            //Create DatabaseManager

            //New the hash tables that are storing breaks and reasons.

            //Will create Hashtable with key of Route|Direction
            //                           object of List<float> mile post which contains breaks Always use 3 decimal
            //                           Only insert in this list if it does not exists.
            //                           
            //Create second hashtable  key Route|Direction
            //                         object is Hashtable  key MilePost - Object String of what caused break, appended.
           
            
            m_hashRoutes = new Hashtable();
            m_hashBreaks = new Hashtable();
            m_hashBegin = new Hashtable();
            m_hashEnd = new Hashtable();
            m_listRoutes = new List<String>();
 
            //Connect to RoadCare SQL base
            //if(m_strServer != "")
			//	DBMgr.OpenConnection("SQL", m_strDataSource, m_strServer, m_strUserID, m_strPassword);

            //Get the NetworkID that goes with the input network
            String strNetworkID;
            DataSet ds = DBMgr.ExecuteQuery("SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME='" + strNetwork + "'");
            strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            m_strNetworkID = strNetworkID;
            ds.Dispose();
            
            
            //Routes to Dymanically Segment can be retrieved from RawAlias
			String strQuery;
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					strQuery = "SELECT ROUTES, BEGIN_STATION, END_STATION, DIRECTION FROM NETWORK_DEFINITION WHERE ROUTES <> '' ORDER BY ROUTES";
					break;
				case "ORACLE":
					strQuery = "SELECT ROUTES, BEGIN_STATION, END_STATION, DIRECTION FROM NETWORK_DEFINITION WHERE ROUTES LIKE '_%' ORDER BY ROUTES";
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
					
			}
            //SqlDataReader dr = DBMgr.CreateDataReader(strQuery);
            DataReader dr = new DataReader(strQuery);
			while (dr.Read())
			{
				String strKey = dr["ROUTES"].ToString() + "|" + dr["DIRECTION"].ToString();
				m_listRoutes.Add(strKey);
				//Stores a list of break points that go with a Route
				List<float> listBreaks = new List<float>();
				//Make sure break is at end or before beginning.
				listBreaks.Add(float.Parse(dr["BEGIN_STATION"].ToString()));
				listBreaks.Add(float.Parse(dr["END_STATION"].ToString()));
				m_hashRoutes.Add(strKey, listBreaks);
				m_hashBegin.Add(strKey, (float.Parse(dr["BEGIN_STATION"].ToString())));
				m_hashEnd.Add(strKey, (float.Parse(dr["END_STATION"].ToString())));

				//Every break is for a reason.  This adds break reasons for the Begin and End.
				Hashtable hashReason = new Hashtable();
				hashReason.Add(float.Parse(dr["BEGIN_STATION"].ToString()), "Route Begin");
				hashReason.Add(float.Parse(dr["END_STATION"].ToString()), "Route End");
				m_hashBreaks.Add(strKey, hashReason);
			}
            dr.Close();

            
            //Get Attributes included in Segmentation.  Search for NetworkID in NETWORKTREE.  Return NODENAME which used to lookup FAMILIES
            //This is an outerloop.  The rest of filling the break fields and breakpoints happens in this loop
            
            
            strQuery = "SELECT NODES FROM NETWORK_TREE WHERE NETWORKID ='" + strNetworkID + "'";
            DataSet dsNodeName = DBMgr.ExecuteQuery(strQuery);

            //Get Latest year for each attribute that is included in segmentation (save Constructuction History and Manual Break)
            //This is accomplished by getting the latest date in each Attribute (Default) or by selected.

            //Figure out what years to show user for Segmentation. 
            GetAttributeYears(dsNodeName);
            
            //TODO: C.Becker - Create Form to allow user to select which years to use.
            // FormSelectYears.Show
            // Modify m_hashYears to contain only years to be used.  Minium one year.  Should be in selection.
            // Temporarily use last year in list only.  Should use all years when proper.

            String strDelete = "DELETE FROM DYNAMIC_SEGMENTATION_RESULT WHERE NETWORKID ='" + strNetworkID + "'"; ;
            DBMgr.ExecuteNonQuery(strDelete);

            foreach (DataRow row in dsNodeName.Tables[0].Rows)
            {
                String strFamilyName  = row.ItemArray[0].ToString();//FamilyName is also known as the break field.
                
                //Need to search for | (pipe) which breaks up compound logical queries.
                Hashtable hashPoints = new Hashtable();

                string[] strFamily = strFamilyName.Split('|');
                //Loop through each 
                for(int i = strFamily.Length-1; i >= 0; i--)
                {
                    // If this is space i, one of two things is occurring.  This is a root only or the last compression.
                    String strLogic = m_hashLogic[strFamily[i].ToString()].ToString();
                    int nBegin = strLogic.IndexOf('[');
                    int nEnd = strLogic.IndexOf(']');
                    String strAttribute = strLogic.Substring(nBegin + 1, nEnd - nBegin - 1);

                    if(i == 0 && strFamily.Length == 1)//Root only. Add breaks.
                    {
                        // Choices here are 
                        // Attribute ANYCHANGE
                        // Construction History Change
                        // Select Statement
                        String strAnyChange = strLogic.ToUpper();
						String strAny;
						if (strAnyChange.Contains("ANYRECORD"))
						{
							strAny = "ANYRECORD";
						}
						else if (strAnyChange.Contains("ANYCHANGE"))
						{
							strAny = "ANYCHANGE";
						}
						else
						{
							strAny = "ANYYEAR";
						}
                        if (strAnyChange.Contains("ANYRECORD")||strAnyChange.Contains("ANYCHANGE") || strAnyChange.Contains("ANYYEAR"))
                        {
                            bool bAnyChange = true;
                            if (strAnyChange.Contains("ANYRECORD")) bAnyChange = false;
                            // Check Construction History.  Add to both hashRoute and hashBreak (AddBreaks)
                            //Select breaks in construction history - always RoadCareNative
							if (strAttribute == "Construction History")
							{
								//SqlDataReader dr1 = ConstructionBreak();
								DataReader dr1 = ConstructionBreak();
								if (bAnyChange) AddBreaksAnyChange(strFamily[i], dr1);
								else AddBreaksAnyRecord(strFamily[i], dr1);
								dr1.Close();
							}
							//Manual Breaks replace with a just a Attribute file with NULLS.  Multiple Breaks (Break1, Break2) could be added.
							else //All Attributes other than Construction History
							{
								DataReader dr1 = null;
								switch (strAny)
								{
									case "ANYCHANGE":
										dr1 = LogicalBreak(strAttribute, "");
										if (dr1 != null)
										{
											AddBreaksAnyChange(strFamily[i], dr1);
										}
										break;
									case "ANYRECORD":
										dr1 = LogicalBreak(strAttribute, "");
										if (dr1 != null)
										{
											AddBreaksAnyRecord(strFamily[i], dr1);
										}
										break;
									case "ANYYEAR":
										dr1 = LogicalBreakAllYears(strAttribute, "");
										if (dr1 != null)
										{
											AddBreaksAnyYear(strFamily[i], dr1);
										}
										break;
								}
								if (dr1 != null)
								{
									dr1.Close();
								}
							}
                        }
                        else
                        {
                            DataReader dr1 = LogicalBreak(strAttribute, strLogic);
							if (dr1 != null)
							{
								CombineThenAddBreaks(strFamily[i].ToString(), dr1);
                                dr1.Close();
							}
							else
							{
								SegmentationMessaging.AddMessage("Error: Dynamic Segmentation encountered an error. " + strFamily[i] + " Aborting operation.");
								return;
							}
                        }
                    }

                    // Multiple logical criteria to combine and merge
                    else if(strFamily.Length > 1) //Combine.  Add breaks.
                    {
                        if (IsNativeRoadCare(strAttribute))
                        {
                            strLogic = strLogic.Replace("|", "'");
                            DataReader dr1 = LogicalBreak(strAttribute, strLogic);
                           
							if (dr1 != null)
							{
								CombineFamilyPts(ref hashPoints, dr1);
                                dr1.Close();
							}
							else
							{
								SegmentationMessaging.AddMessage("Error: Dynamic Segmentation encountered an error. " + strLogic + " Aborting operation.");
								return;
							}

                        }
                        //Add hashPoints to hashBreak and hashRoutes
                        //
						if( i == 0 )
						{
							float fLong = 0;
							float fShort = 1000000;
							float fAverage = 0;
							int iCount = 0;
							float fDelta;

							//Loop through Routes
							foreach( String strRouteDir in m_listRoutes )
							{
								if( m_hashRoutes.Contains( strRouteDir ) && hashPoints.Contains( strRouteDir ) )
								{
									if( !m_hashBreaks.Contains( strRouteDir ) )
									{
										m_hashBreaks.Add( strRouteDir, new Hashtable() );
									}

									Hashtable hash = ( Hashtable )m_hashBreaks[strRouteDir];
									List<float> list = ( List<float> )m_hashRoutes[strRouteDir];
									List<PointF> pts = ( List<PointF> )hashPoints[strRouteDir];

									m_hashBreaks.Remove( strRouteDir );
									m_hashRoutes.Remove( strRouteDir );
									String strBreak;
									foreach( PointF pt in pts )
									{
										fDelta = pt.Y - pt.X;
										if( fDelta > fLong )
											fLong = fDelta;
										if( fDelta < fShort )
											fShort = fDelta;
										fAverage += fDelta;
										iCount++;

										if( !list.Contains( pt.X ) )
											list.Add( pt.X );
										if( !list.Contains( pt.Y ) )
											list.Add( pt.Y );


										if( hash.Contains( pt.X ) )
										{
											strBreak = hash[pt.X].ToString();
											hash.Remove( pt.X );
											if( !strBreak.Contains( strFamilyName.ToString() ) )
											{
												strBreak = strBreak + ";" + strFamilyName.ToString();
											}
										}
										else
										{
											strBreak = strFamilyName.ToString();
										}

										hash.Add( pt.X, strBreak );


										if( hash.Contains( pt.Y ) )
										{
											strBreak = hash[pt.Y].ToString();
											hash.Remove( pt.Y );
											if( !strBreak.Contains( strFamilyName.ToString() ) )
											{
												strBreak = strBreak + ";" + strFamilyName.ToString();
											}
										}
										else
										{
											strBreak = strFamilyName.ToString();
										}
										hash.Add( pt.Y, strBreak );
									}

									//Add hash and list back to m_hashBreak and m_hashRoute
									m_hashBreaks.Add( strRouteDir, hash );
									m_hashRoutes.Add( strRouteDir, list );

								}
							}
							fAverage = fAverage / ( float )iCount;
							String strInsert = "";
							switch( DBMgr.NativeConnectionParameters.Provider )
							{
								case "MSSQL":
									strInsert = "INSERT INTO DYNAMIC_SEGMENTATION_RESULT (NETWORKID,BREAKCAUSE,SHORTEST,LONGEST,AVERAGE,NUMBER_) VALUES ('"
										+ m_strNetworkID + "','"
										+ strFamily + "','"
										+ fShort.ToString( "#.###" ) + "','"
										+ fLong.ToString( "#.###" ) + "','"
										+ fAverage.ToString( "#.###" ) + "','"
										+ iCount.ToString() + "')";
									break;
								case "ORACLE":

									strInsert = "INSERT INTO DYNAMIC_SEGMENTATION_RESULT (NETWORKID,BREAKCAUSE,SHORTEST,LONGEST,AVERAGE,NUMBER_) VALUES ('"
										+ m_strNetworkID + "','"
										+ strFamily + "','"
										+ fShort.ToString( "f3" ) + "','"
										+ fLong.ToString( "f3" ) + "','"
										+ fAverage.ToString( "f3" ) + "','"
										+ iCount.ToString() + "')";
									break;
								default:
									throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
									
							}
							DBMgr.ExecuteNonQuery( strInsert );
						}
                    }
                }
            }
            dsNodeName.Dispose();


            //Throw away Hashtable entries that are not in Raw Alias.
            //SORT LISTS.
            // create a writer and open the file
			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\segment.txt";
            TextWriter tw = new StreamWriter(strOutFile);
//            String strHeader = "ID,NETWORKID,ROUTES,SECTION_BEGIN,SECTION_END,DIRECTION,BREAKCAUSE";
//            tw.WriteLine(strHeader);                    
            foreach(String strKey in m_listRoutes)
            {
                List<float> list = (List<float>)m_hashRoutes[strKey];
                list.Sort();
                
                //Remove breaks past end, or before beginning.
				
                float fRouteBegin = float.Parse(m_hashBegin[strKey].ToString());
                float fRouteEnd = float.Parse(m_hashEnd[strKey].ToString());

                //Create list of Begins and Ends for Sections
                List<PointF> pts= new List<PointF>();
                PointF pt = new PointF(-1,-1);
				float fTemp;   
                foreach(float f in list)
                {
					// Handles rounding error issues.
					fTemp = float.Parse(f.ToString());

                    //Only use sections between beginning and end.
					if (fTemp >= fRouteBegin && fTemp <= fRouteEnd)
                    {
                        if (pt.X >= 0) //Second and subsequent passes fill out the end, and the beginning of the next.
                        {

							if ((fTemp - pt.X) > 0.001)//Ignore sections if subsequent points are too close
                            {
								pt.Y = fTemp;
                                pts.Add(pt);
								pt = new PointF(fTemp, -1);
                            }
                            else
                            {
                                //TODO: COMBINE BREAK FIELDS
                            }
                        }
                        else // First pass that is in bounds, fill out the start.
                        {
							pt.X = fTemp;
                        }
                    }
                }
                
                //We now have list of Begin,End pairs.  The last PointF not complete.
                Hashtable hash = (Hashtable)m_hashBreaks[strKey];
                int nPipe = strKey.IndexOf('|');
                String strRoutes = strKey.Substring(0,nPipe);
                String strDir = strKey.Substring(nPipe + 1);
                foreach (PointF ptf in pts)
                {
                    //Write to a file

					String strOut = strNetworkID + "\t" + strRoutes + "\t" + ptf.X.ToString() + "\t" + ptf.Y.ToString() + "\t" + strDir + "\t" + hash[ptf.X] + "\t";
					//String strOut = strNetworkID + "\t" + strRoutes + "\t" + ptf.X.ToString() + "\t" + ptf.Y.ToString() + "\t" + strDir + "\t" + hash[ptf.X];
					tw.WriteLine(strOut);                    
                }
            }
            //Check to remove minimum tolerances. Combine break reasons into one.
            //Build segment.csv for Bulk Insert
            //While doing this calculate summary information.
            //Insert strings with 3 numbers to the right of decimal point.
            tw.Close();
            strDelete = "DELETE FROM DYNAMIC_SEGMENTATION WHERE NETWORKID ='" + strNetworkID + "'"; ;
            int n = DBMgr.ExecuteNonQuery(strDelete);

			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					DBMgr.SQLBulkLoad("DYNAMIC_SEGMENTATION", strOutFile, '\t');
					break;
				case "ORACLE":
					List<string> columnNames = new List<string>();
					columnNames.Add("NETWORKID");
					columnNames.Add("ROUTES");
					columnNames.Add("BEGIN_STATION");
					columnNames.Add("END_STATION");
					columnNames.Add("DIRECTION");
					columnNames.Add("BREAKCAUSE");
					DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, "DYNAMIC_SEGMENTATION", strOutFile, columnNames, "\\t");
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
			}
        }

        /// <summary>
        /// Gets the Years associated with each Attribute.  While doing this store the logic for each family
        /// </summary>
        /// <param name="ds"></param>
        /// <remarks>Fills two hashtables storing years for each Attribute or  logic for each Family</remarks>
        private void GetAttributeYears(DataSet ds)
        {
            m_hashLogic = new Hashtable();
            m_hashYears = new Hashtable();

            foreach(DataRow row in ds.Tables[0].Rows)
            {
                String strRow = row.ItemArray[0].ToString();
                string[] strLogical = strRow.Split('|');
                foreach(string str in strLogical)
                {
                    
                    String strFamily = str;
                    String strQuery = "SELECT FAMILY_EXPRESSION FROM CRITERIA_SEGMENT WHERE FAMILY_NAME ='" + strFamily + "'";
                    DataSet dsLogic = DBMgr.ExecuteQuery(strQuery);
                    if (dsLogic.Tables[0].Rows.Count == 1)
                    {
                        String strLogic = dsLogic.Tables[0].Rows[0].ItemArray[0].ToString();
						if (m_hashLogic.Contains(strFamily))
						{
							SegmentationMessaging.AddMessage("Error: Cannot add duplicate family name to segmentation tree.");
							return;
						}
						m_hashLogic.Add(strFamily, strLogic);
                        int nBegin = strLogic.IndexOf('[');
                        int nEnd = strLogic.IndexOf(']');
                        String strAttribute = strLogic.Substring(nBegin + 1, nEnd - nBegin - 1);
                        if (!m_hashYears.Contains(strAttribute))
                        {
                            DataSet dsYears = new DataSet() ;

							if( strAttribute != "Construction History" )
							{
								ConnectionParameters cp = DBMgr.GetAttributeConnectionObject( strAttribute );
								switch( cp.Provider )
								{
									case "MSSQL":
										strQuery = "SELECT DISTINCT year(DATE_) [Year] FROM " + strAttribute + " ORDER BY [YEAR]";
										break;
									case "ORACLE":
										strQuery = "SELECT DISTINCT TO_CHAR(DATE_,'YYYY') AS YEAR FROM " + strAttribute + " ORDER BY YEAR";
										break;
									default:
										throw new NotImplementedException( "TODO: Create ANSI implementation for GetAttributeYears()" );
										
								}
								dsYears = DBMgr.ExecuteQuery( strQuery, cp );
							}

                            if (dsYears.Tables.Count != 0)
                            {
                                List<String> list = new List<string>();
                                foreach (DataRow rowYear in dsYears.Tables[0].Rows)
                                {
                                    //list.Add(rowYear.ItemArray[0].ToString());
                                    if (rowYear.ItemArray[0].ToString() != "")
                                    {
                                        list.Add(rowYear.ItemArray[0].ToString());
                                    }
                                }
                                m_hashYears.Add(strAttribute, list);
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if this database is a native attribute (in RoadCare datase).
        /// </summary>
        /// <param name="strAttribute">Raw data tablename</param>
        /// <returns></returns>
        private bool IsNativeRoadCare(String strAttribute)
        {
            bool bNative = true;

            if (m_hashNative == null)
            {
                m_hashNative = new Hashtable();
                DataSet ds = DBMgr.ExecuteQuery("SELECT ATTRIBUTE_,NATIVE_ FROM ATTRIBUTES_");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strAttributeTable = row.ItemArray[0].ToString();
					bool bN = false;

					switch (DBMgr.NativeConnectionParameters.Provider)
					{
						case "MSSQL":
							bN = bool.Parse(row.ItemArray[1].ToString());
							break;
						case "ORACLE":
							bN = row.ItemArray[1].ToString() != "0";
							break;
						default:
							throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
							
					}
					m_hashNative.Add( strAttributeTable, bN );
                }
            }
            if (m_hashNative.Contains(strAttribute))
            {
                bNative = bool.Parse(m_hashNative[strAttribute].ToString());
            }
            return bNative;
        }

		private DataReader LogicalBreakAllYears(String strAttribute, String strLogic)
		{
			//If no years available.  We are done.
			if (!m_hashYears.Contains(strAttribute)) return null;
			List<String> list = (List<String>)m_hashYears[strAttribute];
			String strYear = list[list.Count - 1].ToString();
			strLogic = strLogic.Replace("[" + strAttribute + "]", "DATA_");
			strLogic = strLogic.Replace("|", "'");
			if (strLogic.Length > 0) strLogic = " AND " + strLogic;
			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(strAttribute);
			String strQuery;
			switch (cp.Provider)
			{
				case "MSSQL":
					strQuery = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,year(DATE_)AS YEARS,DATA_ FROM " + strAttribute +
									  " WHERE (FACILITY='' OR FACILITY IS NULL) " + strLogic + " ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
					break;
				case "ORACLE":
					strQuery = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,TO_CHAR(DATE_,'YYYY') AS YEARS,DATA_ FROM " + strAttribute +
									  " WHERE (FACILITY='' OR FACILITY IS NULL) " + strLogic + " ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for LogicalBreak()");

			}
			DataReader dr = null;
			try
			{
				dr = new DataReader(strQuery, cp);
			}
			catch (Exception exc)
			{
				SegmentationMessaging.AddMessage("Error: Could not parse family string. " + exc.Message);
			}
			return dr;
		}

        /// <summary>
        /// Performs a database query on the Logical break on SQL Database
        /// </summary>
        /// <param name="strAttribute"></param>
        /// <param name="strLogic"></param>
        private DataReader LogicalBreak(String strAttribute, String strLogic)
        {
            //If no years available.  We are done.
            if(!m_hashYears.Contains(strAttribute))return null;
            List<String> list = (List<String>)m_hashYears[strAttribute];
            String strYear = list[list.Count-1].ToString();
            strLogic = strLogic.Replace("[" + strAttribute + "]","DATA_");
            strLogic = strLogic.Replace("|", "'");
            if (strLogic.Length > 0) strLogic = " AND " + strLogic;
			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(strAttribute);
			String strQuery;

			switch (cp.Provider)
			{
				case "MSSQL":
					strQuery = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,year(DATE_)AS YEARS,DATA_ FROM " + strAttribute +
									  " WHERE year(DATE_)='" + strYear + "' AND (FACILITY='' OR FACILITY IS NULL) " + strLogic + " ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
					break;
				case "ORACLE":
					strQuery = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,TO_CHAR(DATE_,'YYYY') AS YEARS,DATA_ FROM " + strAttribute +
									  " WHERE TO_CHAR(DATE_,'YYYY')='" + strYear + "' AND (FACILITY='' OR FACILITY IS NULL) " + strLogic + " ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for LogicalBreak()");

			}
            //SqlDataReader dr = DBMgr.CreateDataReader(strQuery);
			DataReader dr = null;
			try
			{
				dr = new DataReader(strQuery, cp);
			}
			catch (Exception exc)
			{
				SegmentationMessaging.AddMessage("Error: Could not parse family string. " + exc.Message);
			}
			return dr;
        }

        /// <summary>
        /// Performs a database query on the Construction History break on SQL Database
        /// </summary>
        /// <param name="strAttribute"></param>
        /// <param name="strLogic"></param>
        private DataReader ConstructionBreak()
        {
            //If no years available.  We are done.
            String strQuery = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION FROM CONSTRUCTION_HISTORY ORDER BY ROUTES,DIRECTION,END_STATION";
            //SqlDataReader dr = DBMgr.CreateDataReader(strQuery);
            DataReader dr = new DataReader(strQuery);
            return dr;
        } 
        
        /// <summary>
        /// Does a logical break for a non-native RoadCare database. TODO C.Becker.
        /// </summary>
        /// <param name="strAttribute"></param>
        /// <param name="strLogic"></param>
        /// <returns></returns>
        private DataReader OleDbLogicalBreak(String strAttribute, String strLogic)
        {
            //String strQuery;
            //OleDbDataReader dr = null;// = db.CreateOleDbDataReader(strQuery);
            //return dr;
            return null;
        }

        /// <summary>
        /// Adds breaks to hashRoutes and hashBreaks.  For Root level selections (AnyChange)
        /// </summary>
        /// <param name="strFamily"></param>
        /// <param name="dr"></param>
        ///<param name="bAnyChange">True if ANYCHANGE, false if ANYRECORD</param>
        private void AddBreaksAnyRecord(String strFamily, DataReader dr)
        {
            //SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,year(DATE)AS YEARS,DATA
            //OR Check if Construction History
            String strLogic = m_hashLogic[strFamily].ToString();
            String strRouteDir = "";
            String strRouteDirOld = "";
            List<float> list = new List<float>();
            Hashtable hash = new Hashtable();

            float fShort = 1000000;
            float fLong = 0;
            float fAverage = 0;
            int iCount = 0;

            while(dr.Read())
            {
                strRouteDir = dr["ROUTES"].ToString() + "|" + dr["DIRECTION"].ToString();
                if (strRouteDir != strRouteDirOld)
                {
                    if(strRouteDirOld != "")
                    {
                        m_hashRoutes.Remove(strRouteDirOld);
                        m_hashBreaks.Remove(strRouteDirOld);

                        m_hashRoutes.Add(strRouteDirOld,list);
                        m_hashBreaks.Add(strRouteDirOld,hash);
                    }
                    if (m_hashRoutes.Contains(strRouteDir) && m_hashBreaks.Contains(strRouteDir))
                    {
                        strRouteDirOld = strRouteDir;
                        list = (List<float>)m_hashRoutes[strRouteDir];
                        hash = (Hashtable)m_hashBreaks[strRouteDir];
                    }
                    else
                    {
                        continue;
                    }
                }
                float fBegin;
                float fEnd;

                fBegin = float.Parse(dr["BEGIN_STATION"].ToString());
                fEnd = float.Parse(dr["END_STATION"].ToString());
                float fDifference = fEnd - fBegin;
                if (fDifference < fShort) fShort = fDifference;
                if (fDifference > fLong) fLong = fDifference;
                fAverage += fDifference;
                iCount++;
                
                
                String strBreak;
                if (!list.Contains(fBegin))
                {
                    list.Add(fBegin);
                    hash.Add(fBegin,"");
                }

                if(hash.ContainsKey(fBegin))
                {
                    strBreak = hash[fBegin].ToString();
                    if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                    {
                        hash.Remove(fBegin);
                        strBreak = strBreak + ";" + strFamily;
                        hash.Add(fBegin, strBreak);
                    }
                    else if (strBreak.Length == 0)
                    {
                        hash.Remove(fBegin);
                        hash.Add(fBegin, strFamily);
                    }
                }

                if (!list.Contains(fEnd))
                {
                    list.Add(fEnd);
                    hash.Add(fEnd,"");
                }
 
                if(hash.ContainsKey(fEnd))
                {
                    strBreak = hash[fEnd].ToString();
                    

                    if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                    {
                        hash.Remove(fEnd);
                        strBreak = strBreak + ";" + strFamily;
                        hash.Add(fEnd, strBreak);
                    }
                    else if (strBreak.Length == 0)
                    {
                        hash.Remove(fEnd);
                        hash.Add(fEnd, strFamily);
                    }
                }
            }
           
            if (strRouteDirOld != "")
            {
                m_hashRoutes.Remove(strRouteDirOld);
                m_hashBreaks.Remove(strRouteDirOld);

                m_hashRoutes.Add(strRouteDirOld, list);
                m_hashBreaks.Add(strRouteDirOld, hash);
            }

            if (iCount == 0)
            {
                fAverage = 0;
            }
            else
            {
                fAverage = fAverage / (float)iCount;
            }
            String strInsert = "INSERT INTO DYNAMIC_SEGMENTATION_RESULT (NETWORKID,BREAKCAUSE,SHORTEST,LONGEST,AVERAGE,NUMBER_) VALUES ('"
                + m_strNetworkID + "','"
                + strFamily + "','"
                + fShort.ToString("f3") + "','"
                + fLong.ToString("f3") + "','"
                + fAverage.ToString("f3") + "','"
                + iCount.ToString() + "')";
            DBMgr.ExecuteNonQuery(strInsert);
        }

		private void AddBreaksAnyYear(String strFamily, DataReader dr)
		{
			//SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,year(DATE)AS YEARS,DATA
			//OR Check if Construction History
			String strLogic = m_hashLogic[strFamily].ToString();
			String strRouteDir = "";
			String strRouteDirOld = "";
			List<float> list = new List<float>();
			Hashtable hash = new Hashtable();

			float fShort = 1000000;
			float fLong = 0;
			float fAverage = 0;
			int iCount = 0;

			while (dr.Read())
			{
				strRouteDir = dr["ROUTES"].ToString() + "|" + dr["DIRECTION"].ToString();
				if (strRouteDir != strRouteDirOld)
				{
					if (strRouteDirOld != "")
					{
						m_hashRoutes.Remove(strRouteDirOld);
						m_hashBreaks.Remove(strRouteDirOld);

						m_hashRoutes.Add(strRouteDirOld, list);
						m_hashBreaks.Add(strRouteDirOld, hash);
					}

                    if (m_hashRoutes.Contains(strRouteDir) && m_hashBreaks.Contains(strRouteDir))
                    {

                        strRouteDirOld = strRouteDir;
                        list = (List<float>)m_hashRoutes[strRouteDir];
                        hash = (Hashtable)m_hashBreaks[strRouteDir];
                    }
                    else
                    {
                        continue;
                    }
				}
				float fBegin;
				float fEnd;

				fBegin = float.Parse(dr["BEGIN_STATION"].ToString());
				fEnd = float.Parse(dr["END_STATION"].ToString());
				float fDifference = fEnd - fBegin;
				if (fDifference < fShort) fShort = fDifference;
				if (fDifference > fLong) fLong = fDifference;
				fAverage += fDifference;
				iCount++;


				String strBreak;
				if (!list.Contains(fBegin))
				{
					list.Add(fBegin);
					hash.Add(fBegin, "");
				}

				if (hash.ContainsKey(fBegin))
				{
					strBreak = hash[fBegin].ToString();
					if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
					{
						hash.Remove(fBegin);
						strBreak = strBreak + ";" + strFamily;
						hash.Add(fBegin, strBreak);
					}
					else if (strBreak.Length == 0)
					{
						hash.Remove(fBegin);
						hash.Add(fBegin, strFamily);
					}
				}

				if (!list.Contains(fEnd))
				{
					list.Add(fEnd);
					hash.Add(fEnd, "");
				}

				if (hash.ContainsKey(fEnd))
				{
					strBreak = hash[fEnd].ToString();


					if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
					{
						hash.Remove(fEnd);
						strBreak = strBreak + ";" + strFamily;
						hash.Add(fEnd, strBreak);
					}
					else if (strBreak.Length == 0)
					{
						hash.Remove(fEnd);
						hash.Add(fEnd, strFamily);
					}
				}
			}

			if (strRouteDirOld != "")
			{
				m_hashRoutes.Remove(strRouteDirOld);
				m_hashBreaks.Remove(strRouteDirOld);

				m_hashRoutes.Add(strRouteDirOld, list);
				m_hashBreaks.Add(strRouteDirOld, hash);
			}

			if (iCount == 0)
			{
				fAverage = 0;
			}
			else
			{
				fAverage = fAverage / (float)iCount;
			}
			String strInsert = "INSERT INTO DYNAMIC_SEGMENTATION_RESULT (NETWORKID,BREAKCAUSE,SHORTEST,LONGEST,AVERAGE,NUMBER_) VALUES ('"
				+ m_strNetworkID + "','"
				+ strFamily + "','"
				+ fShort.ToString("#.###") + "','"
				+ fLong.ToString("#.###") + "','"
				+ fAverage.ToString("#.###") + "','"
				+ iCount.ToString() + "')";
			DBMgr.ExecuteNonQuery(strInsert);
		}

        /// <summary>
        /// Adds breaks to hashRoutes and hashBreaks.  For Root level selections (AnyChange)
        /// </summary>
        /// <param name="strFamily"></param>
        /// <param name="dr"></param>
        ///<param name="bAnyChange">True if ANYCHANGE, false if ANYRECORD</param>
        private void AddBreaksAnyChange(String strFamily, DataReader dr)
        {
            //SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,year(DATE)AS YEARS,DATA
            //OR Check if Construction History
            String strLogic = m_hashLogic[strFamily].ToString();
            String strRouteDir = "";
            String strRouteDirOld = "";
            List<float> list = new List<float>();
            Hashtable hash = new Hashtable();

            float fShort = 1000000;
            float fLong = 0;
            float fAverage = 0;
            int iCount = 0;
            String strBreak;
            String strData;
            String strDataOld = "INITIALdefaultOLDdata"; 
            float fBegin;
            float fBeginStart = -1;
            float fEnd;
            float fEndLast = -1;
            

            while (dr.Read())
            {
                strRouteDir = dr["ROUTES"].ToString() + "|" + dr["DIRECTION"].ToString();
                strData = dr["DATA_"].ToString();

                if (strRouteDir != strRouteDirOld)
                {
                    //Output last strDataOld
                    if (strRouteDirOld != "")
                    {
                        if (!m_hashRoutes.Contains(strRouteDir)) continue;

                        float fDifference = fEndLast - fBeginStart;
                        if (fDifference < fShort) fShort = fDifference;
                        if (fDifference > fLong) fLong = fDifference;
                        fAverage += fDifference;
                        iCount++;


                        if (!list.Contains(fBeginStart))
                        {
                            list.Add(fBeginStart);
                            hash.Add(fBeginStart, "");
                        }

                        if (hash.ContainsKey(fBeginStart))
                        {
                            strBreak = hash[fBeginStart].ToString();
                            if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                            {
                                hash.Remove(fBeginStart);
                                strBreak = strBreak + ";" + strFamily;
                                hash.Add(fBeginStart, strBreak);
                            }
                            else if (strBreak.Length == 0)
                            {
                                hash.Remove(fBeginStart);
                                hash.Add(fBeginStart, strFamily);
                            }
                        }



                        if (!list.Contains(fEndLast))
                        {
                            list.Add(fEndLast);
                            hash.Add(fEndLast, "");
                        }

                        if (hash.ContainsKey(fEndLast))
                        {
                            strBreak = hash[fEndLast].ToString();


                            if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                            {
                                hash.Remove(fEndLast);
                                strBreak = strBreak + ";" + strFamily;
                                hash.Add(fEndLast, strBreak);
                            }
                            else if (strBreak.Length == 0)
                            {
                                hash.Remove(fEndLast);
                                hash.Add(fEndLast, strFamily);
                            }
                        }
                        
                        m_hashRoutes.Remove(strRouteDirOld);
                        m_hashBreaks.Remove(strRouteDirOld);
                        m_hashRoutes.Add(strRouteDirOld, list);
                        m_hashBreaks.Add(strRouteDirOld, hash);
                    }

                    //strRouteDirOld = strRouteDir; moved inside if statement.  this should only happen on valid change
                    if (m_hashRoutes.Contains(strRouteDir) && m_hashBreaks.Contains(strRouteDir))
                    {
                        strRouteDirOld = strRouteDir;
                        list = (List<float>)m_hashRoutes[strRouteDir];
                        hash = (Hashtable)m_hashBreaks[strRouteDir];
                    }
                    fBeginStart = -1;
                    fEndLast = -1;
                    strDataOld = "INITIALdefaultOLDdata";
                }


                fBegin = float.Parse(dr["BEGIN_STATION"].ToString());
                fEnd = float.Parse(dr["END_STATION"].ToString());

                if (fBeginStart < 0)
                {
                    fBeginStart = fBegin;
                    strDataOld = strData;
                }
              
                //Only when data changes.
                if (strData != strDataOld && fEndLast > 0)
                {

                    float fDifference = fEndLast - fBeginStart;
                    if (fDifference < fShort) fShort = fDifference;
                    if (fDifference > fLong) fLong = fDifference;
                    fAverage += fDifference;
                    iCount++;




                    if (!list.Contains(fBeginStart))
                    {
                        list.Add(fBeginStart);
                        hash.Add(fBeginStart, "");
                    }

                    if (hash.ContainsKey(fBeginStart))
                    {
                        strBreak = hash[fBeginStart].ToString();
                        if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                        {
                            hash.Remove(fBeginStart);
                            strBreak = strBreak + ";" + strFamily;
                            hash.Add(fBeginStart, strBreak);
                        }
                        else if (strBreak.Length == 0)
                        {
                            hash.Remove(fBeginStart);
                            hash.Add(fBeginStart, strFamily);
                        }
                    }



                    if (!list.Contains(fEndLast))
                    {
                        list.Add(fEndLast);
                        hash.Add(fEndLast, "");
                    }

                    if (hash.ContainsKey(fEndLast))
                    {
                        strBreak = hash[fEndLast].ToString();


                        if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                        {
                            hash.Remove(fEndLast);
                            strBreak = strBreak + ";" + strFamily;
                            hash.Add(fEndLast, strBreak);
                        }
                        else if (strBreak.Length == 0)
                        {
                            hash.Remove(fEndLast);
                            hash.Add(fEndLast, strFamily);
                        }
                    }
                    strDataOld = strData;
                    fBeginStart = fBegin;
                    fEndLast = fEnd;

                }
                else
                {
                    fEndLast = fEnd;
                }
            }

            if (strRouteDirOld != "")
            {
                
                if (hash.ContainsKey(fBeginStart))
                {
                    strBreak = hash[fBeginStart].ToString();
                    if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                    {
                        hash.Remove(fBeginStart);
                        strBreak = strBreak + ";" + strFamily;
                        hash.Add(fBeginStart, strBreak);
                    }
                    else if (strBreak.Length == 0)
                    {
                        hash.Remove(fBeginStart);
                        hash.Add(fBeginStart, strFamily);
                    }
                }



                if (!list.Contains(fEndLast))
                {
                    list.Add(fEndLast);
                    hash.Add(fEndLast, "");
                }

                if (hash.ContainsKey(fEndLast))
                {
                    strBreak = hash[fEndLast].ToString();


                    if (strBreak.Length > 0 && !strBreak.Contains(strFamily))
                    {
                        hash.Remove(fEndLast);
                        strBreak = strBreak + ";" + strFamily;
                        hash.Add(fEndLast, strBreak);
                    }
                    else if (strBreak.Length == 0)
                    {
                        hash.Remove(fEndLast);
                        hash.Add(fEndLast, strFamily);
                    }
                }


                m_hashRoutes.Remove(strRouteDirOld);
                m_hashBreaks.Remove(strRouteDirOld);
                m_hashRoutes.Add(strRouteDirOld, list);
                m_hashBreaks.Add(strRouteDirOld, hash);

            }

            if (iCount == 0)
            {
                fAverage = 0;
            }
            else
            {
                fAverage = fAverage / (float)iCount;
            }
			 String strInsert = "INSERT INTO DYNAMIC_SEGMENTATION_RESULT (NETWORKID,BREAKCAUSE,SHORTEST,LONGEST,AVERAGE,NUMBER_) VALUES ('"
			                + m_strNetworkID + "','"
			                + strFamily + "','"
			                + fShort.ToString("f3") + "','"
			                + fLong.ToString("f3") + "','"
			                + fAverage.ToString("f3") + "','"
			                + iCount.ToString() + "')";

            DBMgr.ExecuteNonQuery(strInsert);
        }

        /// <summary>
        /// For root level Logical breaks.  Combines results and the adds to hashRoutes and hashBreaks
        /// </summary>
        /// <param name="strFamily"></param>
        /// <param name="dr"></param>
        private void CombineThenAddBreaks(String strFamily, DataReader dr)
        {
            String strRouteDir = "";
            String strRouteDirOld = "";
            List<PointF> pts = new List<PointF>();
            List<float> list;
            Hashtable hash;
            while (dr.Read())
            {
                strRouteDir = dr["ROUTES"].ToString() + "|" + dr["DIRECTION"].ToString();
                if (strRouteDir != strRouteDirOld)//When a new route appears
                {                    
                    if (strRouteDirOld != "")
                    {
                        pts = CombinePoints(pts);
                        //Get the current Route and Break reasons 
                        hash = (Hashtable)m_hashBreaks[strRouteDirOld];
                        list = (List<float>)m_hashRoutes[strRouteDirOld];
                        
                        //Can't update value objects.  Must delete and Add
                        m_hashRoutes.Remove(strRouteDirOld);
                        m_hashBreaks.Remove(strRouteDirOld);

                       
                        //Loop through points and add to list (ROUTES) and hash (REASONS FOR BREAK)

                        foreach (PointF pt in pts)
                        {
                            if (!list.Contains(pt.X))
                            {
                                list.Add(pt.X);
                                hash.Add(pt.X, strFamily);
                            }
                            else
                            {
                                String strBreak = hash[pt.X].ToString();
                                if (strBreak.Length > 0 && strBreak.IndexOf(strFamily) < 0)
                                {
                                    hash.Remove(pt.X);
                                    strBreak = strBreak + ";" + strFamily;
                                    hash.Add(pt.X, strBreak);
                                }
                            }

                            if (!list.Contains(pt.Y))
                            {
                                list.Add(pt.Y);
                                hash.Add(pt.Y, strFamily);
                            }
                            else
                            {
                                String strBreak = hash[pt.Y].ToString();
                                if (strBreak.Length > 0 && strBreak.IndexOf(strFamily) < 0)
                                {
                                    hash.Remove(pt.Y);
                                    strBreak = strBreak + ";" + strFamily;
                                    hash.Add(pt.Y, strBreak);
                                }
                            }
                        }
                        m_hashRoutes.Add(strRouteDirOld, list);
                        m_hashBreaks.Add(strRouteDirOld, hash);
                    }
                    strRouteDirOld = strRouteDir;
                    pts = new List<PointF>();
                }
                PointF p = new PointF(float.Parse(dr["BEGIN_STATION"].ToString()), float.Parse(dr["END_STATION"].ToString()));
                pts.Add(p);
            }

            //Pick up last set of points.
            if (strRouteDirOld != "")
            {
                pts = CombinePoints(pts);
                //Get the current Route and Break reasons 
                hash = (Hashtable)m_hashBreaks[strRouteDirOld];
                list = (List<float>)m_hashRoutes[strRouteDirOld];

                //Can't update value objects.  Must delete and Add
                m_hashRoutes.Remove(strRouteDirOld);
                m_hashBreaks.Remove(strRouteDirOld);


                //Loop through points and add to list (ROUTES) and hash (REASONS FOR BREAK)

                foreach (PointF pt in pts)
                {
                    if (!list.Contains(pt.X))
                    {
                        list.Add(pt.X);
                        hash.Add(pt.X, strFamily);
                    }
                    else
                    {
                        String strBreak = hash[pt.X].ToString();
                        if (strBreak.Length > 0 && strBreak.IndexOf(strFamily) < 0)
                        {
                            hash.Remove(pt.X);
                            strBreak = strBreak + ";" + strFamily;
                            hash.Add(pt.X, strBreak);
                        }
                    }

                    if (!list.Contains(pt.Y))
                    {
                        list.Add(pt.Y);
                        hash.Add(pt.Y, strFamily);
                    }
                    else
                    {
                        String strBreak = hash[pt.Y].ToString();
                        if (strBreak.Length > 0 && strBreak.IndexOf(strFamily) < 0)
                        {
                            hash.Remove(pt.Y);
                            strBreak = strBreak + ";" + strFamily;
                            hash.Add(pt.Y, strBreak);
                        }
                    }
                }
                m_hashRoutes.Add(strRouteDirOld, list);
                m_hashBreaks.Add(strRouteDirOld, hash);
            }
            //TODO: Add summary information


        }


        /// <summary>
        /// Combines sections that End_Station matches next Begin_Station
        /// </summary>
        /// <param name="pts">List of PointF represent section begin and ends</param>
        /// <returns>List of PointF where continuous begin->end are combined.</returns>
        private List<PointF> CombinePoints(List<PointF> pts)
        {
            List<PointF> ptsReturn = new List<PointF>();
            PointF pointPrevious = new PointF(-1, -1);
            foreach (PointF pt in pts)
            {
                if (pointPrevious.X == -1)
                {
                    pointPrevious.X = pt.X;
                    pointPrevious.Y = pt.Y;
                }
                else
                {
                    if (pointPrevious.Y == pt.X)//Sections are continuous
                    {
                        pointPrevious.Y = pt.Y;
                    }
                    else
                    {
                        ptsReturn.Add(pointPrevious);
                        pointPrevious.X = pt.X;
                        pointPrevious.Y = pt.Y;
                    }
                }
            }
            if (pointPrevious.X >= 0) ptsReturn.Add(pointPrevious);

            return ptsReturn;
        }



        private void CombineFamilyPts(ref Hashtable hashPoints, DataReader dr)
        {
            String strRouteDir = "";
            String strRouteDirOld = "";
            List<PointF> pts = new List<PointF>();
            while (dr.Read())
            {
                strRouteDir = dr["ROUTES"].ToString() + "|" + dr["DIRECTION"].ToString();
                if (strRouteDir != strRouteDirOld)//When a new route appears
                {
                    if (strRouteDirOld != "")
                    {
                        pts = CombinePoints(pts);
                        //Get the current Route and Break reasons 
                        if (hashPoints.Contains(strRouteDirOld))
                        {
                            //Combine old points with new points.
                            List<PointF> ptsPrev = (List<PointF>)hashPoints[strRouteDirOld];
                            hashPoints.Remove(strRouteDirOld);
                            pts = MergePoints(ptsPrev, pts);
                            hashPoints.Add(strRouteDirOld, pts);
                        }
                        else
                        {
                            hashPoints.Add(strRouteDirOld, pts);
                        }
                    }

                    strRouteDirOld = strRouteDir;
                    pts = new List<PointF>();
                }
                PointF p = new PointF(float.Parse(dr["BEGIN_STATION"].ToString()), float.Parse(dr["END_STATION"].ToString()));
                pts.Add(p);

            }
            // Pick up last set of points.
            if (strRouteDirOld != "")
            {
                pts = CombinePoints(pts);
                //Get the current Route and Break reasons 
                if (hashPoints.Contains(strRouteDirOld))
                {
                    //Combine old points with new points.
                    List<PointF> ptsPrev = (List<PointF>)hashPoints[strRouteDirOld];
                    hashPoints.Remove(strRouteDirOld);
                    pts = MergePoints(ptsPrev, pts);
                    hashPoints.Add(strRouteDirOld, pts);
                }
                else
                {
                    hashPoints.Add(strRouteDirOld, pts);
                }
            }







        }
        private List<PointF> MergePoints(List<PointF> ptsPrev, List<PointF> pts)
        {

            // if pts.X > ptsPrev.Y   discard ptsPrev Pair
            // if ptsPrev.X  > pts.Y  discar pts pair.
            // Take larger of pts.X or ptsPrev.X and smaller of pts.Y or ptsPrev.Y
                // discard smaller .Y pair.  ;Set smaller .Y to larger .X
            List<PointF> ptsMerge = new List<PointF>();
            int nCurrent = 0;// Index for pts
            int nPrevious = 0;// Index for ptsPrev

            while (nCurrent < pts.Count && nPrevious < ptsPrev.Count)
            {
                if (pts[nCurrent].X > ptsPrev[nPrevious].Y)
                {
                    nPrevious++;
                    continue;
                }

                if (ptsPrev[nPrevious].X > pts[nCurrent].Y)
                {
                    nCurrent++;
                    continue;
                }


                PointF pt = new PointF();

                if (pts[nCurrent].X > ptsPrev[nPrevious].X) pt.X = pts[nCurrent].X;
                else pt.X = ptsPrev[nPrevious].X;

                if (pts[nCurrent].Y < ptsPrev[nPrevious].Y)
                {
                    PointF ptUpdate = (PointF)ptsPrev[nPrevious];
                    ptsPrev.RemoveAt(nPrevious);
                    ptUpdate.X = pts[nCurrent].Y;
                    ptsPrev.Insert(nPrevious, ptUpdate);    
                    pt.Y = pts[nCurrent].Y;
                    nCurrent++; 
                }
                else
                {
                    PointF ptUpdate = (PointF)pts[nCurrent];
                    pts.RemoveAt(nCurrent);
                    ptUpdate.X = ptsPrev[nPrevious].Y;
                    pts.Insert(nCurrent, ptUpdate);
                    pt.Y = ptsPrev[nPrevious].Y;
                    nPrevious++;
                }

                ptsMerge.Add(pt);


            }
            return ptsMerge;





        }
    }
}
