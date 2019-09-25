using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Drawing;
using System.Data;
using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Smo;
using DatabaseManager;
using System.Globalization;
using OpenGISExtension;
using SharpMap.Geometries;
using CalculateEvaluate;
using RoadCareGlobalOperations;
using DataObjects;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using static Simulation.Simulation;
using DataAccessLayer;

namespace RollupSegmentation
{
    public class RollupSegmentation
    {
        String m_strServer="";
        String m_strDataSource="";
        String m_strUserID="";
        String m_strPassword="";

        string m_strNetworkID = "";
        bool apiCall = false;
        string mongoConnection = "";

        List<String> m_listAttributes;
        Hashtable m_hashAttribute;
        int m_nCountNumber = 0;//Count of Number fields
        int m_nCountString = 0;//Count of String fields
        
        List<String> m_listCalculated;
		List<string> m_listCalculatedColumns = new List<string>();
        Hashtable m_hashCalculated;//Key = attribute  Value = Attribute Objcet
		Dictionary<string, float> m_sectionIDsToArea = new Dictionary<string, float>();
        Dictionary<string, float> m_sectionIDsToLength = new Dictionary<string, float>();
        
        //Hashtable m_hashMethods;//key = method; value = List<String> L_1,L_2,etc.
        String m_strRow;//Row to be bulkloaded into segmented table.
        List<String> m_listSegmentTables;// List of tables that are created for segmentation.
        List<String> m_listPCITables;
        List<String> m_listPCIYear;
        public String strNetwork;
        List<String> m_listRoutes;
        int m_nIndexData = -1;
        int m_nIndexReader = -1;


        String m_strSegmentTable = "";// Added by G.Larson 12/25/2008 to delay creation of SEGMENT_ until after calculation of CALCULATED_FIELDS
        List<DatabaseManager.TableParameters> m_listColumnSegment = new List<DatabaseManager.TableParameters>(); // Added by G.Larson 12/25/2008 to delay creation of SEGMENT_ until after calculation of CALCULATED_FIELDS

        int m_nPreviousNumberSections = 0;


        private CalculateEvaluate.CalculateEvaluate m_crArea;//Area equation to be evaluated.
        private List<String> m_listArea;//Area variables
        private Hashtable m_hashAttributeDefaults; //List of default values for all attributes.  Needed to calculate area.




        public RollupSegmentation(String strServer, String strDataSource, String strUserID, String strPassword)
        {
            m_strServer = strServer;
            m_strDataSource = strDataSource;
            m_strUserID = strUserID;
            m_strPassword = strPassword;

            //Hashtable hashDistress = new Hashtable();
            //hashDistress.Add("H_1", 0.25);
            //hashDistress.Add("L_13", 1);
            //hashDistress.Add("L_14", 1);
            //String strArea = "100";
            //String strMethod = "acmpr";
            //String strPCI = CalculatePCI(strMethod, strArea, hashDistress);
        }
        public RollupSegmentation()
        {

        }

        public RollupSegmentation(String m_strNetwork, String strNetworkID, bool isAPI, string connection)
        {
            strNetwork = m_strNetwork;
            m_strNetworkID = strNetworkID;
            apiCall = isAPI;
            mongoConnection = connection;
        }

        public class RollupModel
        {
            public string networkName { get; set; }
            public string rollupName { get; set; }
            public int networkId { get; set; }
            public string rollupStatus { get; set; }
            public DateTime? Created { get; set; }
            public DateTime? LastRun { get; set; }
        }

        public IMongoDatabase MongoDatabase;
        public IMongoCollection<RollupModel> Rollup;

        public void DoRollup()
		{
            if (apiCall == true)
            {
                MongoClient client = new MongoClient(mongoConnection);
                MongoDatabase = client.GetDatabase("BridgeCare");
                Rollup = MongoDatabase.GetCollection<RollupModel>("networks");

                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Running rollup");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }
            Boolean isOMS = false;
            String omsConnectionString = DataAccessLayer.ImportOMS.GetOMSConnectionString(DBMgr.GetNativeConnection().ConnectionString);
            if(!String.IsNullOrWhiteSpace(omsConnectionString))
            {
                isOMS = true;
            }

            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            //TextWriter log = new StreamWriter(strMyDocumentsFolder + "\\debugLog.txt");

			// Just get the envelope from the network definition page (along with all this other stuff).
			m_listAttributes = new List<String>();
			m_hashAttribute = new Hashtable();
			m_listSegmentTables = new List<String>();
			m_listPCITables = new List<String>();
			m_listPCIYear = new List<String>();


            RollupMessaging.AddMessge("Begin rollup of network: " + strNetwork + " at " + DateTime.Now.ToString("HH:mm:ss"));
			if( !CompileAreaEquation() )
			{
				RollupMessaging.AddMessge( "Rollup of network aborted: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" ) );
                if(apiCall == true)
                {
                    var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Rollup aborted");
                    Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                }
				return;
			}
            
			//Get NetworkID
			RollupMessaging.AddMessge("Getting NETWORKID");
			DataSet ds = DBMgr.ExecuteQuery( "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME='" + strNetwork + "'" );
			String strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();

			//Check if this network has been rolled up previously.
			//If it has, get number of sections.

			//you have to actually write the check,
			//not just depend on the exception handling.  That's pointless, unclear, and slow.
			if (DBMgr.IsTableInDatabase("SECTION_" + strNetworkID))
			{
				m_nPreviousNumberSections = GetNumberPreviousSections(strNetworkID);
			}
			else
			{
				m_nPreviousNumberSections = 0;
			}

			// Check the SEGMENT_CONTROL table to see what tables were generated by previous rollups.
			RollupMessaging.AddMessge("Getting segments from SEGMENT_CONTROL");
			ds = DBMgr.ExecuteQuery( "SELECT DISTINCT SEGMENT_TABLE FROM SEGMENT_CONTROL WHERE NETWORKID='" + strNetworkID + "'" );
			String strTable;
			String strDrop;

			// Drop the tables returned (if any)
			foreach( DataRow row in ds.Tables[0].Rows )
			{
				strTable = row.ItemArray[0].ToString();
				strDrop = "DROP TABLE " + strTable;
				try
				{
					DBMgr.ExecuteNonQuery( strDrop );
				}
				catch( Exception exc )
				{
					RollupMessaging.AddMessge( "Warning: Drop table " + strTable + " failed. " + exc.Message );
                }
			}
			bool bPCI = false;
			String strSelectPCI = "SELECT COUNT(*) FROM PCI";
			try
			{
				if( DBMgr.ExecuteScalar( strSelectPCI ) > 0 )
				{
					bPCI = true;
				}
			}
			catch( Exception exc )
			{
				RollupMessaging.AddMessge( "Warning: Error retrieving PCI count.  PCI not included in rollup." + exc.Message );
				bPCI = true;
            }
			DBMgr.ExecuteNonQuery( "DELETE FROM SEGMENT_CONTROL WHERE NETWORKID ='" + strNetworkID + "'" );

			// Get list of all attributes (ATTRIBUTE TABLE) and list of all YEARS (Individual Attribute Tables)
            RollupMessaging.AddMessge("Getting ATTRIBUTE list from ATTRIBUTES table...");

            if (apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Getting ATTRIBUTE list from ATTRIBUTES table");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }

            String strSelectAttribute = "SELECT ATTRIBUTE_,NATIVE_,TYPE_,FORMAT FROM ATTRIBUTES_ WHERE CALCULATED='0' OR CALCULATED IS NULL";
			if( bPCI )
				strSelectAttribute += " OR ATTRIBUTE_ = 'PCI' ORDER BY ATTRIBUTE_";
			else
				strSelectAttribute += " ORDER BY ATTRIBUTE_";
			ds = DBMgr.ExecuteQuery( strSelectAttribute );
			String strAttribute;
			foreach( DataRow row in ds.Tables[0].Rows )
			{
				Attributes attribute = new Attributes();
				strAttribute = row.ItemArray[0].ToString();
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						attribute.m_bNative = bool.Parse( row.ItemArray[1].ToString() );
						break;
					case "ORACLE":
						attribute.m_bNative = row.ItemArray[1].ToString().Trim() != "0";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
						//break;
				}
				attribute.m_strType = row.ItemArray[2].ToString();
				attribute.m_strFormat = row.ItemArray[3].ToString();

				DataSet dsRollup = DBMgr.ExecuteQuery( "SELECT ROLLUPTYPE FROM ROLLUP_CONTROL WHERE NETWORKID = '" + strNetworkID + "' AND ATTRIBUTE_ = '" + strAttribute + "'" );
				if( dsRollup.Tables[0].Rows.Count == 1 )
				{
					attribute.m_strRollupType = dsRollup.Tables[0].Rows[0].ItemArray[0].ToString();
				}

				// None excludes attributes from the rollup entirely.
				if (attribute.m_strRollupType != "None")
				{
					m_listAttributes.Add(strAttribute);
					m_hashAttribute.Add(strAttribute, attribute);
				}
			}
			RollupMessaging.AddMessge("Finished selection of all ATTIRBUTES for all YEARS");

            if (apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Finished selection of all ATTIRBUTES");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }

            #region Create Attribute Tables LRS and SRS

            bool bRollupError = false;
			m_nCountString = 0;
			m_nCountNumber = 0;


			//Get year list for attribute
			String strQuery;
			foreach( String str in m_listAttributes )
			{
				bRollupError = false;
				Attributes attribute = ( Attributes )m_hashAttribute[str];
				m_hashAttribute.Remove( str );// Going to update Year list to each attribute

				// Get YEARS from the individual attribute fields
				ConnectionParameters cp = DBMgr.GetAttributeConnectionObject( str );
				switch( cp.Provider )
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT year(DATE_) [Year] FROM " + str + " ORDER BY [YEAR] ASC";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT TO_CHAR(DATE_,'YYYY') AS Year FROM " + str + " ORDER BY YEAR ASC";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for DoRollup()" );
				}
				try
				{
					ds = DBMgr.ExecuteQuery( strQuery, cp );
				}
				catch( Exception exc )
				{
					RollupMessaging.AddMessge( "Error: Could not rollup attribute " + str + ". " + exc.Message );
					bRollupError = true;
                }

				attribute.m_listYears = new List<String>();
				if( !bRollupError )
				{
					if( attribute.m_strType == "NUMBER" )
					{
						m_nCountNumber += ds.Tables[0].Rows.Count;
					}
					else // STRING - Treat unknown as STRING.
					{
						m_nCountString += ds.Tables[0].Rows.Count;
					}

					foreach( DataRow row in ds.Tables[0].Rows )
					{
						// Must have a date when creating a column in the SEGMENT_ table.
						if( row.ItemArray[0].ToString() != "" )
						{
							attribute.m_listYears.Add( row.ItemArray[0].ToString() );
						}
					}
				}
				m_hashAttribute.Add( str, attribute );
			}
			// This table is for SECTIONID, FACILITY, BEGIN_STATION, END_STATION, DIRECTION, SECTION,AREA,UNITS
			RollupMessaging.AddMessge("Creating the SECTION_# table");
			try
			{
				List<DatabaseManager.TableParameters> listColumn = new List<DatabaseManager.TableParameters>();
				listColumn.Add(new DatabaseManager.TableParameters("SECTIONID", DataType.Int, false, true));
				listColumn.Add(new DatabaseManager.TableParameters("FACILITY", DataType.VarChar(4000), false));
				listColumn.Add(new DatabaseManager.TableParameters("BEGIN_STATION", DataType.Float, true));
				listColumn.Add(new DatabaseManager.TableParameters("END_STATION", DataType.Float, true));
				listColumn.Add(new DatabaseManager.TableParameters("DIRECTION", DataType.VarChar(50), true));
				listColumn.Add(new DatabaseManager.TableParameters("SECTION", DataType.VarChar(4000), false));
				listColumn.Add(new DatabaseManager.TableParameters("AREA", DataType.Float, true));
				listColumn.Add(new DatabaseManager.TableParameters("UNITS", DataType.VarChar(50), true));

				if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
				{
					int IsClobColumn = 1;
					listColumn.Add(new DatabaseManager.TableParameters("GEOMETRY", DataType.VarChar(-1), true, IsClobColumn));
				}
				else
				{
					listColumn.Add(new DatabaseManager.TableParameters("GEOMETRY", DataType.VarCharMax, true));
				}
				listColumn.Add(new DatabaseManager.TableParameters("ENVELOPE_MINX", DataType.Float, true));
				listColumn.Add(new DatabaseManager.TableParameters("ENVELOPE_MAXX", DataType.Float, true));
				listColumn.Add(new DatabaseManager.TableParameters("ENVELOPE_MINY", DataType.Float, true));
				listColumn.Add(new DatabaseManager.TableParameters("ENVELOPE_MAXY", DataType.Float, true));
				strTable = "SECTION_" + strNetworkID;
				DBMgr.CreateTable(strTable, listColumn, false);
				DBMgr.ExecuteNonQuery("INSERT INTO SEGMENT_CONTROL (NETWORKID,SEGMENT_TABLE) VALUES ('" + strNetworkID + "','" + strTable + "')");
			}
			catch(Exception exc)
			{
				RollupMessaging.AddMessge("Exception handled during SECTION_# table creation. " + exc.Message);
				return;
			}
			m_listColumnSegment = new List<DatabaseManager.TableParameters>();
			m_listColumnSegment.Add( new DatabaseManager.TableParameters( "SECTIONID", DataType.Int, false, true ) );

			// SEGMENT_networkid_NS0
			RollupMessaging.AddMessge("Count of total columns in segment_ table" + ((m_nCountNumber + m_nCountString).ToString()));
			int columnLimit = 500;
			if(DBMgr.NativeConnectionParameters.Provider == "ORACLE")
			{
				columnLimit = 1000;
			}
			if( ( m_nCountNumber + m_nCountString ) < columnLimit )
			{
				strTable = "SEGMENT_" + strNetworkID + "_NS0";
				m_strSegmentTable = strTable;
				RollupMessaging.AddMessge("Inserting SEGMENT_ table attributes into segment control");

                if (apiCall == true)
                {
                    var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Inserting SEGMENT_ table attributes");
                    Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                }

                foreach ( String str in m_listAttributes )
				{
					
					DBMgr.ExecuteNonQuery( "INSERT INTO SEGMENT_CONTROL (NETWORKID,SEGMENT_TABLE,ATTRIBUTE_) VALUES ('" + strNetworkID + "','" + strTable + "','" + str + "')" );
					Attributes attribute = ( Attributes )m_hashAttribute[str];

					foreach( String year in attribute.m_listYears )
					{
						String strColumn = str.ToUpper() + "_" + year;
						if( attribute.m_strType == "NUMBER" )
						{
							m_listColumnSegment.Add( new DatabaseManager.TableParameters( strColumn, DataType.Float, true ) );
						}
						else
						{
							m_listColumnSegment.Add( new DatabaseManager.TableParameters( strColumn, DataType.VarChar( 255 ), true ) );
						}
					}

					if( attribute.m_strType == "NUMBER" )
					{
						m_listColumnSegment.Add( new DatabaseManager.TableParameters( str.ToUpper(), DataType.Float, true ) );
					}
					else
					{
						m_listColumnSegment.Add( new DatabaseManager.TableParameters( str.ToUpper(), DataType.VarChar( 255 ), true ) );
					}
				}
				AddCalculatedFieldColumns();
				DBMgr.CreateTable( m_strSegmentTable, m_listColumnSegment, false );
				m_listSegmentTables.Add( strTable );
			}
			else // For a large number of columns we need to
			{
				int nNumberTable = m_nCountNumber / columnLimit;
				int nStringTable = m_nCountString / columnLimit;
			}

#endregion

#region Create Section ID for LRS AND SRS
			//At this point all necessary tables for this network rollup have been created and stored in SEGMENT_CONTROL

			//Take data from Dynamic Segmentation Table (LRS) or from (RAWSECTIONALIAS) and put in the
			// SECTION_networkID
			Directory.CreateDirectory(strMyDocumentsFolder);
			
			//Store the SECTION_networkid since it needs to be iterated MANY times.  Once for each year for each variable.
			List<DataPoint> listSections = new List<DataPoint>();
			String strOutFile = strMyDocumentsFolder + "\\sectionLRS.txt";
			TextWriter tw;// = new StreamWriter(strOutFile);

			if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
			{
				strQuery = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION FROM DYNAMIC_SEGMENTATION WHERE NETWORKID ='" + strNetworkID + "' ORDER BY NLSSORT(ROUTES, 'NLS_SORT=LATIN'), NLSSORT(DIRECTION, 'NLS_SORT=LATIN'), BEGIN_STATION";
			}
			else
			{
				strQuery = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION FROM DYNAMIC_SEGMENTATION WHERE NETWORKID ='" + strNetworkID + "' ORDER BY ROUTES, DIRECTION,BEGIN_STATION";
			}
			DataReader dr = new DataReader( strQuery );

			if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
			{
				strQuery = "SELECT GEOMETRY, ROUTES, DIRECTION, BEGIN_STATION, END_STATION, ID_ FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL ORDER BY NLSSORT(ROUTES, 'NLS_SORT=LATIN'), NLSSORT(DIRECTION, 'NLS_SORT=LATIN')";
			}
			else
			{
				strQuery = "SELECT GEOMETRY, ROUTES, DIRECTION, BEGIN_STATION, END_STATION, ID_ FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL ORDER BY ROUTES, DIRECTION";
			}
			DataReader drGeometry = new DataReader( strQuery );

			int nSection = 1001;
			String strRow;
			String strGeometry = "";
			String strPreviousRoute = "";
			String strPreviousDirection = "";
			LineString lineString = null;
			SharpLine sl = null;
			List<SharpLine> multiLines = new List<SharpLine>();

			// Start reading through the dynamic seg table.
			Geometry testGeometry;
			RollupMessaging.AddMessge("Starting through dynamic segmentation table for geometries.");

            if (apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Starting through dynamic segmentation table for geometries");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }

            while ( dr.Read() )
			{
				double logicalBegin = double.Parse( dr["BEGIN_STATION"].ToString() );
				double logicalEnd = double.Parse( dr["END_STATION"].ToString() );

				testGeometry = null;
				// Check for a change in route or direction
				if( strPreviousRoute != dr["ROUTES"].ToString() || strPreviousDirection != dr["DIRECTION"].ToString() )
				{
					// Route or direction change has occured, read from the network definition table
					// until a matching route and direction has been found.
					while( drGeometry.Read() )
					{
						strPreviousRoute = dr["ROUTES"].ToString();
						strPreviousDirection = dr["DIRECTION"].ToString();

						// Create a SharpLine for linestring parsing, this is so we can match the BMP and EMP
						// for each section with the proper vertex supplied from network definition table.
						String str = drGeometry["ROUTES"].ToString();
						String str2 = drGeometry["DIRECTION"].ToString();
						if( drGeometry["ROUTES"].ToString() == dr["ROUTES"].ToString() && drGeometry["DIRECTION"].ToString() == dr["DIRECTION"].ToString() )
						{
							// Only calculate a lineString and SharpLine if there is a route direction match.
							String strGeom = drGeometry["GEOMETRY"].ToString();

							if( !String.IsNullOrEmpty( strGeom ) )
							{
								testGeometry = Geometry.GeomFromText( strGeom );
								if( !( testGeometry is MultiLineString ) )
								{
									lineString = ( LineString )testGeometry;
									sl = new SharpLine( lineString.Vertices, drGeometry["BEGIN_STATION"].ToString(), drGeometry["END_STATION"].ToString() );
								}
								else
								{
									try
									{
										// Reset the multiLines list.
										multiLines = new List<SharpLine>();
										string multiToSingleQuery = "SELECT LINESTRING, BEGIN_SEGMENT, END_SEGMENT FROM MULTILINE_GEOMETRIES_DEF WHERE FACILITY_ID = '" + drGeometry["ID_"].ToString() + "' ORDER BY BEGIN_SEGMENT";
										DataSet multiToSingleSet = DBMgr.ExecuteQuery( multiToSingleQuery );
										foreach( DataRow lineSegmentRow in multiToSingleSet.Tables[0].Rows )
										{
											multiLines.Add( new SharpLine
												( ( ( LineString )lineSegmentRow["LINESTRING"] ).Vertices, lineSegmentRow["BEGIN_SEGMENT"].ToString(), lineSegmentRow["END_SEGMENT"].ToString() ) );
										}
									}
									catch( Exception exc )
									{
										RollupMessaging.AddMessge( "Error processing LINESTRING segment. " + exc.Message );

                                        if (apiCall == true)
                                        {
                                            var updateStatus = Builders<RollupModel>.Update
                                                 .Set(s => s.rollupStatus, "Error processing INESTRING segment");
                                            Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                                        }
                                    }
								}
							}
							else
							{
								sl = null;
							}
							break;
						}
					}
					
				}
				// The geometry string for the section.
				if( sl != null )
				{
					strGeometry = sl.GetLineSegment( dr["BEGIN_STATION"].ToString(), dr["END_STATION"].ToString() );
				}
				else if( multiLines.Count > 0 )
				{
					MultiSharpLine msl = new MultiSharpLine( multiLines, dr["BEGIN_STATION"].ToString(), dr["END_STATION"].ToString(), listSections );
				}
				if( strGeometry != "" )
				{
					if( sl != null )
					{
						listSections.Add( new DataPoint( nSection, dr["ROUTES"].ToString(), dr["BEGIN_STATION"].ToString(), dr["END_STATION"].ToString(), dr["DIRECTION"].ToString(), strGeometry.ToString(), sl.Envelope.Envelope_MinX.ToString(), sl.Envelope.Envelope_MaxX.ToString(), sl.Envelope.Envelope_MinY.ToString(), sl.Envelope.Envelope_MaxY.ToString() ) );
					}
					else if( testGeometry != null )
					{
						listSections.Add( new DataPoint( nSection, dr["ROUTES"].ToString(), dr["BEGIN_STATION"].ToString(), dr["END_STATION"].ToString(), dr["DIRECTION"].ToString(), strGeometry.ToString(), testGeometry.GetBoundingBox().Min.X.ToString(), testGeometry.GetBoundingBox().Max.X.ToString(), testGeometry.GetBoundingBox().Min.Y.ToString(), testGeometry.GetBoundingBox().Max.Y.ToString() ) );
					}
					else
					{
						listSections.Add( new DataPoint( nSection, dr["ROUTES"].ToString(), dr["BEGIN_STATION"].ToString(), dr["END_STATION"].ToString(), dr["DIRECTION"].ToString(), "", "", "", "", "" ) );
					}
				}
				else
				{
					listSections.Add( new DataPoint( nSection, dr["ROUTES"].ToString(), dr["BEGIN_STATION"].ToString(), dr["END_STATION"].ToString(), dr["DIRECTION"].ToString(), "", "", "", "", "" ) );
				}
				nSection++;
			}
			RollupMessaging.AddMessge("Finished Geometries");

            if (apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Finished Geometries");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }
            dr.Close();
			drGeometry.Close();

		    //foreach (var section in listSections)
		    //{
		    //    double area = Math.Abs(Convert.ToDouble(section.m_strEnd) - Convert.ToDouble(section.m_strBegin));
		    //    strRow = section.m_nSection + "\t" + section.m_strRoutes + "\t" + section.m_strBegin + "\t" + section.m_strEnd +
		    //             "\t" + section.m_strDirection
		    //             + "\t\t" + area
		    //             + "\t\t" + section.m_strGeometry
		    //             + "\t" + section.m_strMinX
		    //             + "\t" + section.m_strMaxX
		    //             + "\t" + section.m_strMinY
		    //             + "\t" + section.m_strMaxY;
      //          tw.WriteLine(strRow);
		    //}
      //      tw.Close();
            List<string> columnNames = new List<string>();


      //      if (listSections.Count > 0)
      //      {
      //          switch (DBMgr.NativeConnectionParameters.Provider)
      //          {
      //              case "MSSQL":
      //                  DBMgr.SQLBulkLoad("SECTION_" + strNetworkID.ToString(), strOutFile, '\t');
      //                  break;
      //              case "ORACLE":
      //                  columnNames.Clear();
      //                  columnNames.Add("SECTIONID");
      //                  columnNames.Add("FACILITY");
      //                  columnNames.Add("BEGIN_STATION");
      //                  columnNames.Add("END_STATION");
      //                  columnNames.Add("DIRECTION");
      //                  columnNames.Add("SECTION");
      //                  columnNames.Add("AREA");
      //                  columnNames.Add("UNITS");
      //                  columnNames.Add("GEOMETRY");
      //                  columnNames.Add("ENVELOPE_MINX");
      //                  columnNames.Add("ENVELOPE_MAXX");
      //                  columnNames.Add("ENVELOPE_MINY");
      //                  columnNames.Add("ENVELOPE_MAXY");
      //                  DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, "SECTION_" + strNetworkID, strOutFile, columnNames, "\\t");
      //                  break;
      //              default:
      //                  throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
      //                  //break;
      //          }
      //      }
           


            // Handle the SRS based Section next, so we can loop tables.
            List<DataPoint> listSectionSRS = new List<DataPoint>();
			strOutFile = strMyDocumentsFolder + "\\sectionSRS.txt";
			tw = new StreamWriter( strOutFile );

			// Just get the envelope from the network definition page (along with all this other stuff).
			RollupMessaging.AddMessge("Getting envelope data from NETWORK_DEFINITION");
			if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
			{
				strQuery = "SELECT FACILITY,SECTION,AREA, GEOMETRY, Envelope_MinX, Envelope_MaxX, Envelope_MaxY, Envelope_MinY FROM NETWORK_DEFINITION WHERE ROUTES IS NULL AND FACILITY IS NOT NULL ORDER BY NLSSORT(FACILITY, 'NLS_SORT=LATIN'), NLSSORT(SECTION, 'NLS_SORT=LATIN')";
			}
			else
			{
				strQuery = "SELECT FACILITY,SECTION,AREA, GEOMETRY, Envelope_MinX, Envelope_MaxX, Envelope_MaxY, Envelope_MinY FROM NETWORK_DEFINITION WHERE ROUTES IS NULL AND FACILITY IS NOT NULL ORDER BY FACILITY, SECTION";
			}
			dr = new DataReader( strQuery );
			nSection = 1000001;
			while( dr.Read() )
			{
				float area = 1f;
				if(!float.TryParse(dr["AREA"].ToString(), out area))
				{
					RollupMessaging.AddMessge("Area for Facility: " + dr["FACILITY"].ToString() + " Section: " + dr["SECTION"].ToString() + " could not be parsed.  Value of 1 used.");
				}
                //Uses OMS OID as SectionID
                if (isOMS)
                {
                    String omsSection = Convert.ToString(dr["SECTION"]);
                    if (omsSection.Contains("(") && omsSection.Contains(")"))
                    {
                        int nBegin = omsSection.IndexOf('(');
                        int nEnd = omsSection.IndexOf(')');
                        string section = omsSection.Substring(nBegin + 1, nEnd - nBegin - 1);
                        nSection = Convert.ToInt32(section);
                    }
                }

				m_sectionIDsToArea.Add(nSection.ToString(), area);
                m_sectionIDsToLength.Add(nSection.ToString(), 1);
				strRow = nSection.ToString() + "\t" + dr["FACILITY"].ToString()
											 + "\t"
											 + "\t"
											 + "\t"
											 + "\t" + dr["SECTION"].ToString()
											 + "\t" + area.ToString()
											 + "\tft^2";// +dr["AREATYPE"].ToString();
				if( dr["GEOMETRY"] != DBNull.Value )
				{
					strRow += "\t" + dr["GEOMETRY"].ToString()
					+ "\t" + dr["Envelope_MinX"].ToString()
					+ "\t" + dr["Envelope_MaxX"].ToString()
					+ "\t" + dr["Envelope_MinY"].ToString()
					+ "\t" + dr["Envelope_MaxY"].ToString();
				}
				else
				{
					strRow += "\t\t\t\t\t";
				}
				listSectionSRS.Add(new DataPoint(nSection.ToString(), dr["FACILITY"].ToString(), dr["SECTION"].ToString(), area.ToString(), "1"));

				tw.WriteLine( strRow );
				nSection++;
			}
			RollupMessaging.AddMessge("Finished adding SRS sections");
			dr.Close();
			tw.Close();
			////Use batch Loader to load SRS data.
			//dsmelser
			//needed for Oracle
			RollupMessaging.AddMessge("Bulk loading SECTION_ table.");

            if (apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Bulk loading SECTION_ table");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }

            if (listSectionSRS.Count > 0)
			{
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad("SECTION_" + strNetworkID.ToString(), strOutFile, '\t');
						break;
					case "ORACLE":
						columnNames.Clear();
						columnNames.Add("SECTIONID");
						columnNames.Add("FACILITY");
						columnNames.Add("BEGIN_STATION");
						columnNames.Add("END_STATION");
						columnNames.Add("DIRECTION");
						columnNames.Add("SECTION");
						columnNames.Add("AREA");
						columnNames.Add("UNITS");
						columnNames.Add("GEOMETRY");
						columnNames.Add("ENVELOPE_MINX");
						columnNames.Add("ENVELOPE_MAXX");
						columnNames.Add("ENVELOPE_MINY");
						columnNames.Add("ENVELOPE_MAXY");
						DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, "SECTION_" + strNetworkID, strOutFile, columnNames, "\\t");
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
			}
			RollupMessaging.AddMessge("Finished Bulk Loading SECTION_ table.");

            if (apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Finished Bulk Loading SECTION_ table");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }

            #endregion

            #region LRS AND SRS ATTRIBUTES

            //Only load one tables worth of attributes at a time.  SO... need to get list of tables and attributes therein.
            List<String> listAttributeInTable;
			foreach( String strSegmentTable in m_listSegmentTables )
			{
				listAttributeInTable = new List<String>();
				ds = DBMgr.ExecuteQuery( "SELECT ATTRIBUTE_ FROM SEGMENT_CONTROL WHERE SEGMENT_TABLE ='" + strSegmentTable + "' ORDER BY ATTRIBUTE_" );
				foreach( DataRow row in ds.Tables[0].Rows )
				{
					listAttributeInTable.Add( row.ItemArray[0].ToString() );
				}

				//Since this information originally came from this function.  Just store previous.
				//Make a list of new tables.  Store in hashtable key=table  object = list attributes.

				//Will need to make dr for SECTION_networkID (which we just made).  To save a datareader, could just read table and parse.
				//Need to see if can store DataReader object in Hashtable and keep open?
				//If can hashtable then strKey = attribute  obect = SqlDataReader.
				//Perhaps better would be an array or list datareaders.

				//From SECTION_networkid read SECTIONID,ROUTES,BEGIN_STATION,END_STATION
				//From each of the attributes read ROUTES, DIRECTION, BEGIN_STATION,END_STATION,year(Date) AS YEARS, ORDER BY ROUTE, DIRECTION, BEGIN 
				//This orders groups years together.
				//Old code with parallel DataReader.
				//strOutFile = strMyDocumentsFolder + "\\" + strSegmentTable + ".csv";
				//tw = new StreamWriter(strOutFile);


				//Bracket this with TABLE loop... TODO
				//Make this work with the OleDB connection.

				List<DataPoint> listDP;
				foreach( String attribute in listAttributeInTable )
				{
					RollupMessaging.AddMessge( "Rolling up Linear Attribute:" + attribute + " at " + DateTime.Now.ToString( "HH:mm:ss" ) );
					bRollupError = false;
					ConnectionParameters cp;
					String strSelect;
                    m_listRoutes = new List<string>();
                    string strSelectRoutes = "";

					if( attribute != "PCI" && attribute != "CLIMATE_PCI" && attribute != "LOAD_PCI" && attribute != "OTHER_PCI" )
					{
						cp = DBMgr.GetAttributeConnectionObject( attribute );
						switch( cp.Provider )
						{
							case "MSSQL":
								strSelect = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,year(DATE_) AS YEARS,DATA_ FROM " + attribute + " WHERE (ROUTES <> '' AND ROUTES IS NOT NULL) ORDER BY ROUTES, DIRECTION,BEGIN_STATION,YEARS";
								break;
							case "ORACLE":
								strSelect = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,TO_CHAR(DATE_,'YYYY') AS YEARS,DATA_ FROM " + attribute + " WHERE (ROUTES LIKE '_%' AND ROUTES IS NOT NULL) ORDER BY NLSSORT(ROUTES, 'NLS_SORT=LATIN'), NLSSORT(DIRECTION, 'NLS_SORT=LATIN'), BEGIN_STATION, YEARS";
								//strSelect = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,TO_CHAR(DATE_,'YYYY') AS YEARS,DATA_ FROM " + attribute + " WHERE (ROUTES LIKE '_%' AND ROUTES IS NOT NULL) ORDER BY ROUTES, DIRECTION,BEGIN_STATION,YEARS";
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for DoRollup()" );
								//break;
						}
                    
   				    }
					else
					{
						cp = DBMgr.NativeConnectionParameters;
						switch( cp.Provider )
						{
							case "MSSQL":
								strSelect = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,year(DATE_) AS YEARS, PCI AS DATA_ FROM PCI WHERE (ROUTES <> '' AND ROUTES IS NOT NULL) ORDER BY ROUTES, DIRECTION,BEGIN_STATION,YEARS";
								break;
							case "ORACLE":
								strSelect = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,TO_CHAR(DATE_,'YYYY') AS YEARS, " + attribute + "AS DATA_,AREA FROM PCI WHERE (ROUTES LIKE '_%' AND ROUTES IS NOT NULL  AND SAMPLE_TYPE<>'IGNORE') ORDER BY NLSSORT(ROUTES, 'NLS_SORT=LATIN'), NLSSORT(DIRECTION, 'NLS_SORT=LATIN'), BEGIN_STATION, YEARS";
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for DoRollup()" );
								//break;
						}

					}

                    switch (cp.Provider)
                    {
                        case "MSSQL":
                            strSelectRoutes = "SELECT DISTINCT ROUTES FROM " + attribute + " WHERE (ROUTES <> '' AND ROUTES IS NOT NULL) ORDER BY ROUTES";
                            break;
                        case "ORACLE":
                            strSelectRoutes = "SELECT DISTINCT ROUTES FROM " + attribute + " WHERE (ROUTES LIKE '_%' AND ROUTES IS NOT NULL) ORDER BY NLSSORT(ROUTES, 'NLS_SORT=LATIN')";
                            //strSelect = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,TO_CHAR(DATE_,'YYYY') AS YEARS,DATA_ FROM " + attribute + " WHERE (ROUTES LIKE '_%' AND ROUTES IS NOT NULL) ORDER BY ROUTES, DIRECTION,BEGIN_STATION,YEARS";
                            break;
                        default:
                            throw new NotImplementedException("TODO: Create ANSI implementation for DoRollup()");
                            //break;
                    }
	
                    //DataSet dataSetRoutes = DBMgr.ExecuteQuery(strSelectRoutes);
					try
					{
						DataSet dataSetRoutes = DBMgr.ExecuteQuery(strSelectRoutes, cp);
						foreach (DataRow rowRoutes in dataSetRoutes.Tables[0].Rows)
						{
							m_listRoutes.Add(rowRoutes[0].ToString());
						}
					}
					catch(Exception exc)
					{
						RollupMessaging.AddMessge("Error rolling up attribute: " + attribute + " - " + exc.Message);
					}
                    
                    
                    
                    strOutFile = strMyDocumentsFolder + "\\" + attribute + ".txt";
					tw = new StreamWriter( strOutFile );
					try
					{
						dr = new DataReader( strSelect, cp );
					}
					catch( Exception exc )
					{
						RollupMessaging.AddMessge( "Warning: Could not open attribute table " + attribute + ". " + exc.Message );
						bRollupError = true;
					}
					listDP = new List<DataPoint>();

					foreach( DataPoint dp in listSections ) // For each section
					{
						m_strRow = dp.m_nSection.ToString();

						//Pass in the attribute (so we can figure the column).   
						//Pass in the current datareader for the attribute.
						//Pass the limits of the current section.
						//Pass in a list of points not yet completely consumed.
						//Return new list of points not yet completely consumed.
						//Store this list for next Section
						if( !bRollupError )
						{
							listDP = MakeRow( attribute, dr, dp, listDP );
						}
						else
						{
							m_strRow += "\t";
						}
						tw.WriteLine( m_strRow );
					}
					dr.Close();
					tw.Close();
				}

				//Calculate area for each section
				strOutFile = strMyDocumentsFolder + "\\sectionLRS.txt";
				tw = new StreamWriter( strOutFile );

				foreach( DataPoint dp in listSections ) // For each section
				{
					object[] input = new object[m_listArea.Count];
					int i = 0;
					foreach( String str in m_listArea )
					{
						if( str == "LENGTH" )
						{
							int iExtent = ( int )( ( dp.m_ptsExtent.Y - dp.m_ptsExtent.X ) * 1000 );
							input[i] = ( ( double )iExtent ) / 1000.0;
						}
						else
						{
							if( dp.m_hashAreaAttributeValue.Contains( str ) )
							{
								input[i] = dp.m_hashAreaAttributeValue[str];
							}
							else
							{
								input[i] = double.Parse( m_hashAttributeDefaults[str].ToString() );
							}
						}
						i++;
					}

					try
					{
						object result = m_crArea.RunMethod(input);
						float fArea = Convert.ToSingle(result);
						dp.m_strArea = fArea.ToString();
						m_sectionIDsToArea.Add(dp.m_nSection.ToString(), fArea);


                        int iExtent = (int)((dp.m_ptsExtent.Y - dp.m_ptsExtent.X) * 1000);
                        float fLength  = (float)Math.Abs(((double)iExtent) / 1000.0);
                        m_sectionIDsToLength.Add(dp.m_nSection.ToString(), fLength);


						strRow = dp.m_nSection.ToString() + "\t" + dp.m_strRoutes
													 + "\t" + dp.m_strBegin
													 + "\t" + dp.m_strEnd
													 + "\t" + dp.m_strDirection
													 + "\t" + dp.m_strBegin + "-" + dp.m_strEnd + "(" + dp.m_strDirection + ")\t" + dp.m_strArea + "\t\t";
						if (dp.m_strGeometry != "")
						{
							strRow += dp.m_strGeometry
							+ "\t" + dp.m_strMinX
							+ "\t" + dp.m_strMaxX
							+ "\t" + dp.m_strMinY
							+ "\t" + dp.m_strMaxY;
						}
						else
						{
							strRow += "\t\t\t\t";
						}
						tw.WriteLine(strRow);
					}
					catch(Exception exc)
					{
						RollupMessaging.AddMessge("Error in RunMethod. " + exc.Message);
					}
				}

				tw.Close();
				//Use batch Loader
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad("SECTION_" + strNetworkID.ToString(), strOutFile, '\t');
						break;
					case "ORACLE":
						columnNames.Clear();
						columnNames.Add("SECTIONID");
						columnNames.Add("FACILITY");
						columnNames.Add("BEGIN_STATION");
						columnNames.Add("END_STATION");
						columnNames.Add("DIRECTION");
						columnNames.Add("SECTION");
						columnNames.Add("AREA");
						columnNames.Add("UNITS");
						columnNames.Add("GEOMETRY");
						columnNames.Add("ENVELOPE_MINX");
						columnNames.Add("ENVELOPE_MAXX");
						columnNames.Add("ENVELOPE_MINY");
						columnNames.Add("ENVELOPE_MAXY");
						DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, "SECTION_" + strNetworkID.ToString(), strOutFile, columnNames, "\\t");
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}



				String strMessage = "Linear attribute rollup complete: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" );
				RollupMessaging.AddMessge( strMessage );

                if (apiCall == true)
                {
                    var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Linear attribute rollup complete");
                    Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                }

                strOutFile = strMyDocumentsFolder + "\\" + strSegmentTable + ".txt";
				tw = new StreamWriter( strOutFile );

				TextReader tr;
				Hashtable hashAttributeTextReader = new Hashtable();
				foreach( String attribute in listAttributeInTable )
				{
					strOutFile = strMyDocumentsFolder + "\\" + attribute + ".txt";
					tr = new StreamReader( strOutFile );
					hashAttributeTextReader.Add( attribute, tr );
				}

				string strAttributePartial;
				int nCount = 0;
				//listAttributeInTable.Sort();
				while( nCount < listSections.Count )
				{
					strRow = "";
					foreach( String attribute in listAttributeInTable )
					{
						tr = ( TextReader )hashAttributeTextReader[attribute];
						strAttributePartial = tr.ReadLine();

						if( strRow == "" )
						{
							strRow = strAttributePartial.ToString();
						}
						else
						{
							strRow += strAttributePartial.Substring( strAttributePartial.IndexOf( '\t' ) );
						}
					}
					//Calculate calculated fields for this row.

					strRow = CalculatedFieldRow( strRow );
					tw.WriteLine( strRow );
					nCount++;
				}
				tw.Close();
				strMessage = "Linear Calculated Field rollup complete: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" );
				RollupMessaging.AddMessge( strMessage );

                if (apiCall == true)
                {
                    var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Linear Calculated Field rollup complete");
                    Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                }

                String strMessageLRSBulk = "LRS Bulk Load beginning: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" );
				strOutFile = strMyDocumentsFolder + "\\" + strSegmentTable + ".txt";
				RollupMessaging.AddMessge( strMessageLRSBulk );

				if( listSections.Count > 0 )
				{
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							DBMgr.SQLBulkLoad( strSegmentTable, strOutFile, '\t' );
							break;
						case "ORACLE":
							columnNames.Clear();
							columnNames.Add( "SECTIONID" );
							try
							{
								foreach( String attribute in listAttributeInTable )
								{
									Regex yearSelector = new Regex( "^" + attribute.ToUpper() + "_[1-9][0-9][0-9][0-9]$" );
									string strYearColumns = "SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + strSegmentTable + "' AND COLUMN_NAME LIKE '" + attribute.ToUpper() + "_%' ORDER BY COLUMN_NAME";

									DataSet dsAttributeYears = null;
									dsAttributeYears = DBMgr.ExecuteQuery( strYearColumns );
									foreach( DataRow drAttributeYear in dsAttributeYears.Tables[0].Rows )
									{
										if( yearSelector.IsMatch( drAttributeYear["COLUMN_NAME"].ToString() ) )
										{
											if( !columnNames.Contains( ( string )drAttributeYear["COLUMN_NAME"] ) )
											{
												columnNames.Add( ( string )drAttributeYear["COLUMN_NAME"] );
											}
										}
									}
									if( !columnNames.Contains( attribute ) )
									{
										columnNames.Add( attribute );
									}
								}
								foreach( string calcColumn in m_listCalculatedColumns )
								{
									columnNames.Add( calcColumn );
								}
								DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, strSegmentTable, strOutFile, columnNames, "\\t" );
							}
							catch( Exception ex )
							{
								RollupMessaging.AddMessge( "Error with Oracle bulk load of segment table: " + ex.Message );
							}
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
							//break;
					}
				}


				foreach( String str in m_listAttributes )
				{
					tr = ( TextReader )hashAttributeTextReader[str];
					tr.Close();
				}
				hashAttributeTextReader.Clear();



				//Begin the Section Loop.  This can POTENTIALLY run simulataneously with the 
				//the LRS rollup, though it may have issues with TOO many SQL connections.
				//The section rollup has the same table structure as the linear rollup.
				//Therefore do it inside the same outer loop.

				foreach( String attribute in listAttributeInTable )
				{
					RollupMessaging.AddMessge( "Rolling up Section Attribute:" + attribute + " at " + DateTime.Now.ToString( "HH:mm:ss" ) );

                    if (apiCall == true)
                    {
                        var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Rolling up Section Attribute: " + attribute);
                        Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                    }
                    bRollupError = false;
					ConnectionParameters cp;
					String strSelect;

					if( attribute != "PCI" && attribute != "CLIMATE_PCI" && attribute != "LOAD_PCI" && attribute != "OTHER_PCI" )
					{
						cp = DBMgr.GetAttributeConnectionObject( attribute );
						switch( cp.Provider )
						{
							case "MSSQL":
								strSelect = "SELECT FACILITY,SECTION,SAMPLE_,year(DATE_) AS YEARS,DATA_ FROM " + attribute + " WHERE (FACILITY <> '' AND FACILITY IS NOT NULL) ORDER BY FACILITY,SECTION,SAMPLE_,YEARS";
								break;
							case "ORACLE":
								strSelect = "SELECT FACILITY,SECTION,SAMPLE_,TO_CHAR(DATE_,'YYYY') AS YEARS,DATA_ FROM " + attribute + " WHERE (FACILITY LIKE '_%' AND FACILITY IS NOT NULL) ORDER BY NLSSORT(FACILITY, 'NLS_SORT=LATIN'), NLSSORT(SECTION, 'NLS_SORT=LATIN')";
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for DoRollup()" );
								//break;
						}
                    }
                    else
                    {
                        cp = DBMgr.NativeConnectionParameters;
                        switch (cp.Provider)
                        {
                            case "MSSQL":
                                strSelect = "SELECT FACILITY,SECTION,SAMPLE_,year(DATE_) AS YEARS," + attribute + " AS DATA_,AREA FROM PCI WHERE (FACILITY <> '' AND SAMPLE_TYPE<>'IGNORE' AND FACILITY IS NOT NULL AND SAMPLE_TYPE<>'IGNORE') ORDER BY FACILITY,SECTION,SAMPLE_,YEARS";
                                break;
                            case "ORACLE":
								strSelect = "SELECT FACILITY,SECTION,SAMPLE_,TO_CHAR(DATE_,'YYYY') AS YEARS," + attribute + " AS DATA_,AREA FROM PCI WHERE (FACILITY LIKE '_%' AND AND SAMPLE_TYPE<>'IGNORE' FACILITY IS NOT NULL) ORDER BY NLSSORT(FACILITY, 'NLS_SORT=LATIN'), NLSSORT(SECTION, 'NLS_SORT=LATIN'), SAMPLE, YEARS";
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for DoRollup()");
                                //break;
                        }
                    }
					strOutFile = strMyDocumentsFolder + "\\" + attribute + "_SRS.txt";
                    tw = new StreamWriter(strOutFile);
					try
					{
						dr = new DataReader( strSelect, cp );
					}
					catch( Exception exc )
					{
						RollupMessaging.AddMessge( "Warning: Could not open SRS attribute table " + attribute + ". " + exc.Message );
						bRollupError = true;
					}
					
					///// DEBUG
					//string query = "SELECT SYS_CONTEXT ('USERENV', 'NLS_SORT') FROM DUAL;";
					//DataSet ds = DBMgr.ExecuteQuery(query);
					//string test = ds.Tables[0].Rows[0].ItemArray[0].ToString();
					//System.Diagnostics.Debug.WriteLine(test);

					listDP = new List<DataPoint>();
					foreach( DataPoint dp in listSectionSRS ) // For each section
					{
						m_strRow = dp.m_nSection.ToString();
						//Pass in the attribute (so we can figure the column).   
						//Pass in the current datareader for the attribute.
						//Pass the limits of the current section.
						//Pass in a list of points not yet completely consumed.
						//Return new list of points not yet completely consumed.
						//Store this list for next Section
						if( !bRollupError )
						{
							listDP = MakeSRSRow( attribute, dr, dp, listDP );
						}
						else
						{
							m_strRow += "\t";
						}
						//Write strRow;
						tw.WriteLine( m_strRow );
					}
					dr.Close();
					tw.Close();
				}


				strMessage = "Section attribute rollup complete: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" );
				RollupMessaging.AddMessge( strMessage );

                if (apiCall == true)
                {
                    var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Section attribute rollup complete");
                    Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                }

                strOutFile = strMyDocumentsFolder + "\\SRS_" + strSegmentTable + ".txt";
				tw = new StreamWriter( strOutFile );


				hashAttributeTextReader = new Hashtable();
				foreach( String attribute in listAttributeInTable )
				{
					strOutFile = strMyDocumentsFolder + "\\" + attribute + "_SRS.txt";
					tr = new StreamReader( strOutFile );
					hashAttributeTextReader.Add( attribute, tr );
				}

				nCount = 0;
				while( nCount < listSectionSRS.Count )
				{
					strRow = "";
					foreach( String attribute in listAttributeInTable )
					{
						tr = ( TextReader )hashAttributeTextReader[attribute];
						strAttributePartial = tr.ReadLine();

						if( strRow == "" )
						{
							strRow = strAttributePartial.ToString();
						}
						else
						{
							strRow += strAttributePartial.Substring( strAttributePartial.IndexOf( '\t' ) );
						}
					}
					strRow = CalculatedFieldRow( strRow );
					tw.WriteLine( strRow );
					nCount++;
				}

				tw.Close();
				strMessage = "SRS Calculated Field rollup complete: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" );
				RollupMessaging.AddMessge( strMessage );

				String strMessageSRSBulk = "SRS Bulk Load beginning: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" );
				strOutFile = strMyDocumentsFolder + "\\SRS_" + strSegmentTable + ".txt";
				RollupMessaging.AddMessge( strMessageSRSBulk );

                if (apiCall == true)
                {
                    var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "SRS Bulk Load beginning");
                    Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
                }


                if ( listSectionSRS.Count > 0 )
				{

						switch( DBMgr.NativeConnectionParameters.Provider )
						{
							case "MSSQL":
								DBMgr.SQLBulkLoad( strSegmentTable, strOutFile, '\t' );
								break;
							case "ORACLE":
								//dsmelser  2009.01.05
								//needed for oracle
								columnNames.Clear();
								columnNames.Add("SECTIONID");
								try
								{
									foreach (String attribute in listAttributeInTable)
									{
										Regex yearSelector = new Regex(attribute + "_[1-9][0-9][0-9][0-9]");
										string strYearColumns = "SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + strSegmentTable + "' AND COLUMN_NAME LIKE '" + attribute + "_%'";
										DataSet dsAttributeYears = null;
										dsAttributeYears = DBMgr.ExecuteQuery(strYearColumns);
										foreach (DataRow drAttributeYear in dsAttributeYears.Tables[0].Rows)
										{
											if (yearSelector.IsMatch(drAttributeYear["COLUMN_NAME"].ToString()))
											{
												if (!columnNames.Contains((string)drAttributeYear["COLUMN_NAME"]))
												{
													columnNames.Add((string)drAttributeYear["COLUMN_NAME"]);
												}
											}
										}
										if (!columnNames.Contains(attribute))
										{
											columnNames.Add(attribute);
										}
									}
									DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, strSegmentTable, strOutFile, columnNames, "\\t");
								}
								catch (Exception ex)
								{
									RollupMessaging.AddMessge("Error with Oracle bulk load of segment table: " + ex.Message);
								}
								//dsmelser  2009.01.05
								//needed for oracle
								//throw new NotImplementedException( "TODO: Implement ORACLE version of DoRollup()" );
								//foreach( String attribute in listAttributeInTable )
								//{
								//THIS IS WRONG!  NEED TO ADD COLUMN NAME FOR EACH YEAR!!!!!
								//columnNames.Add( attribute );
								//}
								//DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, strSegmentTable, strOutFile, listAttributeInTable, "\\t" );
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
								//break;
						}

				}

				foreach( String str in m_listAttributes )
				{
					tr = ( TextReader )hashAttributeTextReader[str];
					tr.Close();
				}
				hashAttributeTextReader.Clear();
			}



#endregion

			//#region LRS AND SRS PCI


			////There is only one single Raw PCI table.   It actually contains calculated PCI.
			////If PCI is calculated, do not recalculate.

			//dr = new DataReader( "SELECT * FROM PCI" );

			//foreach( DataPoint dp in listSections )
			//{
			//    //Get
			//}
			//dr.Close();
			//#endregion

			SummarizeImageLocation(strMyDocumentsFolder);
			CleanCommittedOnSectionNumberChange( strNetworkID );
			RollupMessaging.AddMessge( "End rollup of network: " + strNetwork + " at " + DateTime.Now.ToString( "HH:mm:ss" ) );

            if (apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                .Set(s => s.rollupStatus, "End rollup of network: " + strNetwork);
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);
            }

            if (bRollupError && apiCall == true)
            {
                var updateStatus = Builders<RollupModel>.Update
                    .Set(s => s.rollupStatus, "Rollup aborted");
                Rollup.UpdateOne(s => s.networkId == Convert.ToInt32(m_strNetworkID), updateStatus);

                return;
            }
		}



        private void CreateCalculatedAssets()
        {
            //Retrieve List of Calculated Asset Tables
            List<CalculatedAssetObject> listAssets = GlobalDatabaseOperations.GetCalculatedAssets();

            List<String> listTables = new List<string>();

            //Retrieve all AssetTables.
            foreach (CalculatedAssetObject asset in listAssets)
            {
                if (!listTables.Contains(asset.Asset))
                {
                    listTables.Add(asset.Asset);
                }
            }
            //Drop unique table.
            foreach (String strTable in listTables)
            {
                String strDrop = "DROP TABLE " + strTable;
                try
                {
					DBMgr.ExecuteNonQuery( strDrop );
					//switch( DBMgr.NativeConnectionParameters.Provider )
					//{
					//    case "MSSQL":
					//        break;
					//    case "ORACLE":
					//        DBMgr.ExecuteNonQuery( DBMgr.GenerateOracleSequenceDrop( strTable ) );
					//        break;
					//    default:
					//        throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
					//        break;
					//}
               
                }
                catch (Exception exc)
                {
                    RollupMessaging.AddMessge("Warning: Drop table " + strTable + " failed. " + exc.Message);
                }

            }


            foreach (String strTable in listTables)
            {
                String strCalculatedTable = strTable + "_CALCULATE";
                //RollupMessaging.AddMessge("Creating and populating Calculated Asset Table:" + strCalculatedTable);
                List<DatabaseManager.TableParameters> listColumn = new List<DatabaseManager.TableParameters>();
                listColumn.Add(new DatabaseManager.TableParameters("ID", DataType.Int, false, true, false));
                List<CalculatedAssetObject> listCalculatedAsset = listAssets.FindAll((
					delegate(CalculatedAssetObject a) 
						{
							return a.Asset == strTable; 
						}));
                List<String> listProperty = new List<String>();

                foreach (CalculatedAssetObject a in listCalculatedAsset)
                {
                    if (!listProperty.Contains(a.CalculatedProperty))
                    {
                        DataType dataType = null;
                        if (a.Type == "NUMBER")
                        {
                            dataType = DataType.Float;
                        }
                        else
                        {
                            dataType = DataType.VarChar(-1);
                        }

                        listColumn.Add(new DatabaseManager.TableParameters(a.CalculatedProperty, dataType, false, false, false));
                    }
                    a.Calculate = new CalculateEvaluate.CalculateEvaluate();
                    a.Calculate.BuildTemporaryClass(a.Equation, true);
                    a.Evaluate = new CalculateEvaluate.CalculateEvaluate();
                    a.Evaluate.BuildTemporaryClass(a.Criteria, false);
                }
                try
                {
                    DBMgr.CreateTable(strCalculatedTable, listColumn);
                }
                catch (Exception exception)
                {
                    RollupMessaging.AddMessge("Error: Creating Calculated Asset table " + strCalculatedTable +". " + exception.Message);
                    continue;
                }

                Hashtable hashColumnType = new Hashtable();
                List<String> listColumnsAsset = DBMgr.GetTableColumns(strTable);
                foreach (String strColumn in listColumnsAsset)
                {
                    String strType = DBMgr.IsColumnTypeString(strTable, strColumn);
                    hashColumnType.Add(strColumn, strType);
                }

                
                
                String strSelect = "SELECT * FROM " + strTable + " ORDER BY ID";
                DataReader dr = new DataReader(strSelect);
                while(dr.Read())
                {
                    
                    foreach (CalculatedAssetObject a in listCalculatedAsset)
                    {
                        object[] inputs = new object[a.Evaluate.Parameters.Count];
                        int nIndex = 0;
                        foreach (String strParameter in a.Evaluate.Parameters)
                        {
                            String strType = hashColumnType[strParameter].ToString();
                            if (strType == "STRING")
                            {
                                inputs[nIndex] = dr[strParameter].ToString();
                            }
                            else
                            {
                                inputs[nIndex] = (double)dr[strParameter];
                            }
                            nIndex++;
                        }
						try
						{


							bool bResult = (bool)a.Evaluate.RunMethod(inputs);

							if (bResult)
							{
								object[] parameters = new object[a.Calculate.Parameters.Count];
								nIndex = 0;
								foreach (String strParameter in a.Calculate.Parameters)
								{
									String strType = hashColumnType[strParameter].ToString();
									if (strType == "STRING")
									{
										parameters[nIndex] = dr[strParameter].ToString();
									}
									else
									{
										parameters[nIndex] = (double)dr[strParameter];
									}
									nIndex++;
								}

								String strResult = a.Calculate.RunMethod(parameters).ToString();
							}
						}
						catch(Exception exc)
						{
							RollupMessaging.AddMessge("Error in RunMethod. " + exc.Message);
						}
                    }
                }
                dr.Close();
           }
           







        }

        private void SummarizeImageLocation(String strMyDocumentFolder)
        {
            //Summarize ImageLocationTable
            List<string> listFacilitySectionDirection = new List<string>();
            String strOutFile = strMyDocumentFolder + "\\DistinctFacility.txt";
            TextWriter tw = new StreamWriter(strOutFile);
            bool bOutput = false;
            DataReader dr = new DataReader("SELECT FACILITY,SECTION,DIRECTION,PRECEDENT FROM IMAGELOCATION ORDER BY PRECEDENT");
            String strFacilityLast = "";
            String strSectionLast = "";
            String strDirectionLast = "";
            String strFacility;
            String strSection;
            String strDirection;
            int nOrder = 0;
            
            while (dr.Read())
            {
                strFacility = dr["FACILITY"].ToString();
                strSection = dr["SECTION"].ToString();
                strDirection = dr["DIRECTION"].ToString();

                string strFacilitySectionDirection = strFacility + strSection + strDirection;
                if (listFacilitySectionDirection.Contains(strFacilitySectionDirection)) continue;
                listFacilitySectionDirection.Add(strFacilitySectionDirection);


                if (strSection != strSectionLast || strDirection != strDirectionLast || strFacility != strFacilityLast)
                {
                    tw.WriteLine(strFacility + "\t" + strSection + "\t" + strDirection + "\t" + nOrder.ToString());
                    strSectionLast = strSection;
                    strFacilityLast = strFacility;
                    strDirectionLast = strDirection;
                    nOrder++;
                }


                
                bOutput = true;
            }
            tw.Close();
            dr.Close();




            if (bOutput)
            {
                String strDelete = "DELETE FROM IMAGE_FACILITY";
                DBMgr.ExecuteNonQuery(strDelete);

                List<string> columnNames = new List<string>();
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad("IMAGE_FACILITY", strOutFile, '\t');
						break;
					case "ORACLE":
						columnNames.Clear();
						columnNames.Add("FACILITY");
						DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, "IMAGE_FACILITY", strOutFile, columnNames, "\\t");
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
            }
        }

        private bool AddPCIAttributes(string strNetworkID)
        {
            //Only add PCI if there is PCI data
            String strSelect = "SELECT COUNT(PCI)FROM PCI";
            //DataSet ds = DBMgr.ExecuteQuery(strSelect);
            //int nCount = (int)ds.Tables[0].Rows[0].ItemArray[0];
			int nCount = DBMgr.ExecuteScalar( "SELECT COUNT(PCI)FROM PCI" );
            if (nCount == 0) return false;

            //Now get rollup method for PCI
            String strRollupType = "";
            String strFormat = "f1";
            DataSet dsRollupPCI = DBMgr.ExecuteQuery("SELECT ROLLUPTYPE FROM ROLLUP_CONTROL WHERE NETWORKID = '" + strNetworkID + "' AND ATTRIBUTE_ = 'PCI'");
            if (dsRollupPCI.Tables[0].Rows.Count == 1)
            {
                strRollupType = dsRollupPCI.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            else
            {
                return false;
            }

            // Add PCI to SEGMENT_ table

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    strSelect = "SELECT DISTINCT year(DATE_) [Year] FROM PCI ORDER BY [YEAR] ASC";
                    break;
                case "ORACLE":
                    strSelect = "SELECT DISTINCT TO_CHAR(DATE_,'YYYY') AS Year FROM PCI ORDER BY YEAR ASC";
                    break;
                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for DoRollup()");
                    //break;
            }
            DataSet dsYears = DBMgr.ExecuteQuery(strSelect);
            List<String> listYear = new List<String>();
            foreach (DataRow dr in dsYears.Tables[0].Rows)
            {
                String strYear = dr[0].ToString();
                listYear.Add(strYear);
            }
            // Add PCI Related Attributes
            Attributes attributePCI = new Attributes();
            attributePCI.m_strAttribute = "PCI";
            attributePCI.m_bNative = true;
            attributePCI.m_strType = "NUMBER";
            attributePCI.m_strFormat = strFormat;
            attributePCI.m_strRollupType = strRollupType;
            attributePCI.m_listYears = listYear;
            m_nCountNumber += (listYear.Count + 1);
            m_listAttributes.Add(attributePCI.m_strAttribute);
            m_hashAttribute.Add(attributePCI.m_strAttribute, attributePCI);

            attributePCI = new Attributes();
            attributePCI.m_strAttribute = "CLIMATE_PCI";
            attributePCI.m_bNative = true;
            attributePCI.m_strType = "NUMBER";
            attributePCI.m_strFormat = strFormat;
            attributePCI.m_strRollupType = strRollupType;
            attributePCI.m_listYears = listYear;
            m_nCountNumber += (listYear.Count + 1);
            m_listAttributes.Add(attributePCI.m_strAttribute);
            m_hashAttribute.Add(attributePCI.m_strAttribute, attributePCI);

            attributePCI = new Attributes();
            attributePCI.m_strAttribute = "LOAD_PCI";
            attributePCI.m_bNative = true;
            attributePCI.m_strType = "NUMBER";
            attributePCI.m_strFormat = strFormat;
            attributePCI.m_strRollupType = strRollupType;
            attributePCI.m_listYears = listYear;
            m_nCountNumber += (listYear.Count + 1);
            m_listAttributes.Add(attributePCI.m_strAttribute);
            m_hashAttribute.Add(attributePCI.m_strAttribute, attributePCI);

            attributePCI = new Attributes();
            attributePCI.m_strAttribute = "OTHER_PCI";
            attributePCI.m_bNative = true;
            attributePCI.m_strType = "NUMBER";
            attributePCI.m_strFormat = strFormat;
            attributePCI.m_strRollupType = strRollupType;
            attributePCI.m_listYears = listYear;
            m_nCountNumber += (listYear.Count + 1);
            m_listAttributes.Add(attributePCI.m_strAttribute);
            m_hashAttribute.Add(attributePCI.m_strAttribute, attributePCI);

            return true;
        }
        /// <summary>
        /// Retrieves the number of section if this table previously existed
        /// </summary>
        /// <param name="strNetworkID"></param>
        /// <returns></returns>
		private int GetNumberPreviousSections(string strNetworkID)
		{
			int nNumberSection = 0;
			String strSelect = "SELECT COUNT(SECTIONID) AS NUMBER_SECTIONS FROM SECTION_" + strNetworkID;
			//This is the worst way to do this.
			//try
			//{
			DataSet ds = DBMgr.ExecuteQuery(strSelect);
			String strNumber = ds.Tables[0].Rows[0].ItemArray[0].ToString();
			nNumberSection = int.Parse(strNumber);
			//}
			//catch
			//{
			//    nNumberSection = 0;
			//}
			return nNumberSection;
		}
        /// <summary>
        /// If number of sections change, the committed project file is wrong.
        /// TODO: Still need check to see if the sections are identical if number of sections does not change.
        /// </summary>
        /// <param name="strNetworkID"></param>
        private void CleanCommittedOnSectionNumberChange(String strNetworkID)
        {

            int nNumberSection = 0;
            String strSelect = "SELECT COUNT(SECTIONID) AS NUMBER_SECTIONS FROM SECTION_" + strNetworkID;
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                String strNumber = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                nNumberSection = int.Parse(strNumber);
            }
            catch
            {
                nNumberSection = 0;
            }

            if (nNumberSection != m_nPreviousNumberSections)
            {
                strSelect = "SELECT SIMULATIONID FROM SIMULATIONS WHERE NETWORKID=" + strNetworkID;
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        String strSimulationID = dr["SIMULATIONID"].ToString();
                        String strDelete = "DELETE FROM COMMITTED_ WHERE SIMULATIONID=" + strSimulationID;
                        DBMgr.ExecuteNonQuery(strDelete);
                    }
                }
                catch
                {
                    RollupMessaging.AddMessge("Error: Dropping Committed Projects on number of Section change.");
                }
            }
        }






        private string CalculatedFieldRow(string strRow)
        {
			m_listCalculatedColumns.Clear();
            String strOutputValue;
            //string[] values = strRow.Split(',');
			string[] values = strRow.Split('\t');
            
            foreach(String strCalculatedAttribte in m_listCalculated)//Loop through all calculated fields
            {
                 Attributes attribute = (Attributes)m_hashCalculated[strCalculatedAttribte]; //Get all the information for a single calculated field
                //Loop through years
                foreach(String strYear in attribute.m_listYears)//Get all years that are being calculated.
                {
					m_listCalculatedColumns.Add( attribute.m_strAttribute.ToUpper() + "_" + strYear );
                    //Loop through Criteria (non-blank first).
                    strOutputValue = SingleCalculatedValue(values,strYear, attribute);
                    // We have calculated a CALCULATED_FIELD for a calculatedField_Year
                    //strRow += ",";
					strRow += "\t";
                    if (strOutputValue != null)
                    {
                        strRow += strOutputValue;
                    }
                }
                // Now repeat for value were there is no year

				m_listCalculatedColumns.Add( attribute.m_strAttribute.ToUpper() );

                 strOutputValue = SingleCalculatedValue(values,"", attribute);
                // We have calculated a CALCULATED_FIELD for a calculatedField_Year
                //strRow += ",";
				 strRow += "\t";
                if (strOutputValue != null)
                {
                    strRow += strOutputValue;
                }
            }
            return strRow;
        }

        private String SingleCalculatedValue(string[] values, String strYear,Attributes attribute)
        {
            //Loop through Criteria (non-blank first).
            String strOutputValue = null;
            AttributesCalculated acDefault = null;
            foreach (AttributesCalculated ac in attribute.m_listAttributesCalculated) //Go through each criteria/equation pair
            {
                if (ac.Criteria.Trim() == "")
                {
                    acDefault = ac;
                    continue;
                }
                if (ac.Criteria.Trim() != "")//Non-blank critera
                {
                    object[] inputs = new object[ac.CriteriaAttributes.Count];
                    int i = 0; //Index for inputs
                    bool bAllInputsAvailable = true;
                    foreach (String strCriteriaAttributes in ac.CriteriaAttributes)
                    {
						if (strCriteriaAttributes == "AREA")
						{
							double dArea = (double)m_sectionIDsToArea[values[0]];
							inputs[i] = dArea;
							i++;
						}
                        if (strCriteriaAttributes == "LENGTH")
                        {
                            double dLength= (double)m_sectionIDsToLength[values[0]];
                            inputs[i] = "LENGTH";
                            i++;
                        }
						else
						{
							String strColumn = strCriteriaAttributes + "_" + strYear;
							if (strYear == "")
							{
								strColumn = strCriteriaAttributes;
							}
							int nIndex = m_listColumnSegment.FindIndex(delegate(TableParameters tp) { return tp.GetColumnName() == strColumn; });
							if (nIndex < 0)
							{
								bAllInputsAvailable = false;
								continue;
							}
							Attributes criteriaAttribute = (Attributes)m_hashAttribute[strCriteriaAttributes];
							if (criteriaAttribute.m_strType == "STRING")
							{
								inputs[i] = values[nIndex].ToString();
								i++;
							}
							else
							{
								if (String.IsNullOrEmpty(values[nIndex]))
								{
									bAllInputsAvailable = false;
									continue;
								}
								inputs[i] = double.Parse(values[nIndex].ToString());
								i++;
							}
						}
                    }
                    if (bAllInputsAvailable)
                    {
						try
						{
							bool bCriteriaMet = (bool)ac.Evaluate.RunMethod(inputs);
							if (bCriteriaMet)
							{
								if (strOutputValue != null)
								{
									RollupMessaging.AddMessge("Warning: Multiple valid criteria for " + attribute.m_strAttribute + " section: " + values[0].ToString() + ". " + ac.Criteria.Replace("|", "'") + " discarded.");
								}
								object[] inputEquation = new object[ac.EquationAttributes.Count];
								bool bEquationInputsAvailable = true;
								int j = 0;
								foreach (String strEquationAttribute in ac.EquationAttributes)
								{
									if (strEquationAttribute == "AREA")
									{
										double dArea = (double)m_sectionIDsToArea[values[0]];
										inputEquation[j] = dArea;
										j++;
									}
                                    else if (strEquationAttribute == "LENGTH")
                                    {
                                        double dLength = (double)m_sectionIDsToLength[values[0]];
                                        inputEquation[j] = dLength;
                                        j++;
                                    }
                                    else
                                    {
                                        String strColumn = strEquationAttribute + "_" + strYear;
                                        if (strYear == "")
                                        {
                                            strColumn = strEquationAttribute;
                                        }
                                        int nIndex = m_listColumnSegment.FindIndex(delegate(TableParameters tp) { return tp.GetColumnName() == strColumn; });
                                        
                                        if(strColumn == "SAI" &&  String.IsNullOrEmpty(values[nIndex]))
                                        {
                                            values[nIndex] = "100";
                                        }

                                            
                                        if (nIndex < 0)
                                        {
                                            bEquationInputsAvailable = false;
                                            continue;
                                        }
                                        if (String.IsNullOrEmpty(values[nIndex]))
                                        {
                                            bEquationInputsAvailable = false;
                                            continue;
                                        }


                                        Attributes equationAttribute = (Attributes)m_hashAttribute[strEquationAttribute];
                                        if (equationAttribute.m_strType == "STRING")
                                        {
                                            inputEquation[j] = values[nIndex].ToString();
                                            j++;
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(values[nIndex]))
                                            {
                                                bEquationInputsAvailable = false;
                                                continue;
                                            }
                                            inputEquation[j] = double.Parse(values[nIndex].ToString());
                                            j++;
                                        }
                                    }
								}
								if (bEquationInputsAvailable)
								{
									object result = ac.Calculate.RunMethod(inputEquation);
									if (result != null)
									{
										double dOut = (double)result;
										strOutputValue = dOut.ToString(attribute.m_strFormat, CultureInfo.InvariantCulture);
									}
								}
							}
						}
						catch(Exception exc)
						{
							RollupMessaging.AddMessge("Error with RunMethod." + exc.Message);
						}
                    }
                }
            }
            if (strOutputValue == null)//Must apply default
            {
                if (acDefault != null)
                {
                    object[] inputEquation = new object[acDefault.EquationAttributes.Count];
                    bool bEquationInputsAvailable = true;
                    int j = 0;
                    foreach (String strEquationAttribute in acDefault.EquationAttributes)
                    {
						if (strEquationAttribute == "AREA")
						{
							double dArea = (double)m_sectionIDsToArea[values[0]];
							inputEquation[j] = dArea;
							j++;
						}
                        else if (strEquationAttribute == "LENGTH")
                        {
                            double dLength = (double)m_sectionIDsToLength[values[0]];
                            inputEquation[j] = dLength;
                            j++;
                        }
						else
						{
							String strColumn = strEquationAttribute.ToUpper() + "_" + strYear;
							if (strYear == "")
							{
								strColumn = strEquationAttribute;
							}
							int nIndex = m_listColumnSegment.FindIndex(delegate(TableParameters tp) { return tp.GetColumnName() == strColumn; });


                            if (strColumn.Contains("SN") && String.IsNullOrEmpty(values[nIndex]))
                            {
                                values[nIndex] = "100";
                            }

							if (nIndex < 0)
							{
								bEquationInputsAvailable = false;
								continue;
							}
							if (String.IsNullOrEmpty(values[nIndex]))
							{
								bEquationInputsAvailable = false;
								continue;
							}

                            Attributes equationAttribute = (Attributes)m_hashAttribute[strEquationAttribute];
                            if (equationAttribute.m_strType == "STRING")
                            {
                                inputEquation[j] = values[nIndex].ToString();
                                j++;
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(values[nIndex]))
                                {
                                    bEquationInputsAvailable = false;
                                    continue;
                                }
                                inputEquation[j] = double.Parse(values[nIndex].ToString());
                                j++;
                            }
						}
					}
						
                    if (bEquationInputsAvailable)
                    {
						try
						{
							object result = acDefault.Calculate.RunMethod(inputEquation);
							if (result != null)
							{
								strOutputValue = result.ToString();
							}
						}
						catch(Exception exc)
						{
							RollupMessaging.AddMessge("Error in RunMethod. " + exc.Message);
						}
                    }
                }
            }
            return strOutputValue;
        }



        private void AddCalculatedFieldColumns()
        {
            //Retrieve a list of all calculated fields ATTRIBUTES.CALCULATED = 1
            m_listCalculated = new List<String>();
            m_hashCalculated = new Hashtable();

            String strSelect = "SELECT ATTRIBUTE_,TYPE_,FORMAT FROM ATTRIBUTES_ WHERE CALCULATED=1 AND ATTRIBUTE_<>'PCI' AND ATTRIBUTE_<>'CLIMATE_PCI' AND ATTRIBUTE_<>'LOAD_PCI' AND ATTRIBUTE_<>'OTHER_PCI'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String strAttribute = dr["ATTRIBUTE_"].ToString();
                String strType = dr["TYPE_"].ToString();
				String strFormat = dr["FORMAT"].ToString();

                Attributes attributes = new Attributes();
                m_hashCalculated.Add(strAttribute, attributes);
                attributes.m_strAttribute = strAttribute;
                attributes.m_strType = strType;
				attributes.m_strFormat = strFormat;
                attributes.m_listAttributesCalculated = new List<AttributesCalculated>();
                attributes.m_listEquationCriteriaAttributes = new List<String>();
                attributes.m_listEquationAttributes = new List<String>();
                attributes.m_listCriteriaAttributes = new List<String>();
                m_listCalculated.Add(strAttribute);

                String strSelectCalculated = "SELECT EQUATION,CRITERIA FROM ATTRIBUTES_CALCULATED WHERE ATTRIBUTE_='" + strAttribute + "'";
                DataSet dsCalculated = DBMgr.ExecuteQuery(strSelectCalculated);
                foreach(DataRow drCalculated in dsCalculated.Tables[0].Rows)
                {
                    String strEquation = drCalculated["EQUATION"].ToString();
                    String strCriteria = drCalculated["CRITERIA"].ToString();

					string strAtCriteria = strCriteria;
					foreach (String str in ParseAttribute(strCriteria))
					{
						if(m_hashAttribute.Contains(str))
						{
							Attributes attributeParsed = (Attributes)m_hashAttribute[str];
							if(attributeParsed.m_strType == "STRING")
							{
								strAtCriteria = strAtCriteria.Replace("[" + str + "]", "[@" + str + "]");
							}
						}
					}
                    string strAtEquation = strEquation;
                    foreach (String str in ParseAttribute(strEquation))
                    {
                        if (m_hashAttribute.Contains(str))
                        {
                            Attributes attributeParsed = (Attributes)m_hashAttribute[str];
                            if (attributeParsed.m_strType == "STRING")
                            {
                                strAtEquation = strAtEquation.Replace("[" + str + "]", "[@" + str + "]");
                            }
                        }
                    }

					AttributesCalculated attributesCalculated = new AttributesCalculated(strAtEquation, strAtCriteria);
                    attributes.m_listAttributesCalculated.Add(attributesCalculated);
        
                    //Into each of these store a list of FIELDS necessary TO calculate (from EQUATION and CRITERIA).  Use the Attributes class.
                    foreach(String str in ParseAttribute(strEquation))
                    {
                        if(!attributesCalculated.EquationAttributes.Contains(str))
                        {
                            attributesCalculated.EquationAttributes.Add(str);
                        }
                        if(!attributes.m_listEquationCriteriaAttributes.Contains(str))
                        {
                            attributes.m_listEquationCriteriaAttributes.Add(str);
                        }
                        if(!attributes.m_listEquationAttributes.Contains(str))
                        {
                            attributes.m_listEquationAttributes.Add(str);
                        }
                    }
                    
                    foreach(String str in ParseAttribute(strCriteria))
                    {
                        if(!attributesCalculated.CriteriaAttributes.Contains(str))
                        {
                            attributesCalculated.CriteriaAttributes.Add(str);
                        }
                        if(!attributes.m_listEquationCriteriaAttributes.Contains(str))
                        {
                            attributes.m_listEquationCriteriaAttributes.Add(str);
                        }
                        if(!attributes.m_listCriteriaAttributes.Contains(str))
                        {
                            attributes.m_listCriteriaAttributes.Add(str);
                        }
                    }
                    attributes.m_listEquationCriteriaAttributes.Sort();
                }
            }

            m_listCalculated.Sort();

            List<int> listYears = new List<int>();
            //Make a list of all possible year combination (m_hashAttributes)
            foreach(String strKey in m_hashAttribute.Keys)
            {
                Attributes attribute = (Attributes)m_hashAttribute[strKey];
                foreach(String strYear in attribute.m_listYears)
                {
                    if (strYear != "")
                    {
                        if (!listYears.Contains(int.Parse(strYear)))
                        {
                            listYears.Add(int.Parse(strYear));
                        }
                    }
                }
            }
            listYears.Sort();
            
            //Combine needed attribute with possible year.  If all are available.  The CALCULATED_YEAR is add to SEGMENT table.
			List<string> calculatedFieldErrors = new List<string>();
            foreach(String strCalculated in m_listCalculated)
            {
                Attributes attributeCalculate = (Attributes)m_hashCalculated[strCalculated];
                attributeCalculate.m_listYears = new List<string>();
                foreach(int nYear in listYears)
                {
                    bool bAllAvailable = true;
                    foreach(String strEquationCriteriaAttributes in attributeCalculate.m_listEquationCriteriaAttributes)
                    {
						if (strEquationCriteriaAttributes != "AREA" && strEquationCriteriaAttributes != "LENGTH")
						{
							String strAttributeYear = strEquationCriteriaAttributes + "_" + nYear.ToString();
							TableParameters tpFound = null;
							tpFound = m_listColumnSegment.Find(delegate(TableParameters tp) { return tp.GetColumnName() == strAttributeYear.ToUpper(); });
							if (tpFound == null)
							{
								bAllAvailable = false;
							}
						}
                    }
                    if(bAllAvailable)
                    {
                        attributeCalculate.m_listYears.Add(nYear.ToString());
                        String strColumn = strCalculated + "_" + nYear.ToString();
                        m_listColumnSegment.Add(new DatabaseManager.TableParameters(strColumn.ToUpper(), DataType.Float, true));//Calculated Field individual years
                        m_nCountNumber++;
					}
					else
					{
						//RollupMessaging.AddMessge("Warning: Attribute " + strCalculated + " not available for calculation.");
					}

                }
                m_listColumnSegment.Add(new DatabaseManager.TableParameters(strCalculated.ToUpper(), DataType.Float, true));//Calculated Field without years
                m_nCountNumber++;
            }
        }

        private List<String> ParseAttribute(String strQuery)
        {
            List<String> list = new List<String>();
            //            strQuery = strQuery.ToUpper();
            String strAttribute;
            int nOpen = -1;

            for (int i = 0; i < strQuery.Length; i++)
            {
                if (strQuery.Substring(i, 1) == "[")
                {
                    nOpen = i;
                    continue;
                }

                if (strQuery.Substring(i, 1) == "]" && nOpen > -1)
                {
                    //Get the value between open and (i)
                    strAttribute = strQuery.Substring(nOpen + 1, i - nOpen - 1);

                    if (!list.Contains(strAttribute))
                    {
                        if (m_listAttributes.Contains(strAttribute) || strAttribute == "AREA" || strAttribute=="LENGTH")
                        {
                            list.Add(strAttribute);
                        }
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// Adds all data for a given attribute for a specific section and date (year).
        /// For LRS analysis.
        /// </summary>
        /// <param name="strAttribute">Attribute being rolled up.  From the ATTRIBUTES table</param>
        /// <param name="dr">Datareader that is being pointed.  Overload this function for OleDB</param>
        /// <param name="dp">Information about current ID</param>
        /// <param name="listdp">Contains data read from DataReader that has yet to be consumed (rolled up)</param>
        /// <returns>List of unused data from DataReader</returns>
        private List<DataPoint> MakeRow(String strAttribute, DataReader dr, DataPoint dp, List<DataPoint> listdp)
        {
            // listdp should contain list of unused or partially used data points.
            // It should be ordered ROUTES,DIRECTION,BEGIN,YEARS  
            // Therefore if ROUTES > list[0].m_ROUTES, remove datapoint.  Data has been entered for 
            // a ROUTE that was not sectioned.
            m_nIndexData = -1;
            m_nIndexReader = -1;
            bool bNumber = true;
            Attributes attribute = (Attributes)m_hashAttribute[strAttribute];
            if(attribute.m_strType == "STRING") bNumber = false;

			try
			{
				if (m_nIndexData < 0)
				{
					m_nIndexData = m_listRoutes.IndexOf(dp.m_strRoutes);
				}
				else if (m_listRoutes[m_nIndexData] != dp.m_strRoutes)
				{
					m_nIndexData = m_listRoutes.IndexOf(dp.m_strRoutes);
				}
			}
			catch
			{
				m_nIndexData = m_listRoutes.IndexOf(dp.m_strRoutes);
			}

            if (listdp == null) listdp = new List<DataPoint>();
            bool bContinue = true;
            bool bInsertBlank = false;
            while (listdp.Count > 0 && bContinue) //If there is something on the list from previous DataRead
            {
                DataPoint dp0 = (DataPoint)listdp[0];
                // int nCompare = listdp[0].m_strRoutes.Replace('-','*').CompareTo(dp.m_strRoutes.Replace('-','*'));
                if (m_nIndexReader < 0)
                {
                    m_nIndexReader = m_listRoutes.IndexOf(dp0.m_strRoutes);
                }
                else if (m_listRoutes[m_nIndexReader] != dp0.m_strRoutes)
                {
                    m_nIndexReader = m_listRoutes.IndexOf(dp0.m_strRoutes);
                }



//                int nCompare = CompareInfo.GetCompareInfo("en-US").Compare(listdp[0].m_strRoutes, dp.m_strRoutes, CompareOptions.StringSort);
//                if (nCompare < 0)//Route has not be previously handled
                if(m_nIndexReader == -1 || m_nIndexReader < m_nIndexData)
                {
                    listdp.RemoveAt(0);
                    continue;
                }
                //If Next Route is GREATER than the Current route, we need to insert a blank and go on.
//                nCompare = CompareInfo.GetCompareInfo("en-US").Compare(listdp[0].m_strRoutes, dp.m_strRoutes, CompareOptions.StringSort);
//                if (nCompare > 0)
                if(m_nIndexData <m_nIndexReader)
                {
                    bInsertBlank = true;
                    break;
                }

                //If Route is the same and direction is less than current, remove previous data.
                if (listdp[0].m_strRoutes == dp.m_strRoutes && listdp[0].m_strDirection.CompareTo(dp.m_strDirection) < 0)
                {
                    listdp.RemoveAt(0);
                    continue;
                }

                //If Route, direction is the same check if end data (Y) is less than begin (X) current section
                if (listdp[0].m_strRoutes == dp.m_strRoutes &&
                    listdp[0].m_strDirection == dp.m_strDirection &&
                    listdp[0].m_ptsExtent.Y <= dp.m_ptsExtent.X)
                {
                    listdp.RemoveAt(0);
                    continue;
                }

                //At this point all previous points that will not be used (wrong route, wrong station, wrong direction)
                //Have been removed.
                //The only thing remaining on this list are potentially valid DataPoints
                bContinue = false;
            }

            //Now we need to start reading new data.  Read until the extents do not match.

             if (!bInsertBlank)
            {
                bContinue = true;
                while (bContinue)
                {
                    //Fill new DataPoint && add to list. It will be dropped next pass.
                    if (!dr.Read())
                    {
                        bContinue = false;
                        continue;
                    }
                    DataPoint pt;
                    if (bNumber)
                    {
						String str;
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								str = dr["ID_"].ToString();
								break;
							case "ORACLE":
								str = dr["ID_"].ToString();
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
						try
						{
							pt = new DataPoint(dr["ROUTES"].ToString(),
												dr["BEGIN_STATION"].ToString(),
												dr["END_STATION"].ToString(),
												dr["DIRECTION"].ToString(),
												dr["YEARS"].ToString(),
												dr["DATA_"].ToString());
						}
						catch
						{
							// Ignore bad points
							continue;
						}
                    }
                    else
                    {
						try
						{
							pt = new DataPoint(dr["ROUTES"].ToString(),
												dr["BEGIN_STATION"].ToString(),
												dr["END_STATION"].ToString(),
												dr["DIRECTION"].ToString(),
												dr["YEARS"].ToString(),
												dr["DATA_"].ToString());
						}
						catch
						{
							// Ignore bad points
							continue;
						}
                    }
                    listdp.Add(pt);
					
//                    int nCompare = CompareInfo.GetCompareInfo("en-US").Compare(pt.m_strRoutes, dp.m_strRoutes, CompareOptions.StringSort);
//                    if (nCompare > 0) //Route is greater than the one we ar looking for.

					try
					{
						if (m_nIndexReader < 0)
						{
							m_nIndexReader = m_listRoutes.IndexOf(pt.m_strRoutes);
						}
						else if (m_listRoutes[m_nIndexReader] != pt.m_strRoutes)
						{
							m_nIndexReader = m_listRoutes.IndexOf(pt.m_strRoutes);
						}
					}
					catch
					{
						m_nIndexReader = m_listRoutes.IndexOf(pt.m_strRoutes);
					}



                    if (m_nIndexReader == -1)
                    {
                        bContinue = true;
                        continue;
                    }

                    if (m_nIndexData < 0)
                    {
                        bContinue = false;
                        continue;
                    }
                    
                    if(m_nIndexReader > m_nIndexData)
                    {
                        bContinue = false;
                        continue;
                    }
                    //Routes match, but new larger direction.
                    if (pt.m_strRoutes == dp.m_strRoutes && pt.m_strDirection.CompareTo(dp.m_strDirection) > 0)
                    {
                        bContinue = false;
                        continue;
                    }

                    //Routes and direction match, but begin new data >= end section.
                    if (pt.m_strRoutes == dp.m_strRoutes &&
                        pt.m_strDirection == dp.m_strDirection &&
                        pt.m_ptsExtent.X >= dp.m_ptsExtent.Y)
                    {
                        bContinue = false;
                        continue;
                    }

                }

            }
            //Current list for this section (all years) should now be complete.  Time to do 
            // some rollup magic (average, median, etc.)

            // This list will contain all years.  For each attribute we will need to add a space 
            // for each year in the attibute list whether or not there is data for it (null entry).
            // DO NOT insert 0 or "" (blank) for null data
            // Sort by years.

            String strValue;
            String strLastValue = "";
            foreach (String strYearInList in attribute.m_listYears)
            {
                List<DataPoint> list = new List<DataPoint>();
                foreach (DataPoint data in listdp)
                {
                    if (data.m_strYear == strYearInList)
                    {
                        list.Add(data);

                    }
                }

                //Now have a list of data for a specific section AND a specific year.  
                //Combining it is all that remains
                strValue = "";
				
                if (bNumber)
                {

                    strValue = RollupNumber(list,dp,attribute);
                    if (strValue.Trim().Length > 0) strLastValue = strValue;

                }
                else
                {
                    strValue = RollupString(list, dp, attribute);
                    if (strValue.Trim().Length > 0) strLastValue = strValue;
                }
                if (m_strRow.Length > 0) m_strRow += "\t";
                m_strRow += strValue;

            }
            if (m_strRow.Length > 0) m_strRow += "\t";
            m_strRow += strLastValue;



            //Check and see if this attribute is part of the [AREA].

            if (m_listArea.Contains(strAttribute))
            {
                if (strLastValue != "")
                {
                    dp.SetAreaAttribute(strAttribute, strLastValue);
                }
            }


            return listdp;
        }



        /// <summary>
        /// Adds all data for a given attribute for a specific section and date (year).
        /// For SRS analysis.
        /// </summary>
        /// <param name="strAttribute">Attribute being rolled up.  From the ATTRIBUTES table</param>
        /// <param name="dr">Datareader that is being pointed.  Overload this function for OleDB</param>
        /// <param name="dp">Information about current ID</param>
        /// <param name="listdp">Contains data read from DataReader that has yet to be consumed (rolled up)</param>
        /// <returns>List of unused data from DataReader</returns>
        private List<DataPoint> MakeSRSRow(String strAttribute, DataReader dr, DataPoint dp, List<DataPoint> listdp)
        {
            // listdp should contain list of unused or partially used data points.
            // It should be ordered FACILITY,SECTION,SAMPLE,YEARS  
            // Therefore if FACILITY > list[0].FACILITY, remove datapoint.  Data has been entered for 
            // a ROUTE that was not sectioned.

            bool bNumber = false;
            Attributes attribute = (Attributes)m_hashAttribute[strAttribute];
            if (attribute.m_strType == "NUMBER") bNumber = true;


            if (listdp == null) listdp = new List<DataPoint>();
            bool bContinue = true;
            bool bBlankInsert = false;
            while (listdp.Count > 0 && bContinue) //If there is something on the list from previous DataRead
            {
                DataPoint dp0 = (DataPoint)listdp[0];


                int nCompare = CompareInfo.GetCompareInfo("en-US").Compare(listdp[0].m_strRoutes, dp.m_strRoutes, CompareOptions.StringSort);
                if (nCompare < 0)//FACILITY is stored in the ROUTES variable.
                {
                    //Since list sorted by database in alphanumeric order. As are sections.  If true this data not needed.
                    listdp.RemoveAt(0);
                    continue;
                }

                if(nCompare > 0) // The Route is past this route.
                {
                    bBlankInsert = true;
                    break;
                }

                //If Route (FACILITY) is the same and section is less than current, remove previous data.
                nCompare = CompareInfo.GetCompareInfo("en-US").Compare(listdp[0].m_strSection, dp.m_strSection, CompareOptions.StringSort);
                if (nCompare < 0)
                {
                    listdp.RemoveAt(0);
                    continue;

                }
                //Unlike LRS, for SRS if ROUTE(FACILITY) and SECTION match, we are done removing previous.
                //At this point all previous points that will not be used (wrong route(FACILITY), wrong SECTION)
                //Have been removed.
                //The only thing remaining on this list are potentially valid DataPoints
                bContinue = false;
            }

            //Now we need to start reading new data.  Read until the extents do not match.

            if (!bBlankInsert)
            {
                bContinue = true;
                while (bContinue)
                {
                    //Fill new DataPoint && add to list. It will be dropped next pass.
                    if (!dr.Read())
                    {
                        bContinue = false;
                        continue;
                    }
                    //The false is here in case we add an IGNORE field.
					DataPoint pt = null;


                    if (strAttribute != "PCI" && strAttribute != "CLIMATE_PCI" && strAttribute != "LOAD_PCI" && strAttribute != "OTHER_PCI")
                    {
                        pt = new DataPoint(dr["FACILITY"].ToString(),
                           dr["SECTION"].ToString(),
                           dr["SAMPLE_"].ToString(), "1", // The one is here if we add sample area to SRS
                           dr["YEARS"].ToString(),
                           dr["DATA_"].ToString(), false);
                    }
                    else
                    {
                         pt = new DataPoint(dr["FACILITY"].ToString(),
										dr["SECTION"].ToString(),
                                        dr["SAMPLE_"].ToString(), dr["AREA"].ToString(), // The one is here if we add sample area to SRS
										dr["YEARS"].ToString(),
										dr["DATA_"].ToString(), false);
                    }
                    listdp.Add(pt);

                    //Check if should read another
                    int nCompare = CompareInfo.GetCompareInfo("en-US").Compare(pt.m_strRoutes, dp.m_strRoutes, CompareOptions.StringSort);
                    if (nCompare > 0) //Route is greater than the one we ar looking for.
                    {
                        bContinue = false;
                        continue;
                    }
                    //Routes match, but new larger direction.
                    nCompare = CompareInfo.GetCompareInfo("en-US").Compare(pt.m_strSection, dp.m_strSection, CompareOptions.StringSort);
                    if (pt.m_strRoutes == dp.m_strRoutes && nCompare < 0)
                    {
                        bContinue = false;
                        continue;
                    }
                }

            }

            //Current list for this section (all years) should now be complete.  Time to do 
            // some rollup magic (average, median, etc.)

            // This list will contain all years.  For each attribute we will need to add a space 
            // for each year in the attibute list whether or not there is data for it (null entry).
            // DO NOT insert 0 or "" (blank) for null data
            // Sort by years.

            String strValue;
            String strLastValue = "";
            foreach (String strYearInList in attribute.m_listYears)
            {
                List<DataPoint> list = new List<DataPoint>();
                foreach (DataPoint data in listdp)
                {
                    if (data.m_strYear == strYearInList)
                    {
                        list.Add(data);

                    }
                }

                //Now have a list of data for a specific section AND a specific year.  
                //Combining it is all that remains
                strValue = "";
                if (bNumber)
                {

                    strValue = RollupSRSNumber(list, dp, attribute);
                    if (strValue.Trim().Length > 0) strLastValue = strValue;

                }
                else
                {
                    strValue = RollupSRSString(list, dp, attribute);
                    if (strValue.Trim().Length > 0) strLastValue = strValue;

                }
                if (m_strRow.Length > 0) m_strRow += "\t";
                m_strRow += strValue;

            }
            if (m_strRow.Length > 0) m_strRow += "\t";
            m_strRow += strLastValue;
            return listdp;
        }


        /// <summary>
        /// Rollup a NUMBER value for a specific LRS section for a specific date (year) 
        /// </summary>
        /// <param name="list">list of data for this year section.  Last entry on list may be first entry for next section</param>
        /// <param name="dpSection">Boundarys (route, begin, end, direction) for section</param>
        /// <param name="attribute">Attribute being rolled up.  Is it STRING or NUMBER</param>
        /// <returns>Value for a specific Attribute, Section and Date (year)</returns>
        private String RollupNumber(List<DataPoint> list, DataPoint dpSection, Attributes attribute)
        {
            String strNumber="";
            float fX;
            float fY;
            bool bAtLeastOne = false;
            float fMinimum;
            float fMaximum;
            float fAverage;
            float fCount;
            Hashtable hashPredominant;
            List<float> listMedian;
            float fExtent = 0;
            float fMaxExtent = 0;
            float fPredominat = 0;
            float fData;
            List<double> listValues;
            switch (attribute.m_strRollupType)
            {
                case "None":
                    strNumber = "";
                    break;
                case "Predominant": //Predominant
                    hashPredominant = new Hashtable();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;

                        fData = float.Parse(pt.m_strData);
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            bAtLeastOne = true;
                            if (hashPredominant.Contains(fData))
                            {
                                fExtent = (float)hashPredominant[fData];
                                hashPredominant.Remove(fData);
                            }
                            else
                            {
                                fExtent = 0;
                            }
                            fExtent += (fY-fX);
                            hashPredominant.Add(fData, fExtent);
                        }
                    }
                    fExtent = 0;
                    foreach (DictionaryEntry de in hashPredominant)
                    {
                        if ((float)de.Value > fMaxExtent)
                        {
                            fMaxExtent = (float)de.Value;
                            fPredominat = (float)de.Key;
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fPredominat.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";

                    break;
                case "Average": //Average
                    fAverage = 0;
                    fCount = 0;
                    
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
						try
						{
							fData = float.Parse(pt.m_strData);
							if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
							else fX = dpSection.m_ptsExtent.X;

							if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
							else fY = dpSection.m_ptsExtent.Y;

							if ((fY - fX) > 0)
							{
								fAverage += fData * (fY - fX);
								fCount += (fY - fX);
							}
						}
						catch (Exception exc)
						{
							RollupMessaging.AddMessge("Error: " + exc.Message + ". At " + pt.m_strRoutes + ", " + pt.m_strSection + ", " + pt.m_strBegin + ", " + pt.m_strEnd);
						}
                    }
                    if (fCount > 0)
                    {
                        fAverage = fAverage / fCount;
                        strNumber = fAverage.ToString(attribute.m_strFormat);
                    }
                    else
                    {
                        strNumber = "";
                    }
                    break;
                case "Count":
                    int nCount = 0;
                    
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
						try
						{
                            nCount++;
						}
						catch (Exception exc)
						{
							RollupMessaging.AddMessge("Error: " + exc.Message + ". At " + pt.m_strRoutes + ", " + pt.m_strSection + ", " + pt.m_strBegin + ", " + pt.m_strEnd);
						}
                    }
                        
                    strNumber = nCount.ToString();
                    break;
 
                case "Median": //Median
                    //Create list of all floats.
                    //Create hash of floats + lengths
                    //Sort float list.
                    //Iterate through once and get total length.
                    //Iterate again so that the halfway mark can be found.
                    hashPredominant = new Hashtable();
                    listMedian = new List<float>();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        fData = float.Parse(pt.m_strData);
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        
                        if ((fY - fX) > 0)
                        {
                            if (!listMedian.Contains(fData)) listMedian.Add(fData);
                            bAtLeastOne = true;
                            if (hashPredominant.Contains(fData))
                            {
                                fExtent = (float)hashPredominant[fData];
                                hashPredominant.Remove(fData);
                            }
                            else
                            {
                                fExtent = 0;
                            }
                            fExtent += (fY-fX);
                            hashPredominant.Add(fData, fExtent);
                        }
                    }
                    if (listMedian.Count > 0)
                    {
                        listMedian.Sort();
                        fExtent = 0;
                        // Iterate through list and find Median extent.
                        foreach (float f in listMedian)
                        {
                            fExtent += float.Parse(hashPredominant[f].ToString());
                        }
                        float fMedian = fExtent / 2;

                        // Iterate and find half way point.
                        fExtent = 0;
                        foreach (float f in listMedian)
                        {
                            fExtent += float.Parse(hashPredominant[f].ToString());
                            if (fExtent > fMedian)
                            {
                                if (fExtent == 0) strNumber = f.ToString(attribute.m_strFormat);
                            }
                        }
                    }
                    else
                    {
                        strNumber = "";
                    }
                    break;
                case "Maximum": //Maximum

                    bAtLeastOne = false;
                    fMaximum = float.MinValue;

                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        fData = float.Parse(pt.m_strData);
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            if (fData > fMaximum)
                            {
                                fMaximum = fData;
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMaximum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;
                case "Minimum":// Minimum
                    fMinimum = float.MaxValue;
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        fData = float.Parse(pt.m_strData);
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            if (fData < fMinimum)
                            {
                                fMinimum = fData;
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMinimum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;
                case "Absolute Maximum":// Absolute value maximum
                    bAtLeastOne = false;
                    fMaximum = 0;
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        fData = float.Parse(pt.m_strData);
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            if (Math.Abs(fData) > fMaximum)
                            {
                                fMaximum = Math.Abs(fData);
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMaximum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;
                case "Absolute Minimum":// Absolute value minimum
                    fMinimum = float.MaxValue;
                    bAtLeastOne = false;

                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        fData = float.Parse(pt.m_strData);
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            if (Math.Abs(fData) < fMinimum)
                            {
                                fMinimum = Math.Abs(fData);
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMinimum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;
                case "Standard Deviation":
                    listValues = new List<double>();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            listValues.Add(double.Parse(pt.m_strData));
                        }
                    }
                    double dStandardDeviation = CalculateStandardDeviation(listValues);
                    strNumber = dStandardDeviation.ToString(attribute.m_strFormat);
                    break;
                case "Sum":
                    listValues = new List<double>();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;

                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            listValues.Add(double.Parse(pt.m_strData));
                        }
                    }
                    double dSum = CalculateSum(listValues);
                    strNumber = dSum.ToString(attribute.m_strFormat);
                    break;

                case "First": //First
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            if (String.IsNullOrEmpty(strNumber))
                            {
                                strNumber = pt.m_strData;
                            }
                        }
                    }
                    break;

                case "Last": //Last
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            strNumber = pt.m_strData;
                        }
                    }
                    break;

                case "Weighted"://Weighted TODO:
                    strNumber = "";
                    break;
                default:
                    strNumber = "";
                    break;
            }
            return strNumber;
        }
    

        /// <summary>
        /// Rollup a STRING value for a specific LRS section for a specific date (year)  
        /// </summary>
        /// <param name="list">list of data for this year section.  Last entry on list may be first entry for next section</param>
        /// <param name="dpSection">Boundarys (route, begin, end, direction) for section</param>
        /// <param name="attribute">Attribute being rolled up.  Is it STRING or NUMBER</param>
        /// <returns>Value for a specific Attribute, Section and Date (year)</returns>
        private String RollupString(List<DataPoint> list, DataPoint dpSection, Attributes attribute)
        {
            String strNumber="";
            float fX;
            float fY;
            bool bAtLeastOne = false;
            Hashtable hashPredominant;
            float fExtent = 0;
            float fMaxExtent = 0;


            switch (attribute.m_strRollupType)
            {
                case "None":
                    strNumber = "";
                    break;
                case "Predominant": //Predominant
                    hashPredominant = new Hashtable();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            bAtLeastOne = true;
                            if (hashPredominant.Contains(pt.m_strData))
                            {
                                fExtent = (float)hashPredominant[pt.m_strData];
                                hashPredominant.Remove(pt.m_strData);
                            }
                            else
                            {
                                fExtent = 0;
                            }
                            fExtent += (fY-fX);
                            hashPredominant.Add(pt.m_strData, fExtent);
                        }
                    }
                    fExtent = 0;
                    foreach (DictionaryEntry de in hashPredominant)
                    {
                        if ((float)de.Value > fMaxExtent)
                        {
                            fMaxExtent = (float)de.Value;
                            strNumber = de.Key.ToString();
                        }
                    }
                    if (!bAtLeastOne)
                        strNumber = "";

                    break;
                case "Count":
                    int nCount = 0;
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        try
                        {
                            nCount++;
                        }
                        catch (Exception exc)
                        {
                            RollupMessaging.AddMessge("Error: " + exc.Message + ". At " + pt.m_strRoutes + ", " + pt.m_strSection + ", " + pt.m_strBegin + ", " + pt.m_strEnd);
                        }
                    }

                    strNumber = nCount.ToString();
                    break;
                case "First": //First
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            if (String.IsNullOrEmpty(strNumber))
                            {
                                strNumber = pt.m_strData;
                            }
                        }
                    }
                    break;

                case "Last": //Last
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strData == "") continue;
                        if (pt.m_strRoutes != dpSection.m_strRoutes) continue;
                        if (pt.m_strDirection != dpSection.m_strDirection) continue;
                        if (pt.m_ptsExtent.X >= dpSection.m_ptsExtent.Y) continue;
                        if (pt.m_ptsExtent.X > dpSection.m_ptsExtent.X) fX = pt.m_ptsExtent.X;
                        else fX = dpSection.m_ptsExtent.X;

                        if (pt.m_ptsExtent.Y < dpSection.m_ptsExtent.Y) fY = pt.m_ptsExtent.Y;
                        else fY = dpSection.m_ptsExtent.Y;

                        if ((fY - fX) > 0)
                        {
                            strNumber = pt.m_strData;
                        }
                    }
                    break;

                default:
                    strNumber = "";
                    break;
            }
            return strNumber;
        }


        /// <summary>
        /// Rollup a NUMBER value for a specific SRS section for a specific date (year) 
        /// </summary>
        /// <param name="list">list of data for this year section.  Last entry on list may be first entry for next section</param>
        /// <param name="dpSection">Boundarys (route, begin, end, direction) for section</param>
        /// <param name="attribute">Attribute being rolled up.  Is it STRING or NUMBER</param>
        /// <returns>Value for a specific Attribute, Section and Date (year)</returns>
        private String RollupSRSNumber(List<DataPoint> list, DataPoint dpSection, Attributes attribute)
        {
            String strNumber = "";
            bool bAtLeastOne = false;
            float fMinimum;
            float fMaximum;
            float fAverage;
            float fCount;
            float fArea;
            Hashtable hashPredominant;
            List<float> listMedian;
            float fExtent = 0;
            float fMaxExtent = 0;
            float fPredominat = 0;
            float fData;
            List<double> listValues;
            switch (attribute.m_strRollupType)
            {
                case "None":
                    strNumber = "";
                    break;
                case "Predominant": //Predominant
                    hashPredominant = new Hashtable();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        fData = float.Parse(pt.m_strData);
                        fArea = float.Parse(pt.m_strArea.ToString());
                        if (fArea > 0)
                        {
                            bAtLeastOne = true;
                            if (hashPredominant.Contains(fData))
                            {
                                fExtent = (float)hashPredominant[fData];
                                hashPredominant.Remove(fData);
                            }
                            else
                            {
                                fExtent = 0;
                            }
                            fExtent += fArea;
                            hashPredominant.Add(fData, fExtent);
                        }
                    }
                    fExtent = 0;
                    foreach (DictionaryEntry de in hashPredominant)
                    {
                        if ((float)de.Value > fMaxExtent)
                        {
                            fMaxExtent = (float)de.Value;
                            fPredominat = (float)de.Key;
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fPredominat.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";

                    break;
                case "Average": //Average
                    fAverage = 0;
                    fCount = 0;

                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        fData = float.Parse(pt.m_strData);
                        fArea = float.Parse(pt.m_strArea.ToString());
                        if ( fArea > 0)
                        {
                            fAverage += fData * fArea;
                            fCount += fArea;
                        }
                    }
                    if (fCount > 0)
                    {
                        fAverage = fAverage / fCount;
                        strNumber = fAverage.ToString(attribute.m_strFormat);
                    }
                    else
                    {
                        strNumber = "";
                    }
                    break;
                case "Count":
                    int nCount = 0;
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        try
                        {
                            nCount++;
                        }
                        catch (Exception exc)
                        {
                            RollupMessaging.AddMessge("Error: " + exc.Message + ". At " + pt.m_strRoutes + ", " + pt.m_strSection + ", " + pt.m_strBegin + ", " + pt.m_strEnd);
                        }
                    }
                    strNumber = nCount.ToString();
                    break;
                case "Median": //Median
                    //Create list of all floats.
                    //Create hash of floats + lengths
                    //Sort float list.
                    //Iterate through once and get total length.
                    //Iterate again so that the halfway mark can be found.
                    hashPredominant = new Hashtable();
                    listMedian = new List<float>();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        fData = float.Parse(pt.m_strData);
                        fArea = float.Parse(pt.m_strArea.ToString());
                        if (fArea > 0)
                        {
                            if (!listMedian.Contains(fData)) listMedian.Add(fData);
                            bAtLeastOne = true;
                            if (hashPredominant.Contains(fData))
                            {
                                fExtent = (float)hashPredominant[fData];
                                hashPredominant.Remove(fData);
                            }
                            else
                            {
                                fExtent = 0;
                            }
                            fExtent += fArea;
                            hashPredominant.Add(fData, fExtent);
                        }
                    }
                    if (listMedian.Count > 0)
                    {
                        listMedian.Sort();
                        fExtent = 0;
                        // Iterate through list and find Median extent.
                        foreach (float f in listMedian)
                        {
                            fExtent += float.Parse(hashPredominant[f].ToString());
                        }
                        float fMedian = fExtent / 2;

                        // Iterate and find half way point.
                        fExtent = 0;
                        foreach (float f in listMedian)
                        {
                            fExtent += float.Parse(hashPredominant[f].ToString());
                            if (fExtent > fMedian)
                            {
                                if (fExtent == 0) strNumber = f.ToString(attribute.m_strFormat);
                            }
                        }
                    }
                    else
                    {
                        strNumber = "";
                    }
                    break;
                case "Maximum": //Maximum

                    bAtLeastOne = false;
					fMaximum = float.MinValue;

                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        fData = float.Parse(pt.m_strData);
                        fArea = float.Parse(pt.m_strArea.ToString());
                        if (fArea > 0)
                        {
                            if (fData > fMaximum)
                            {
                                fMaximum = fData;
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMaximum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;
                case "Minimum":// Minimum
                    fMinimum = float.MaxValue; //float.Parse("1.0e+99");
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        fData = float.Parse(pt.m_strData);
                        fArea = float.Parse(pt.m_strArea.ToString());
                        if (fArea > 0)
                        {
                            if (fData < fMinimum)
                            {
                                fMinimum = fData;
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMinimum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;
                case "Absolute Maximum":// Absolute value maximum
                    bAtLeastOne = false;
                    fMaximum = 0;
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        fData = float.Parse(pt.m_strData);
                        fArea = float.Parse(pt.m_strArea.ToString());
                        if (fArea > 0)
                        {
                            if (Math.Abs(fData) > fMaximum)
                            {
                                fMaximum = Math.Abs(fData);
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMaximum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;
                case "Absolute Minimum":// Absolute value minimum
                    fMinimum = float.Parse("1.0e+99");
                    bAtLeastOne = false;

                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        fData = float.Parse(pt.m_strData);
                        fArea = float.Parse(pt.m_strArea.ToString());
                        if (fArea > 0)
                        {
                            if (Math.Abs(fData) < fMinimum)
                            {
                                fMinimum = Math.Abs(fData);
                                bAtLeastOne = true;
                            }
                        }
                    }
                    if (bAtLeastOne)
                        strNumber = fMinimum.ToString(attribute.m_strFormat);
                    else
                        strNumber = "";
                    break;

                case "Standard Deviation":
                    listValues = new List<double>();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        {
                            listValues.Add(double.Parse(pt.m_strData));
                        }
                    }
                    double dStandardDeviation = CalculateStandardDeviation(listValues);
                    strNumber = dStandardDeviation.ToString(attribute.m_strFormat);
                    break;
                case "Sum":
					if (list.Count != 0)
					{
						listValues = new List<double>();
						foreach (DataPoint pt in list)
						{
							if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
							if (pt.m_strData == "") continue;
							{
								listValues.Add(double.Parse(pt.m_strData));
							}
						}
						if (listValues.Count > 0)
						{
							double dSum = CalculateSum(listValues);
							strNumber = dSum.ToString(attribute.m_strFormat);
						}
						else
						{
							strNumber = "";
						}
					}
					else
					{
						strNumber = "";
					}
                    break;
                case "Weighted"://Weighted TODO:
                    strNumber = "";
                    break;

                case "First": //First
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        if (String.IsNullOrEmpty(strNumber))
                        {
                            strNumber = pt.m_strData;
                        }
                    }
                    break;

                case "Last": //Last
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        strNumber = pt.m_strData;
                    }
                    break;




                default:
                    strNumber = "";
                    break;
            }
            return strNumber;
        }

        private double CalculateStandardDeviation(List<double> listValues)
        {
            if (listValues.Count < 2) return 0;

            double sumOfDerivation = 0;  
            double average = 0;
            foreach (double value in listValues)
            {
                average += value;
                sumOfDerivation += (value) * (value);
            }
            average /= (double)listValues.Count;
            double sumOfDerivationAverage = sumOfDerivation / listValues.Count;  
            return Math.Sqrt(sumOfDerivationAverage - (average*average));  
        }

        private double CalculateSum(List<double> listValues)
        {
            double sum = 0;
            foreach (double value in listValues)
            {
                sum += value;
            }
            return sum;
        }

        /// <summary>
        /// Rollup a STRING value for a specific SRS section for a specific date (year)  
        /// </summary>
        /// <param name="list">list of data for this year section.  Last entry on list may be first entry for next section</param>
        /// <param name="dpSection">Boundarys (route, begin, end, direction) for section</param>
        /// <param name="attribute">Attribute being rolled up.  Is it STRING or NUMBER</param>
        /// <returns>Value for a specific Attribute, Section and Date (year)</returns>
        private String RollupSRSString(List<DataPoint> list, DataPoint dpSection, Attributes attribute)
        {
            String strNumber = "";
            float fArea;
            bool bAtLeastOne = false;
            Hashtable hashPredominant;
            float fExtent = 0;
            float fMaxExtent = 0;


            switch (attribute.m_strRollupType)
            {
                case "None":
                    strNumber = "";
                    break;
                case "Predominant": //Predominant
                    hashPredominant = new Hashtable();
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue; 
                        if (pt.m_strData == "") continue;
                        fArea = float.Parse(pt.m_strArea);
                        if (fArea > 0)
                        {
                            bAtLeastOne = true;
                            if (hashPredominant.Contains(pt.m_strData))
                            {
                                fExtent = (float)hashPredominant[pt.m_strData];
                                hashPredominant.Remove(pt.m_strData);
                            }
                            else
                            {
                                fExtent = 0;
                            }
                            fExtent += fArea;
                            hashPredominant.Add(pt.m_strData, fExtent);
                        }
                    }
                    fExtent = 0;
                    foreach (DictionaryEntry de in hashPredominant)
                    {
                        if ((float)de.Value > fMaxExtent)
                        {
                            fMaxExtent = (float)de.Value;
                            strNumber = de.Key.ToString();
                        }
                    }
                    if (!bAtLeastOne)
                        strNumber = "";

                    break;
                case "Count":
                    int nCount = 0;
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        try
                        {
                            nCount++;
                        }
                        catch (Exception exc)
                        {
                            RollupMessaging.AddMessge("Error: " + exc.Message + ". At " + pt.m_strRoutes + ", " + pt.m_strSection + ", " + pt.m_strBegin + ", " + pt.m_strEnd);
                        }
                    }
                    strNumber = nCount.ToString();
                    break;

                case "First": //First
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        if (String.IsNullOrEmpty(strNumber))
                        {
                            strNumber = pt.m_strData;
                        }
                    }
                    break;

                case "Last": //Last
                    strNumber = "";
                    foreach (DataPoint pt in list)
                    {
                        if (pt.m_strRoutes != dpSection.m_strRoutes || pt.m_strSection != dpSection.m_strSection) continue;
                        if (pt.m_strData == "") continue;
                        strNumber = pt.m_strData;
                    }
                    break;


                default:
                    strNumber = "";
                    break;
            }
            return strNumber;
        }

        private String CalculatePCI(String strMethod, String strArea, Hashtable hashDistress)
        {
            //hashDistress key = H_1   object = extent
            String strPCI = "";
            String strLevel;
            String strDistress;
            double dLargeDeductLimit = 5.0;

            if (strMethod == "acmpr")
            {
                strMethod = "ac.mpr";
                dLargeDeductLimit = 2.0;
            }
            else if (strMethod == "pccmpr")
            {
                strMethod = "pcc.mpr";
                dLargeDeductLimit = 2.0;
            }
            else if (strMethod == "acfaa")
            {
                strMethod = "ac.faa";
            }
            else if (strMethod == "pccfaa")
            {
                strMethod = "pcc.faa";
            }

            List<double> listDeduct = new List<double>();



            foreach (DictionaryEntry de in hashDistress)
            {
                strLevel = de.Key.ToString().Substring(0, 1);
                strDistress = de.Key.ToString().Substring(2);
                int nDistress = int.Parse(strDistress);

                double dAmount = double.Parse(de.Value.ToString());
                double dArea = double.Parse(strArea);

                double dDeduct = PCI.Distress.pvt_ComputePCIDeduct(nDistress, strLevel, dAmount, dArea);
                listDeduct.Add(double.Parse(dDeduct.ToString()));
            }
            listDeduct.Sort();
            String strDeduct = "";

            for (int i = listDeduct.Count - 1; i >= 0; i--) 
            {
                strDeduct += listDeduct[i].ToString();
                strDeduct += ",";
            }
            strDeduct += "0";

            double dPCI = 100 - PCI.Distress.pciCorrectedDeductValue(strMethod, strDeduct, dLargeDeductLimit);
            //strPCI = dPCI.ToString("#");


            return strPCI;
        }

        /// <summary>
        /// Compiles equation for calculating area.  To calculate area for ALL LRS sections, default values must be available.  This function compiles and loads necessary defaults.
        /// </summary>
        /// <returns></returns>
        private bool CompileAreaEquation()
        {
            //private CalculateEvaluate.CalculateEvaluate m_crArea;//Area equation to be evaluated.
            //private List<String> m_listArea;//Area variables
            //rivate Hashtable m_hashAttributeDefaults; //List of default values for all attributes.  Needed to calculate area.

            //Fill default hashtable
            DataSet ds;
            String strSelect = "SELECT ATTRIBUTE_,DEFAULT_VALUE FROM ATTRIBUTES_ WHERE TYPE_='NUMBER'";
            try
            {
				//RollupMessaging.AddMessge("Running ATTRIBUTE_ query");
                ds = DBMgr.ExecuteQuery(strSelect);
                m_hashAttributeDefaults = new Hashtable();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strAttribute = row["ATTRIBUTE_"].ToString();
                    String strDefault = row["DEFAULT_VALUE"].ToString();
                    m_hashAttributeDefaults.Add(strAttribute, strDefault);
                }
            }
            catch (Exception exception)
            {
                RollupMessaging.AddMessge("Error in filling default value table for area calculation." + exception.Message);
                return false;
            }


            //Compile AREA equation
            String strQuery = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME='AREA_CALCULATION'";
			String strArea = null;
            try
            {
				//RollupMessaging.AddMessge("Running OPTION_VALUE query");
                ds = DBMgr.ExecuteQuery(strQuery);
				strArea = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (Exception exception)
            {
                RollupMessaging.AddMessge("Error: Error in compiling AREA function. SQL message - " + exception.Message);
                return false;
            }
            
            
            m_crArea = new CalculateEvaluate.CalculateEvaluate();
			//RollupMessaging.AddMessge("Building C/E Class");
            m_crArea.BuildTemporaryClass(strArea, true);
			//RollupMessaging.AddMessge("Comiling C/E Assembly");
            m_crArea.CompileAssembly();
            if (m_crArea.m_listError.Count > 0)
            {
                RollupMessaging.AddMessge("Error: Error in compiling AREA function. SQL message - " + m_crArea.m_listError[0].ToString());
                return false;
            }
            m_listArea = new List<String>();

			//RollupMessaging.AddMessge("Calculating AREA");
            string[] listAreaParameters = strArea.Split(']');
            for (int i = 0; i < listAreaParameters.Length; i++)
            {
                if(listAreaParameters[i].Contains("["))
                {
                    m_listArea.Add(listAreaParameters[i].Substring(listAreaParameters[i].IndexOf('[') + 1));
                }
            }

            foreach (String str in m_listArea)
            {
                if (str != "LENGTH" && str != "AREA")
                {
                    if (!m_hashAttributeDefaults.Contains(str))
                    {
                        RollupMessaging.AddMessge("Error: Default values must be set for all inputs used to calculate area.  Default is not set for attribute:" + str);
                        return false;
                    }
                }
            }
			//RollupMessaging.AddMessge("Successfully calculated AREA");
            return true;
        }
    }

    public class AttributesCalculated
    {
        private String m_strEquation;
        private String m_strCritera;
        private CalculateEvaluate.CalculateEvaluate m_calculate;
        private CalculateEvaluate.CalculateEvaluate m_evaluate;
        private List<String> m_listCriteriaAttributes = new List<String>();
        private List<String> m_listEquationAttributes = new List<String>();

        public String Equation
        {
            get { return m_strEquation; }
        }

        public String Criteria
        {
            get { return m_strCritera; }
        }

        public CalculateEvaluate.CalculateEvaluate Calculate
        {
            get { return m_calculate; }
        }
        
        public CalculateEvaluate.CalculateEvaluate Evaluate
        {
            get { return m_evaluate; }
        }

        public List<String> CriteriaAttributes
        {
            get { return m_listCriteriaAttributes; }
        }

        public List<String> EquationAttributes
        {
            get { return m_listEquationAttributes; }
        }

        public AttributesCalculated(String strEquation, String strCriteria)
        {
            m_strEquation = strEquation;
            m_strCritera = strCriteria;

            if (m_strEquation.Trim() != "")
            {
				m_calculate = new CalculateEvaluate.CalculateEvaluate();
				m_calculate.BuildTemporaryClass(m_strEquation, true);
				m_calculate.CompileAssembly();
            }

            if (m_strCritera.Trim() != "")
            {
				m_evaluate = new CalculateEvaluate.CalculateEvaluate();
				m_evaluate.BuildTemporaryClass(m_strCritera, false);
				m_evaluate.CompileAssembly();
            }
        }
    }

    /// <summary>
    /// Stores information for RoadCare attributes.  Gives information on how attribute is constructed and
    /// where to get raw table.
    /// </summary>
    public class Attributes
    {
        public String m_strAttribute;
        public String m_strType;
        public bool m_bNative;
        public String m_strFormat;
        public List<String> m_listYears;
        public String m_strRollupType;//From ROLLUP_CONTROL table
        public List<String> m_listEquationCriteriaAttributes;
        public List<String> m_listEquationAttributes;
        public List<String> m_listCriteriaAttributes;
        public List<AttributesCalculated> m_listAttributesCalculated;
    }

    public class DataPoint
    {
        public int m_nSection;
        public String m_strRoutes;
        public PointF m_ptsExtent;
        public String m_strDirection;
        public String m_strYear;
        public String m_strData;
        public String m_strSection;
        public String m_strSample;
        public String m_strArea;
        public String m_strUnit;
        public bool m_bIgnore = false;
        public String m_strBegin;
        public String m_strEnd;
        public Hashtable m_hashAreaAttributeValue = new Hashtable();
        public String m_strGeometry;
        public String m_strMinX;
        public String m_strMaxX;
        public String m_strMinY;
        public String m_strMaxY;

        public override string ToString()
        {
            if(m_strSection != null)
            {
                return m_strRoutes + " " + m_strSection + " " + m_strData;
            }
            else
            {
                return m_strRoutes + "(" + m_strBegin + "-" + m_strEnd + ")" + m_strDirection + " " + m_strData;
            }
        }

        public DataPoint(String strSectionID, String strFacility, String strSection, String strArea, String strUnit)
        {
            m_nSection = int.Parse(strSectionID.ToString());
            m_strRoutes = strFacility;
            m_strSection = strSection;
            m_strArea = strArea;
            m_strUnit = strUnit;
        }

        public DataPoint(int nSectionID, String strRoutes, String strBegin, String strEnd, String strDirection, String strGeometry, String strMinX,String strMaxX,String strMinY,String strMaxY)
        {
            m_nSection = nSectionID;
            m_strRoutes = strRoutes;
            m_ptsExtent = new PointF(float.Parse(strBegin), float.Parse(strEnd));
            m_strDirection = strDirection;
            m_strBegin = strBegin;
            m_strEnd = strEnd;
            m_strGeometry = strGeometry;
            m_strMaxX = strMaxX;
            m_strMinX = strMinX;
            m_strMaxY = strMaxY;
            m_strMinY = strMinY;
        }


        public DataPoint(String strRoutes, String strBegin, String strEnd, String strDirection, String strYear, String strData)
        {
            m_strRoutes = strRoutes;
            m_ptsExtent = new PointF(float.Parse(strBegin), float.Parse(strEnd));
            m_strDirection = strDirection;
            m_strYear = strYear;
            m_strData = strData;
        }

        public DataPoint(String strFacility, String strSection, String strSample, String strArea, String strYear, String strData,bool bIgnore)
        {
            m_strRoutes = strFacility;
            m_strSection = strSection;
            m_strSample = strSample;
            m_strYear = strYear;
            m_strData = strData;
            m_bIgnore = false;
            m_strArea = strArea;
        }

        public void SetAreaAttribute(String strAttribute, String strValue)
        {
            try
            {
                double dValue = double.Parse(strValue);
                m_hashAreaAttributeValue.Add(strAttribute, dValue);
            }
            catch
            {
                return;
            }
        }
    }

	public class MultiSharpLine
	{
		List<SharpLine> m_sharpLines = new List<SharpLine>();
		Geometry m_geoToReturn;

		public Geometry MultiLineGeometry
		{
			get { return m_geoToReturn; }
		}

		public MultiSharpLine(List<SharpLine> sharpLines, string beginStationToCompare, string endStationToCompare, List<DataPoint> listSections)
		{
			double begin_station = double.Parse(beginStationToCompare);
			double end_station = double.Parse(endStationToCompare);
			m_sharpLines = sharpLines;
			int indexBegin = -1;
			int indexEnd = -1;
			string geometry = "";

			for (int i = 0; i < m_sharpLines.Count; i++)
			{
				SharpLine sl = sharpLines[i];
				if (begin_station >= sl.Begin_Seg && begin_station < sl.End_Seg)
				{
					indexBegin = i;
				}
				if (end_station > sl.Begin_Seg && end_station <= sl.End_Seg)
				{
					indexEnd = i;
				}
			}
			// Logical break occurs entirely within one LINESTRING
			if (indexBegin == indexEnd)
			{
				geometry = sharpLines[indexBegin].GetLineSegment(beginStationToCompare, endStationToCompare);
			}
			else
			{
				string multiLineString = "MULTILINESTRING(";
				for (int i = indexBegin; i <= indexEnd; i++)
				{
					if (i == indexBegin)
					{
						// Parse out sharpline for beginning
						geometry = sharpLines[indexBegin].GetLineSegment(beginStationToCompare, sharpLines[indexBegin].End_Seg.ToString());
						geometry = geometry.Replace("LINESTRING", "");
						multiLineString += geometry + ", ";
					}
					else if (i == indexEnd)
					{
						// Parse sharpline for the end
						geometry = sharpLines[indexEnd].GetLineSegment(sharpLines[indexEnd].Begin_Seg.ToString(), endStationToCompare);
						geometry = geometry.Replace("LINESTRING", "");
						multiLineString += geometry + ")";
					}
					else
					{
						// Add the whole linestring
						geometry = sharpLines[indexEnd].GetLineSegment(sharpLines[i].Begin_Seg.ToString(), sharpLines[i].End_Seg.ToString());
						geometry = geometry.Replace("LINESTRING", "");
						multiLineString += geometry + ", ";
					}
				}
				geometry = multiLineString;
			}
			m_geoToReturn = Geometry.GeomFromText(geometry);
		}
	}
}
