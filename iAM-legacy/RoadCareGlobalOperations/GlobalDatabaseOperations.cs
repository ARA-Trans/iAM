using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DatabaseManager;
using System.Windows.Forms;
using DataObjects;
using System.IO;
using System.Collections;
using System.Data.Common;
using System.Data.OleDb;

namespace RoadCareGlobalOperations
{
    public static class GlobalDatabaseOperations
    {
        public static int SaveBinaryObjectToDatabase(string ID, string idColumn, string tableName, string binaryColumnName, byte[] fileBinary)
        {
            int numRowsAffected = 0;
            //SqlCommand command = null;
            //string provider = DBMgr.NativeConnectionParameters.Provider;
            //switch (provider)
            //{
            //    case "MSSQL":
            //        command = new SqlCommand("UPDATE " + tableName + " SET " + binaryColumnName + " = @fileBinary WHERE " + idColumn + " = " + ID, DBMgr.NativeConnectionParameters.SqlConnection);
            //        SqlParameter param0 = new SqlParameter("@fileBinary", SqlDbType.VarBinary, -1);
            //        param0.Value = fileBinary;
            //        command.Parameters.Add(param0);
            //        numRowsAffected = command.ExecuteNonQuery();
            //        break;
            //    case "ORACLE":
            //        string updateStatement = "UPDATE " + tableName + " SET " + binaryColumnName + " = ? WHERE " + idColumn + " = " + ID;
            //        List<DbParameter> statementParameters = new List<DbParameter>();
            //        statementParameters.Add( new OleDbParameter( "@fileBinary", fileBinary ));
            //        numRowsAffected = DBMgr.ExecuteParameterizedNonQuery( updateStatement, statementParameters );
            //        break;
            //    default:
            //        throw new Exception();
            //}

            //string path = Path.GetTempPath() + "\\DecisionEngine";
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            //path += "\\" + tableName + "_" + binaryColumnName + "_" + ID + ".bin";
            //using (FileStream file = new FileStream(path,FileMode.OpenOrCreate,FileAccess.Write))
            //{
            //    file.Write(fileBinary, 0, fileBinary.Length);
            //    file.Close();
            //}
            return numRowsAffected;
        }

        public static byte[] GetBinaryObjectFromDatabase(string ID, string idColumn, string tableName, string binaryColumnName)
        {
            byte[] binaryFile;
            string query = "SELECT " + binaryColumnName + " FROM " + tableName + " WHERE " + idColumn + " = " + ID;
            DataSet binaryFileData = DBMgr.ExecuteQuery(query);
            binaryFile = (byte[])binaryFileData.Tables[0].Rows[0][0];
            return binaryFile;
        }

		/// <summary>
		/// Creates a path for a bulk loader file.
		/// </summary>
		/// <param name="fileName">Name of file to bulk load.</param>
		/// <param name="fileExtension">file type.</param>
		/// <returns>A fully qualified path to the RoadCare temporary directory.</returns>
		public static string CreateBulkLoaderPath(string fileName, string fileExtension)
		{
			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);


			String strOutFile = strMyDocumentsFolder + "\\" + fileName + "." + fileExtension;
			return strOutFile;
		}

        /// <summary>
        /// Retrieves a list of Attributes from previously run simulation
        /// </summary>
        /// <param name="strSimulationID">SimulationID for Simulation to retreive attributes for.</param>
        /// <returns></returns>
        public static List<String> GetSimulationAttributes(String strSimulationID)
        {
            List<String> listAttributeSimulation = new List<String>();

            String strSelect = "SELECT SIMULATION_VARIABLES FROM SIMULATIONS WHERE SIMULATIONID='" + strSimulationID + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            if (ds.Tables[0].Rows.Count == 1)
            {
                string[] attributes = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split(new string[] { "\t" }, StringSplitOptions.None);
                for (int i = 0; i < attributes.Length; i++)
                {
                    listAttributeSimulation.Add(attributes[i]);
                }
            }
            return listAttributeSimulation;
        }


        /// <summary>
        /// Retrieves list of non-Calculated Field Attributes. Use TRY/CATCH
        /// </summary>
        /// <returns></returns>
        public static List<String> GetRawAttributes()
        {
            List<String> listRawAttribute = new List<String>();

            String strSelect = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE CALCULATED IS NULL OR CALCULATED=0 ORDER BY ATTRIBUTE_";
            DataSet dataSetAttributes = DBMgr.ExecuteQuery(strSelect);
            foreach(DataRow dataRowAttribute in dataSetAttributes.Tables[0].Rows)
            {
                listRawAttribute.Add(dataRowAttribute["ATTRIBUTE_"].ToString());
            }
            return listRawAttribute;
        }

        /// <summary>
        /// Retrieves all of the data for Networks.  Use TRY/CATCH
        /// </summary>
        /// <returns></returns>
        public static List<NetworkObject> GetNetworks()
        {
            List<DataObjects.NetworkObject> listNetworks = new List<DataObjects.NetworkObject>();
            String strSelect = "SELECT * FROM NETWORKS";
            DataSet dataSetNetworks = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in dataSetNetworks.Tables[0].Rows)
            {
                NetworkObject networkObject = new NetworkObject(dr);
                listNetworks.Add(networkObject);
            }
            return listNetworks;
        }


        /// <summary>
        /// Retrieves RCImageView VIEWS. Use TRY/CATCH
        /// </summary>
        /// <returns></returns>
        public static List<String> GetViews()
        {
            List<String> listViews = new List<String>();
            List<String> listColumns = DBMgr.GetTableColumns("IMAGELOCATION");
            int i = 0;
            foreach (String strColumn in listColumns)
            {
                if(i > 7)
                {
                    listViews.Add(strColumn);
                }
                i++;
            }
            return listViews;
        }

        /// <summary>
        /// Retrieves Referencing system choices SRS(Section) and LRS(Linear).Use Try/Catch
        /// </summary>
        /// <returns></returns>
        public static List<String> GetReferenceTypes()
        {
            List<String> listReferenceTypes = new List<String>();
			String strSelect = "SELECT TOP (1) FACILITY FROM IMAGELOCATION WHERE SECTION IS NULL";
			int nLRS = DBMgr.ExecuteQuery(strSelect).Tables[0].Rows.Count;
			if (nLRS > 0) listReferenceTypes.Add("Linear");

			strSelect = "SELECT TOP (1) FACILITY FROM IMAGELOCATION WHERE SECTION IS NOT NULL";
			int nSRS = DBMgr.ExecuteQuery(strSelect).Tables[0].Rows.Count;
			if (nSRS > 0) listReferenceTypes.Add("Section");
            return listReferenceTypes;
        }

        /// <summary>
        /// Retrieves DISTINCT Facility from IMAGELOCATION. Use Try/Catch
        /// </summary>
        /// <returns></returns>
        public static List<String> GetNavigationFacility(String strType)
        {
            List<String> listFacility = new List<string>();
            String strSelect = "SELECT DISTINCT FACILITY FROM IMAGE_FACILITY ORDER BY FACILITY";
            DataSet dataSetFacility = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in dataSetFacility.Tables[0].Rows)
            {
                String strFacility = dr["FACILITY"].ToString();
                listFacility.Add(strFacility);
            }
            return listFacility;
        }

        /// <summary>
        /// Returns List of Directions from IMAGELOCATION for a given. Use TRY/CATCH
        /// </summary>
        /// <param name="strFacility"></param>
        /// <returns></returns>
        public static List<String> GetNavigationDirection(String strFacility)
        {
            List<String> listDirection = new List<String>();
            if (String.IsNullOrEmpty(strFacility)) return listDirection;

            String strSelect = "SELECT DISTINCT DIRECTION FROM IMAGE_FACILITY WHERE FACILITY='" + strFacility + "'";
            DataSet dataSetDirection = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in dataSetDirection.Tables[0].Rows)
            {
                String strDirection = dr["DIRECTION"].ToString();
                listDirection.Add(strDirection);
            }
            return listDirection;
        }

        /// <summary>
        /// Returns List of Directions from IMAGELOCATION for a given FACILITY and DIRECTION. Use TRY/CATCH
        /// </summary>
        /// <param name="strFacility"></param>
        /// <returns></returns>
        public static String GetNavigationStation(String strFacility,String strDirection)
        {
            String sInitialStation ="";
            if (String.IsNullOrEmpty(strFacility) ||String.IsNullOrEmpty(strDirection)) return sInitialStation;
            String strSelect = "SELECT MILEPOST FROM IMAGELOCATION WHERE(PRECEDENT =  (SELECT MIN(PRECEDENT) AS MINPRECEDENT  FROM  IMAGELOCATION AS IMAGELOCATIONMIN WHERE (DIRECTION = '" + strDirection + "' AND FACILITY='" + strFacility + "')))";
            DataSet dataSetStation = DBMgr.ExecuteQuery(strSelect);
            if (dataSetStation.Tables[0].Rows.Count == 1)
            {
                DataRow dr = dataSetStation.Tables[0].Rows[0];
                sInitialStation = dr["MILEPOST"].ToString();
            }
            return sInitialStation;
        }
        /// <summary>
        /// Returns List of Years from IMAGELOCATION for a give FACILITY and DIRECTION. Use TRY/CATCH 
        /// </summary>
        /// <param name="strFacility"></param>
        /// <param name="strDirection"></param>
        /// <returns></returns>
        public static List<String> GetNavigationYear(String strFacility, String strDirection)
        {
            List<String> listYears = new List<String>();
            if (String.IsNullOrEmpty(strFacility) || String.IsNullOrEmpty(strDirection)) return listYears;
            String strSelect = "SELECT DISTINCT YEAR_ FROM IMAGELOCATION WHERE FACILITY='" + strFacility + "' AND DIRECTION='" + strDirection + "'";
            DataSet dataSetYear = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in dataSetYear.Tables[0].Rows)
            {
                String strYear = dr["YEAR_"].ToString();
                listYears.Add(strYear);
            }
            return listYears;
        }


        /// <summary>
        /// Returns List of Years from IMAGELOCATION for a give FACILITY and DIRECTION. Use TRY/CATCH 
        /// </summary>
        /// <param name="strFacility"></param>
        /// <param name="strDirection"></param>
        /// <returns></returns>
        public static List<String> GetNavigationYearSRS(String strFacility)
        {
            List<String> listYears = new List<String>();
            if (String.IsNullOrEmpty(strFacility)) return listYears;
            String strSelect = "SELECT DISTINCT YEAR_ FROM IMAGELOCATION WHERE FACILITY='" + strFacility + "' AND SECTION IS NOT NULL";
            DataSet dataSetYear = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in dataSetYear.Tables[0].Rows)
            {
                String strYear = dr["YEAR_"].ToString();
                listYears.Add(strYear);
            }
            return listYears;
        }
        /// <summary>
        /// Returns List of Sections for a given Facility from IMAGELOCATION.  Use TRY/CATCH
        /// </summary>
        /// <param name="strFacility"></param>
        /// <returns></returns>
        public static List<String> GetNavigationSection(String strFacility)
        {
            List<String> listSections = new List<String>();
            if (String.IsNullOrEmpty(strFacility)) return listSections;

            String strSelect = "SELECT DISTINCT SECTION, SECTIONORDER FROM IMAGE_FACILITY WHERE FACILITY='" + strFacility + "' AND SECTION IS NOT NULL ORDER BY SECTIONORDER";
            DataSet dataSetSection = DBMgr.ExecuteQuery(strSelect);
            String strSectionLast = "";
            foreach (DataRow dr in dataSetSection.Tables[0].Rows)
            {
                String strSection = dr["SECTION"].ToString();
                if(strSection != strSectionLast)
                {
                    if (!listSections.Contains(strSection))
                    {
                        listSections.Add(strSection);
                    }
                    strSectionLast = strSection;
                }
            }
            return listSections;
        }


        /// <summary>
        /// Returns List of Sections for a given Facility from IMAGELOCATION.  Use TRY/CATCH
        /// </summary>
        /// <param name="strFacility"></param>
        /// <returns></returns>
        public static List<String> GetNavigationSection(String strFacility,String strDirection)
        {
            List<String> listSections = new List<String>();
            if (String.IsNullOrEmpty(strFacility)) return listSections;

            String strSelect = "SELECT DISTINCT SECTION, SECTIONORDER FROM IMAGE_FACILITY WHERE FACILITY='" + strFacility + "' AND DIRECTION='" + strDirection + "' AND SECTION IS NOT NULL ORDER BY SECTIONORDER";
            DataSet dataSetSection = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in dataSetSection.Tables[0].Rows)
            {
                String strSection = dr["SECTION"].ToString();
                if (!listSections.Contains(strSection))
                {
                    listSections.Add(strSection);
                }
            }
            return listSections;
        }

        /// <summary>
        /// Retrieves all information from ATTRIBUTES table and stores it in a list.
        /// </summary>
        /// <returns></returns>
        public static List<AttributeObject> GetAttributes()
        {
            List<AttributeObject> listAttributes = new List<AttributeObject>();

            String strSelect = "SELECT * FROM ATTRIBUTES_";
            DataSet dsAttributes = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in dsAttributes.Tables[0].Rows)
            {
                listAttributes.Add(new AttributeObject(dr));
            }
            return listAttributes;

        }
        
        /// <summary>
        /// Retrieves a typical list of values for a given raw attribute.
        /// </summary>
        /// <param name="strAttribute">Raw attribute table</param>
        /// <param name="bShowAll">True if show all unique</param>
        /// <returns></returns>
        public static List<String> GetRawAttributeValue(String strAttribute, bool bShowAll)
        {
            List<String> listValue = new List<String>();
            int nCount = GetRawAttributeCount(strAttribute);
            int nMode = 1;

            while (nCount / nMode > 100)
            {
                nMode = nMode * 2;
            }
            ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(strAttribute);

            String strSelect = "SELECT DISTINCT DATA_ FROM " + strAttribute + " ";
            if (!bShowAll)
            {
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        strSelect += "WHERE ID%" + nMode.ToString() + "=0 ";
                        break;
                    case "ORACLE":
                        //throw new NotImplementedException("TODO: Create ORACLE implementation for GetRawAttributeValue()");
                        break;
                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for GetRawAttributeValue()");
                }
            }
            strSelect += "ORDER BY DATA";
            DataSet ds = DBMgr.ExecuteQuery(strSelect,cp);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                listValue.Add(dr["DATA"].ToString());
            }
            return listValue;
        }

        /// <summary>
        /// Returns the number of rows in a input Raw Attribute table.
        /// </summary>
        /// <param name="strAttribute"></param>
        /// <returns></returns>
        public static int GetRawAttributeCount(String strAttribute)
        {
            int nCount = 0;
            String strSelect = "SELECT COUNT(*) FROM " + strAttribute;
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            if (ds.Tables[0].Rows.Count == 1)
            {
                nCount = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
            }
            return nCount;
        }

        /// <summary>
        /// Returns list of Segmentation Rollup parameters and their attributes.
        /// </summary>
        /// <param name="strNetworkID">NetworkID for Segmentation</param>
        /// <returns></returns>
        public static List<RollupSegmentObject> GetSegmentRollup(String strNetworkID)
        {
            List<RollupSegmentObject> listRollupSegment = new List<RollupSegmentObject>();

            String strSelect = "SELECT ATTRIBUTE_,ROLLUPTYPE,SEGMENTTYPE FROM ROLLUP_CONTROL WHERE NETWORKID='" + strNetworkID + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listRollupSegment.Add(new RollupSegmentObject(row));
            }
            return listRollupSegment;
        }
        /// <summary>
        /// Insert into ROLLUP_CONTROL on Segment Rollup
        /// </summary>
        /// <param name="listRollupSegment"></param>
        /// <param name="strNetworkID">NetworkID of Network to rollup</param>
        public static void SetSegmentRollup(List<RollupSegmentObject> listRollupSegment, String strNetworkID)
        {
            String strDelete = "DELETE FROM ROLLUP_CONTROL WHERE NETWORKID='" + strNetworkID + "'";
            DBMgr.ExecuteNonQuery(strDelete);

            foreach (RollupSegmentObject rollupSegment in listRollupSegment)
            {
                String strAttribute = rollupSegment.Attribute;
                String strRollup = rollupSegment.RollupMethod;
                String strSegment = rollupSegment.SegmentMethod;

                String strInsert = "INSERT INTO ROLLUP_CONTROL (NETWORKID,ATTRIBUTE_,ROLLUPTYPE,SEGMENTTYPE) VALUES ('" + strNetworkID + "','" +strAttribute + "','" + strRollup + "','" + strSegment + "')";
                DBMgr.ExecuteNonQuery(strInsert);
            }
        }

        /// <summary>
        /// Remove selected Network (Use TRY/CATCH block)
        /// </summary>
        /// <param name="strNetworkID">Network Name of Network to delete</param>
        public static void RemoveNetwork(String strNetworkName)
        {
            String strDelete = "DELETE FROM NETWORKS WHERE NETWORK_NAME='" + strNetworkName + "'";
            DBMgr.ExecuteNonQuery(strDelete);
        }

        /// <summary>
        /// Retrieves number of SECTIONS in a rolled up network.  Will throw exception if network has not been rolled up. (USE TRY/CATCH)
        /// </summary>
        /// <param name="strNetworkID">NetworkID to retrieve sections for.</param>
        /// <returns></returns>
        public static int GetSectionCountForNetwork(String strNetworkID)
        {
            int nCount = 0;
            String strSelect = "SELECT COUNT(*) FROM SECTION_" + strNetworkID;
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            if (ds.Tables[0].Rows.Count == 1)
            {
                nCount = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
            }
            return nCount;
        }

        /// <summary>
        /// Delete all Segmentation Criteria for a given NETWORK.  For image view.  (USE TRY/CATCH)
        /// </summary>
        /// <param name="strNetworkID">NETWORKID for which to delete Dynamic Segmentation Criteria</param>
        public static void DeleteCriteriaSegmentForNetwork(String strNetworkID)
        {
            String strDelete = "DELETE FROM CRITERIA_SEGMENT WHERE NETWORKID ='" + strNetworkID + "'";
            DBMgr.ExecuteNonQuery(strDelete);

            strDelete = "DELETE FROM NETWORK_TREE WHERE NETWORKID='" + strNetworkID + "'";
            DBMgr.ExecuteNonQuery(strDelete);
        }

        /// <summary>
        /// Add all non-blank Segment Criteria for a given NETWORK. For IMAGEVIEW.  (USE TRY/CATCH)
        /// </summary>
        /// <param name="strNetworkID"></param>
        /// <param name="rollupSegment"></param>
        public static void SaveCriteriaSegmentForNetwork(String strNetworkID, RollupSegmentObject rollupSegment)
        {
            String strMethod = "";
            if (rollupSegment.SegmentMethod == " Any Record") strMethod = "Anyrecord";
            else strMethod = "Anychange";

            String strFamilyName = rollupSegment.Attribute + "_" + strNetworkID + "_" + rollupSegment.SegmentMethod;

            String strInsert = "INSERT INTO CRITERIA_SEGMENT (NETWORKID, FAMILY_NAME, FAMILY_EXPRESSION) VALUES ('" + strNetworkID + "','" + strFamilyName + "','" + strMethod.ToUpper() + "[" + rollupSegment.Attribute + "]')";
            DBMgr.ExecuteNonQuery(strInsert);

            strInsert = "INSERT INTO NETWORK_TREE (NETWORKID,NODES) VALUES('" + strNetworkID + "','" + strFamilyName + "')";
            DBMgr.ExecuteNonQuery(strInsert);
        }


        /// <summary>
        /// Retrieves a list of all image objects for a given ROUTE/DIRECTION (USE TRY/CATCH)
        /// </summary>
        /// <param name="strRoute">ROUTE to play</param>
        /// <param name="strDirection">DIRECTION to play images</param>
        /// <param name="listViews">Views available to play</param>
        /// <returns></returns>
        public static List<ImageObject> GetImageObjectList(String strRoute, String strDirection,String strYear, List<String> listViews)
        {
            List<ImageObject> listImageObjects = new List<ImageObject>();
            String strSelect = "SELECT * FROM IMAGELOCATION WHERE FACILITY='" + strRoute + "' AND DIRECTION='" + strDirection + "' AND YEAR_='" + strYear + "' ORDER BY PRECEDENT";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ImageObject imageObject = new ImageObject(row, listViews);
                listImageObjects.Add(imageObject);
            }
            return listImageObjects;
        }


        /// <summary>
        /// Retrieves a list of all image objects for a SECTION based FACILITY
        /// </summary>
        /// <param name="strFaciity">FACILITY</param>
        /// <param name="strYear">Image Year</param>
        /// <param name="listViews">List of Image Views</param>
        /// <returns></returns>
        public static List<ImageObject> GetImageObjectList(String strFacility, String strYear, List<String> listViews)
        {
            List<ImageObject> listImageObjects = new List<ImageObject>();
            String strSelect = "SELECT * FROM IMAGELOCATION WHERE FACILITY='" + strFacility + "' AND DIRECTION IS NULL AND YEAR_='" + strYear + "' ORDER BY PRECEDENT";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ImageObject imageObject = new ImageObject(row, listViews);
                listImageObjects.Add(imageObject);
            }
            return listImageObjects;
        }



        /// <summary>
        /// Retrieves list of Sections on a given Route/Direction  (USE TRY/CATCH)
        /// </summary>
        /// <param name="strRoute"></param>
        /// <param name="strDirection"></param>
        /// <param name="strNetworkID"></param>
        /// <returns></returns>
        public static List<SectionObject> GetSections(String strRoute, String strDirection, String strNetworkID)
        {
            List<SectionObject> listSectionObjects = new List<SectionObject>();
            String strSelect = "SELECT * FROM SECTION_" + strNetworkID + " WHERE FACILITY='" + strRoute + "' AND DIRECTION='" + strDirection + "' ORDER BY SECTIONID";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SectionObject sectionObject = new SectionObject(row);
                listSectionObjects.Add(sectionObject);
            }
            return listSectionObjects;
        }


        /// <summary>
        /// Retrieves list of Sections on a given Route/Direction  (USE TRY/CATCH)
        /// </summary>
        /// <param name="strRoute"></param>
        /// <param name="strDirection"></param>
        /// <param name="strNetworkID"></param>
        /// <returns></returns>
        public static List<SectionObject> GetSections(String strFacility,  String strNetworkID)
        {
            List<SectionObject> listSectionObjects = new List<SectionObject>();
            String strSelect = "SELECT * FROM SECTION_" + strNetworkID + " WHERE FACILITY='" + strFacility + "' AND DIRECTION IS NULL ORDER BY SECTIONID";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SectionObject sectionObject = new SectionObject(row);
                listSectionObjects.Add(sectionObject);
            }
            return listSectionObjects;
        }
        /// <summary>
        /// Retrieves list of Assets for a given RoadCare Database
        /// </summary>
        /// <returns></returns>
        public static List<AssetObject> GetAssets()
        {
            List<AssetObject> listAssetObjects = new List<AssetObject>();
            String strSelect = "SELECT * FROM ASSETS";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                AssetObject assetObject = new AssetObject(row);
                listAssetObjects.Add(assetObject);
            }
            return listAssetObjects;
        }

        /// <summary>
        /// Retrieves list of all calculated assets ordered by Asset Table
        /// </summary>
        /// <returns></returns>e
        public static List<CalculatedAssetObject> GetCalculatedAssets()
        {
            List<CalculatedAssetObject> listCalculatedObjects = new List<CalculatedAssetObject>();
            String strSelect = "SELECT * FROM ASSETS_CALCULATED ORDER BY ASSET";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                CalculatedAssetObject assetObject = new CalculatedAssetObject(row);
                listCalculatedObjects.Add(assetObject);
            }
            return listCalculatedObjects;
        }

        /// <summary>
        /// Get List of Years available for given Facility/Section
        /// </summary>
        /// <param name="strFacility"></param>
        /// <param name="strSection"></param>
        /// <returns></returns>
        public static List<String> GetImageYears(String strFacility, String strSection,String strDirection)
        {
            List<String> listYears = new List<String>();
            String strSelect = "SELECT DISTINCT YEAR_ FROM IMAGELOCATION WHERE FACILITY='" + strFacility + "' AND SECTION='" +strSection +"' AND DIRECTION='" + strDirection +"'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listYears.Add(row[0].ToString());
            }
            return listYears;
        }


        public static List<String> GetImageDirections(String strFacility, String strSection,String strYear)
        {
            List<String> listDirection = new List<String>();
            String strSelect = "SELECT DISTINCT DIRECTION FROM IMAGELOCATION WHERE FACILITY='" + strFacility + "' AND SECTION='" + strSection + "' AND YEAR_='" + strYear + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listDirection.Add(row[0].ToString());
            }
            return listDirection;
        }



        /// <summary>
        /// Gets Attributes field where IS_ASSET is true
        /// </summary>
        /// <returns></returns>
        public static List<AttributeObject> GetAssetToAttributes()
        {
            List<AttributeObject> listAssetAttribute = new List<AttributeObject>();

            String strSelect = "SELECT * FROM ATTRIBUTES_ WHERE ASSET IS NOT NULL AND ASSET_PROPERTY IS NOT NULL AND CALCULATED=1 ORDER BY ATTRIBUTE_";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listAssetAttribute.Add(new AttributeObject(row));
            }
            return listAssetAttribute;
        }


        /// <summary>
        /// Retrieves list of PCI METHODS
        /// </summary>
        /// <returns></returns>
        public static List<String> GetPCIMethods()
        {
            List<string> listMethods = new List<string>();
            String strSelect = "SELECT DISTINCT METHOD_ FROM PCI_DISTRESS";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listMethods.Add(row[0].ToString().Trim());
            }

            return listMethods;
        }
        /// <summary>
        /// Retrieves list of PCI METHODS, DISTRESS and NUMBER pairs
        /// </summary>
        /// <returns></returns>
        public static Hashtable GetPCIDistresses()
        {
            Hashtable hashMethodDistresses = new Hashtable();
            String strMethod;
            String strName;
            String strNumber;
            List<string> listMethods = new List<string>();
            String strSelect = "SELECT DISTINCT METHOD_,DISTRESSNAME,DISTRESSNUMBER FROM PCI_DISTRESS";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strMethod = row[0].ToString().Trim();
                strName = row[1].ToString().Trim();
                strNumber = row[2].ToString();
                Hashtable hashDistress;
                if (!hashMethodDistresses.Contains(strMethod))
                {
                    hashDistress = new Hashtable();
                    hashMethodDistresses.Add(strMethod, hashDistress);
                }
                else
                {
                    hashDistress = (Hashtable)hashMethodDistresses[strMethod];
                }

                hashDistress.Add(strName, strNumber);
            }

            return hashMethodDistresses;
        }


        public static int GetNextPCIID()
        {
            int nID = 0;
            String strSelect = "SELECT MAX(ID_) FROM PCI";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            try
            {
                if (ds.Tables[0].Rows.Count == 1)
                {
                    String strID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    nID = int.Parse(strID);
                    nID++;
                }
            }
            catch
            {

                nID = 0;
            }
            return nID;
        }
        /// <summary>
        /// Export committed project file for given NetworkID and SimulationID
        /// </summary>
        /// <param name="strNetworkID"></param>
        /// <param name="strSimulationID"></param>
        public static void ExportCommitted(string strNetworkID, string strSimulationID)
        {
            List<CommittedExport> listCommitExport = new List<CommittedExport>();
            String strCommitID = "";
            string strSelect = "SELECT COMMITTED_.SECTIONID,FACILITY,SECTION,BEGIN_STATION,END_STATION,DIRECTION,YEARS,BUDGET,COST_,AREA,YEARSAME,YEARANY,COMMIT_CONSEQUENCES.COMMITID, ATTRIBUTE_,CHANGE_,TREATMENTNAME FROM COMMITTED_ INNER JOIN COMMIT_CONSEQUENCES ON COMMITTED_.COMMITID = COMMIT_CONSEQUENCES.COMMITID INNER JOIN SECTION_" + strNetworkID + " ON SECTION_" + strNetworkID + ".SECTIONID=COMMITTED_.SECTIONID WHERE SIMULATIONID=" + strSimulationID;
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            CommittedExport commit = null;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string strNewCommitID = row[0].ToString();
                if (strCommitID != strNewCommitID)
                {
                    if (commit != null) listCommitExport.Add(commit);
                    commit = new CommittedExport();
                    commit.Facility = row[1].ToString();
                    commit.Section = row[2].ToString();
                    commit.BeginStation = row[3].ToString();
                    commit.EndStation = row[4].ToString();
                    commit.Direction = row[5].ToString();
                    commit.Year = row[6].ToString();
                    commit.Budget = row[7].ToString();
                    commit.Cost = row[8].ToString();
                    commit.Area = row[9].ToString();
                    commit.Same = row[10].ToString();
                    commit.Any = row[11].ToString();
                    commit.ConsequenceID = row[12].ToString();
                    commit.Treatment = row[15].ToString();
                }
                ConsequenceExport consequenceExport = new ConsequenceExport();
                consequenceExport.Attribute = row[13].ToString();
                consequenceExport.Change = row[14].ToString();
            }
            // Get header information
            List<String> listAttribute = new List<string>();
            strSelect = "SELECT DISTINCT COMMITTED_.SIMULATIONID,ATTRIBUTE_ FROM COMMIT_CONSEQUENCES,COMMITTED_ WHERE COMMITTED_.SIMULATIONID=" + strSimulationID + " ORDER BY ATTRIBUTE_";
            ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listAttribute.Add(row[1].ToString());
            }
            //Create Header
            String strHeader = "FACILITY\tSECTION\tSECTION_BEGIN\tSECTION_END\tDIRECTION\tYEARS\tTREATMENT\tBUDGET\tCOST\tAREA\tYEAR_SAME\tYEAR_ANY";
            foreach (String strAttribute in listAttribute)
            {
                strHeader += "\t";
                strHeader += strAttribute;
            }

            foreach (CommittedExport committedExport in listCommitExport)
            {
                WriteCommittedExportLine(listAttribute,committedExport);
            }
        }


        //Write single line of Committed project export.
        public static string WriteCommittedExportLine(List<String>listAttribute, CommittedExport commit)
        {
            string strReturn = "";
            strReturn = commit.Facility + "\t" + commit.Section + "\t" + commit.BeginStation + "\t" + commit.EndStation + "\t" + commit.Direction + "\t" + commit.Year +
                "\t" + commit.Treatment + "\t" + commit.Budget + "\t" + commit.Cost + "\t" + commit.Area + "\t" + commit.Same + "\t" + commit.Any;
            foreach (string strAttribute in listAttribute)
            {
                ConsequenceExport consequenceExport = commit.Consequence.Find(delegate(ConsequenceExport ce) { return ce.Attribute == strAttribute; });
                strReturn += "\t";
                if (consequenceExport != null)
                {
                    strReturn += consequenceExport.Change;
                }
            }
            return strReturn;
        }
    }
}
