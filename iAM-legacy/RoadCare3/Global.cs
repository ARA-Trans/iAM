using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using DatabaseManager;
using ListNetworkComputers;
using RoadCareDatabaseOperations;
using RoadCareSecurityOperations;
using SharpMap.Data;
using SharpMap.Data.Providers;

namespace RoadCare3
{
    public static class Global
    {
        public static string YearOfLicense => "2018";

        public static string LastDayOfLicense => $"{YearOfLicense}-12-31";

        public static DateTime ExactMomentWhenLicenseExpires => DateTime.ParseExact(YearOfLicense, "yyyy", null).AddYears(1);

        public static DateTime FirstDayOfLicenseExpirationWarning => ExactMomentWhenLicenseExpires.AddMonths(-1);

        public static string BrandCaptionForMessageBoxes => "RoadCare";

        private static String m_strLinearUnits = "miles";
        private static bool m_bIsLinear = true;
        private static int m_nMaxReturn = 15000;

        static private Hashtable m_hashNetworks = new Hashtable(); // key is network property, value is network database name
        static private Hashtable m_hashAttributeType = new Hashtable();  //The type of each attribute.
        static private Hashtable m_hashAttributeDefault = new Hashtable();  //The type of each attribute.
        static private Hashtable m_hashAttributeMinimum = new Hashtable();//Key attribute. Object double of minimum value.
        static private Hashtable m_hashAttributeMaximum = new Hashtable();//Key attribute. Object double of maximum value.
        static private Hashtable m_hashAttributeAscending = new Hashtable();
        static private Hashtable m_hashAttributeFormat = new Hashtable();
        public static Hashtable m_htFieldMapping = new Hashtable();

        private static SecOps m_globalSecurityOperations = null;

        private static List<String> m_listAttribute = new List<String>();
        private static bool m_bMostRecentFirst = true;

        private static Regex timeFilter = new Regex( "(2[0-3]|1[0-9]|0?[0-9]):([1-5][0-9]|0?[0-9]):([1-5][0-9]|0?[0-9])" );


        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        private static FormProperties m_formPropertyWindow = null;
        private static FormPropertiesModal m_formPropertyModal = null;

        //Committed Project Simulation To Simulation Copy
        private static String m_strTreatmentName;
        private static String m_strYearAny;
        private static String m_strYearSame;
        private static String m_strCost;
        private static String m_strBudget;
        private static String m_strArea;
        private static List<CommitAttributeChange> m_listAttributeChange;

        private static Dictionary<string, object> m_clipBoard = new Dictionary<string,object>();

        public static void PushDataToClipboard( string identifier, object data )
        {
            m_clipBoard[identifier] = data;
        }

        public static object PopDataFromClipboard( string identifier )
        {
            object data = m_clipBoard[identifier];
            m_clipBoard.Remove( identifier );
            return data;
        }

        public static bool ClipboardHasData( string identifier )
        {
            return m_clipBoard.ContainsKey( identifier );
        }

        public static SecOps SecurityOperations
        {
            get
            {
                if( m_globalSecurityOperations == null )
                {
                    m_globalSecurityOperations = new SecOps();
                }
                return m_globalSecurityOperations;
            }
            set
            {
                m_globalSecurityOperations = value;
            }
        }

        public static String CommitTreatmentName
        {
            get { return m_strTreatmentName; }
        }

        public static String CommitYearAny
        {
            get { return m_strYearAny; }
        }

        public static String CommitYearSame
        {
            get { return m_strYearSame; }
        }

        public static String CommitCost
        {
            get { return m_strCost; }
        }

        public static String CommitBudget
        {
            get { return m_strBudget; }
        }
        
        public static String CommitArea
        {
            get { return m_strArea; }
        }

        public static List<CommitAttributeChange> CommitAttributeChange
        {
            get { return m_listAttributeChange; }
        }

        public static string ApplicationName
        {
            get { return "RoadCare"; }
        }

        public static string GetTimeStampPrefix()
        {
            return "[" + DateTime.Now + "]: ";
        }

        /// <summary>
        /// Writes the input string to the output window.
        /// </summary>
        /// <param name="strOutputText">the string to write</param>
        public static void WriteOutput(String strOutputText)
        {
            String strTemp = strOutputText.ToUpper();
            if (strTemp.Contains("ERROR"))
            {
                System.Media.SystemSounds.Exclamation.Play();  
            }

            var outputWindow = FormManager.GetOutputWindow();
            if (outputWindow != null)
            {
                if (!string.IsNullOrEmpty(strOutputText))
                {
                    if (!timeFilter.IsMatch(strOutputText))
                    {
                        strOutputText = GetTimeStampPrefix() + strOutputText;
                    }

                    // We wrap the SetOutputText call with Invoke if the
                    // currently executing thread is not on the same thread as
                    // the outputWindow control (i.e. if InvokeRequired is
                    // true). This happens in some cases when "background
                    // worker" logic might call this method to update the UI.
                    // Without Invoke, a call to SetOutputText from another
                    // thread would cause a cross-thread access violation
                    // exception.
                    if (outputWindow.InvokeRequired)
                    {
                        outputWindow.Invoke(new Action(() =>
                            outputWindow.SetOutputText(strOutputText)));
                    }
                    else
                    {
                        outputWindow.SetOutputText(strOutputText);
                    }
                }
            }
        }

        public static void ReplaceOutput(String strOutputText)
        {
            var outputWindow = FormManager.GetOutputWindow();
            if (outputWindow != null)
            {
                if (!string.IsNullOrEmpty(strOutputText))
                {
                    // We wrap the SetOutputText call with Invoke if the
                    // currently executing thread is not on the same thread as
                    // the outputWindow control (i.e. if InvokeRequired is
                    // true). This happens in some cases when "background
                    // worker" logic might call this method to update the UI.
                    // Without Invoke, a call to SetOutputText from another
                    // thread would cause a cross-thread access violation
                    // exception.
                    if (outputWindow.InvokeRequired)
                    {
                        outputWindow.Invoke(new Action(() =>
                            outputWindow.ReplaceOutputText(strOutputText)));
                    }
                    else
                    {
                        outputWindow.ReplaceOutputText(strOutputText);
                    }
                }
            }
        }


        public static String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
 
        public static void SerializeObject(Object pObject, String strPath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(pObject.GetType());
                
                // Create an XmlTextWriter using a FileStream.
                Stream fs = new FileStream(strPath, FileMode.Create);
                XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);

                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, pObject);
                writer.Close();
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public static Object DeserializeObject(Object pObject, String filename)
        {
            // Create an instance of the XmlSerializer specifying type and namespace.
            XmlSerializer serializer = new XmlSerializer(pObject.GetType());

            // A FileStream is needed to read the XML document.
            FileStream fs = null;
            XmlReader reader = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                reader = XmlReader.Create(fs);
            }
            catch (Exception e)
            {
                Global.WriteOutput("Error: Opening file " + filename + "." + e.Message);
                return null;
            }

            // Declare an object variable of the type to be deserialized.
            Object obj;

            // Use the Deserialize method to restore the object's state.
            obj = serializer.Deserialize(reader);
            fs.Close();
            return obj;
        }

        public static Microsoft.SqlServer.Management.Smo.DataType ConvertStringToDataType(string strDataType)
        {
            Microsoft.SqlServer.Management.Smo.DataType dataType;
            switch (strDataType)
            {
                case "bigint":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.BigInt;
                    break;
                case "binary(50)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Binary(50);
                    break;
                case "bit":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Bit;
                    break;
                case "char(10)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Char(10);
                    break;
                case "datetime":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.DateTime;
                    break;
                case "decimal(18, 0)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Decimal(18, 0);
                    break;
                case "float":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Float;
                    break;
                case "image":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Image;
                    break;
                case "int":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Int;
                    break;
                case "money":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Money;
                    break;
                case "nchar(10)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.NChar(10);
                    break;
                case "ntext":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.NText;
                    break;
                case "numeric(18, 0)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Numeric(18, 0);
                    break;
                case "nvarchar(50)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.NVarChar(50);
                    break;
                case "nvarchar(MAX)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.NVarCharMax;
                    break;
                case "real":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Real;
                    break;
                case "smalldatetime":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.SmallDateTime;
                    break;
                case "smallint":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.SmallInt;
                    break;
                case "smallmoney":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.SmallMoney;
                    break;
                case "text":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Text;
                    break;
                case "timestamp":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.Timestamp;
                    break;
                case "tinyint":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.TinyInt;
                    break;
                case "uniqueidentifier":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.UniqueIdentifier;
                    break;
                case "varbinary(50)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.VarBinary(50);
                    break;
                case "varbinary(MAX)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.VarBinaryMax;
                    break;
                case "varchar(50)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.VarChar(50);
                    break;
                case "varchar(MAX)":
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1);
                    break;
                default:
                    dataType = Microsoft.SqlServer.Management.Smo.DataType.VarChar(200);
                    break;
            }
            return dataType;
        }

        public static Bitmap ConvertBitmapColor(Color colorOld, Color colorNew, Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color cTemp = bmp.GetPixel(i, j);
                    if (cTemp.R == colorOld.R && cTemp.B == colorOld.B && cTemp.G == colorOld.G)
                    {
                        bmp.SetPixel(i, j, colorNew);
                    }
                }
            }
            return bmp;
        }

        /// <summary>
        /// Clears output window
        /// </summary>
        /// <param name="strOutputText">Output window string.</param>
        public static void ClearOutputWindow()
        {
            try
            {
                if(FormManager.GetOutputWindow() == null)
                {
                    FormManager.AddOutputWindow(new FormOutputWindow());
                }
                FormManager.GetOutputWindow().ClearWindow();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error: Could not clear output window.  Check to see if it is open." + exc.Message);
            }

        }

        public static bool MostRecentFirst
        {
            get
            {
                return m_bMostRecentFirst;
            }
            set
            {
                m_bMostRecentFirst = value;
            }

        }

        public static List<String> Attributes
        {
            get
            {
                return m_listAttribute;
            }
            set
            {
                m_listAttribute = value;
            }

        }
       
        //public static String UserID
        //{
        //    get
        //    {
        //        return m_strUserID;
        //    }
        //    set
        //    {
        //        m_strUserID = value;
        //    }
        //}

        public static Hashtable AttributeHash
        {
            get 
            { 
                return m_htFieldMapping;
            }
            set 
            {
                m_htFieldMapping = value;
            }
        }

        public static Hashtable NetworkHash
        {
            get { return m_hashNetworks; }
            //set { m_hashNetworks = value; }
        }

        public static String LinearUnits
        {
            get { return m_strLinearUnits; }
            set { m_strLinearUnits = value; }


        }

        public static bool IsLinear
        {
            get { return m_bIsLinear; }
            set { m_bIsLinear = value; }

        }

        public static int MaximumReturn
        {
            get { return m_nMaxReturn; }
            set { m_nMaxReturn = value; }

        }

        public static void LoadNetworkHash()
        {
            m_hashNetworks.Add("Network Name", "NETWORK_NAME");
            m_hashNetworks.Add("Description", "DESCRIPTION");
            m_hashNetworks.Add("Designer UserID", "DESIGNER_USERID");
            m_hashNetworks.Add("Designer Name", "DESIGNER_NAME");
            m_hashNetworks.Add("Date Created", "DATE_CREATED");
            m_hashNetworks.Add("Date Last Edit", "DATE_LAST_EDIT");
            m_hashNetworks.Add("Number Sections", "NUMBER_SECTIONS");
            //m_hashNetworks.Add("Lock", "LOCK");
            //m_hashNetworks.Add("Private", "PRIVATE");
        }

        /// <summary>
        /// Creates a hashtable that maps the database values of the ATTRIBUTES table 
        /// </summary>
        public static void LoadAttributeHash()
        {
            m_htFieldMapping.Add("Network Definition Name", "NETWORK_DEFINITION_NAME");
            m_htFieldMapping.Add("One", "LEVEL1");
            m_htFieldMapping.Add("Two", "LEVEL2");
            m_htFieldMapping.Add("Three", "LEVEL3");
            m_htFieldMapping.Add("Four", "LEVEL4");
            m_htFieldMapping.Add("Five", "LEVEL5");
            m_htFieldMapping.Add("Type", "TYPE");
            m_htFieldMapping.Add("Password", "PASSWORD");
            m_htFieldMapping.Add("Login", "USERID");
            m_htFieldMapping.Add("Database", "DATASOURCE");
            m_htFieldMapping.Add("Server", "SERVER");
            m_htFieldMapping.Add("Provider", "PROVIDER");
            m_htFieldMapping.Add("Native", "NATIVE_");
            m_htFieldMapping.Add("Table", "TABLENAME");
            m_htFieldMapping.Add("Routes", "COL_ROUTES");
            m_htFieldMapping.Add("Begin_Station", "COL_BEGIN_STATION");
            m_htFieldMapping.Add("End_Station", "COL_END_STATION");
            m_htFieldMapping.Add("Direction", "COL_DIRECTION");
            m_htFieldMapping.Add("Facility", "COL_FACILITY");
            m_htFieldMapping.Add("Section", "COL_SECTION");
            m_htFieldMapping.Add("Sample", "COL_SAMPLE");
            m_htFieldMapping.Add("Date", "COL_DATE");
            m_htFieldMapping.Add("Data", "COL_DATA");
            m_htFieldMapping.Add("Ascending", "ASCENDING");
            m_htFieldMapping.Add("Format", "FORMAT");
            m_htFieldMapping.Add("Minimum", "MINIMUM_");
            m_htFieldMapping.Add("Maximum", "MAXIMUM");
            m_htFieldMapping.Add("Grouping", "GROUPING");
            m_htFieldMapping.Add("Default_Value", "DEFAULT_VALUE");
            m_htFieldMapping.Add("View", "SQLVIEW");
            m_htFieldMapping.Add("Integrated Security", "INTEGRATED_SECURITY");
            m_htFieldMapping.Add("SID", "SID_");
            m_htFieldMapping.Add("Port", "PORT");
            m_htFieldMapping.Add("Connection Type", "CONNECTION_TYPE");
            m_htFieldMapping.Add("Network Alias", "SERVICE_NAME");
        }

        /// <summary>
        /// Creates a new modal property dialog is bIsModal is true, otherwise creates OR shows a property tool window.
        /// </summary>
        /// <param name="o">A modal properties dialog, or a Properties tool window.</param>
        /// <param name="bIsModal">True if the o is a modal properties dialog, false if it is a properties tool window. </param>
        /// <param name="strTitle">Name of the modal dialog properties window, empty string if o is a properties tool window.</param>
        public static void ShowPropertyGrid(object o, bool bIsModal, String strTitle)
        {
            // If we are working with a tool window we need to see if it has been closed, or not yet created.
            // in either case, we will new the property grid.  No such check is needed for the modal dialog.
            if (!bIsModal)
            {
                if (m_formPropertyWindow == null || m_formPropertyWindow.IsDisposed)
                {
                    m_formPropertyWindow = new FormProperties();
                    m_formPropertyWindow.SetPropertyGrid(o, bIsModal);
                    m_formPropertyWindow.TabText = "Properties";
                }
                else
                {
                    m_formPropertyWindow.SetPropertyGrid(o, bIsModal);
                }
            }
            else
            {
                m_formPropertyModal = new FormPropertiesModal();
                m_formPropertyModal.SetPropertyGrid(o, bIsModal);
                m_formPropertyModal.Text = strTitle;
            }
        }

        /// <summary>
        /// Returns the property tool window.
        /// </summary>
        /// <returns></returns>
        public static FormProperties GetPropertyWindow()
        {
            return m_formPropertyWindow;
        }

        /// <summary>
        /// Returns the property modal dialog window.
        /// </summary>
        /// <returns></returns>
        public static FormPropertiesModal GetModalPropertyWindow()
        {
            return m_formPropertyModal;
        }

        public static String GetUniqueConsequencID()
        {
            String strSelect = "SELECT IDNUMBER FROM UNIQUEIDNUMBER";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            String strUniqueID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            int nUniqueID = int.Parse(strUniqueID);
            nUniqueID++;
            String strUpdate = "UPDATE UNIQUEIDNUMBER SET IDNUMBER='" + nUniqueID.ToString() + "' WHERE IDNUMBER='" + strUniqueID + "'";
            DBMgr.ExecuteNonQuery(strUpdate);
            return strUniqueID;
        }

        static public void LoadAttributes()
        {
            String strAttribute;
            String strType;
            String strDefault;
            String strMinimum;
            String strMaximum;
            String strAscending;
            String strFormat="";

            m_hashAttributeType.Clear();
            m_hashAttributeAscending.Clear();
            m_hashAttributeDefault.Clear();
            m_hashAttributeMaximum.Clear();
            m_hashAttributeMinimum.Clear();
            m_hashAttributeFormat.Clear();
            m_listAttribute.Clear();
            m_hashAttributeType.Add("SECTIONID", "NUMBER");


            String strSelect = "SELECT ATTRIBUTE_,TYPE_,DEFAULT_VALUE,MINIMUM_,MAXIMUM,ASCENDING,FORMAT FROM ATTRIBUTES_";

            // TODO: Error handling code here somewhere?
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strAttribute = row[0].ToString();
                strType = row[1].ToString();
                if (strType == "STRING")
                {
                    strDefault = row[2].ToString();
                }
                else
                {
                    strDefault = row[2].ToString();
                    strMinimum = row[3].ToString();
                    strMaximum = row[4].ToString();
                    strAscending = row[5].ToString();
                    strFormat = row[6].ToString();
                    

                    if (strMinimum != "")
                    {
                        m_hashAttributeMinimum.Add(strAttribute, double.Parse(strMinimum));
                    }
                    if (strMaximum != "")
                    {
                        m_hashAttributeMaximum.Add(strAttribute, double.Parse(strMaximum));
                    }
                    if (strAscending != "")
                    {

                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                m_hashAttributeAscending.Add(strAttribute, bool.Parse(strAscending));
                                break;
                            case "ORACLE":
                                m_hashAttributeAscending.Add(strAttribute, strAscending.Trim() != "0");
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                                //break;
                        }
                    }
                }
                m_hashAttributeDefault.Add(strAttribute, strDefault);
                m_hashAttributeType.Add(strAttribute, strType);
                m_hashAttributeFormat.Add(strAttribute, strFormat);
                m_listAttribute.Add(strAttribute);
            }

            if(!m_listAttribute.Contains("LENGTH"))
            {
                m_hashAttributeDefault.Add("LENGTH", "1");
                m_hashAttributeType.Add("LENGTH", "NUMBER");
                m_hashAttributeFormat.Add("LENGTH", "f3");
                m_listAttribute.Add("LENGTH");
            }

            if (!m_listAttribute.Contains("AREA"))
            {
                m_hashAttributeDefault.Add("AREA", "1");
                m_hashAttributeType.Add("AREA", "NUMBER");
                m_hashAttributeFormat.Add("AREA", "f0");
                m_listAttribute.Add("AREA");
            }
        }

        static public String GetAttributeType(String strAttribute)
        {
            String strType = "";
            if (m_hashAttributeType.Contains(strAttribute))
            {
                strType = (String)m_hashAttributeType[strAttribute];


            }
            return strType;
        }

        static public String GetAttributeFormat(String strAttribute)
        {
            String strFormat = "";
            if (m_hashAttributeFormat.Contains(strAttribute))
            {
                strFormat = (String)m_hashAttributeFormat[strAttribute];
            }
            return strFormat;
        }

        static public String GetAttributeDefault(String strAttribute)
        {
            String strDefault = "";
            if (m_hashAttributeDefault.Contains(strAttribute))
            {
                strDefault = (String)m_hashAttributeDefault[strAttribute];
            }
            return strDefault;
        }

        static public bool GetAttributeMinimum(String strAttribute, out double dMinimum)
        {
            if (m_hashAttributeMinimum.Contains(strAttribute))
            {
                dMinimum = (double)m_hashAttributeMinimum[strAttribute];
                return true;
            }
            dMinimum = 0;
            return false;
        }

        static public bool GetAttributeMaximum(String strAttribute, out double dMaximum)
        {
            if (m_hashAttributeMaximum.Contains(strAttribute))
            {
                dMaximum = (double)m_hashAttributeMaximum[strAttribute];
                return true;
            }
            dMaximum = 0;
            return false;
        }

        static public bool GetAttributeAscending(String strAttribute)
        {
            bool bAscending = true;
            if (m_hashAttributeAscending.Contains(strAttribute))
            {
                bAscending = (bool)m_hashAttributeAscending[strAttribute];
            }
            return bAscending;
        }

        /// <summary>
        /// Returns true if the attribute passed in has TYPE = STRING, false if TYPE = NUMBER
        /// </summary>
        /// <param name="strAttribute">The attribute to determine TYPE as STRING or NUMBER</param>
        /// <returns></returns>
        static public bool IsStringAttribute(String strAttribute)
        {
            String strQuery = "Select TYPE_ From ATTRIBUTES_ Where ATTRIBUTE_ = '" + strAttribute + "'";
            DataSet ds = DBMgr.ExecuteQuery(strQuery);
            String strAttributeType = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            if (strAttributeType == "STRING")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds an asset table update insert delete or even query if desired, to the global asset update table.
        /// This table will be used to update the central database with all remote asset actions.
        /// </summary>
        /// <param name="strSqlStatement">The sql statement to run on the global table</param>
        /// <param name="strAssetTableName">The name of the asset table to update.</param>
        /// <returns></returns>
        static public bool AddToGlobalAssetTable(String strSqlStatement, String strAssetTableName)
        {
            String strInsert = "INSERT INTO " + strAssetTableName + "(GLOBAL_SYNCH_STRING) VALUES '" + strSqlStatement + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strInsert);
            }
            catch (Exception e)
            {
                WriteOutput("Error inserting into Global Asset table. " + e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets cost when comparing one treatment to another.
        /// </summary>
        /// <param name="fCost">Cost of treatment</param>
        /// <param name="strArea"></param>
        /// <param name="strArea"></param>
        /// 
        static public bool GetCost(float fInCost, String strInArea, String strOutArea, out float fOutCost )
        {
            fOutCost = 0;
            float fOutArea;
            float fInArea;
            if (fInCost == 0)return true;
            if (strInArea.Trim().Length == 0) return false;
            if (strOutArea.Trim().Length == 0) return false;

            try
            {
                fInArea = float.Parse(strInArea);
                if (fInArea == 0) return false;
            }
            catch
            {
                return false;
            }


            try
            {
                fOutArea = float.Parse(strOutArea);
                if (fOutArea == 0) return false;
            }
            catch
            {
                return false;
            }



            //Calculate cost per square foot 
            //Default unit is lane-mile.


            fOutCost = fInCost * fOutArea / fInArea;
            return true;
        }

        /// <summary>
        /// Used to create the syntax stored in each asset table to draw the asset on the screen using SharpMap API
        /// </summary>
        /// <param name="strTableName">Name of asset table to create geometry strings for.</param>
        static public void CalcGeom(String strTableName, String strLatColumnName, String strLongColumnName)
        {
            // First step is to parse the latitude on longitude fields for each row in the table
            String strQuery = "SELECT " + strLatColumnName + ", " + strLongColumnName + " FROM " + strTableName;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter( strQuery, DBMgr.NativeConnectionParameters.ConnectionString );
            DataTable dtLatLong = new DataTable();

            // Step two: Take parsed information and write it out to a csv file
        }

        /// <summary>
        /// A generic bulk paste for large database inserts.
        /// </summary>
        /// <param name="strTableName">Name of the table to insert the data into</param>
        /// <param name="sqlDataAdapter">Adapter to hold the data</param>
        /// <param name="binding">binding</param>
        /// <param name="dgvToFill">dgv to fill with resulting paste data</param>
        static public void BulkPaste(String strTableName, DatabaseManager.DataAdapter dataAdapter, BindingSource binding, DataGridView dgvToFill)
        {
            Global.ClearOutputWindow();

            dataAdapter.Update((DataTable)binding.DataSource);
            dataAdapter.Dispose();

            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);

            String strOutFile = strMyDocumentsFolder + "\\paste.txt";
            TextWriter tw = new StreamWriter(strOutFile);

            string s = Clipboard.GetText();
            s = s.Replace("\r\n", "\n");
            string[] lines = s.Split('\n');
            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    //Split on either commas or tabs
                    string[] cells = line.Split(new string[] {"\t"}, StringSplitOptions.None);

                    //Trim white space.
                    for (int i = 0; i < cells.Length; i++)
                    {
                        cells[i] = cells[i].Trim();
                    }

                    if (cells[0].ToString() != "")
                    {
                        //if (!m_listFacility.Contains(cells[0]))
                        {
                            Global.WriteOutput(line + "\t not imported because it does not exist in Facility/Route list");
                            continue;
                        }
                    }
                    
                    String strOut = "";
                    for (int i = 0; i < cells.Length; i++)
                    {
                        // Put a tab between each asset attribute.
                        strOut += "\t";
                        
                        strOut += cells[i];
                    }
                    tw.WriteLine(strOut);
                }
                else
                {
                    continue;
                }
            }
            tw.Close();

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(strTableName, strOutFile, '\t');
                    break;
                case "ORACLE":
                    DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, strTableName, strOutFile, DBMgr.GetTableColumns(strTableName, DBMgr.NativeConnectionParameters), "\\t");
                    break;
                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }
            CreateDataGridView(strTableName, dgvToFill);
        }

        /// <summary>
        /// Searches through a Query or Equation and extracts all of the database attributes encased in square brackets [attribute].
        /// </summary>
        /// <param name="strQuery">Query or Equation to parse.</param>
        /// <returns>List of variables in global variable.</returns>
        static public List<String> ParseAttribute(String strQuery)
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
                        if (GetAttributeType(strAttribute) == "")
                        {
                            WriteOutput("Attribute - " + strAttribute + " is not included in the RoadCare Attribute types.  Please check spelling.");
                        }
                        else
                        {
                            list.Add(strAttribute);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Searches through a Query or Equation and extracts all of the database attributes encased in square brackets [attribute].
        /// </summary>
        /// <param name="strQuery">Query or Equation to parse.</param>
        /// <returns>List of variables in global variable.</returns>
        static public List<String> TryParseAttribute(String strQuery, out List<string> listError)
        {
            List<String> list = new List<String>();
            //            strQuery = strQuery.ToUpper();
            String strAttribute;
            int nOpen = -1;
            listError = new List<string>();
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
                        if (strAttribute == "LENGTH" || strAttribute == "AREA")
                        {
                            list.Add(strAttribute);
                            continue;

                        }
                        if (GetAttributeType(strAttribute) == "")
                        {
                            String strError ="Attribute - " + strAttribute + " is not included in the RoadCare Attribute types.  Please check spelling.";
                            listError.Add(strError);
                        }
                        else
                        {
                            list.Add(strAttribute);
                        }
                    }
                }
            }
            return list;
        }

        static private void CreateDataGridView(String strTableName, DataGridView dgvToFill)
        {
            SqlConnection sqlConn = new SqlConnection();

            if (!DBMgr.NativeConnectionParameters.IsOleDBConnection)
            {
                sqlConn = DBMgr.NativeConnectionParameters.SqlConnection;
            }
            BindingSource binding = new BindingSource();


            String strQuery;
            strQuery = "SELECT * FROM " + strTableName;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(strQuery, sqlConn.ConnectionString.ToString());
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder((SqlDataAdapter)sqlDataAdapter);

            // Populate a new data table and bind it to the BindingSource.
            DataTable table = new DataTable();

            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            sqlDataAdapter.Fill(table);

            binding.DataSource = table;
            dgvToFill.DataSource = binding;
            dgvToFill.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        }

        /// <summary>
       /// Checks string if allowed SQL entry (no single or double quotes
       /// </summary>
       /// <param name="strSQL"></param>
       /// <returns></returns>
        static public bool CheckString(String strSQL)
        {
            if (strSQL.Contains("'") || strSQL == "")
            {
                MessageBox.Show("Illegal character in input string, or Simulation name is blank.");
                return false;
            }
            else
            {
                return true;
            }

        }

        static public ArrayList GetMSSQLInstances()
        {
            ArrayList listMSSQLComputerNames = new ArrayList();
            System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;

            // Create a data table to hold the names of available instances
            // and add them to the combo box of available servers to choose from
            DataTable dtInstanceNames = instance.GetDataSources();
            foreach (DataRow dr in dtInstanceNames.Rows)
            {
                listMSSQLComputerNames.Add(dr[0].ToString());
            }
            return listMSSQLComputerNames;
        }

        static public ArrayList GetNetworkComputers()
        {
            NetworkBrowser netBrowser = new NetworkBrowser();
            ArrayList alTemp = new ArrayList();
            alTemp = netBrowser.getNetworkComputers();
            return alTemp;
        }

        static public int GetPropertyIndex(String strPropertyName, PropertyGridEx.PropertyGridEx pg)
        {
            for (int i = 0; i < pg.Item.Count; i++)
            {
                if (pg.Item[i].Name != null)
                {
                    if (strPropertyName == pg.Item[i].Name)
                    {
                        return i;
                    }
                }
            }

            // Couldnt find the label, or another error has occured
            return -1;
        }

        static public void CreateTempShapeFile(String strShapeFilePath)
        {
            // Open the shapefile at the given path
            ShapeFile shapefile = new SharpMap.Data.Providers.ShapeFile(strShapeFilePath);
            try
            {
                shapefile.Open();
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
                return;
            }

            // This should get us every row in the shapefile.
            // We want to take each row and write it out to a file, then bulk load the file into 
            // the database table TEMP_SHAPEFILE.  From that table, we can run the selects necessary
            // to build the section based network defition, including the geoms from the shapefile.
            int iNumFeatureRows = shapefile.GetFeatureCount();
            FeatureDataRow fdr;
            fdr = shapefile.GetFeature(0);

            TextWriter tw = null;
            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);

            String strOutFile = strMyDocumentsFolder + "\\paste.txt";
            try
            {
                tw = new StreamWriter(strOutFile);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
                return;
            }
            for (uint i = 0; i < iNumFeatureRows; i++)
            {
                fdr = shapefile.GetFeature(i);
                if (fdr.Geometry != null)
                {
                    tw.Write(fdr.Geometry.ToString());
                    for (int j = 0; j < fdr.Table.Columns.Count; j++)
                    {
                        tw.Write("\t" + fdr.ItemArray[j].ToString());
                    }
                    tw.Write("\r\n");
                }
                else
                {
                    for (int j = 0; j < fdr.Table.Columns.Count; j++)
                    {
                        tw.Write("\t" + fdr.ItemArray[j].ToString());
                    }
                    tw.Write("\r\n");
                }
            }
            tw.Close();

            // Now create the TEMP_SHAPEFILE table in the database, this will be dropped after its used.
            List<DatabaseManager.TableParameters> listColumn = new List<DatabaseManager.TableParameters>();
            //listColumn.Add(new TableParameters("FACILITY", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("SECTION", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("BEGIN_STATION", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("END_STATION", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("MILEPOST", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("DIRECTION", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("LATITUDE", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("LONGITUDE", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("LAST_MODIFIED", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("EnvelopeMaxX", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("EnvelopeMaxY", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("EnvelopeMinX", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            //listColumn.Add(new TableParameters("EnvelopeMinY", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            listColumn.Add(new TableParameters("GEOMETRY", Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
            for (int i = 0; i < fdr.Table.Columns.Count; i++)
            {
                if (fdr.Table.Columns[i].DataType.ToString() == "System.Double" || fdr.Table.Columns[i].DataType.ToString() == "System.Single")
                {
                    listColumn.Add(new TableParameters(fdr.Table.Columns[i].ColumnName, Microsoft.SqlServer.Management.Smo.DataType.Float, true, false));
                }
                else
                {
                    listColumn.Add(new TableParameters(fdr.Table.Columns[i].ColumnName, Microsoft.SqlServer.Management.Smo.DataType.VarChar(-1), true, false));
                }
            }
            try
            {
                DBMgr.ExecuteNonQuery("DROP TABLE TEMP_SHAPEFILE");
            }
            catch
            {
                // If TEMP_SHAPEFILE dne then just keep on GOING!
            }
            try
            {
                DBMgr.CreateTable("TEMP_SHAPEFILE", listColumn);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
                return;
            }
            shapefile.Close();

            // Now bulk load into the database

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad("TEMP_SHAPEFILE", strOutFile, '\t');
                    break;
                case "ORACLE":
                    List<string> columnNames = new List<string>();
                    foreach (TableParameters column in listColumn)
                    {
                        columnNames.Add(column.GetColumnName());
                    }
                    DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, "TEMP_SHAPEFILE", strOutFile, columnNames, "\\t");
                    break;
                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }
        }

        static public Hashtable GetAttributeYear(String strNetworkID)
        {
            String strTable = "SEGMENT_" + strNetworkID.ToString() + "_NS0";
            Hashtable hashAttributeYear = new Hashtable();
            List<string> listColumns = DBMgr.GetTableColumns(strTable);

            Regex yearSelection = new Regex("^[1-9][0-9][0-9][0-9]$");
            
            foreach (string strColumn in listColumns)
            {
                int nYear;
                if(strColumn == "SECTIONID") continue;
                string[] columns = strColumn.Split('_');

                //totally the wrong way to do this.
                //try
                //{
                //    nYear = int.Parse(columns[columns.Length - 1]);
                //}
                //catch//This is just an attribute
                //{
                //    //Ignore 
                //    continue;
                //}

                Match yearHit = yearSelection.Match(columns[columns.Length - 1]);
                if (yearHit.Success)
                {
                    nYear = int.Parse(yearHit.Value);
                    //I can't make myself do it.
                    //}
                    //else
                    //{
                    //    //continue;		
                    //}

                    String strAttribute = strColumn.Substring(0, strColumn.Length - columns[columns.Length - 1].Length - 1);
                    List<String> listYear;
                    if (!hashAttributeYear.Contains(strAttribute))
                    {
                        listYear = new List<String>();
                        hashAttributeYear.Add(strAttribute, listYear);
                    }
                    else
                    {
                        listYear = (List<String>)hashAttributeYear[strAttribute];
                    }
                    listYear.Add(nYear.ToString());
                }
            }
            return hashAttributeYear;
        }


        static public Hashtable GetAttributeYear(String strNetworkID,String strSimulationID)
        {
            Hashtable hashAttributeYear= GetAttributeYear(strNetworkID);
            if (strSimulationID == "") return hashAttributeYear;


            String strTable = "SIMULATION_" + strNetworkID.ToString() + "_" + strSimulationID; ;

            List<string> listColumns = DBMgr.GetTableColumns(strTable);
            foreach (string strColumn in listColumns)
            {
                int nYear;
                if (strColumn == "SECTIONID") continue;
                string[] columns = strColumn.Split('_');
                try
                {
                    nYear = int.Parse(columns[columns.Length - 1]);
                }
                catch//This is just an attribute
                {
                    //Ignore 
                    continue;
                }

                String strAttribute = strColumn.Substring(0, strColumn.Length - columns[columns.Length - 1].Length - 1);
                List<String> listYear;
                if (!hashAttributeYear.Contains(strAttribute))
                {
                    listYear = new List<String>();
                    hashAttributeYear.Add(strAttribute, listYear);
                }
                else
                {
                    listYear = (List<String>)hashAttributeYear[strAttribute];
                }
                listYear.Add(nYear.ToString());
            }
            return hashAttributeYear;
        }

        public static List<String> GetAttributeYears(String attributeName, Hashtable hash)
        {
            return (List<String>)hash[attributeName];
        }

        //Committed Project Simulation To Simulation Copy
        public static void CopyCommittedProject(String strTreatment, String strYearAny, String strYearSame, String strCost, String strBudget,String strArea, List<CommitAttributeChange> attributeChange)
        {
            m_strTreatmentName = strTreatment;
            m_strYearAny = strYearAny;
            m_strYearSame = strYearSame;
            m_strCost = strCost;
            m_strBudget = strBudget;
            m_listAttributeChange = attributeChange;
            m_strArea = strArea;
        }

        internal static LockInformation GetLockInfo( string networkID, string simulationID )
        {
            LockInformation lockCheck = null;
            DataSet lockData = null;
            if( String.IsNullOrEmpty( simulationID ) )
            {
                lockData = DBOp.GetCurrentNetworkLockData( networkID );
            }
            else
            {
                lockData = DBOp.GetCurrentSimulationLockData( networkID, simulationID );
            }

            if( lockData.Tables.Count > 0 && lockData.Tables[0].Rows.Count > 0)
            {
                lockCheck = new LockInformation( lockData.Tables[0].Rows[0] );
            }
            else
            {
                lockCheck = new LockInformation();
            }

            return lockCheck;

        }

        internal static LockInformation LockNetwork( string networkID, bool readable )
        {
            DBOp.AddNetworkLock( networkID, m_globalSecurityOperations.CurrentUser.Name, readable );

            return GetLockInfo( networkID, "" );
        }

        internal static LockInformation LockSimulation( string networkID, string simulationID, bool readable )
        {
            DBOp.AddSimulationLock( networkID, simulationID, m_globalSecurityOperations.CurrentUser.Name, readable );

            return GetLockInfo( networkID, simulationID );
        }
    }
}
