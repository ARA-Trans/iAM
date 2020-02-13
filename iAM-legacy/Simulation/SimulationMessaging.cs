using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DatabaseManager;
using System.Data;
using System.IO;
using Simulation.Interface;
using System.Configuration;

namespace Simulation
{
    public static class SimulationMessaging
    {

        static private List<SimulationMessage> m_strListProgress = new List<SimulationMessage>();
        public static bool IsDesktop { private get; set; } = true;
        [ThreadStatic]
        static private bool m_bCancel = false;
        [ThreadStatic]
        static private Hashtable m_hashAttributeType;  //The type of each attribute.
        [ThreadStatic]
        static private Hashtable m_hashAttributeDefault;  //The type of each attribute.
        [ThreadStatic]
        static private Hashtable m_hashAttributeMinimum;//Key attribute. Object double of minimum value.
        [ThreadStatic]
        static private Hashtable m_hashAttributeMaximum;//Key attribute. Object double of maximum value.
        [ThreadStatic]
        static private Hashtable m_hashAttributeAscending;
        [ThreadStatic]
        static private Hashtable m_hashAttributeFormat;
        [ThreadStatic]
        static private Hashtable m_hashAttributeOMS;
        [ThreadStatic]
        static private List<String> m_listDeteriorate;
        [ThreadStatic]
        static private List<String> m_listAttribute;
        [ThreadStatic]
        static private Hashtable m_hashTargets;
        [ThreadStatic]
        static private AnalysisMethod m_Method;
        [ThreadStatic]
        static private float m_fAreaConversionFactor = 0;
        [ThreadStatic]
        static private String m_sSimulationTable;
        [ThreadStatic]
        static private String m_sBCTable;
        [ThreadStatic]
        static private String m_sBCTargetTable;
        [ThreadStatic]
        static private String m_sReportTable;
        [ThreadStatic]
        static private String m_sTargetTable;
        [ThreadStatic]
        static private String m_sReasonsTable;
        [ThreadStatic]
        static private String m_strLanesVariable;
        [ThreadStatic]
        static private CRS m_CRS;
        [ThreadStatic]
        static private CalculateEvaluate.CalculateEvaluate m_crArea;
        [ThreadStatic]
        static private List<String> m_listArea;
        [ThreadStatic]
        static private string _simulationID;
        [ThreadStatic]
        static private string _alternateID;
		[ThreadStatic]
		static private List<OCIWeight> _conditionCategoryWeight;
        [ThreadStatic]
        static private bool _isOMS;
        [ThreadStatic]
        static private List<Deteriorate> _deteriorates;
        [ThreadStatic]
        static private double _lengthLastRun;
        [ThreadStatic]
        static private DateTime _dateTimeStart;
        [ThreadStatic]
        static private int _numberAssets;
        [ThreadStatic]
        static private int _analysisPeriod;
        [ThreadStatic]
        static private List<ConditionalRSL> _attributeConditionalRSL;
        [ThreadStatic]
        static private Jurisdiction _jurisdiction;
        [ThreadStatic]
        static private List<RemainingLife> _attributeRemainingLife;
        [ThreadStatic]
        static private Dictionary<string,string> _noTreatmentRemainingLife;
        [ThreadStatic]
        static private List<BudgetCriteria> _budgetCriteria;
        [ThreadStatic]
        static private List<ISplitTreatment> _splitTreatments;

        public static void ReleaseSimulationData()
        {
            m_hashAttributeType.Clear();
            m_hashAttributeDefault.Clear();
            m_hashAttributeMinimum.Clear();
            m_hashAttributeMaximum.Clear();
            m_hashAttributeAscending.Clear();
            m_hashAttributeFormat.Clear();
            m_hashAttributeOMS.Clear();
            _deteriorates.Clear();
            _attributeConditionalRSL.Clear();
            _attributeRemainingLife.Clear();
            _noTreatmentRemainingLife.Clear();
        }


        public static List<ConditionalRSL> AttributeConditionalRSL
        {
            get { return _attributeConditionalRSL; }
            set {_attributeConditionalRSL = value; }
        }


        public static int AnalysisPeriod
        {
            get { return _analysisPeriod; }
            set { _analysisPeriod = value; }
        }

        public static int NumberAssets
        {
            get { return _numberAssets; }
            set { _numberAssets = value; }
        }


        public static DateTime DateTimeStart
        {
            get { return _dateTimeStart; }
            set { _dateTimeStart = value; }
        }

        public static double LengthLastRun
        {
            get { return _lengthLastRun; }
            set { _lengthLastRun = value; }
        }

        internal static List<Deteriorate> Deteriorates
        {
            get { return _deteriorates; }
            set { _deteriorates = value; }
        }


        public static bool IsOMS
        {
            get { return _isOMS; }
            set { _isOMS = value; }
        }


        public static string SimulationID
        {
            get { return _simulationID; }
            set { _simulationID = value; }
        }


        public static string AlternateID
        {
            get { return _alternateID; }
            set { _alternateID = value; }
        }

        static public bool Valid
        {
            get {return true;}
        }

        static public CalculateEvaluate.CalculateEvaluate Area
        {
            get { return m_crArea; }
            set { m_crArea = value; }
        }

        static public List<String> ListArea
        {
            get { return m_listArea; }
            set { m_listArea = value; }
        }

        static public CRS crs
        {
            get { return m_CRS; }
            set { m_CRS = value; }
        }

        public static Hashtable AttributeMinimum
        {
            get { return SimulationMessaging.m_hashAttributeMinimum; }
            set { SimulationMessaging.m_hashAttributeMinimum = value; }
        }

        public static Hashtable AttributeMaximum
        {
            get { return SimulationMessaging.m_hashAttributeMaximum; }
            set { SimulationMessaging.m_hashAttributeMaximum = value; }
        }

        public static Hashtable AttributeOMS
        {
            get { return SimulationMessaging.m_hashAttributeOMS; }
            set { SimulationMessaging.m_hashAttributeOMS = value; }
        }

        static public float AreaConversionFactor
        {
            get
            {
                if (m_fAreaConversionFactor == 0)
                {
                    String strQuery = "SELECT OPTION_VALUE FROM OPTIONS WHERE" + cgOMS.Prefix + " OPTION_NAME = 'NETWORK_DEFINITION_UNITS'";
                    try
                    {
                        DataSet ds = DBMgr.ExecuteQuery(strQuery);
                        String strUnit = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        switch (strUnit)
                        {
                            case "Miles":
                                m_fAreaConversionFactor = 5280;
                                break;
                            case "Meters":
                                m_fAreaConversionFactor = 3.2808399f;
                                break;
                            case "Kilometers":
                                m_fAreaConversionFactor = 3280.8399f;
                                break;
                            case "Feet":
                                m_fAreaConversionFactor = 1;
                                break;
                            case "Yards":
                                m_fAreaConversionFactor = 3;
                                break;
                            case "Rods":
                                m_fAreaConversionFactor = 16.5f;
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error: Problem retrieving area conversion factor. Defaulting to 5280. " + exc.Message));
                        m_fAreaConversionFactor = 5280;
                    }
                }
                return m_fAreaConversionFactor;
            }
        }

        /// <summary>
        /// Simulation Consequence Table.
        /// </summary>
        static public Hashtable Targets
        {
            get { return m_hashTargets; }
            set { m_hashTargets = value; }
        }

        static public List<String>ListAttributes
        {
            get { return m_listAttribute; }
            set { m_listAttribute = value; }
        }

        static public List<RemainingLife> RemainingLifes
        {
            get { return _attributeRemainingLife; }
            set { _attributeRemainingLife = value; }
        }

        static public Dictionary<string,string> NoTreatmentRemainingLife
        {
            get { return _noTreatmentRemainingLife; }
            set { _noTreatmentRemainingLife = value; }
        }

        static public List<BudgetCriteria> BudgetCriterias
        {
            get { return _budgetCriteria; }
            set { _budgetCriteria = value; }
        }

        static public List<ISplitTreatment> SplitTreatments
        {
            get { return _splitTreatments; }
            set { _splitTreatments = value; }
        }

        /// <summary>
        /// Simulation Consequence Table.
        /// </summary>
        static public String LanesVariable
        {
            get { return m_strLanesVariable; }
            set { m_strLanesVariable = value; }
        }


        /// <summary>
        /// Target Table.
        /// </summary>
        static public String TargetTable
        {
            get { return m_sTargetTable; }
            set { m_sTargetTable = value; }
        }

        /// <summary>
        /// Simulation Table which stores simulation attributes.
        /// </summary>
        static public String SimulationTable
        {
            get { return m_sSimulationTable; }
            set { m_sSimulationTable = value; }
        }

        /// <summary>
        /// Stores what occured to each section for the simulation.
        /// </summary>
        static public String ReportTable
        {
            get { return m_sReportTable; }
            set { m_sReportTable = value; }
        }
        /// <summary>
        /// Stores the BC of each feasible treatment for ordering
        ///
        /// </summary>
        static public String BenefitCostTable
        {
            get { return m_sBCTable; }
            set { m_sBCTable = value; }
        }

        /// <summary>
        /// Stores the BC modified for target for each feasible treatment for ordering
        ///
        /// </summary>
        static public String BenefitCostTargetTable
        {
            get { return m_sBCTargetTable; }
            set { m_sBCTargetTable = value; }
        }
        /// <summary>
        /// Stores costs that make up CumulativeCost
        /// </summary>
        public static string CumulativeCostTable { get; set; }

        public static string ReasonsTable
        {
            get { return m_sReasonsTable; }
            set { m_sReasonsTable = value; }
        }

        static public List<SimulationMessage> GetProgressList()
        {
            lock (m_strListProgress)
            {
                return m_strListProgress;
            }
        }


        static public void AddMessage(string baseMessage, Exception ex)
		{
			Exception currentException = ex;
			if (currentException != null)
			{
				baseMessage += "{" + currentException.Message + "}";
			}
			//AddMessage(baseMessage);
		}
		
		static public void AddMessage(SimulationMessage message)
        {
            if (!IsDesktop)
            {
                return;
            }

            if (message.Percent != 100)
            {
                message.Percent = PercentComplete();
            }

            lock (m_strListProgress)
            {
                m_strListProgress.Add(message);
            }
        }
        static public void ClearProgressList()
        {
            lock (m_strListProgress)
            {
                m_strListProgress.Clear();
            }
        }


        static public void ClearProgressList(string simulationID)
        {
            lock (m_strListProgress)
            {
                m_strListProgress.RemoveAll(delegate (SimulationMessage sm) { return sm.SimulationID == simulationID;});
            }
        }


        static public void SetCancel(bool bCancel)
        {
            m_bCancel = bCancel;
        }

        static public bool GetCancel()
        {
            return m_bCancel;
        }

        static public AnalysisMethod Method
        {
            get { return m_Method; }
            set { m_Method = value; }
        }

		public static List<OCIWeight> ConditionCategoryWeight
		{
			get { return _conditionCategoryWeight; }
			set { _conditionCategoryWeight = value; }
		}

        public static Jurisdiction Jurisdiction
        {
            get { return _jurisdiction;}
            set { _jurisdiction = value; }
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
                        if(strAttribute == "LENGTH" || strAttribute == "AREA")
                        {
                                list.Add(strAttribute);
                        }
                        else if (GetAttributeType(strAttribute) == "")
                        {
                            AddMessage(new SimulationMessage("Attribute - " + strAttribute + " is not included in the RoadCare Attribute types.  Please check spelling."));
                            return null;
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


        static public List<String> ParseAttributeNoCheck(String strQuery)
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
                     list.Add(strAttribute);
                }
            }
            return list;
        }

        static public void LoadAttributes(string simulationID)
        {
            String strAttribute;
            String strType;
            String strDefault;
            String strMinimum;
            String strMaximum;
            String strAscending;
            String strFormat;
            String strAttributeOMS;

            m_hashAttributeType = new Hashtable();
            m_hashAttributeAscending = new Hashtable();
            m_hashAttributeDefault = new Hashtable();
            m_hashAttributeMaximum = new Hashtable();
            m_hashAttributeMinimum = new Hashtable();
            m_hashAttributeFormat = new Hashtable();
            m_hashAttributeOMS = new Hashtable();
 

            string attributesTable = "ATTRIBUTES_";
            if (IsOMS)
            {
                attributesTable = cgOMS.Prefix + "OMS_ATTRIBUTES";
            }

            if (DBMgr.IsTableInDatabase(attributesTable))
            {
                m_hashAttributeType.Add("SECTIONID", "NUMBER");
                //Chad why was this changed to 
                String strSelect = "SELECT ATTRIBUTE_,TYPE_,DEFAULT_VALUE,MINIMUM_,MAXIMUM,ASCENDING,FORMAT FROM " + attributesTable;
                if(IsOMS) strSelect =  "SELECT ATTRIBUTE_,TYPE_,DEFAULT_VALUE,MINIMUM_,MAXIMUM,ASCENDING,FORMAT,ATTRIBUTE_OMS FROM " + attributesTable + " WHERE SIMULATIONID='" + simulationID + "'";
                // TODO: Error handling code here somewhere?
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    strAttribute = row[0].ToString();
                    strType = row[1].ToString();
                    if (strType == "STRING" || strType == "DATETIME")
                    {
                        strDefault = row[2].ToString();
                    }
                    else
                    {
                        strDefault = row[2].ToString();
                        strMinimum = row[3].ToString();
                        strMaximum = row[4].ToString();
                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                strAscending = row[5].ToString();
                                break;
                            case "ORACLE":
                                strAscending = (row[5].ToString() == "1").ToString();
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                            //break;
                        }
                        strFormat = row[6].ToString();
                        if (!String.IsNullOrEmpty(strMinimum))
                        {
                            m_hashAttributeMinimum.Add(strAttribute, double.Parse(strMinimum));
                        }
                        if (!String.IsNullOrEmpty(strMaximum))
                        {
                            m_hashAttributeMaximum.Add(strAttribute, double.Parse(strMaximum));
                        }
                        if (strAscending != "")
                        {
                            m_hashAttributeAscending.Add(strAttribute, bool.Parse(strAscending));

                        }
                        m_hashAttributeFormat.Add(strAttribute, strFormat);
                    }
                    m_hashAttributeDefault.Add(strAttribute, strDefault);
                    m_hashAttributeType.Add(strAttribute, strType);

                    if(_isOMS)
                    {
                        strAttributeOMS = row["ATTRIBUTE_OMS"].ToString();
                        m_hashAttributeOMS.Add(strAttributeOMS,strAttribute);//Used to look up RoadCare attribute from OMSHierarchy
                    }
                }
            }
        }

        /// <summary>
        /// Fills attribute type for calculate/evaluate.  Added to allow Strings and DateTime in function equations.
        /// </summary>
        public static void FillAttributeType()
        {
            try
            {
                //Table which stores attribute properties
                m_hashAttributeType = new Hashtable();
                String select = "SELECT ATTRIBUTE_,TYPE_,DEFAULT_VALUE,MINIMUM_,MAXIMUM,ASCENDING,FORMAT FROM ATTRIBUTES_";
                DataSet ds = DBMgr.ExecuteQuery(select);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String attribute = row[0].ToString();
                    String type = row[1].ToString();
                    m_hashAttributeType.Add(attribute, type);
                }
            }
            catch
            {
                m_hashAttributeType = new Hashtable();
            }
        }


        /// <summary>
        /// Retrieves the type (String, Double or DateTime) of the given attribute.
        /// </summary>
        /// <param name="strAttribute"></param>
        /// <returns></returns>
        static public String GetAttributeType(String strAttribute)
        {
            //Fill attribute type if currently is null. Do once per execution.
            if(m_hashAttributeType == null)
            {
                FillAttributeType();             
            }
            String strType = "";
            if (m_hashAttributeType.Contains(strAttribute))
            {
                strType = m_hashAttributeType[strAttribute].ToString();
            }
            else if(strAttribute.ToUpper() == "LENGTH" || strAttribute.ToUpper() == "AREA")
            {
                strType = "NUMBER";
            }
            else
            {
                AddMessage(new SimulationMessage("Attribute - " + strAttribute + " does not exist in RoadCare Database.  Is the variable included in criteria or equation spelled and capitalized correctly."));

            }
            return strType;
        }

        static public String GetAttributeFormat(String strAttribute)
        {
            String strFormat = "";
            if (m_hashAttributeFormat.Contains(strAttribute))
            {
                strFormat = m_hashAttributeFormat[strAttribute].ToString();
            }
            return strFormat;
        }



        static public String GetAttributeDefault(String strAttribute)
        {
            String strDefault = "";
            if (m_hashAttributeDefault.Contains(strAttribute))
            {
                strDefault = m_hashAttributeDefault[strAttribute].ToString();
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

        static public bool IsAttribute(String strAttribute)
        {
            if (m_hashAttributeType.Contains(strAttribute)) return true;
            else return false;

        }


        static  public void GetUniqueDeteriorateAttributes(String strSimulationID)
        {
            m_listDeteriorate = new List<string>();
            String strSelect = "SELECT DISTINCT ATTRIBUTE_ FROM " + cgOMS.Prefix + "PERFORMANCE WHERE SIMULATIONID='" + strSimulationID + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                m_listDeteriorate.Add(row[0].ToString());
            }
        }

        static public List<String> DeteriorateAttributes
        {
            get { return m_listDeteriorate; }
        }

        static public bool GetDeficientLevel(String strAttribute, Hashtable hashAttributeValue, out double dDeficient)
        {
            dDeficient = 0;
            Hashtable hashAttributeListTargets;
            if (Targets.Contains("All"))
            {
                hashAttributeListTargets = (Hashtable)Targets["All"];
                if (!hashAttributeListTargets.Contains(strAttribute)) return false;

                List<Targets> listTargets = (List<Targets>)hashAttributeListTargets[strAttribute];
                foreach (Targets target in listTargets)
                {
                    if (target.IsDeficient)
                    {
                        if (target.IsAllSections)
                        {
                            dDeficient = target.Deficient;
                            return true;
                        }
                        else
                        {
                            if (target.Criteria.IsCriteriaMet(hashAttributeValue))
                            {
                                dDeficient = target.Deficient;
                                return true;
                            }

                        }
                    }
                }
            }
            return false;
        }

        static public TextWriter CreateTextWriter(String strFile, out String strOutFile)
        {
            //String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //strMyDocumentsFolder += "\\RoadCare Projects\\Temp";

            // This path will be "..\BridgeCare\iAMApp\BridgeCareApp\BridgeCare"
            //string workingDirectory = HostingEnvironment.ApplicationPhysicalPath;
            string val = ConfigurationManager.AppSettings["TempFilePath"];
            string path = System.Environment.ExpandEnvironmentVariables(val);

            Directory.CreateDirectory(path);

			strOutFile = path + "\\" + strFile;
            TextWriter tw = new StreamWriter(strOutFile);
            return tw;
        }




        public static void SaveSerializedCalculateEvaluate(string tableName, string binaryColumnName, string ID, CalculateEvaluate.CalculateEvaluate equationCritera)
        {
            byte[] assembly = RoadCareGlobalOperations.AssemblySerialize.SerializeObjectToByteArray(equationCritera);
            string path = Path.GetTempPath() + "DecisionEngine";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "\\" + tableName + "_" + binaryColumnName + "_" + ID + ".bin";
            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                file.Write(assembly, 0, assembly.Length);
                file.Close();
            }
        }


        public static byte[] GetSerializedCalculateEvaluate(string tableName, string binaryColumnName, string ID, byte[] assembly)
        {
            string path = Path.GetTempPath() + "DecisionEngine\\" + tableName + "_" + binaryColumnName + "_" + ID + ".bin";
            if (File.Exists(path))
            {
                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    assembly = new byte[file.Length];
                    file.Read(assembly, 0, (int)file.Length);
                    file.Close();
                }
            }
            return assembly;
        }

        public static int PercentComplete()
        {
            if (_lengthLastRun == 100 && _numberAssets > 0)
            {
                if (_analysisPeriod == 0)
                {
                    return 0;
                }
                else
                {
                    double _approximateLength = 30 + (_numberAssets * _analysisPeriod) / 100;
                    _lengthLastRun = _approximateLength;
                }
            }

            if (_lengthLastRun == 0) return 0;

            TimeSpan spanCurrent = DateTime.Now - _dateTimeStart;
            double currentSeconds = (double)spanCurrent.Seconds;
            int percent = (int)(100 * currentSeconds / _lengthLastRun);
            if (percent > 99) percent = 99;
            return percent;
        }


        public static object ConvertToRoadCareObject(object value, string attribute)
        {

            string type = "";
            switch(attribute)
            {
                case "LENGTH":
                case "AREA":
                    type = "NUMBER";
                    break;
                default:
                    type =  SimulationMessaging.GetAttributeType(attribute);
                    break;
            }
            object rcValue = null;
            if (type == "STRING")
            { 
                if(value == null)
                {
                    rcValue = null;
                }
                else
                { 
                    rcValue = value.ToString();
                }
            }
            else if (type == "DATETIME")
            {
                if (value == null)
                {
                    rcValue = null;
                }
                else
                {
                    rcValue = Convert.ToDateTime(value);
                }
            }
            else
            {
                if (value == null)
                {
                    rcValue = null;
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(value.ToString()))
                    {
                        rcValue = null;
                    }
                    else //At this point this should only contain valid numbers, catch block is in case.
                    {
                        try
                        {
                            rcValue = Convert.ToDouble(value);
                        }
                        catch
                        {
                            rcValue = null;
                        }
                    }
                }
            }
            return rcValue;


        }
    }
}
