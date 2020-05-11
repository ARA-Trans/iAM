using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadCareDatabaseOperations;
using DatabaseManager;
using System.Data;
using System.IO;
using System.Drawing;
using Microsoft.SqlServer.Management.Smo;
using System.Globalization;

namespace AssetRollup
{
	public class AssetRollup
	{
		private String m_ServerName;
		private String m_DataSource;
		private String m_UserID;
		private String m_Password;
		private String m_networkID;

		private List<DataPoint> dp = new List<DataPoint>();

		private List<String> m_assetNames = new List<String>();
		
		public AssetRollup(String strServer, String strDataSource, String strUserID, String strPassword, String networkID)
        {
			m_ServerName = strServer;
			m_DataSource = strDataSource;
			m_UserID = strUserID;
			m_Password = strPassword;
			m_networkID = networkID;

			// First get the list of assets from the asset table.
			m_assetNames = DBOp.GetRawAssetNames();
        }

		public void DoAssetRollup()
		{
			// Loop through each asset and fill the ASSET_SECTION_<network_id> table according to the rollup logic.
			AssetRollupMessaging.AddMessage("Begin asset rollup in network: " + m_networkID + " at " + DateTime.Now.ToString("HH:mm:ss"));
			String query = "";
			StreamWriter tw = null;

			if (DBOp.IsTableInDatabase("ASSET_SECTION_" + m_networkID))
			{
				// Drop the table as we are going to make a new one.
				try
				{
					DBMgr.ExecuteNonQuery("DROP TABLE ASSET_SECTION_" + m_networkID);
				}
				catch (Exception exc)
				{
					throw exc;
				}
			}

			// Creating the ASSET_SECTION_<networkID> table.
			AssetRollupMessaging.AddMessage("Creating ASSET_SECTION table...");
			List<DatabaseManager.TableParameters> listColumn = new List<DatabaseManager.TableParameters>();
			listColumn.Add(new DatabaseManager.TableParameters("GEO_ID", DataType.Int, false, false));
			listColumn.Add(new DatabaseManager.TableParameters("SECTIONID", DataType.Int, false, false));
			listColumn.Add(new DatabaseManager.TableParameters("ASSET_TYPE", DataType.VarChar(-1), false));
			listColumn.Add(new DatabaseManager.TableParameters("FACILITY", DataType.VarChar(-1), false));
			listColumn.Add(new DatabaseManager.TableParameters("BEGIN_STATION", DataType.Float, true));
			listColumn.Add(new DatabaseManager.TableParameters("END_STATION", DataType.Float, true));
			listColumn.Add(new DatabaseManager.TableParameters("DIRECTION", DataType.VarChar(50), true));
			listColumn.Add(new DatabaseManager.TableParameters("SECTION", DataType.VarChar(-1), true));
			listColumn.Add(new DatabaseManager.TableParameters("AREA", DataType.Float, true));
			listColumn.Add(new DatabaseManager.TableParameters("UNITS", DataType.VarChar(50), true));

			String strTable = "ASSET_SECTION_" + m_networkID;
			try
			{
				DBMgr.CreateTable(strTable, listColumn);
			}
			catch (Exception exc)
			{
				throw exc;
			}

			// Get a text writer and file ready to do a bulk copy.
			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			// LRS, Get the LRS data from each asset table...
			foreach (String assetName in m_assetNames)
			{
				AssetRollupMessaging.AddMessage("Rolling up LRS based asset " + assetName + "...");
				ConnectionParameters cp = DBMgr.GetAssetConnectionObject(assetName);
				List<String> assetColumnNames = DBMgr.GetTableColumns(assetName, cp);
				if (assetColumnNames.Contains("BEGIN_STATION"))
				{
					switch (cp.Provider)
					{
						case "MSSQL":
							query = "SELECT GEO_ID, FACILITY, DIRECTION, BEGIN_STATION, END_STATION FROM " + assetName + " WHERE (FACILITY <> '' AND FACILITY IS NOT NULL) ORDER BY FACILITY, DIRECTION, BEGIN_STATION";
							break;
						case "ORACLE":
							query = "SELECT GEO_ID, FACILITY, DIRECTION, BEGIN_STATION, END_STATION FROM " + assetName + " WHERE (FACILITY LIKE '_%' AND FACILITY IS NOT NULL) ORDER BY FACILITY, DIRECTION, BEGIN_STATION";
							break;
						default:
							throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
							//break;
					}
					String strOutFile = strMyDocumentsFolder + "\\" + assetName + ".txt";
					tw = new StreamWriter(strOutFile);
					DataSet sectionSet;
					DataReader assetReader;
					try
					{
						// Get the Segmented network data from the SECTION_<networkID> table.
						// sectionSet will hold the section data, and assetReader will loop through each asset.
						sectionSet = DBMgr.ExecuteQuery("SELECT SECTIONID, FACILITY, BEGIN_STATION, END_STATION, DIRECTION FROM SECTION_" + m_networkID + " WHERE BEGIN_STATION IS NOT NULL ORDER BY FACILITY, DIRECTION, BEGIN_STATION");
						assetReader = new DataReader(query, cp);
					}
					catch (Exception exc)
					{
						throw exc;
					}

					// If there is data to read, start reading it.
					if (assetReader.Read())
					{
						DataPoint assetInfo;
						DataRow sectionRow;
						DataPoint sectionInfo;

						bool bMoreData = true;

						int iCurrentSection = 0;

						// bMoreData is true while there is more data to read, and false when the dataReader is finished.
						// we then go back to the foreach loop (outside the while) and start rolling up the next asset.
						while (bMoreData)
						{
							// AssetInfo is going to hold this particular row of asset data.
							assetInfo = new DataPoint(-1, (int)assetReader["GEO_ID"], assetReader["FACILITY"].ToString(), assetReader["BEGIN_STATION"].ToString(), assetReader["END_STATION"].ToString(), assetReader["DIRECTION"].ToString());

							// SectionInfo is going to hold this particular row of sections data.
							sectionRow = sectionSet.Tables[0].Rows[iCurrentSection];
							sectionInfo = new DataPoint((int)sectionRow["SECTIONID"], sectionRow["FACILITY"].ToString(), sectionRow["BEGIN_STATION"].ToString(), sectionRow["END_STATION"].ToString(), sectionRow["DIRECTION"].ToString());

							// We increment the section if
							// We increment the asset if
							// AssetInSection returns:
							// -1 increments asset
							// 0 adds asset to Asset Rollup Table
							// 1 increments section
							bool bIncrementSection = false;
							bool bIncrementAsset = false;
							int assetSectionComparison = AssetInSection(assetInfo, sectionInfo);

							// Based on the result from AssetInSection we are going to increment something.  Here its the asset
							if (assetSectionComparison < 0)
							{
								bIncrementAsset = true;
							}
							// Here, we have a match and we need to look ahead to see how many sections a linear asset might belong to
							// before moving on to the next asset.  In either case, point or linear, we add the asset to the Rollup table.
							else if (assetSectionComparison == 0)
							{
								AddAssetToRollupTable(assetInfo, sectionInfo, assetName, tw);
								if (assetInfo.m_ptsExtent.Y != -1)	//don't bother with looking ahead if we're using point assets
								{
									// Keep looping through the sections and checking to see if this asset is still valid for each
									// consecutive section.  When it fails on a section, we are done with the linear asset, otherwise
									// we add the asset to the new section. (This is why we needed the sections in a DataSet, as a
									// dataReader would not allow this type of operation...easily).
									for (int iSectionLookAhead = 1; iSectionLookAhead + iCurrentSection < sectionSet.Tables[0].Rows.Count; iSectionLookAhead++)
									{
										sectionRow = sectionSet.Tables[0].Rows[iCurrentSection + iSectionLookAhead];
										sectionInfo = new DataPoint((int)sectionRow["SECTIONID"], sectionRow["FACILITY"].ToString(), sectionRow["BEGIN_STATION"].ToString(), sectionRow["END_STATION"].ToString(), sectionRow["DIRECTION"].ToString());
										if (AssetInSection(assetInfo, sectionInfo) == 0)
										{
											AddAssetToRollupTable(assetInfo, sectionInfo, assetName, tw);
										}
										else
										{
											break;
										}
									}
								}
								// Point asset match...we assigned the section already so just tell the loop to move to the next asset.
								bIncrementAsset = true;
							}
							// AssetInSection returned non-zero, and was not negative.  Which is a long way of saying, it returned positive.
							// so we need to increment the section on a positive result.
							else
							{
								bIncrementSection = true;
							}
							if (bIncrementAsset)
							{
								if (bIncrementSection)
								{
									// This can't happen logically, but was useful during debugging.
									throw new Exception();
								}
								else
								{
									// Read in the new data if we are incrementing the asset
									bMoreData = assetReader.Read();
								}
							}
							else
							{
								// Increment the section row in the section data set. (Assuming there are sections remaining)
								// If there arent any sections remaining, then we can't assign any more assets can we?
								// so that means we are done.
								if (bIncrementSection)
								{
									if (iCurrentSection + 1 < sectionSet.Tables[0].Rows.Count)
									{
										iCurrentSection++;
										bMoreData = true;
									}
									else
									{
										bMoreData = false;
									}
								}
								else
								{
									// Again, impossible, but useful for debugging.
									throw new Exception();
								}
							}
						}
					}
					tw.Close();
					assetReader.Close();


					AssetRollupMessaging.AddMessage("Bulk loading rolled up LRS asset data...");
					// Now try to load all that beautifully segmented data into an Asset Rollup table. (tab delimited).
					try
					{
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								DBMgr.SQLBulkLoad("ASSET_SECTION_" + m_networkID, strOutFile, '\t');
								break;
							case "ORACLE":
								throw new NotImplementedException("TODO: Figure out tables for DoAssetRollup()");
								//DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, "ASSET_SECTION_" + m_networkID, strOutFile, 
								//break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
					}
					catch (Exception exc)
					{
						throw exc;
					}
				}
			}

			AssetRollupMessaging.AddMessage("Finished LRS asset data rollup...");

			//foreach (String assetName in m_assetNames)
			//{
			//    AssetRollupMessaging.AddMessge("Rolling up SRS asset " + assetName + "...");
			//    ConnectionParameters cp = DBMgr.GetAssetConnectionObject(assetName);
			//    List<String> assetColumnNames = DBMgr.GetTableColumns(assetName, cp);
			//    if (assetColumnNames.Contains("SECTION"))
			//    {
			//        query = "SELECT GEO_ID, FACILITY, SECTION FROM " + assetName + " WHERE (SECTION <> '' AND SECTION IS NOT NULL) ORDER BY FACILITY, SECTION";
			//        String strOutFile = strMyDocumentsFolder + "\\" + assetName + ".txt";
			//        tw = new StreamWriter(strOutFile);
			//        DataReader sectionReader = null;
			//        DataReader assetReader = null;
			//        try
			//        {
			//            // Get the Segmented network data from the SECTION_<networkID> table.
			//            // sectionSet will hold the section data, and assetReader will loop through each asset.
			//            //sectionSet = DBMgr.ExecuteQuery("SELECT SECTIONID, FACILITY, SECTION SECTION_" + m_networkID + " WHERE SECTION IS NOT NULL ORDER BY FACILITY, SECTION");
			//            sectionReader = new DataReader("SELECT SECTIONID, FACILITY, SECTION FROM SECTION_" + m_networkID + " WHERE SECTION IS NOT NULL ORDER BY FACILITY, SECTION");
			//            assetReader = new DataReader(query, cp);
			//        }
			//        catch (Exception exc)
			//        {
			//            throw exc;
			//        }

			//        bool bContinue = true;
			//        String strFacility = "";
			//        String strSection = "";
			//        String strSectionID = "";
			//        String strAssetFacility = "";
			//        String strAssetSection = "";
			//        String strGeoID = "";



			//        while (bContinue)
			//        {
			//            if (strFacility == "")
			//            {
			//                if (!sectionReader.Read())
			//                {
			//                    bContinue = false;
			//                    continue;
			//                }
			//                strFacility = sectionReader["FACILITY"].ToString();
			//                strSection = sectionReader["SECTION"].ToString();
			//                strSectionID = sectionReader["SECTIONID"].ToString();
			//                //if (strSectionID == "1006136")
			//                //{ }
			//                //strFacility = strFacility.Replace(" ", "");
			//                //strSection = strSection.Replace(" ", "");

			//            }

			//            if (strAssetFacility == "")
			//            {
			//                if (!assetReader.Read())
			//                {
			//                    bContinue = false;
			//                    continue;
			//                }
			//                strAssetFacility = assetReader["FACILITY"].ToString();
			//                strAssetSection = assetReader["SECTION"].ToString();
			//                strGeoID = assetReader["GEO_ID"].ToString();
			//                //if (strAssetFacility == "NW - Connecticut Ave")
			//                //{ }

			//                //strAssetFacility = strAssetFacility.Replace(" ", "");
			//                //strAssetSection = strAssetSection.Replace(" ", "");
			//            }
			//            if (CompareInfo.GetCompareInfo("en-US").Compare(strFacility, strAssetFacility) < 0)
			//            {
			//                strFacility = "";
			//            }
			//            else if (CompareInfo.GetCompareInfo("en-US").Compare(strFacility, strAssetFacility) == 0)
			//            {
			//                if (CompareInfo.GetCompareInfo("en-US").Compare(strSection, strAssetSection) < 0)
			//                {
			//                    strFacility = "";
			//                }
			//                else if (CompareInfo.GetCompareInfo("en-US").Compare(strSection, strAssetSection) == 0)
			//                {
			//                    //Write out to file
			//                    tw.WriteLine(strGeoID
			//                    + "\t" + strSectionID
			//                    + "\t" + assetName
			//                    + "\t" + sectionReader["FACILITY"].ToString()
			//                    + "\t" //+ a.m_ptsExtent.X.ToString() 
			//                    + "\t" //+ ((a.m_ptsExtent.Y == -1) ? "" : a.m_ptsExtent.Y.ToString()) 
			//                    + "\t" //+ a.m_strDirection 
			//                    + "\t" + sectionReader["SECTION"].ToString()
			//                    + "\t" //+ a.m_strArea 
			//                    + "\t"); //+ a.m_strUnit);
			//                    strAssetFacility = "";
			//                }
			//                else
			//                {
			//                    strAssetFacility = "";
			//                }
			//            }
			//            else
			//            {
			//                strAssetFacility = "";
			//            }
						
			//        }
			//        tw.Close();
			//        assetReader.Close();
			//        sectionReader.Close();

			//        AssetRollupMessaging.AddMessge("Bulk loading rolled up SRS asset data...");
			//        // Now try to load all that beautifully segmented data into an Asset Rollup table. (tab delimited).
			//        try
			//        {
			//            switch (cp.Provider)
			//            {
			//                case "MSSQL":
			//                    //query = "SELECT GEO_ID, FACILITY, SECTION FROM " + assetName + " WHERE (SECTION <> '' AND SECTION IS NOT NULL) ORDER BY FACILITY, SECTION";
			//                    DBMgr.SQLBulkLoad("ASSET_SECTION_" + m_networkID, strOutFile, '\t');
			//                    break;
			//                case "ORACLE":
			//                    query = "SELECT GEO_ID, FACILITY, SECTION FROM " + assetName + " WHERE (SECTION LIKE '_%' AND SECTION IS NOT NULL) ORDER BY FACILITY, SECTION";
			//                    break;
			//                default:
			//                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
			//                    break;
			//            }
			//        }
			//        catch (Exception exc)
			//        {
			//            throw exc;
			//        }
			//    }
			//}
			AssetRollupMessaging.AddMessage("Asset Rollup complete.");
		}

		protected void AddAssetToRollupTable(DataPoint asset, DataPoint section, String assetName, TextWriter tw)
		{
			
			DataPoint a = new DataPoint(section.m_nSection, asset.m_nGeo_ID, asset.m_strRoutes, asset.m_ptsExtent.X.ToString(), asset.m_ptsExtent.Y.ToString(), asset.m_strDirection );
			dp.Add(a);
			tw.WriteLine(a.m_nGeo_ID.ToString() 
				+ "\t" + a.m_nSection.ToString() 
				+ "\t" + assetName 
				+ "\t" + a.m_strRoutes 
				+ "\t" + a.m_ptsExtent.X.ToString() 
				+ "\t" + ((a.m_ptsExtent.Y == -1) ? "" : a.m_ptsExtent.Y.ToString()) 
				+ "\t" + a.m_strDirection 
				+ "\t" + a.m_strSection 
				+ "\t" + a.m_strArea 
				+ "\t" + a.m_strUnit);
		}

		/// <summary>
		/// This functions determines whether an asset is before, touching, after, or fully contained by a given section.
		/// </summary>
		/// <param name="asset">The asset data point to compare</param>
		/// <param name="section">The section data point to compare</param>
		/// <returns> -1 increments asset. 0 adds asset to RollupTable. 1 increments section.
		/// </returns>
		protected int AssetInSection(DataPoint asset, DataPoint section)
		{
			// Alias our asset and section Begin and End Mileposts.
			int iReturnValue = 0;
			float aBMP = asset.m_ptsExtent.X;
			float aEMP = asset.m_ptsExtent.Y;
			float sBMP = section.m_ptsExtent.X;
			float sEMP = section.m_ptsExtent.Y;

			// We need to use CompareInfo, because C# sorts its strings (by default) differently than SQL does.
			// This is important as the ordering of our query results allows the rollup to work properly.
			// So first we check to see if the asset route is less than the section route, if it is, then we need to increment
			// the asset we are on, because this asset has missed the pervervial boat, and will not be assigned to a section.
			if(CompareInfo.GetCompareInfo("en-US").Compare(asset.m_strRoutes, section.m_strRoutes, CompareOptions.StringSort) < 0)
			{
				iReturnValue = -1;
			}
			// Ok, if the routes match, we continue to check to see if this asset should be assigned to this section
			// by checking the direction field.  If the asset direction is less than the section direction, then again,
			// this asset has missed the boat and will not be assigned a section.  We know this because of the ordered
			// results of our data.
			else if (asset.m_strRoutes == section.m_strRoutes)
			{
				if (CompareInfo.GetCompareInfo("en-US").Compare(asset.m_strDirection, section.m_strDirection, CompareOptions.StringSort) < 0)
				{
					iReturnValue = -1;
				}
				// If the directions match, then we need to compare the asset mileposts with the section mileposts.
				// There are several cases.  First we determine what kind of asset we are dealing with,
				// the choices will always be point, or linear, as the only part of a non-linear asset we care about
				// will exist as a line along the section of road.  If the asset end milepost is -1 then we have a point asset
				// if it has a positive value (or 0) then we have a linear asset.
				else if (asset.m_strDirection == section.m_strDirection)
				{
					if (aEMP != -1)
					{
						// Linear assets only.  If the END milepost is less or equal to the BEGINING of the section milepost
						// Then this asset has missed the boat and we need to go on to the next one.
						if (aEMP <= sBMP)
						{
							iReturnValue = -1;
						}
						else
						{
							// If the asset END milepost is AFTER the section BEGIN milepost, and the asset BEGIN milepost
							// is AFTER or ON the section END milepost, then we need to increment the section we are on.
							// cause this asset will probably be in the next one.
							if (aBMP >= sEMP)
							{
								iReturnValue = 1;
							}
							// Otherwise, we have a match, and this asset needs to be added to this section in the Rollup table.
							else
							{
								iReturnValue = 0;
							}
						}
					}
					else
					{
						// Point assets only.  We treat the point assets a bit differently when it comes to landing directly on
						// the section mileposts.  We always put a point asset which has its milepost value on the END milepost value
						// of the section, in the NEXT section.  However, this is not done, in the event that we are looking at the
						// last section. (As there would be no more sections to assign the asset to, but it still falls in the network).
						// The asset is before the section, so on to the next asset
						if (aBMP < sBMP)
						{
							iReturnValue = -1;
						}
						else
						{
							// The asset is after the section BEGIN milepost, but BEFORE the section END milepost
							// so add it to the asset Rollup table.
							if (aBMP < sEMP)
							{
								iReturnValue = 0;
							}
							// Here is our fringe case, the asset falls on the END milepost of the section.
							// so we check to see if this is the last section in the network, if it is,
							// then we add the asset to the section, otherwise, we go on to the next section.
							else if (aBMP == sEMP)
							{
								if (DBOp.IsAtEndOfNetwork(m_networkID, section.m_nSection.ToString()))
								{
									iReturnValue = 0;
								}
								else
								{
									iReturnValue = 1;
								}
							}
							// Otherwise, the asset is after the section BEGIN milepost, but BEFORE the section END milepost
							// so we need to increment the section
							else
							{
								iReturnValue = 1;
							}
						}
					}
				}
				// The asset direction and section direction do not match, so we should increment the section.
				else
				{
					iReturnValue = 1;
				}
			}
			// The asset Route and section Route are not equal so we increment the section
			else
			{
				iReturnValue = 1;
			}
			return iReturnValue;
		}
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
		public int m_nGeo_ID;

		public DataPoint(String strSectionID, String strFacility, String strSection, String strArea, String strUnit)
		{
			m_nSection = int.Parse(strSectionID.ToString());
			m_strRoutes = strFacility;
			m_strSection = strSection;
			m_strArea = strArea;
			m_strUnit = strUnit;
		}

		public DataPoint(int nSectionID, String strRoutes, String strBegin, String strEnd, String strDirection)
		{
			m_nSection = nSectionID;
			m_strRoutes = strRoutes;
			m_ptsExtent = new PointF(float.Parse(strBegin), float.Parse(strEnd));
			m_strDirection = strDirection;
		}

		public DataPoint(int nSectionID, int nGeo_ID, String strRoutes, String strBegin, String strEnd, String strDirection)
		{
			m_nSection = nSectionID;
			m_nGeo_ID = nGeo_ID;
			m_strRoutes = strRoutes;
			if (strEnd == "")
			{
				m_ptsExtent = new PointF(float.Parse(strBegin), -1);
			}
			else
			{
				m_ptsExtent = new PointF(float.Parse(strBegin), float.Parse(strEnd));
			}
			m_strDirection = strDirection;
		}


		public DataPoint(String strRoutes, String strBegin, String strEnd, String strDirection, String strYear, String strData)
		{
			m_strRoutes = strRoutes;
			m_ptsExtent = new PointF(float.Parse(strBegin), float.Parse(strEnd));
			m_strDirection = strDirection;
			m_strYear = strYear;
			m_strData = strData;
		}

		public DataPoint(String strFacility, String strSection, String strSample, String strArea, String strYear, String strData, bool bIgnore)
		{
			m_strRoutes = strFacility;
			m_strSection = strSection;
			m_strSample = strSample;
			m_strYear = strYear;
			m_strData = strData;
			m_bIgnore = false;
			m_strArea = strArea;
		}
	}
}
