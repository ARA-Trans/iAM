using DatabaseManager;
using Microsoft.SqlServer.Management.Smo;
using MongoDB.Driver;
using RoadCareGlobalOperations;
using Simulation.Interface;
using SimulationDataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;

namespace Simulation
{
    public class Simulation
    {
        private String m_strSimulation;// Name of simulation being run.
        private String m_strSimulationID;// SIMID for simulation which is being run.
        private String m_strNetworkID;// NetworkID which simulation is occurring on.
        private String m_strNetwork; // Name of network that simulation is occuring on.
        private String m_strJurisdiction = ""; // Determines which sections included in Simulation.
        private String m_strWeighting = "";// Weighting variable.
        private Treatments noTreatments;
        private AnalysisMethod m_Method;
        private List<Deteriorate> m_listDeteriorate = new List<Deteriorate>();
        private List<Deteriorate> m_listCalculated = new List<Deteriorate>();
        private List<Treatments> m_listTreatments = new List<Treatments>();
        private static List<CompoundTreatment> _compoundTreatments = new List<CompoundTreatment>();
        private List<Priorities> m_listPriorities = new List<Priorities>();
        private Hashtable m_hashTargets = new Hashtable();//Key == year OR ALL which stores a Hash of Targets and Deficiencys for each attribute
        private Investments m_Investment;
        private List<String> m_listAttributes;
        private bool m_bLanes = false;//Flag to check if lanes exist for calculating

        // Variables for Target/Deficient Met.
        private Hashtable m_hashTargetSectionID = new Hashtable(); //Hash of Target and SectionID.  Target is TargetID,   SectionID is section for which treatment was feasible.

                                                                   //When target is met, all values is ApplyTreatment list are decremented and removed from hash
        private List<AppliedTreatment> m_listApplyTreatment = new List<AppliedTreatment>();//List of all AppliedTreatment objects for Target and Deficient met

        private List<AppliedTreatment> m_listMultipleYearTreatment = new List<AppliedTreatment>();
        private List<Targets> m_listTargets = new List<Targets>();
        private List<Sections> m_listSections = new List<Sections>();// List of Section objects for this analysis.
        private Committed m_committedSingleSection = new Committed();
        private String m_strSectionID = ""; //Single SectionID
        private String m_strYear = "";//Single Section Year

        private Dictionary<string, CommittedEquation> m_dictionaryCommittedEquations;//Stores equations stored in committed equations.
        private Dictionary<string, List<AttributeChange>> m_dictionaryCommittedConsequences; //Stores all the committed project consequences for the simulation
        private List<CalculatedAttribute> m_listCalculatedAttribute;//List of all calculated attributes.

        private Dictionary<string, int> m_dictionaryAttributeSimulationTable;
        private Dictionary<string, List<TableParameters>> m_dictionarySimulationTables;

        private TimeSpan _spanRead = new TimeSpan();
        private TimeSpan _spanAnalysis = new TimeSpan();
        private TimeSpan _spanReport = new TimeSpan();
        private DateTime _dateTimeLast = DateTime.Now;

        //Variable for OMS Single section run.
        private string _actionOMS;

        private string _valueOMS;
        private string _treatmentOMS;
        private int _yearOMS;
        private bool _isUpdateOMS = false;

        private IMongoCollection<SimulationModel> Simulations;
        string mongoConnection = "";
        public IMongoDatabase MongoDatabase;

        public bool IsUpdateOMS
        {
            get { return _isUpdateOMS; }
            set { _isUpdateOMS = value; }
        }

        public string ActionOMS
        {
            get { return _actionOMS; }
            set { _actionOMS = value; }
        }

        public string ValueOMS
        {
            get { return _valueOMS; }
            set { _valueOMS = value; }
        }

        public string TreatmentOMS
        {
            get { return _treatmentOMS; }
            set { _treatmentOMS = value; }
        }

        public int YearOMS
        {
            get { return _yearOMS; }
            set { _yearOMS = value; }
        }

        public static List<CompoundTreatment> CompoundTreatments
        {
            get { return _compoundTreatments; }
        }

        public Committed SingleSection
        {
            set { m_committedSingleSection = value; }
            get { return m_committedSingleSection; }
        }

        public String SingleSectionID
        {
            set { m_strSectionID = value; }
            get { return m_strSectionID; }
        }

        public String SingleSectionYear
        {
            set { m_strYear = value; }
            get { return m_strYear; }
        }

        public bool IsLanes
        {
            get { return m_bLanes; }
            set { m_bLanes = value; }
        }

        public Investments Investment
        {
            get { return m_Investment; }
            set { m_Investment = value; }
        }

        public AnalysisMethod Method
        {
            get { return m_Method; }
            set { m_Method = value; }
        }

        public Hashtable TargetHash
        {
            get { return m_hashTargets; }
            set { m_hashTargets = value; }
        }

        public Simulation(String strSimulation, String strNetwork, String strSimulationID, String strNetworkID)
        {
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            m_strNetworkID = strNetworkID;
            m_strSimulationID = strSimulationID;
            _isUpdateOMS = false;
        }

        public Simulation(String strSimulation, String strNetwork, String strSimulationID, String strNetworkID, string connectionString)
        {
            if (connectionString != null)
            {
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");
            }

            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            m_strNetworkID = strNetworkID;
            m_strSimulationID = strSimulationID;
            _isUpdateOMS = false;
            cgOMS.Prefix = "";
        }

        public Simulation(string simulationID, string sectionID, string action, string treatment, int year, string value, string connectionString)
        {
            if (connectionString != null)
            {
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");
            }
            m_strNetworkID = "1";
            m_strSimulationID = simulationID;
            m_strSectionID = sectionID;
            _actionOMS = action;
            _treatmentOMS = treatment;
            _yearOMS = year;
            _valueOMS = value;
            _isUpdateOMS = true;
            cgOMS.Prefix = "";
        }

        public Simulation(string strSimulation, string strNetwork, int simulationID, int networkID, string simulations)
        {
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            m_strNetworkID = networkID.ToString();
            m_strSimulationID = simulationID.ToString();
            mongoConnection = simulations;
            _isUpdateOMS = false;
        }

        public class SimulationModel
        {
            public int simulationId { get; set; }
            public string simulationName { get; set; }
            public string networkName { get; set; }
            public int networkId { get; set; }
            public string status { get; set; }

            public DateTime? Created { get; set; }

            public DateTime? LastRun { get; set; }
        }

        public object APICall;

        /// <summary>
        /// Start and run a complete simulation. Creates necessary Simulation Tables.
        /// </summary>
        public void CompileSimulation(object isAPICall)
        {
            APICall = isAPICall;
            SimulationMessaging.IsDesktop = !isAPICall.Equals(true);

            SimulationMessaging.DateTimeStart = DateTime.Now;
            //Get Attribute types
            _dateTimeLast = DateTime.Now;
            SimulationMessaging.SimulationID = m_strSimulationID;
            FillOCIWeights();//This checks if this is an OMS analysis (and fills ConditionCategoryWeight if it is).
            SimulationMessaging.LoadAttributes(m_strSimulationID);
            SimulationMessaging.Method = this.Method;
            SimulationMessaging.AddMessage(new SimulationMessage("Begin compile simulation: " + DateTime.Now.ToString("HH:mm:ss")));

            UpdateDefinition<SimulationModel> updateStatus=null;
            if (isAPICall.Equals(true))
            {
                MongoClient client = new MongoClient(mongoConnection);
                MongoDatabase = client.GetDatabase("BridgeCare");
                Simulations = MongoDatabase.GetCollection<SimulationModel>("scenarios");

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Begin compile simulation");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            // Clear the compound treatments from the new structure.
            Simulation.CompoundTreatments.Clear();

            if (!DropPreviousSimulation(m_strSimulation, m_strNetworkID)) return;

            if (!GetSimulationMethod()) return;

            if (Method.IsConditionalRSL)
            {
                LoadConditionalRSL();
            }

            //Retrieve all attributes used in Simulation
            if (!GetSimulationAttributes()) return;
            m_listAttributes.Sort();
            _spanRead += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;

            //Create table for each attribute year pair into the future.
            SimulationMessaging.AddMessage(new SimulationMessage("Compile simulation complete: " + DateTime.Now.ToString("HH:mm:ss")));
            SimulationMessaging.AddMessage(new SimulationMessage("Beginning run simulation: " + DateTime.Now.ToString("HH:mm:ss")));
            Console.WriteLine("Compile simulation complete: " + DateTime.Now.ToString("HH:mm:ss"));

            if (isAPICall.Equals(true))
            {
                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Beginning run simulation");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);

                updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Success");
            }

            try
            {
                RunSimulation();
            }
            catch (Exception ex)
            {
                if (isAPICall.Equals(true))
                {
                    updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Simulation failed");
                }


                SimulationMessaging.AddMessage(new SimulationMessage("ERROR: [" + ex.Message + "]"));
                SimulationMessaging.AddMessage(new SimulationMessage("Aborting simulation."));
            }
            _spanAnalysis += DateTime.Now - _dateTimeLast;

            SimulationMessaging.AddMessage(new SimulationMessage("Time spent reading and compiling from database = " + _spanRead.ToString()));
            SimulationMessaging.AddMessage(new SimulationMessage("Time spent performing optimization = " + _spanAnalysis.ToString()));
            SimulationMessaging.AddMessage(new SimulationMessage("Time spent bulk loading = " + _spanReport.ToString()));
            TimeSpan spanTotal = _spanRead + _spanAnalysis + _spanReport;

            //Save the amount of time it took to run the simualtion to the database.
            SaveSimulationRunTime(spanTotal);

            if (isAPICall.Equals(true))
            {
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            return;
        }

        /// <summary>
        /// Save the amount of time it takes to run the analysis.
        /// </summary>
        /// <param name="spanTotal"></param>
        private void SaveSimulationRunTime(TimeSpan spanTotal)
        {
            SimulationMessaging.AddMessage(new SimulationMessage("Simulation complete total run time = " + spanTotal.ToString(), 100));
            string update = "UPDATE " + cgOMS.Prefix + "SIMULATIONS SET RUN_TIME='" + spanTotal.Seconds.ToString() + "',DATE_CREATED='" + DateTime.Now.ToString() + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
            {
                update = "UPDATE " + cgOMS.Prefix + "SIMULATIONS SET RUN_TIME='" + spanTotal.Seconds.ToString() + "',DATE_CREATED=to_date('" + DateTime.Now.ToShortDateString() + "','mm/dd/yyyy') WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }

            DBMgr.ExecuteNonQuery(update);
        }

        /// <summary>
        /// Updates a single section year combination with user selected treatment.
        /// </summary>
        /// <param name="strSectionID">SectionID for section to update</param>
        /// <param name="strYear">Year to begin update for.</param>
        public void SingleSectionSimulation()
        {
            String strSectionID = this.SingleSectionID;
            String strYear = this.SingleSectionYear;
            Committed singleSection = this.SingleSection;

            SimulationMessaging.Method = this.Method;
            SimulationMessaging.AddMessage(new SimulationMessage("Begin Single Section Run: " + DateTime.Now.ToString("HH:mm:ss")));

            if (this.Method == null)
            {
                if (!GetSimulationMethod()) return;
            }

            if (!GetSingleSectionSimulationAttributes(strSectionID, strYear)) return;

            m_listAttributes.Sort();

            SimulationMessaging.AddMessage(new SimulationMessage("Compile Single Section Run complete: " + DateTime.Now.ToString("HH:mm:ss")));
            SimulationMessaging.AddMessage(new SimulationMessage("Beginning single section update simulation: " + DateTime.Now.ToString("HH:mm:ss")));

            RunSingleSection(strSectionID, strYear, singleSection, singleSection.IsCommitted);
            SimulationMessaging.AddMessage(new SimulationMessage("End Single Section Run."));
        }

        /// <summary>
        /// Gets all the Simualtion ATTRIBUTES used in this single section
        /// simulation run.
        /// </summary>
        /// <returns></returns>
        public bool GetSingleSectionSimulationAttributes(String strSectionID, String strYear)
        {
            SimulationMessaging.LoadAttributes(m_strSimulationID);
            // Retrieve simulation Attributes for this Simulation
            m_listAttributes = GlobalDatabaseOperations.GetSimulationAttributes(m_strSimulationID);

            //Compile the area equation
            if (!CompileAreaEquation()) return false;

            //Load Jurisdiction if not already loaded
            if (String.IsNullOrEmpty(m_strJurisdiction))
            {
                //Load Jurisdiction if not already loaded
                if (!GetJurisdiction()) return false;
            }

            //InvestmentID does not have SIMULATION ATTRIBUTES
            //Deterioration
            //Adds to list of Simulation Attributes
            //Creates a list of Deterioration objects which contain all Deterioration data for this simulation
            //This data is stored in m_listDeteriorate which will be iterated every year (rolling forward and simulation).

            //Get calculated field data
            //Populates Calculated field data with fields from the get treatment data function.
            if (!GetCalculatedFieldData()) return false;

            if (m_listDeteriorate.Count == 0)
            {
                if (!GetDeteriorationData()) return false;
            }
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Performance Equations and Criteria complete: " + DateTime.Now.ToString("HH:mm:ss")));
            SimulationMessaging.GetUniqueDeteriorateAttributes(m_strSimulationID);

            //Get Investment Data
            if (this.Investment == null)
            {
                if (!GetInvestmentData()) return false;
            }
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Investments complete: " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Verifying Investments complete");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            //Get Target Data
            if (m_hashTargets.Count == 0)
            {
                if (!GetTargetData()) return false;
            }
            SimulationMessaging.Targets = m_hashTargets;
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Targets and deficiency complete: " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Verifying Targets and deficiency complete");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            if (SimulationMessaging.RemainingLifes == null)
            {
                if (!GetRemainingLifes()) return false;
            }

            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Remaining Life Limits complete: " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Verifying Remaining Life Limits complete");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            //Treatments
            //Adds to the list of Simulation Attibutes
            //Creates list of Treatments
            if (m_listTreatments.Count == 0)
            {
                if (!GetTreatmentData()) return false;
            }
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Treatments complete: " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Verifying Treatments complete");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            //Priority
            //Adds to the list of Simulation Attributes
            //Creates a list of Priority objects (which will be run though). Iterated every year when Budgets are spent.
            if (m_listPriorities.Count == 0)
            {
                if (!GetPriorityData()) return false;
            }
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Priorities complete: " + DateTime.Now.ToString("HH:mm:ss")));

            //Remove section from previous run.
            if (!RemoveSectionYearFromResults(strSectionID, strYear)) return false;

            return true;
        }

        private bool RemoveSectionYearFromResults(string strSectionID, string strYear)
        {
            String strDelete = "DELETE FROM TARGET_" + m_strNetworkID + "_" + m_strSimulationID + " WHERE YEARS >='" + strYear + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Dropping section from TARGET table. " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Dropping section from TARGET table: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            strDelete = "DELETE FROM BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID + " WHERE YEARS >='" + strYear + "' AND SECTIONID='" + strSectionID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Dropping section from BENEFITCOST table. " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Dropping section from BENEFITCOST table: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            strDelete = "DELETE FROM REPORT_" + m_strNetworkID + "_" + m_strSimulationID + " WHERE YEARS >='" + strYear + "' AND SECTIONID='" + strSectionID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Dropping section from REPORT table. " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Dropping section from REPORT table: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            //SIMULATION_networkid_simulationid is overwritten so no need to delete from simulation

            return true;
        }

        /// <summary>
        /// Loads Jurisdiction data and adds attributes to attribute list
        /// </summary>
        /// <returns>True if successful.</returns>
        private bool GetJurisdiction()
        {
            //Get the Jurisdiction from the simulation table.
            DataSet ds = null;
            String strQuery = "SELECT JURISDICTION,WEIGHTING FROM " + cgOMS.Prefix + "SIMULATIONS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Error in retrieving JURISDICTION from SIMULATIONS table. SQL message - " + exception.Message));
                return false;
            }

            m_strJurisdiction = ds.Tables[0].Rows[0].ItemArray[0].ToString();

            m_strJurisdiction = ConvertOMSAttribute(m_strJurisdiction);

            m_strWeighting = ds.Tables[0].Rows[0].ItemArray[1].ToString();
            if (m_strWeighting != "none" && m_strWeighting != "")
            {
                if (!m_listAttributes.Contains(m_strWeighting))
                {
                    m_listAttributes.Add(m_strWeighting);
                }
            }

            //Allow blank JURISDICTION
            if (m_strJurisdiction.Trim().Length > 0)
            {
                m_strJurisdiction = m_strJurisdiction.Replace("|", "'");
                if (m_strJurisdiction.IndexOf("[SECTIONID]") > -1)
                {
                    m_strJurisdiction = m_strJurisdiction.Replace("[SECTIONID]", "SECTION_" + m_strNetworkID + ".SECTIONID");
                }

                //Check JURIDICITON for Attributes.
                ParseAttribute(m_strJurisdiction);
            }
            else
            {
                m_strJurisdiction = "";
            }
            return true;
        }

        public static string ConvertOMSAttribute(string omsCriteria)
        {
            if (SimulationMessaging.IsOMS)//Ignored if not OMS.
            {
                List<string> omsAttributes = SimulationMessaging.ParseAttributeNoCheck(omsCriteria);
                foreach (string omsAttribute in omsAttributes)
                {
                    if (SimulationMessaging.AttributeOMS.ContainsKey(omsAttribute))
                    {
                        string attribute = SimulationMessaging.AttributeOMS[omsAttribute].ToString();
                        omsCriteria = omsCriteria.Replace("[" + omsAttribute + "]", "[" + attribute + "]");
                    }
                }
            }
            return omsCriteria;
        }

        /// <summary>
        /// Compiles the area equation for Simulation
        /// </summary>
        /// <returns></returns>
        private bool CompileAreaEquation()
        {
            String strQuery = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME='AREA_CALCULATION'";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Error in compiling AREA function. SQL message - " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error in compiling AREA function: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }
            SimulationMessaging.ListArea = new List<String>();
            String strArea = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            ParseAttribute(strArea.Replace("[LENGTH]", "").Replace("[AREA]", "")); // Remove made up variable [LENGTH] only used for area.
            SimulationMessaging.Area = new CalculateEvaluate.CalculateEvaluate();
            SimulationMessaging.Area.BuildTemporaryClass(strArea, true);
            SimulationMessaging.Area.CompileAssembly();
            if (SimulationMessaging.Area.m_listError.Count > 0)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Error in compiling AREA function. SQL message - " + SimulationMessaging.Area.m_listError[0].ToString()));
                return false;
            }
            string[] listAreaParameters = strArea.Split(']');
            for (int i = 0; i < listAreaParameters.Length; i++)
            {
                if (listAreaParameters[i].Contains("["))
                {
                    SimulationMessaging.ListArea.Add(listAreaParameters[i].Substring(listAreaParameters[i].IndexOf('[') + 1));
                }
            }
            return true;
        }

        /// <summary>
        /// Gets Method and Checks some common errors.
        /// </summary>
        /// <returns></returns>
        public bool GetSimulationMethod()
        {
            //Replace with Method information.
            String strSelect = "SELECT ANALYSIS, BUDGET_CONSTRAINT,BENEFIT_VARIABLE, BENEFIT_LIMIT,RUN_TIME,USE_CUMULATIVE_COST,USE_ACROSS_BUDGET FROM " + cgOMS.Prefix + "SIMULATIONS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);

                if (ds != null)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    this.Method = new AnalysisMethod();
                    Method.TypeAnalysis = dr["ANALYSIS"].ToString();
                    Method.TypeBudget = dr["BUDGET_CONSTRAINT"].ToString();
                    if (Method.TypeAnalysis.Contains("Benefit"))
                    {
                        Method.IsBenefitCost = true;
                        Method.IsRemainingLife = false;
                    }
                    else
                    {
                        Method.IsBenefitCost = false;
                        Method.IsRemainingLife = true;
                    }
                    Method.BenefitAttribute = dr["BENEFIT_VARIABLE"].ToString();
                    if (dr["RUN_TIME"] != DBNull.Value) SimulationMessaging.LengthLastRun = Convert.ToDouble(dr["RUN_TIME"]);
                    if (dr["USE_CUMULATIVE_COST"] != DBNull.Value) Method.UseCumulativeCost = Convert.ToBoolean(dr["USE_CUMULATIVE_COST"]);
                    Method.UseAcrossBudgets = false;
                    if (dr["USE_ACROSS_BUDGET"] != DBNull.Value) Method.UseAcrossBudgets = Convert.ToBoolean(dr["USE_ACROSS_BUDGET"]);
                    string str = dr["BENEFIT_LIMIT"].ToString();
                    double dLimit = 0;
                    double.TryParse(str, out dLimit);
                    Method.BenefitLimit = dLimit;
                    SimulationMessaging.Method = Method;
                }

                if (SimulationMessaging.IsOMS)
                {
                    switch (Method.TypeBudget)
                    {
                        case "OCI Target":
                            Method.TypeBudget = "Until Targets Met";
                            Method.IsOMSUnlimited = true;
                            Method.IsOMSTargetEnforced = true;
                            break;

                        case "Until OCI or Budget Met":
                            Method.TypeBudget = "Until Targets Met";
                            Method.IsOMSUnlimited = false;
                            Method.IsOMSTargetEnforced = true;
                            break;

                        case "Budget Target":
                            Method.TypeBudget = "Until Targets Met";
                            Method.IsOMSUnlimited = false;
                            Method.IsOMSTargetEnforced = false;
                            break;

                        case "Unlimited":
                            Method.TypeBudget = "As Budget Permits";
                            Method.IsOMSUnlimited = true;
                            Method.IsOMSTargetEnforced = false;
                            break;
                    }
                }

                if (Method.TypeAnalysis == "Conditional RSL/Cost")
                {
                    Method.IsConditionalRSL = true;
                }
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error in initializing analysis: " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error in initializing analysis: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            if (String.IsNullOrEmpty(Method.BenefitAttribute))
            {
                switch (Method.TypeAnalysis)
                {
                    case "Maximum Benefit":
                    case "Multi-year Maximum Benefit":
                    case "Incremental Benefit/Cost":
                    case "Multi-year Incremental Benefit/Cost":
                        SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error:  Before running a benefit cost analysis, a Benefit variable must be selected."));

                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Benefit variable must be selected");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        return false;
                    //break;
                    default:
                        break;
                }
            }

            return true;
        }

        public void RunSingleSection(String strSectionID, String strYear, Committed singleSection, bool bCommited)
        {
            this.SingleSection = singleSection;
            SimulationMessaging.ListAttributes = m_listAttributes;
            //SELECT column_name FROM information_schema.columns WHERE table_name =
            // Get a list of all the columns in the segmented tables
            // The juridiction determines which sections are included in Simulation.
            // Users are expected to enter query with year modifier (i.e DISTRICT_2006 = 'M-11')

            SimulationMessaging.AddMessage(new SimulationMessage("Beginning to ROLL FORWARD missing simulation attribute data and LOAD existing attribute data at " + DateTime.Now.ToString("HH:mm:ss")));
            if (!FillSectionListForSingleSection(strSectionID)) return;
            SimulationMessaging.AddMessage(new SimulationMessage("Complete ROLL FORWARD of missing simulation attribute data and LOAD of existing attribute data at " + DateTime.Now.ToString("HH:mm:ss")));

            if (!FillCommittedProjects(strSectionID)) return;

            //After FillSectionList everything is read to go to start simulating!
            //Everything is rolled up to the year of the start date.
            for (int nYear = int.Parse(strYear); nYear < Investment.StartYear + Investment.AnalysisPeriod; nYear++)
            {
                //Apply Deteriorate/Performance curves.
                SimulationMessaging.AddMessage(new SimulationMessage("Applying Performance/Deterioration equations for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Applying Performance/Deterioration equations for " + nYear.ToString());
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                ApplyDeterioration(nYear, strSectionID);

                //Determine Benefit/Cost
                SimulationMessaging.AddMessage(new SimulationMessage("Determining Treament Feasibilty and calculating benefit/remaining life versus cost ratios for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                DetermineBenefitCost(nYear, strSectionID);

                //Load Committed Projects.  These get comitted (and spent) regardless of budget.
                //Apply committed projects
                if (nYear != int.Parse(strYear))
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Applying committed projects for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Applying committed projects for " + nYear.ToString());
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    ApplyCommitted(nYear, strSectionID);
                }

                //Calculate network averages and deficient base (after committed).
                DetermineTargetAndDeficient(nYear, strSectionID);

                SimulationMessaging.AddMessage(new SimulationMessage("Spending Budget and evaluating Targets for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Spending Budget and evaluating Targets for " + nYear.ToString());
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }

                if (nYear.ToString() == strYear)
                {
                    SpendSingleSection(nYear, strSectionID, singleSection, bCommited);
                }
                else
                {
                    SpendSingleSection(nYear, strSectionID, null, false);
                }
                SimulationMessaging.AddMessage(new SimulationMessage("Creating Deficiency and Network Average Report for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                ReportTargetDeficient(nYear);
            }
        }

        public void RunSimulation()
        {
            SimulationMessaging.ListAttributes = m_listAttributes;
            //SELECT column_name FROM information_schema.columns WHERE table_name =
            // Get a list of all the columns in the segmented tables
            // The juridiction determines which sections are included in Simulation.
            // Users are expected to enter query with year modifier (i.e DISTRICT_2006 = 'M-11')

            SimulationMessaging.AddMessage(new SimulationMessage("Beginning to ROLL FORWARD missing simulation attribute data and LOAD existing attribute data at " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Starting to roll forward missing and existing simulation attribute data");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }
            if (!FillSectionList()) return;
            SimulationMessaging.AddMessage(new SimulationMessage("Complete ROLL FORWARD of missing simulation attribute data and LOAD of existing attribute data at " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Complete roll forward missing and existing simulation attribute data");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            if (!FillCommittedProjects()) return;

            //Calculate areas for all sections.
            int areaYear = Investment.StartYear - 1;
            if (SimulationMessaging.IsOMS) areaYear = Investment.StartDate.Year;

            foreach (Sections section in m_listSections)
            {
                section.CalculateArea(areaYear);
            }

            if (!UpdateSimulationAttributes()) return;


            //Spend all committed projects.
            //Remove money for committed projects from the all budgets, all years so that it is accounted
            //for scheduled and split treatments.
            //The report will show the money spent in the appropriate year.  We just need to move the spending in memory before the analysis.
            foreach(var section in m_listSections)
            {
                foreach(var commit in section.YearCommit)
                {
                    if (commit.Year >= Investment.StartYear)
                    {
                        Investment.SpendBudget(commit.Cost, commit.Budget, commit.Year.ToString());
                    }
                }
            }


            //After FillSectionList everything is read to go to start simulating!
            //Everything is rolled up to the year of the start date.
            for (int nYear = Investment.StartYear; nYear < Investment.StartYear + Investment.AnalysisPeriod; nYear++)
            {
                Console.WriteLine("Begin analysis year =" + nYear);
                //Apply Deteriorate/Performance curves.
                SimulationMessaging.AddMessage(new SimulationMessage("Applying Performance/Deterioration equations for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Applying Performance/Deterioration equations for" + nYear.ToString());
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                Console.WriteLine("Apply deterioration");
                ApplyDeterioration(nYear);
                //Determine Benefit/Cost
                SimulationMessaging.AddMessage(new SimulationMessage("Determining Treament Feasibilty and calculating benefit/remaining life versus cost ratios for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Calculating benefit/remaining life versus cost ratios for " + nYear.ToString());
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }

                m_listApplyTreatment.Clear();
                Console.WriteLine("Determine Benefit/Cost");
                DetermineBenefitCostIterative(nYear);
                
                //Load Committed Projects.  These get comitted (and spent) regardless of budget.
                //Apply committed projects
                SimulationMessaging.AddMessage(new SimulationMessage("Applying committed projects for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Applying committed projects for " + nYear.ToString());
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                Console.WriteLine("Apply committed");
                ApplyCommitted(nYear);

                //Calculate network averages and deficient base (after committed).
                DetermineTargetAndDeficient(nYear);

                SimulationMessaging.AddMessage(new SimulationMessage("Spending Budget and evaluating Targets for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Spending Budget and evaluating Targets for " + nYear.ToString());
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                if (Method.TypeAnalysis.Contains("Multi"))
                {
                    //MULTIBUDGET FIX
                    SpendBudgetPermits(nYear, "None");
                }
                else
                {
                    Console.WriteLine("Spend budget "  + Method.TypeBudget);
                    switch (Method.TypeBudget)
                    {
                        case "No Spending":
                            SpendBudgetPermits(nYear, "None");
                            break;

                        case "As Budget Permits":
                            SpendBudgetPermits(nYear, "");
                            break;

                        case "Unlimited":
                            SpendBudgetPermits(nYear, "Unlimited");
                            break;

                        case "Until Targets Met":
                        case "Until Deficient Met":
                        case "Targets/Deficient Met":
                            SpendUntilTargetsDeficientMet(nYear);
                            break;
                    }
                }
                SimulationMessaging.AddMessage(new SimulationMessage("Creating Deficiency and Network Average Report for " + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Creating Deficiency and Network Average Report for " + nYear.ToString());
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                ReportTargetDeficient(nYear);

                //ResetSectionForNextYear();
            }

            SimulationMessaging.AddMessage(new SimulationMessage("Output per section per attribute report for all years at " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Output per section per attribute report for all years");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }
            if (!CreateSimulationTable(m_strNetworkID, m_strSimulationID)) return;

            List<TextWriter> listSimulationWriters = new List<TextWriter>();
            for (var i = 0; i < m_dictionarySimulationTables.Count; i++)
            {
                listSimulationWriters.Add(SimulationMessaging.CreateTextWriter(SimulationMessaging.SimulationTable + "_" + i + ".txt", out _));
            }

            foreach (Sections section in m_listSections)
            {
                section.WriteSimulation(Investment.StartYear, Investment.StartYear + Investment.AnalysisPeriod - 1, m_dictionaryAttributeSimulationTable, listSimulationWriters);
            }

            foreach (var tw in listSimulationWriters)
            {
                tw.Close();
            }
            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            for (var j = 0; j < m_dictionarySimulationTables.Count; j++)
            {
                string sOutFile = strMyDocumentsFolder + "\\" + SimulationMessaging.SimulationTable + "_" + j + ".txt";
                _spanAnalysis += DateTime.Now - _dateTimeLast;
                _dateTimeLast = DateTime.Now;

                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        DBMgr.SQLBulkLoad(SimulationMessaging.SimulationTable + "_" + j, sOutFile, "\\t");
                        break;

                    case "ORACLE":
                        //throw new NotImplementedException( "TODO: Figure out columns for RunSimulation()" );
                        DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, SimulationMessaging.SimulationTable, sOutFile, DBMgr.GetTableColumns(SimulationMessaging.SimulationTable), "\\t");
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                        //break;
                }
            }
            _spanReport += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;

            //Switch for multi-year
            switch (Method.TypeAnalysis)
            {
                case "Multi-year Incremental Benefit/Cost":
                case "Multi-year Remaining Life/Cost":
                    SolveMultipleYear();
                    break;
            }

            SimulationMessaging.AddMessage(new SimulationMessage("Simulation complete at " + DateTime.Now.ToString("HH:mm:ss"), 100));
            Thread.Sleep(1000);
        }


        private void UpdateSimulationView()
        {
            var dropView = "DROP VIEW IF EXISTS SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID;

            try
            {
                DBMgr.ExecuteNonQuery(dropView);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Dropping simulation view. " + exception.Message));
                return;
            }

            var createView = "CREATE VIEW SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID + " AS SELECT * FROM(SELECT[SECTIONID], [ATTRIBUTE_VALUE], CONCAT([ATTRIBUTE_], '_',[YEAR_]) AS ATTRIBUTE_YEAR FROM SIMULATION_RESULT WHERE SIMULATION_ID = '" + m_strSimulationID + "')";
            createView += " AS REDUCED_TABLE PIVOT(max(ATTRIBUTE_VALUE) FOR ATTRIBUTE_YEAR IN (";

            int index = 0;
            foreach (var attribute in m_listAttributes)
            {
                if (index > 0) createView += ",";
                createView += "[" + attribute + "_0]";

                for (int nYear = Investment.StartYear; nYear < Investment.StartYear + Investment.AnalysisPeriod; nYear++)
                {
                    createView += ",[" + attribute + "_" + nYear + "]";
                }

                index++;
            }

            createView += ")) AS data_";

            try
            {
                DBMgr.ExecuteNonQuery(createView);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Creating simulation view. " + exception.Message));
                return;
            }
        }

        private void LoadConditionalRSL()
        {
            SimulationMessaging.AttributeConditionalRSL = new List<ConditionalRSL>();
            String select = "SELECT * FROM CONDITIONAL_RSL";
            DataSet ds = DBMgr.ExecuteQuery(select);
            //This table exists
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String criteria = "";
                String attribute = dr["ATTRIBUTE"].ToString();
                Int32 bin = Convert.ToInt32(dr["BIN"]);
                Double value = Convert.ToDouble(dr["VALUE"]);
                String id = Convert.ToString(dr["ID"]);
                if (dr["CRITERIA"] != DBNull.Value) criteria = Convert.ToString(dr["CRITERIA"]);

                ConditionalRSL conditionalRSL = SimulationMessaging.AttributeConditionalRSL.Find(delegate (ConditionalRSL c) { return c.Criteria.Criteria == criteria && c.Attribute == attribute; });
                if (conditionalRSL == null)
                {
                    Criterias criterias = new Criterias("CONDITIONAL_RSL", "BINARY_CRITERIA", id);
                    criterias.Criteria = criteria;
                    conditionalRSL = new ConditionalRSL(attribute, criterias);
                    SimulationMessaging.AttributeConditionalRSL.Add(conditionalRSL);
                }
                conditionalRSL.BinValues.Add(bin, value);
            }
        }

        /// <summary>
        /// Solves multi-year optimization
        /// </summary>
        private void SolveMultipleYear()
        {
            //Multiyear must be solved for differing budget types
            switch (Method.TypeBudget)
            {
                case "As Budget Permits":
                    SpendMultipleYearBudgetPermits();
                    break;
                    //case "Unlimited":
                    //    SpendBudgetPermits(nYear, "Unlimited");
                    //    break;
                    //case "Until Targets Met":
                    //case "Until Deficient Met":
                    //case "Targets/Deficient Met":
                    //    SpendUntilTargetsDeficientMet(nYear);
                    //    break;
            }
        }

        private bool UpdateSimulationAttributes()
        {
            String strAttributeList = "";
            foreach (String strSimulationAttribute in m_listAttributes)
            {
                if (strAttributeList.Length > 0) strAttributeList += "\t";
                strAttributeList += strSimulationAttribute;
            }

            String strUpdate = "UPDATE " + cgOMS.Prefix + "SIMULATIONS SET SIMULATION_VARIABLES='" + strAttributeList + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Updating simulation variables. " + exception.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Drop the Simulation table for this NetworkID and SimulationID
        /// </summary>
        /// <param name="strNetworkID"></param>
        /// <param name="strSimulationID"></param>
        /// <returns></returns>
        public bool DropSimulation(String strNetworkID, String strSimulationID)
        {
            //Drop Existing Simulation table when re-running SIMULATION or just Deleting Simulatoin.
            String strTable = cgOMS.Prefix + "SIMULATION_" + strNetworkID + "_" + strSimulationID;
            SimulationMessaging.SimulationTable = strTable;

            for (var i = 0; i < 10; i++)
            {
                //var drop = "DROP TABLE IF EXISTS " + strTable + "_" + i;
                var drop = $"IF OBJECT_ID ( '{strTable}_{i}' , 'U' )  IS NOT NULL DROP TABLE {strTable}_{i}";
                try
                {
                    DBMgr.ExecuteNonQuery(drop);
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Simulation table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed.  Simulation cannot proceed until this table DROPPED. SQL message -" + exception.Message));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Simulation table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed. " + exception.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }
            }

            //Drop Existing BENEFITCOST table when re-running SIMULATION or just Deleting Simulation.
            strTable = cgOMS.Prefix + "BENEFITCOST_" + strNetworkID + "_" + strSimulationID;
            SimulationMessaging.BenefitCostTable = strTable;
            try
            {
                var dropTable = $"IF OBJECT_ID ( '{strTable}' , 'U' )  IS NOT NULL DROP TABLE {strTable}";
                DBMgr.ExecuteNonQuery(dropTable);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Benefit Cost table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed.  Simulation cannot proceed until this table DROPPED. SQL message -" + exception.Message));
                return false;
            }

            //Drop Existing REPORT table when re-running SIMULATION or just Deleting Simulation.
            strTable = cgOMS.Prefix + "REPORT_" + strNetworkID + "_" + strSimulationID;
            SimulationMessaging.ReportTable = strTable;

            try
            {
                var dropTable = $"IF OBJECT_ID ( '{strTable}' , 'U' )  IS NOT NULL DROP TABLE {strTable}";
                DBMgr.ExecuteNonQuery(dropTable);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Benefit Cost table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed.  Simulation cannot proceed until this table DROPPED. SQL message -" + exception.Message));
                return false;
            }

            //Drop Existing TARGET table when re-running SIMULATION or just Deleting Simulation.
            strTable = cgOMS.Prefix + "TARGET_" + strNetworkID + "_" + strSimulationID;
            SimulationMessaging.TargetTable = strTable;

            try
            {
                var dropTable = $"IF OBJECT_ID ( '{strTable}' , 'U' )  IS NOT NULL DROP TABLE {strTable}";
                DBMgr.ExecuteNonQuery(dropTable);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Target table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed.  Simulation cannot proceed until this table DROPPED. SQL message -" + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Target table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed. " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            //Drop Existing TARGET table when re-running SIMULATION or just Deleting Simulation.
            strTable = cgOMS.Prefix + "CUMULATIVECOST_" + strNetworkID + "_" + strSimulationID;
            SimulationMessaging.CumulativeCostTable = strTable;

            //String strDrop = "DROP TABLE IF EXISTS" + strTable;
            var strDrop = $"IF OBJECT_ID ( '{strTable}' , 'U' )  IS NOT NULL DROP TABLE {strTable}";
            try
            {
                DBMgr.ExecuteNonQuery(strDrop);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Dropping CumulativeCost table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed.  Simulation cannot proceed until this table DROPPED. SQL message -" + exception.Message));
                return false;
            }

            //Drop Existing TARGET table when re-running SIMULATION or just Deleting Simulation.
            strTable = cgOMS.Prefix + "REASONS_" + strNetworkID + "_" + strSimulationID;
            SimulationMessaging.ReasonsTable = strTable;

            strDrop = $"IF OBJECT_ID ( '{strTable}' , 'U' )  IS NOT NULL DROP TABLE {strTable}";
            try
            {
                DBMgr.ExecuteNonQuery(strDrop);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Dropping Reasons table for NetworkID:" + m_strNetworkID + " SimulationID:" + m_strSimulationID + " failed.  Simulation cannot proceed until this table DROPPED. SQL message -" + exception.Message));
                return false;
            }

            return true;
        }

        public bool CreateSimulationTable(String strNetworkID, String strSimulationID)
        {
            String strTable = SimulationMessaging.SimulationTable;

            var numberColumns = m_listAttributes.Count * (Investment.AnalysisPeriod + 1);

            var numberTables = Math.Ceiling(Convert.ToDouble(numberColumns) / 900);
            m_dictionarySimulationTables = new Dictionary<string, List<TableParameters>>();
            m_dictionaryAttributeSimulationTable = new Dictionary<string, int>();

            for (var i = 0; i < numberTables; i++)
            {
                m_dictionarySimulationTables.Add(strTable + "_" + i, new List<TableParameters>());
            }

            foreach (var key in m_dictionarySimulationTables.Keys)
            {
                m_dictionarySimulationTables[key].Add(new TableParameters("SECTIONID", DataType.Int, false, true, false));
            }

            int currentTable = 0;
            String strType;
            int nYear;

            foreach (String str in m_listAttributes)
            {
                var currentKey = strTable + "_" + currentTable;

                if (m_dictionarySimulationTables[currentKey].Count + Investment.AnalysisPeriod + 1 > 899)
                {
                    currentTable++;
                    currentKey = strTable + "_" + currentTable;
                }
                m_dictionaryAttributeSimulationTable.Add(str, currentTable);

                for (int n = 0; n < Investment.AnalysisPeriod; n++)
                {
                    nYear = Investment.StartYear + n;
                    if (str == "SECTIONID") continue;
                    strType = SimulationMessaging.GetAttributeType(str);
                    if (strType == "NUMBER")
                    {
                        m_dictionarySimulationTables[currentKey].Add(new TableParameters(str + "_" + nYear.ToString(), DataType.Float, true));
                    }
                    else
                    {
                        m_dictionarySimulationTables[currentKey].Add(new TableParameters(str + "_" + nYear.ToString(), DataType.VarChar(4000), true));
                    }
                }
                strType = SimulationMessaging.GetAttributeType(str);

                if (strType == "NUMBER")
                {
                    m_dictionarySimulationTables[currentKey].Add(new TableParameters(str + "_0", DataType.Float, true));
                }
                else
                {
                    m_dictionarySimulationTables[currentKey].Add(new TableParameters(str + "_0", DataType.VarChar(4000), true));
                }
            }

            foreach (var key in m_dictionarySimulationTables.Keys)
            {
                try
                {
                    DBMgr.CreateTable(key, m_dictionarySimulationTables[key]);
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Creating simulation table " + strTable + " with SQL Message - " + exception.Message));
                    return false;
                }
            }

            return true;
        }

        public bool CreateAdditionalSimulationTable(String strNetworkID, String strSimulationID)
        {
            String strTable = SimulationMessaging.BenefitCostTable;
            List<TableParameters> listColumn = new List<TableParameters>();
            listColumn.Add(new TableParameters("ID_", DataType.Int, false, true, true));
            listColumn.Add(new TableParameters("SECTIONID", DataType.Int, false, false));
            listColumn.Add(new TableParameters("YEARS", DataType.Int, false));
            listColumn.Add(new TableParameters("TREATMENT", DataType.VarChar(50), false));
            listColumn.Add(new TableParameters("YEARSANY", DataType.Int, true));
            listColumn.Add(new TableParameters("YEARSSAME", DataType.Int, true));
            listColumn.Add(new TableParameters("BUDGET", DataType.VarChar(512), true));
            listColumn.Add(new TableParameters("COST_", DataType.Float, true));
            listColumn.Add(new TableParameters("REMAINING_LIFE", DataType.Float, true));
            listColumn.Add(new TableParameters("BENEFIT", DataType.Float, true));
            listColumn.Add(new TableParameters("BC_RATIO", DataType.Float, true));
            listColumn.Add(new TableParameters("CONSEQUENCEID", DataType.Int, true));
            listColumn.Add(new TableParameters("DEFICIENT", DataType.Int, true));
            listColumn.Add(new TableParameters("RLHASH", DataType.VarChar(4000), true));
            listColumn.Add(new TableParameters("CHANGEHASH", DataType.VarChar(4000), true));
            listColumn.Add(new TableParameters("OMS_IGNORE", DataType.Bit, true));
            //listColumn.Add(new TableParameters("CUMULATIVE_COST_ID", DataType.Int, true));
            try
            {
                DBMgr.CreateTable(strTable, listColumn);
            }
            catch (Exception exception)
            {
                //SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Creating simulation benefit cost table " + strTable + " with SQL Message - " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Creating simulation benefit cost table");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            strTable = SimulationMessaging.CumulativeCostTable;
            listColumn = new List<TableParameters>();
            listColumn.Add(new TableParameters("CUMULATIVE_COST_ID", DataType.Int, false, false));
            listColumn.Add(new TableParameters("COST_ID", DataType.Int, false));
            listColumn.Add(new TableParameters("COST", DataType.Float, false));

            try
            {
                // DBMgr.CreateTable(strTable, listColumn);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Creating cumulative cost table " + strTable + " with SQL Message - " + exception.Message));
                return false;
            }

            strTable = SimulationMessaging.ReportTable;
            listColumn = new List<TableParameters>();
            listColumn.Add(new TableParameters("ID_", DataType.Int, false, true, true));
            listColumn.Add(new TableParameters("SECTIONID", DataType.Int, false));
            listColumn.Add(new TableParameters("YEARS", DataType.Int, false));
            listColumn.Add(new TableParameters("TREATMENT", DataType.VarChar(50), false));
            listColumn.Add(new TableParameters("YEARSANY", DataType.Int, true));
            listColumn.Add(new TableParameters("YEARSSAME", DataType.Int, true));
            listColumn.Add(new TableParameters("BUDGET", DataType.VarChar(512), true));
            listColumn.Add(new TableParameters("COST_", DataType.Float, true));
            listColumn.Add(new TableParameters("REMAINING_LIFE", DataType.Float, true));
            listColumn.Add(new TableParameters("BENEFIT", DataType.Float, true));
            listColumn.Add(new TableParameters("BC_RATIO", DataType.Float, true));
            listColumn.Add(new TableParameters("CONSEQUENCEID", DataType.Int, true));
            listColumn.Add(new TableParameters("PRIORITY", DataType.Int, true));
            listColumn.Add(new TableParameters("RLHASH", DataType.VarChar(4000), true));
            listColumn.Add(new TableParameters("COMMITORDER", DataType.Int, true));
            listColumn.Add(new TableParameters("ISCOMMITTED", DataType.Bit, true));
            listColumn.Add(new TableParameters("NUMBERTREATMENT", DataType.Int, true));
            listColumn.Add(new TableParameters("CHANGEHASH", DataType.VarChar(4000), true));
            listColumn.Add(new TableParameters("AREA", DataType.Float, true));
            listColumn.Add(new TableParameters("RESULT_TYPE", DataType.Int, true));//This stores whether repeat, committed, feasible, selected or not allowed.

            try
            {
                DBMgr.CreateTable(strTable, listColumn);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Creating simulation report table " + strTable + " with SQL Message - " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Creating simulation benefit report table: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            strTable = SimulationMessaging.TargetTable;
            listColumn = new List<TableParameters>();
            listColumn.Add(new TableParameters("ID_", DataType.Int, false, true, true));
            listColumn.Add(new TableParameters("TARGETID", DataType.Int, true));
            listColumn.Add(new TableParameters("YEARS", DataType.Int, true));
            listColumn.Add(new TableParameters("TARGETMET", DataType.Float, true));
            listColumn.Add(new TableParameters("AREA", DataType.Float, true));
            listColumn.Add(new TableParameters("ISDEFICIENT", DataType.Bit, true));

            try
            {
                DBMgr.CreateTable(strTable, listColumn);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Creating simulation Target table " + strTable + " with SQL Message - " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Fatal Error: Creating simulation Target table " + strTable + " - " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }


            strTable = SimulationMessaging.ReasonsTable;
            listColumn = new List<TableParameters>();
            listColumn.Add(new TableParameters("SECTIONID", DataType.Int, false, true, false));
            listColumn.Add(new TableParameters("YEARS", DataType.Int, true));
            listColumn.Add(new TableParameters("TREATMENT", DataType.VarChar(50), true));
            listColumn.Add(new TableParameters("REASON", DataType.VarChar(50), true));
            listColumn.Add(new TableParameters("BUDGET", DataType.VarChar(50), true));
            listColumn.Add(new TableParameters("BENEFIT_ORDER", DataType.Int, true));
            listColumn.Add(new TableParameters("PRIORITY", DataType.Int, true));
            listColumn.Add(new TableParameters("WEIGHTED_BENEFIT_COST", DataType.Float, true));
            listColumn.Add(new TableParameters("NUMBER_TARGET", DataType.Int, true));
            listColumn.Add(new TableParameters("BUDGET_HASH", DataType.VarCharMax, true));
            try
            {
                DBMgr.CreateTable(strTable, listColumn);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Creating simulation Reasons table " + strTable + " with SQL Message - " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Fatal Error: Creating simulation Reasons table " + strTable + " - " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }







            SimulationMessaging.AddMessage(new SimulationMessage("Sucessfully created Simulation table " + strTable));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Sucessfully created Simulation table " + strTable);
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }
            return true;
        }

        /// <summary>
        /// Retrieves the Data for this simulution from SEGMENT_newtork
        /// TODO: Extend to all possible SEGMENT_ tables
        /// </summary>
        /// <returns></returns>
        private bool FillSectionList()
        {
            if (!SimulationMessaging.IsOMS) return FillRCSectionList();
            else return FillOMSSectionList();
        }

        private bool FillOMSSectionList()
        {
            Criterias criteria = new Criterias(cgOMS.Prefix + "SIMULATIONS", "JURISDICTION", m_strSimulationID);
            byte[] assemblyCriteria = null;
            string currentCriteria = m_strJurisdiction;

            assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "SIMULATIONS", "JURISDICTION", m_strSimulationID, assemblyCriteria);
            if (assemblyCriteria != null && assemblyCriteria.Length > 0)
            {
                criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                if (criteria.Evaluate.OriginalInput != currentCriteria)
                {
                    criteria.Evaluate = null;
                }
            }

            if (criteria.Evaluate != null && criteria.Evaluate.m_cr != null)
            {
                if (!File.Exists(criteria.Evaluate.m_cr.PathToAssembly))
                {
                    criteria.Evaluate = null;
                }
            }

            criteria.Criteria = m_strJurisdiction;

            using (SqlConnection connection = new SqlConnection(DBMgr.NativeConnectionParameters.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT OID,ATTRIBUTE_,ASSET_DATE,ASSET_VALUE FROM " + cgOMS.Prefix + "OMS_ASSETS WHERE SIMULATIONID='0' OR SIMULATIONID='" + m_strSimulationID + "' ORDER BY OMS_ASSET_ID";//Can order by OMS_ASSET_ID because was Identity inserted by OID (faster return)
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();

                    Sections section = null;
                    while (dr.Read())
                    {
                        string oid = dr["OID"].ToString();
                        if (section == null || section.SectionID != oid)
                        {
                            if (section != null)
                            {
                                section.OMSRollForward(m_listDeteriorate, m_Investment);
                                Hashtable hashInput = (Hashtable)section.m_hashYearAttributeValues[0];
                                if (criteria.IsCriteriaMet(hashInput))
                                {
                                    m_listSections.Add(section);
                                }
                            }
                            section = new Sections();
                            section.SectionID = oid;
                            section.AttributeValueYear = new Dictionary<string, List<DataOMS>>();//The list of DataOMS is in reverse order.
                        }

                        string attribute = dr["ATTRIBUTE_"].ToString();
                        DateTime assetDate = DateTime.Now;
                        object assetValue = null;

                        if (dr["ASSET_DATE"] != DBNull.Value) assetDate = Convert.ToDateTime(dr["ASSET_DATE"]);
                        if (dr["ASSET_VALUE"] != DBNull.Value) assetValue = dr["ASSET_VALUE"];

                        List<DataOMS> omsDatas = null;
                        if (!section.AttributeValueYear.ContainsKey(attribute))
                        {
                            omsDatas = new List<DataOMS>();
                            section.AttributeValueYear.Add(attribute, omsDatas);
                        }
                        else
                        {
                            omsDatas = section.AttributeValueYear[attribute];
                        }
                        omsDatas.Add(new DataOMS(assetValue, assetDate));
                    }
                    if (section != null)
                    {
                        section.OMSRollForward(m_listDeteriorate, m_Investment);
                        Hashtable hashInput = (Hashtable)section.m_hashYearAttributeValues[0];
                        if (criteria.IsCriteriaMet(hashInput))
                        {
                            m_listSections.Add(section);
                        }
                    }
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Loading OMS Asset properties to Fill Sections." + e.Message));
                    return false;
                }
            }
            SimulationMessaging.NumberAssets = m_listSections.Count;
            return true;
        }

        /// <summary>
        /// Fills the section list
        /// </summary>
        /// <returns></returns>
        private bool FillRCSectionList()
        {
            bool filledSuccessfully = true;
            String sSectionTable = "SECTION_" + m_strNetworkID;
            String sDataTable = "SEGMENT_" + m_strNetworkID + "_NS0";

            //Get a list of all possible data from SEGMENT_ table.
            List<String> listColumns = GetSegmentColumns(sDataTable);
            if (listColumns == null) return false;

            //Retrieve all the data from the tables where JURISDICTION is true
            String strSelect = "SELECT * FROM " + sSectionTable + " INNER JOIN " + sDataTable + " ON " + sSectionTable + ".SECTIONID =" + sDataTable + ".SECTIONID";

            if (m_strJurisdiction.Length > 0)
            {
                strSelect += " WHERE " + m_strJurisdiction;
            }
            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    //SqlDataReader dr = DBMgr.CreateDataReader(strSelect);
                    DataTable simulationResults = DBMgr.CreateDataReader(strSelect);
                    if (simulationResults.Rows.Count > 0)
                    {
                        foreach (DataRow dr in simulationResults.Rows)
                        {
                            Sections section = new Sections();
                            section.SectionID = dr["SECTIONID"].ToString();
                            section.Facility = dr["FACILITY"].ToString();
                            section.Section = dr["SECTION"].ToString();
                            if (dr["BEGIN_STATION"].ToString().Trim() != "")
                            {
                                section.Begin = float.Parse(dr["BEGIN_STATION"].ToString());
                            }

                            if (dr["END_STATION"].ToString().Trim() != "")
                            {
                                section.End = float.Parse(dr["END_STATION"].ToString());
                            }

                            section.Direction = dr["DIRECTION"].ToString();

                            if (dr["AREA"].ToString().Trim() != "")
                            {
                                section.Area = float.Parse(dr["AREA"].ToString());
                            }

                            section.RollToYear = Investment.StartYear;
                            foreach (String str in listColumns)
                            {
                                //strValue = dr[str].ToString();
                                if (dr[str] == DBNull.Value) section.AddAttributeValue(str, null);
                                else section.AddAttributeValue(str, dr[str]);
                            }
                            try
                            {
                                section.RollForward(m_listDeteriorate, m_listAttributes, m_listCalculatedAttribute);
                            }
                            catch (Exception exc)
                            {
                                SimulationMessaging.AddMessage(new SimulationMessage("An error occurred rolling forward a section: " + exc.Message));
                            }
                            m_listSections.Add(section);
                        }
                    }
                    break;

                case "ORACLE":
                    try
                    {
                        OleDbDataReader odr = DBMgr.CreateOleDbDataReader(strSelect);
                        String ostrValue;

                        while (odr.Read())
                        {
                            Sections section = new Sections();
                            section.SectionID = odr["SECTIONID"].ToString();
                            section.Facility = odr["FACILITY"].ToString();
                            section.Section = odr["SECTION"].ToString();
                            if (odr["BEGIN_STATION"].ToString().Trim() != "")
                            {
                                section.Begin = float.Parse(odr["BEGIN_STATION"].ToString());
                            }

                            if (odr["END_STATION"].ToString().Trim() != "")
                            {
                                section.End = float.Parse(odr["END_STATION"].ToString());
                            }

                            section.Direction = odr["DIRECTION"].ToString();

                            if (odr["AREA"].ToString().Trim() != "")
                            {
                                section.Area = float.Parse(odr["AREA"].ToString());
                            }

                            section.RollToYear = Investment.StartYear;
                            foreach (String str in listColumns)
                            {
                                if (odr[str] != DBNull.Value)
                                {
                                    ostrValue = odr[str].ToString();
                                    section.AddAttributeValue(str, ostrValue);
                                }
                            }
                            section.RollForward(m_listDeteriorate, m_listAttributes, m_listCalculatedAttribute);
                            m_listSections.Add(section);
                        }
                        odr.Close();
                    }
                    catch (Exception ex)
                    {
                        SimulationMessaging.AddMessage("An error occurred retrieving the jurisdiction validated data: ", ex);
                        filledSuccessfully = false;
                    }
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }
            return filledSuccessfully;
        }

        /// <summary>
        /// Retrieves the Data for this simulution from SEGMENT_newtork
        /// TODO: Extend to all possible SEGMENT_ tables
        /// </summary>
        /// <returns></returns>
        private bool FillSectionListForSingleSection(String strSectionID)
        {
            String sSectionTable = "SECTION_" + m_strNetworkID;
            String sDataTable = "SEGMENT_" + m_strNetworkID + "_NS0";
            String sSimulationTable = "SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID;

            //Get a list of all possible data from SEGMENT_ table.
            List<String> listColumns = GetSegmentColumns(sDataTable);
            if (listColumns == null) return false;
            DataSet ds = null;
            String strSelect = "";
            if (m_listSections.Count == 0)
            {
                //Load all previous Section simulation data into m_listSections.
                strSelect = "SELECT * FROM " + sSimulationTable + " INNER JOIN " + sSectionTable + " ON " + sSimulationTable + ".SECTIONID =" + sSectionTable + ".SECTIONID";
                try
                {
                    ds = DBMgr.ExecuteQuery(strSelect);

                    //Investment.StartYear;
                    //Investment.AnalysisPeriod;
                    // combined with
                    //m_listAttributes;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Sections section = new Sections(dr, Investment.StartYear, Investment.AnalysisPeriod, m_listAttributes);
                        m_listSections.Add(section);
                    }
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Loading previous simulation data. " + exception.Message));
                    return false;
                }
            }

            //Retrieve the single section run from the SEGMENT_ table
            strSelect = "SELECT * FROM " + sSectionTable + " INNER JOIN " + sDataTable + " ON " + sSectionTable + ".SECTIONID =" + sDataTable + ".SECTIONID WHERE " + sDataTable + ".SECTIONID='" + strSectionID + "'";
            //try
            //{
            //SqlDataReader dr = DBMgr.CreateDataReader(strSelect);
            DataReader dr2 = new DataReader(strSelect);
            while (dr2.Read())
            {
                Sections section = m_listSections.Find((delegate (Sections s) { return s.SectionID == strSectionID; }));
                section.RollToYear = Investment.StartYear;
                foreach (String str in listColumns)
                {
                    section.ClearAttributeValues(str);
                    section.AddAttributeValue(str, dr2[str]);
                }
                section.RollForward(m_listDeteriorate, m_listAttributes, m_listCalculatedAttribute);
                section.CalculateArea(Investment.StartYear - 1);
            }
            dr2.Close();
            //}
            //catch (Exception exception)
            //{
            //    SimulationMessaging.AddMessage("Error: Rolling forward selected section. " + exception.Message);
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// Get a list of all columns in SEGMENT_ table
        /// </summary>
        /// <param name="sDataTable"></param>
        /// <returns></returns>
        private List<string> GetSegmentColumns(string sDataTable)
        {
            List<String> listColumns = new List<String>();
            List<string> columnNames;
            try
            {
                columnNames = DBMgr.GetTableColumns(sDataTable);
                // This loop gets a list of all possible values.
                //String strColumn;
                String strYear;
                //foreach (DataRow row in ds.Tables[0].Rows)
                foreach (string columnName in columnNames)
                {
                    //this is necessary because we play with the string and foreach has special requirements about not altering
                    //the value of the iterator variable
                    string tempColumnName = columnName;

                    //strColumn = row[0].ToString();

                    //if( !columnName.Contains( "_" ) )
                    //    continue;//If no underbar not year variable.
                    //if( columnName.Length < 6 )
                    //    continue;//To short.  Can't be year.
                    if (columnName.Substring(2).Contains("_"))
                    {
                        if (columnName.Length >= 6)
                        {
                            strYear = columnName.Substring(columnName.Length - 4, 4); //Gets the year from the string
                            int nYear = 0;
                            if (int.TryParse(strYear, out nYear))
                            {
                                if (nYear <= 2050 && nYear >= 1900)
                                {
                                    tempColumnName = tempColumnName.Substring(0, tempColumnName.Length - 5);//Gets the attibute from the string.
                                                                                                            //finding lower case fails with .Contains
                                    if (m_listAttributes.FindAll(
                                        delegate (string a)
                                        {
                                            return a.ToUpper() == tempColumnName.ToUpper();
                                        }).Count > 0)
                                    {
                                        listColumns.Add(columnName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error retrieving columns for " + sDataTable + ": " + ex.Message));
                listColumns = null;
            }
            return listColumns;
        }

        /// <summary>
        /// Gets the SimulationID, InvestmentID, DeteriorateID, TreatmentID,
        /// PriorityID, CommitID for a given Simulation name and NetworkID.
        /// </summary>
        /// <param name="strSimulation">Simulation name</param>
        /// <param name="strNetworkID">Network ID</param>
        /// <param name="strSimulationID">out SimulationID</param>
        /// <returns>True if successful.</returns>
        private bool DropPreviousSimulation(string strSimulationID, String strNetworkID)
        {
            if (!DropSimulation(m_strNetworkID, m_strSimulationID)) return false;
            return true;
        }

        /// <summary>
        /// Gets all the Simualtion ATTRIBUTES used in this Simulation
        /// </summary>
        /// <returns></returns>
        private bool GetSimulationAttributes()
        {
            DataSet ds;
            String strQuery;
            m_listAttributes = new List<string>();

            if (SimulationMessaging.IsOMS) m_listAttributes.Add("ID");

            string strArea = "1";
            if (!SimulationMessaging.IsOMS)//Areas are handled differently in
            {
                //Compile AREA equation
                try
                {
                    strArea = SimulationData.GetAreaEquation();
                    string networkArea = SimulationData.GetNetworkSpecificArea(m_strNetworkID);
                    if (!string.IsNullOrWhiteSpace(networkArea))
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Simulation using Netwrok Specific Area."));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Simulation using Netwrok Specific Area");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        strArea = networkArea;
                    }
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Error in compiling AREA function. SQL message - " + exception.Message));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error in compiling AREA function: " + exception.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }
            }
            else
            {
                try
                {
                    strArea = SimulationData.GetOMSAreaEquation(m_strSimulationID, cgOMS.Prefix);
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Error in compiling AREA function. SQL message - " + exception.Message));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error in compiling AREA function: " + exception.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }
            }

            SimulationMessaging.ListArea = new List<string>();
            ParseAttribute(strArea.Replace("[LENGTH]", "")); // Remove made up variable [LENGTH] only used for area.

            byte[] assembly = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "AREA", "BINARY_EQUATION", m_strNetworkID, null);
            if (assembly != null && assembly.Length > 0)
            {
                SimulationMessaging.Area = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assembly);
                if (SimulationMessaging.Area.OriginalInput != strArea)
                {
                    SimulationMessaging.Area = null;
                }
            }

            if (SimulationMessaging.Area == null)
            {
                SimulationMessaging.Area = new CalculateEvaluate.CalculateEvaluate();
                SimulationMessaging.Area.BuildClass(strArea, true, "AREA_BINARY_EQUATION_" + m_strNetworkID);
                SimulationMessaging.Area.CompileAssembly();
                SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + "AREA", "BINARY_EQUATION", m_strNetworkID, SimulationMessaging.Area);
            }

            if (SimulationMessaging.Area.m_listError.Count > 0)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Error in compiling AREA function. SQL message - " + SimulationMessaging.Area.m_listError[0].ToString()));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error in compiling AREA function");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }
            string[] listAreaParameters = strArea.Split(']');
            for (int i = 0; i < listAreaParameters.Length; i++)
            {
                if (listAreaParameters[i].Contains("["))
                {
                    SimulationMessaging.ListArea.Add(listAreaParameters[i].Substring(listAreaParameters[i].IndexOf('[') + 1));
                }
            }

            if (!IsUpdateOMS)
            {
                //Get the Jurisdiction from the simulation table.
                strQuery = "SELECT JURISDICTION,WEIGHTING,CREATOR FROM " + cgOMS.Prefix + "SIMULATIONS WHERE SIMULATIONID='" + m_strSimulationID + "'";
                try
                {
                    ds = DBMgr.ExecuteQuery(strQuery);
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Error in retrieving JURISDICTION from SIMULATIONS table. SQL message - " + exception.Message));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error in retrieving JURISDICTION from SIMULATIONS: " + exception.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }
                DataSet creatorCriteriaDataSet;
                string creatorUsername = ds.Tables[0].Rows[0].ItemArray[2]?.ToString();
                bool creatorHasAccess = true;
                string creatorCriteria = null;
                if (!string.IsNullOrEmpty(creatorUsername))
                {
                    try
                    {
                        creatorCriteriaDataSet = DBMgr.ExecuteQuery($"SELECT HAS_ACCESS,CRITERIA FROM {cgOMS.Prefix}USER_CRITERIA WHERE USERNAME='{creatorUsername}'");
                    }
                    catch (Exception exception)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage($"Error: Error in retrieving CRITERIA from USER_CRITERIA table. SQL message - {exception.Message}"));

                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, $"Error in retrieving CRITERIA from USER_CRITERIA: {exception.Message}");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }

                        return false;
                    }
                    creatorHasAccess = Convert.ToBoolean(creatorCriteriaDataSet.Tables[0].Rows[0].ItemArray[0]);
                    creatorCriteria = creatorCriteriaDataSet.Tables[0].Rows[0].ItemArray[1]?.ToString();
                }
                
                if (!creatorHasAccess)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage($"Error: Simulation owner does not have permission to run simulations on any inventory items."));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, $"Simulation owner does not have permission to run simulations on any inventory items.");
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }

                m_strJurisdiction = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                if (!string.IsNullOrEmpty(creatorCriteria))
                {
                    m_strJurisdiction = $"({m_strJurisdiction}) AND ({creatorCriteria})";
                }
                m_strJurisdiction = ConvertOMSAttribute(m_strJurisdiction);
                m_strWeighting = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                if (m_strWeighting != "none" && m_strWeighting != "")
                {
                    if (!m_listAttributes.Contains(m_strWeighting))
                    {
                        m_listAttributes.Add(m_strWeighting);
                    }
                }

                SimulationMessaging.Jurisdiction = new Jurisdiction(m_strNetworkID);

                //Allow blank JURISDICTION
                if (m_strJurisdiction.Trim().Length > 0)
                {
                    m_strJurisdiction = m_strJurisdiction.Replace("|", "'");
                    switch (DBMgr.NativeConnectionParameters.Provider)
                    {
                        case "MSSQL":
                            if (m_strJurisdiction.IndexOf("[SECTIONID]") > -1)
                            {
                                m_strJurisdiction = m_strJurisdiction.Replace("[SECTIONID]", "SECTION_" + m_strNetworkID + ".SECTIONID");
                            }
                            break;

                        case "ORACLE":
                            if (m_strJurisdiction.IndexOf("'SECTIONID'") > -1)
                            {
                                m_strJurisdiction = m_strJurisdiction.Replace("'SECTIONID'", "SECTION_" + m_strNetworkID + ".SECTIONID");
                            }
                            break;

                        default:
                            throw new NotImplementedException("TODO: Create ANSI implementation for GetSimulationAttributes()");
                            //break;
                    }
                    //Check JURIDICITON for Attributes.
                    ParseAttribute(m_strJurisdiction);

                    //byte[] assemblyCriteria = null;

                    // //This checks for local copy of this object.
                    //assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "JURISDICTION", "BINARY_CRITERIA", m_strNetworkID, assemblyCriteria);
                    //if (assemblyCriteria != null && assemblyCriteria.Length > 0)
                    //{
                    //    SimulationMessaging.Jurisdiction.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                    //    if (SimulationMessaging.Jurisdiction.Evaluate.OriginalInput != m_strJurisdiction)
                    //    {
                    //        SimulationMessaging.Jurisdiction.Evaluate = null;
                    //    }
                    //}
                }
                else
                {
                    m_strJurisdiction = "";
                }

                //if (SimulationMessaging.Jurisdiction.Evaluate == null)
                //{
                //    SimulationMessaging.Jurisdiction.Criteria = m_strJurisdiction;
                //}

                SimulationMessaging.AddMessage(new SimulationMessage("Initializing simulation complete: " + DateTime.Now.ToString("HH:mm:ss")));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Initialized simulation");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }

                //SimulationMessaging.AddMessage("DEBUGGING GetSimulationAttributes(): POINT 1");
                ///Check for LANES.  This is used to calculate area
                CheckForLanes();
                //SimulationMessaging.AddMessage("DEBUGGING GetSimulationAttributes(): POINT 2");
            }

            if (!IsUpdateOMS)
            {
                //Get calculated field data
                //Populates Calculated field data with fields from the get treatment data function.
                if (!GetCalculatedFieldData()) return false;
            }

            //InvestmentID does not have SIMULATION ATTRIBUTES
            //Deterioration
            //Adds to list of Simulation Attributes
            //Creates a list of Deterioration objects which contain all Deterioration data for this simulation
            //This data is stored in m_listDeteriorate which will be iterated every year (rolling forward and simulation).
            if (!GetDeteriorationData()) return false;
            //SimulationMessaging.AddMessage("DEBUGGING GetSimulationAttributes(): POINT 3");

            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Performance Equations and Criteria complete: " + DateTime.Now.ToString("HH:mm:ss")));

            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Verified Performance Equations and Criteria");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            //SimulationMessaging.AddMessage("DEBUGGING GetSimulationAttributes(): POINT 4");
            SimulationMessaging.GetUniqueDeteriorateAttributes(m_strSimulationID);
            //SimulationMessaging.AddMessage("DEBUGGING GetSimulationAttributes(): POINT 5");

            //Get Investment Data
            if (!GetInvestmentData()) return false;
            //SimulationMessaging.AddMessage("DEBUGGING GetSimulationAttributes(): POINT 6");

            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Investments complete: " + DateTime.Now.ToString("HH:mm:ss")));

            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Verified Investments");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            ////Get Target Data
            //if (!IsUpdateOMS)
            //{
            if (!GetTargetData()) return false;
            //}

            SimulationMessaging.Targets = m_hashTargets;
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Targets and deficiency complete: " + DateTime.Now.ToString("HH:mm:ss")));

            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Verified Targets and deficiency");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            if (!GetRemainingLifes()) return false;
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Remaining Life Limits complete: " + DateTime.Now.ToString("HH:mm:ss")));

            if (!GetBudgetCriterias()) return false;
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Budget Criteria complete: " + DateTime.Now.ToString("HH:mm:ss")));

            if (!GetSplitTreatments()) return false;
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Split Treatments complete: " + DateTime.Now.ToString("HH:mm:ss")));


            if (!IsUpdateOMS)
            {
                if (!CreateAdditionalSimulationTable(m_strNetworkID, m_strSimulationID)) return false;
            }
            //Treatments
            //Adds to the list of Simulation Attibutes
            //Creates list of Treatments
            if (!GetTreatmentData()) return false;
            SimulationMessaging.AddMessage(new SimulationMessage("Verifying Treatments complete: " + DateTime.Now.ToString("HH:mm:ss")));

            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Verified Treatments");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }

            if (!GetCommittedConsequences()) return false;
            SimulationMessaging.AddMessage(new SimulationMessage("Retrieving committed project consequences complete: " + DateTime.Now.ToString("HH:mm:ss")));

            if (!IsUpdateOMS)
            {
                //Priority
                //Adds to the list of Simulation Attributes
                //Creates a list of Priority objects (which will be run though). Iterated every year when Budgets are spent.
                if (!GetPriorityData()) return false;
                SimulationMessaging.AddMessage(new SimulationMessage("Verifying Priorities complete: " + DateTime.Now.ToString("HH:mm:ss")));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Verified Priorities");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }

                SimulationMessaging.crs = new CRS();
            }
            return true;
        }

        /// <summary>
        /// Searches through a Query or Equation and extracts all of the database
        /// attributes encased in square brackets [attribute].
        /// </summary>
        /// <param name="strQuery">Query or Equation to parse.</param>
        /// <returns>List of variables in global variable.</returns>
        private void ParseAttribute(String strQuery)
        {
            String strAttribute;
            int nOpen = -1;

            //strQuery = strQuery.ToUpper();

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

                    if (!m_listAttributes.Contains(strAttribute))
                    {
                        m_listAttributes.Add(strAttribute);
                    }
                }
            }
        }

        private bool CheckForLanes()
        {
            SimulationMessaging.LanesVariable = "";
            this.IsLanes = false;
            // LANES is not available for OMS
            if (DBMgr.IsTableInDatabase("ATTRIBUTES_"))
            {
                String strSelect = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_";
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strAttribute = row[0].ToString();
                    if (strAttribute.ToUpper() == "LANES")
                    {
                        SimulationMessaging.LanesVariable = strAttribute;
                        this.IsLanes = true;
                        if (!m_listAttributes.Contains(strAttribute))
                        {
                            m_listAttributes.Add(strAttribute);
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets all Deterioration data for this Simulation and Network Creates
        /// list of Deteriorate objects which contains Attribute Display name
        /// Criteria (compiled and uncompiled) Equation (compiled and uncompiled)
        /// List of Attributes Criteria List of Attributes Equation
        /// </summary>
        /// <returns>True if this function is successful</returns>
        private bool GetDeteriorationData()
        {
            String select = "SELECT ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,SHIFT,ISFUNCTION, PIECEWISE,PERFORMANCEID FROM " + cgOMS.Prefix + "PERFORMANCE WHERE SIMULATIONID='" + m_strSimulationID + "'";
            if (SimulationMessaging.IsOMS)
            {
                select = "SELECT ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,SHIFT,NULL AS ISFUNCTION, PIECEWISE,PERFORMANCEID FROM " + cgOMS.Prefix + "PERFORMANCE WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }

            DataSet ds = null;
            try
            {
                ds = DBMgr.ExecuteQuery(select);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving Performance data. SQL Message - " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Retrieving Performance data: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                byte[] assemblyCriteria = null;
                byte[] assemblyEquation = null;
                bool isPiecewise = false;
                bool isFunction = false;
                string id = row["PERFORMANCEID"].ToString();

                if (row["PIECEWISE"] != DBNull.Value)
                {
                    isPiecewise = Convert.ToBoolean(row["PIECEWISE"]);
                }

                Deteriorate deteriorate = new Deteriorate(cgOMS.Prefix + "PERFORMANCE", "BINARY_CRITERIA", id);
                //This checks for local copy of this object.
                assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "PERFORMANCE", "BINARY_CRITERIA", id, assemblyCriteria);
                if (assemblyCriteria != null && assemblyCriteria.Length > 0)
                {
                    string criteria = row["CRITERIA"].ToString();
                    deteriorate.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                    if (deteriorate.Evaluate.OriginalInput != criteria)
                    {
                        deteriorate.Evaluate = null;
                    }
                }

                assemblyEquation = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "PERFORMANCE", "BINARY_EQUATION", id, assemblyEquation);
                if (assemblyEquation != null && assemblyEquation.Length > 0 && !isPiecewise)
                {
                    string equation = row["EQUATION"].ToString();
                    deteriorate.Calculate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyEquation);
                    if (deteriorate.Calculate.OriginalInput != equation)
                    {
                        deteriorate.Calculate = null;
                    }
                }

                deteriorate.Attribute = row["ATTRIBUTE_"].ToString();
                deteriorate.Group = row["EQUATIONNAME"].ToString();
                string performanceCriteria = row["CRITERIA"].ToString();
                if (SimulationMessaging.IsOMS)
                {
                    performanceCriteria = OMSCriteria.GetDecisionEngineCritera(performanceCriteria);
                }

                deteriorate.Criteria = performanceCriteria;
                if (row["ISFUNCTION"] != DBNull.Value) isFunction = Convert.ToBoolean(row["ISFUNCTION"]);
                if (row["EQUATION"].ToString() == "")
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: A equation must be entered for every PERFORMANCE variable. " + row[0].ToString()));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: An equation must be entered for every PERFORMANCE variable. " + row[0].ToString());
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }

                String strShift = row["SHIFT"].ToString();
                if (strShift == "True")
                {
                    deteriorate.Shift = true;
                }

                //Implements piecewise modeling.
                if (isPiecewise)
                {
                    deteriorate.PiecewiseEquation = new SimulationDataAccess.DTO.Piecewise(row["EQUATION"].ToString(), SimulationMessaging.IsOMS);
                    if (deteriorate.PiecewiseEquation.Errors != null)
                    {
                        deteriorate.Errors.Add(deteriorate.PiecewiseEquation.Errors);
                    }
                    //Piecewise have no attributes, but this needs to be created.
                    deteriorate.EquationAttributes = new List<string>();
                }
                else
                {
                    if (!isFunction)
                        deteriorate.Equation = row["EQUATION"].ToString();
                    else
                        deteriorate.SetFunction(row["EQUATION"].ToString());
                }

                if (deteriorate.Attribute == Method.BenefitAttribute)
                {
                    deteriorate.BenefitLimit = Method.BenefitLimit;
                }
                m_listDeteriorate.Add(deteriorate);
                SimulationMessaging.AddMessage(new SimulationMessage("Compile Performance curve " + deteriorate.Group + " for attribute " + deteriorate.Attribute + " at " + DateTime.Now.ToString("HH:mm:ss")));
                foreach (String str in deteriorate.Errors)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Compile error PERFORMANCE curve " + deteriorate.Attribute + "|" + deteriorate.Group + " " + str));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Compile error PERFORMANCE curve" + deteriorate.Attribute + "|" + deteriorate.Group + " " + str);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }

                if (deteriorate.CriteriaAttributes == null)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Unknown variable in PERFORMANCE CRITERIA. " + row[2].ToString()));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Unknown variable in PERFORMANCE CRITERIA" + row[2].ToString());
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }

                foreach (String str in deteriorate.CriteriaAttributes)
                {
                    if (!m_listAttributes.Contains(str))
                    {
                        m_listAttributes.Add(str);
                    }
                }

                if (deteriorate.EquationAttributes == null)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Unknown variable in PERFORMANCE Equation. " + row[3].ToString()));

                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Unknown variable in PERFORMANCE CRITERIA" + row[3].ToString());
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }

                foreach (String str in deteriorate.EquationAttributes)
                {
                    if (!m_listAttributes.Contains(str))
                    {
                        m_listAttributes.Add(str);
                    }
                }

                if (!m_listAttributes.Contains(deteriorate.Attribute))
                {
                    m_listAttributes.Add(deteriorate.Attribute);
                }
            }

            m_listDeteriorate.Sort(delegate (Deteriorate d1, Deteriorate d2) { return d2.Criteria.ToString().CompareTo(d1.Criteria.ToString()); });
            SimulationMessaging.Deteriorates = m_listDeteriorate;

            if (!SimulationMessaging.IsOMS)
            {
                switch (Method.TypeAnalysis)
                {
                    case "Maximum Benefit":
                    case "Multi-year Maximum Benefit":
                    case "Incremental Benefit/Cost":
                    case "Multi-year Incremental Benefit/Cost":
                        foreach (Deteriorate deteriorate in m_listDeteriorate)
                        {
                            if (deteriorate.Attribute == Method.BenefitAttribute)
                            {
                                return true;
                            }
                        }
                        //If the benefit is set to a calculated benefit, accept this as a valid benefit variable.
                        // Benefit can occur due to deterioration of the individual components of the calculated field
                        // Or can occur due to changes in the consequences.
                        // Currently this is to difficult to unravel to see if something changes.  So assumption is that a calculated variable is always accepted.
                        foreach (var calculatedBenefit in m_listCalculatedAttribute)
                        {
                            if (calculatedBenefit.Attribute == Method.BenefitAttribute.ToUpper())
                            {
                                if (!m_listAttributes.Contains(Method.BenefitAttribute))
                                {
                                    m_listAttributes.Add(Method.BenefitAttribute.ToUpper());
                                }

                                return true;
                            }
                        }

                        SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: At least one deterioration equation must be set for the Benefit variable."));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Error: At least one deterioration equation must be set for the Benefit variable");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        return false;
                    //break;
                    default:
                        break;
                }
            }
            return true;
        }

        private bool GetCalculatedFieldData()
        {
            m_listCalculatedAttribute = new List<CalculatedAttribute>();

            var strSelect = "SELECT ATTRIBUTE_,EQUATION,CRITERIA,ID_ FROM " + cgOMS.Prefix + "ATTRIBUTES_CALCULATED";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving Calculated Field data. SQL Message - " + exception.Message));
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var id = Int32.Parse(row["ID_"].ToString());
                var attribute = row["ATTRIBUTE_"].ToString();
                string equation = row["EQUATION"].ToString();
                string criteria = row["CRITERIA"].ToString();
                var calculatedAttribute = new CalculatedAttribute(id, attribute.ToUpper(), equation, criteria);
                m_listCalculatedAttribute.Add(calculatedAttribute);

                if (!m_listAttributes.Contains(attribute.ToUpper()))
                {
                    m_listAttributes.Add(attribute.ToUpper());
                }
                foreach (String str in calculatedAttribute.AttributesCriteria)
                {
                    if (!m_listAttributes.Contains(str))
                    {
                        m_listAttributes.Add(str);
                    }
                }

                foreach (String str in calculatedAttribute.AttributesEquation)
                {
                    if (!m_listAttributes.Contains(str))
                    {
                        m_listAttributes.Add(str);
                    }
                }
            }
            return true;
        }

        private bool GetTreatmentData()
        {
            Treatments treatments;
            // Get list of treatments.
            String strSelect = "SELECT TREATMENT, BEFOREANY,BEFORESAME,BUDGET,TREATMENTID ";

            if (SimulationMessaging.IsOMS)
            {
                strSelect += ", OMS_IS_REPEAT,OMS_REPEAT_START, OMS_REPEAT_INTERVAL, OMS_IS_EXCLUSIVE";
            }

            strSelect += " FROM " + cgOMS.Prefix + "TREATMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            if (SimulationMessaging.IsOMS)
            {
                strSelect += " AND (OMS_IS_SELECTED='1' OR TREATMENT='No Treatment')";
            }

            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error:  Unable to open TREATMENTS table for Analysis.  SQL message - " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Unable to open TREATMENTS table for Analysis: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                treatments = new Treatments();
                treatments.Treatment = row[0].ToString();
                SimulationMessaging.AddMessage(new SimulationMessage("Compiling " + treatments.Treatment + " at " + DateTime.Now.ToString("HH:mm:ss")));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Compiling " + treatments.Treatment);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                int nAny;
                int nSame;

                int.TryParse(row[1].ToString(), out nAny);
                int.TryParse(row[2].ToString(), out nSame);

                treatments.AnyTreatment = nAny;
                treatments.SameTreatment = nSame;

                treatments.Budget = row[3].ToString();
                treatments.TreatmentID = row[4].ToString();
                if (SimulationMessaging.IsOMS)
                {
                    if (row["OMS_IS_REPEAT"] != DBNull.Value) treatments.OMSIsRepeat = Convert.ToBoolean(row["OMS_IS_REPEAT"]);
                    if (row["OMS_REPEAT_START"] != DBNull.Value) treatments.OMSRepeatStart = Convert.ToInt32(row["OMS_REPEAT_START"]);
                    if (row["OMS_REPEAT_INTERVAL"] != DBNull.Value) treatments.OMSRepeatInterval = Convert.ToInt32(row["OMS_REPEAT_INTERVAL"]);
                    if (row["OMS_IS_EXCLUSIVE"] != DBNull.Value) treatments.OMSIsExclusive = Convert.ToBoolean(row["OMS_IS_EXCLUSIVE"]);
                }

                if (!IsUpdateOMS) //To save time on UpdateSimulation for OMS no need to load feasibility or cost (we already have them).
                {
                    //SimulationMessaging.AddMessage("Loading Feasibility");
                    if (!treatments.LoadFeasibility(APICall, Simulations, m_strSimulationID)) return false;
                    //SimulationMessaging.AddMessage("Loading Costs");
                }
                if (!treatments.LoadCost(APICall, Simulations, m_strSimulationID)) return false;
                //SimulationMessaging.AddMessage("Loading Consequences");
                if (!treatments.LoadConsequences(APICall, Simulations, m_strSimulationID)) return false;

                if (!treatments.LoadSupersedes(APICall, Simulations, m_strSimulationID)) return false;

                //if (!treatments.LoadExclusion()) return false;
                foreach (String str in treatments.Attributes)
                {
                    if (!m_listAttributes.Contains(str))
                    {
                        m_listAttributes.Add(str);
                    }
                }
                m_listTreatments.Add(treatments);
            }

            foreach (var treatment in m_listTreatments)
            {
                treatment.LoadScheduled(m_listTreatments, APICall, Simulations, m_strSimulationID);
            }
            return true;
        }

        private bool GetCommittedConsequences()
        {
            m_dictionaryCommittedConsequences = new Dictionary<string, List<AttributeChange>>();

            // Get list of Committed Consequences.
            String strSelect =
                "SELECT CC.COMMITID,ATTRIBUTE_,CHANGE_ FROM COMMIT_CONSEQUENCES CC INNER JOIN COMMITTED_ C ON C.COMMITID = CC.COMMITID WHERE C.SIMULATIONID = '" +
                m_strSimulationID + "'";

            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var id = row["COMMITID"].ToString();
                    var attribute = row["ATTRIBUTE_"].ToString();
                    var change = row["CHANGE_"].ToString();

                    if (!m_dictionaryCommittedConsequences.ContainsKey(id))
                    {
                        var commitedAttributeChange = new List<AttributeChange>();
                        m_dictionaryCommittedConsequences.Add(id, commitedAttributeChange);
                    }
                    m_dictionaryCommittedConsequences[id].Add(new AttributeChange(attribute, change));
                }
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error:  Unable to open COMMITTED_CONSEQUENCES table for Analysis.  SQL message - " + exception.Message));
                return false;
            }
            return true;
        }

        private bool GetPriorityData()
        {
            Priorities priority;
            String strSelect = "SELECT CRITERIA, PRIORITYLEVEL,PRIORITYID,YEARS FROM " + cgOMS.Prefix + "PRIORITY WHERE SIMULATIONID='" + m_strSimulationID + "' ORDER BY PRIORITYLEVEL";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error:  Unable to open PRIORITY table for Analysis.  SQL message - " + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Unable to open PRIORITY table for Analysis: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            if (ds.Tables[0].Rows.Count == 0)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error:  At least one priority level must be entered."));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: At least one priority level must be entered");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string criteria = row["CRITERIA"].ToString();
                string id = row["PRIORITYID"].ToString();
                priority = new Priorities(id);

                priority.PriorityLevel = int.Parse(row[1].ToString());
                String strYears = row["YEARS"].ToString();
                if (!String.IsNullOrEmpty(strYears))
                {
                    priority.Years = int.Parse(strYears);
                }

                SimulationMessaging.AddMessage(new SimulationMessage("Compiling Priority Level " + row["PRIORITYLEVEL"].ToString()));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, $"Compiling Priority Level - {row["PRIORITYLEVEL"].ToString()}");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                if (row[0].ToString().Trim() == "")
                {
                    priority.IsAllSections = true;
                }
                else
                {
                    byte[] assemblyCriteria = null;
                    assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "PRIORITY", "BINARY_CRITERIA", priority.PriorityID, assemblyCriteria);
                    if (assemblyCriteria != null && assemblyCriteria.Length > 0)
                    {
                        priority.Criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                        if (priority.Criteria.Evaluate.OriginalInput != criteria)
                        {
                            priority.Criteria.Evaluate = null;
                        }
                    }

                    priority.Criteria.Criteria = criteria;
                    if (priority.Criteria.Errors.Count > 0)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error: " + priority.Criteria.Errors[0].ToString()));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, $"Error: {priority.Criteria.Errors[0].ToString()}");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        priority.IsAllSections = true;
                    }
                    else
                    {
                        priority.IsAllSections = false;
                    }
                }
                if (!priority.LoadBudgetPercentages(Investment.BudgetOrder, APICall, Simulations, m_strSimulationID)) return false;

                if (!priority.IsAllSections)
                {
                    foreach (String strAttribute in priority.Criteria.CriteriaAttributes)
                    {
                        if (!m_listAttributes.Contains(strAttribute))
                        {
                            m_listAttributes.Add(strAttribute);
                        }
                    }
                }
                m_listPriorities.Add(priority);
            }
            return true;
        }

        public bool GetInvestmentData()
        {
            String strSelect = "SELECT FIRSTYEAR,NUMBERYEARS,INFLATIONRATE,DISCOUNTRATE,BUDGETORDER FROM " + cgOMS.Prefix + "INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            if (SimulationMessaging.IsOMS)
            {
                strSelect = "SELECT FIRSTYEAR,NUMBERYEARS,INFLATIONRATE,DISCOUNTRATE,BUDGETORDER,SIMULATION_START_DATE FROM " + cgOMS.Prefix + "INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }

            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Accessing INVESTMENTS table. SQL message -" + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Accessing INVESTMENTS table: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }
            DataRow row = ds.Tables[0].Rows[0];

            Investment = new Investments();

            int nStartYear = 2007;
            int nNumberYear = 1;
            float fInflation = 0;
            float fDiscount = 0;

            Investment.InvestmentID = m_strSimulationID;
            int.TryParse(row[0].ToString(), out nStartYear);
            int.TryParse(row[1].ToString(), out nNumberYear);
            float.TryParse(row[2].ToString(), out fInflation);
            float.TryParse(row[3].ToString(), out fDiscount);

            DateTime startDate = new DateTime(nStartYear, 1, 1);
            if (SimulationMessaging.IsOMS)
            {
                if (row["SIMULATION_START_DATE"] != DBNull.Value)
                {
                    startDate = Convert.ToDateTime(row["SIMULATION_START_DATE"]);
                }
            }

            Investment.StartYear = nStartYear;
            Investment.AnalysisPeriod = nNumberYear;
            Investment.Inflation = fInflation;
            Investment.Discount = fDiscount;
            Investment.BudgetOrderString = row[4].ToString().Replace('|', ',');
            Investment.StartDate = startDate;
            Investment.LoadBudgets();
            SimulationMessaging.AnalysisPeriod = nNumberYear;
            return true;
        }

        public bool GetRemainingLifes()
        {
            SimulationMessaging.RemainingLifes = new List<RemainingLife>();
            String select = "SELECT REMAINING_LIFE_ID, ATTRIBUTE_, REMAINING_LIFE_LIMIT, CRITERIA FROM " + cgOMS.Prefix + "REMAINING_LIFE_LIMITS WHERE SIMULATION_ID='" + m_strSimulationID + "'";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(select);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Accessing REMAINING_LIFE_LIMITS table. SQL message -" + exception.Message));
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var remainingLifeId = Convert.ToInt32(row["REMAINING_LIFE_ID"]);
                var attribute = row["ATTRIBUTE_"].ToString();
                var remainingLifeLimit = Convert.ToDouble(row["REMAINING_LIFE_LIMIT"]);
                var criteria = "";
                if (row["CRITERIA"] != DBNull.Value) criteria = row["CRITERIA"].ToString();
                var remainingLife = new RemainingLife(remainingLifeId, attribute, remainingLifeLimit, criteria);
                SimulationMessaging.RemainingLifes.Add(remainingLife);

                if (!m_listAttributes.Contains(attribute)) m_listAttributes.Add(attribute);

                if (remainingLife.Criteria != null && remainingLife.Criteria.CriteriaAttributes != null)
                {
                    foreach (var criteriaAttribute in remainingLife.Criteria.CriteriaAttributes)
                    {
                        if (!m_listAttributes.Contains(criteriaAttribute)) m_listAttributes.Add(criteriaAttribute);
                    }
                }
            }
            return true;
        }

        public bool GetSplitTreatments()
        {
            SimulationMessaging.SplitTreatments = new List<ISplitTreatment>();

            var select = "SELECT SPLIT_TREATMENT_ID, DESCRIPTION, CRITERIA FROM SPLIT_TREATMENT WHERE SIMULATIONID='" + m_strSimulationID + "'";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(select);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Accessing SPLIT_TREATMENT table. SQL message -" + exception.Message));
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var splitTreatmentId = row["SPLIT_TREATMENT_ID"].ToString();
                var description = "";
                var criteria = "";
                if (row["DESCRIPTION"] != DBNull.Value) description = row["DESCRIPTION"].ToString();
                if (row["CRITERIA"] != DBNull.Value) criteria = row["CRITERIA"].ToString();
                var limits = new List<ISplitTreatmentLimit>();
                var splitTreatment = new SplitTreatment(splitTreatmentId,description,criteria,limits);
                SimulationMessaging.SplitTreatments.Add(splitTreatment);


                if (splitTreatment.Criteria != null && splitTreatment.Criteria.CriteriaAttributes != null)
                {
                    foreach (var criteriaAttribute in splitTreatment.Criteria.CriteriaAttributes)
                    {
                        if (!m_listAttributes.Contains(criteriaAttribute)) m_listAttributes.Add(criteriaAttribute);
                    }
                }

            }


            foreach(var splitTreatment in SimulationMessaging.SplitTreatments)
            {
                var selectLimits = "SELECT RANK,AMOUNT,PERCENTAGE FROM SPLIT_TREATMENT_LIMIT WHERE SPLIT_TREATMENT_ID='" + splitTreatment.Id + "' ORDER BY RANK";
                try
                {
                    ds = DBMgr.ExecuteQuery(selectLimits);
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Accessing SPLIT_TREATMENT table. SQL message -" + exception.Message));
                    return false;
                }


                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string rank = "1";
                    string percentages = "";
                    string amount = "";

                    if (row["RANK"] != DBNull.Value) rank = row["RANK"].ToString();
                    if (row["AMOUNT"] != DBNull.Value) amount = row["AMOUNT"].ToString();
                    if (row["PERCENTAGE"] != DBNull.Value) percentages = row["PERCENTAGE"].ToString();
                    var splitTreatmentLimit = new SplitTreatmentLimit(rank,amount,percentages);
                    splitTreatment.Limits.Add(splitTreatmentLimit);
                }


            }

            //If user has not set split treatment.  Create a default that does not split.
            if (SimulationMessaging.SplitTreatments.Count == 0)
            {
                var splitTreatment = new SplitTreatment("0", "No split treatment defined", "", new List<ISplitTreatmentLimit>());
                splitTreatment.Limits.Add(new SplitTreatmentLimit("1", "", "100"));
                SimulationMessaging.SplitTreatments.Add(splitTreatment);
            }



            return true;
        }

        public bool GetBudgetCriterias()
        {
            SimulationMessaging.BudgetCriterias = new List<BudgetCriteria>();
            String select = "SELECT BUDGET_CRITERIA_ID, BUDGET_NAME, CRITERIA FROM " + cgOMS.Prefix + "BUDGET_CRITERIA WHERE SIMULATIONID='" + m_strSimulationID + "'";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(select);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Accessing BUDGET_CRITERIA table. SQL message -" + exception.Message));
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var budgetCriteriaId = Convert.ToInt32(row["BUDGET_CRITERIA_ID"]);
                var budgetName = row["BUDGET_NAME"].ToString();
                var criteria = "";
                if (row["CRITERIA"] != DBNull.Value) criteria = row["CRITERIA"].ToString();
                var budgetCriteria = new BudgetCriteria(budgetCriteriaId, budgetName, criteria);
                SimulationMessaging.BudgetCriterias.Add(budgetCriteria);

                if (budgetCriteria.Criteria != null && budgetCriteria.Criteria.CriteriaAttributes != null)
                {
                    foreach (var criteriaAttribute in budgetCriteria.Criteria.CriteriaAttributes)
                    {
                        if (!m_listAttributes.Contains(criteriaAttribute)) m_listAttributes.Add(criteriaAttribute);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets targets and deficents
        /// </summary>
        /// <param name="bTarget"></param>
        /// <returns></returns>
        public bool GetTargetData()
        {
            String strSelect = "SELECT ID_,ATTRIBUTE_,YEARS,TARGETMEAN,CRITERIA,BINARY_CRITERIA,TARGETNAME FROM " + cgOMS.Prefix + "TARGETS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Accessing TARGETS table. SQL message -" + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Accessing TARGETS table: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            String sYear;
            String sTarget;
            String sDeficient;
            String sPercentage;
            String sCriteria;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string id = row["ID_"].ToString();
                Targets targets = new Targets(id, cgOMS.Prefix + "TARGETS");

                String strAttribute = row["ATTRIBUTE_"].ToString();
                String strTargetMean = row["TARGETMEAN"].ToString();
                string targetName = "";
                if (row["TARGETNAME"] != DBNull.Value) targetName = row["TARGETNAME"].ToString();
                targets.TargetName = targetName;

                if (strTargetMean == "")
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: " + strAttribute + " - TARGET is missing. Targets if they are included, must have numeric values input for Target."));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Error: " + strAttribute + " - TARGET is missing. Targets if they are included, must have numeric values input for Target");
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }

                targets.Attribute = row["ATTRIBUTE_"].ToString();

                if (!m_listAttributes.Contains(targets.Attribute))
                {
                    m_listAttributes.Add(targets.Attribute);
                }

                sYear = row["YEARS"].ToString();
                if (sYear == "")
                {
                    targets.IsAllYears = true;
                }
                else
                {
                    targets.Year = int.Parse(sYear);
                    targets.IsAllYears = false;
                }

                sTarget = row["TARGETMEAN"].ToString();
                if (sTarget == "")
                {
                    targets.IsTargets = false;
                }
                else
                {
                    targets.Target = double.Parse(sTarget);
                    //OMS uses Targets for priority.  If a budget analysis, then set OMS Target 100 so target never met.
                    if (SimulationMessaging.IsOMS && !Method.IsOMSTargetEnforced)
                    {
                        if (targets.TargetName == "OCI TARGET")
                        {
                            targets.Target = 100;
                        }
                    }
                    targets.IsTargets = true;
                }

                targets.IsDeficient = false;
                targets.IsDeficiencyPercentage = false;

                sCriteria = row["CRITERIA"].ToString();
                sCriteria = ConvertOMSAttribute(sCriteria);

                if (sCriteria == "")
                {
                    targets.IsAllSections = true;
                }
                else
                {
                    byte[] assemblyCriteria = null;

                    assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "TARGETS", "BINARY_CRITERIA", id, assemblyCriteria);
                    if (assemblyCriteria != null && assemblyCriteria.Length > 0)
                    {
                        targets.Criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                        if (targets.Criteria.Evaluate.OriginalInput != sCriteria)
                        {
                            targets.Criteria.Evaluate = null;
                        }
                    }

                    targets.Criteria.Criteria = sCriteria;

                    if (targets.Criteria.CriteriaAttributes == null)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Attribute " + row[1].ToString() + "  has unknown variable in Target CRITERIA. " + row[4].ToString()));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Error: Attribute " + row[1].ToString() + "  has unknown variable in Target CRITERIA " + row[4].ToString());
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        return false;
                    }

                    foreach (String str in targets.Criteria.CriteriaAttributes)
                    {
                        if (!m_listAttributes.Contains(str))
                        {
                            m_listAttributes.Add(str);
                        }
                    }

                    targets.IsAllSections = false;
                }

                Hashtable hashYearTargets;
                List<Targets> listTargets;
                if (targets.IsAllYears)
                {
                    if (!TargetHash.Contains("All"))
                    {
                        listTargets = new List<Targets>();
                        listTargets.Add(targets);
                        hashYearTargets = new Hashtable();
                        hashYearTargets.Add(targets.Attribute, listTargets);
                        TargetHash.Add("All", hashYearTargets);
                    }
                    else
                    {
                        hashYearTargets = (Hashtable)TargetHash["All"];
                        if (!hashYearTargets.Contains(targets.Attribute))
                        {
                            listTargets = new List<Targets>();
                            listTargets.Add(targets);
                            hashYearTargets.Add(targets.Attribute, listTargets);
                        }
                        else
                        {
                            listTargets = (List<Targets>)hashYearTargets[targets.Attribute];
                            listTargets.Add(targets);
                        }
                    }
                }
                else
                {
                    if (!TargetHash.Contains(targets.Year.ToString()))
                    {
                        listTargets = new List<Targets>();
                        listTargets.Add(targets);
                        hashYearTargets = new Hashtable();
                        hashYearTargets.Add(targets.Attribute, listTargets);
                        TargetHash.Add(targets.Year.ToString(), hashYearTargets);
                    }
                    else
                    {
                        hashYearTargets = (Hashtable)TargetHash[targets.Year.ToString()];
                        if (!hashYearTargets.Contains(targets.Attribute))
                        {
                            listTargets = new List<Targets>();
                            listTargets.Add(targets);
                            hashYearTargets.Add(targets.Attribute, listTargets);
                        }
                        else
                        {
                            listTargets = (List<Targets>)hashYearTargets[targets.Attribute];
                            listTargets.Add(targets);
                        }
                    }
                }
            }

            strSelect = "SELECT ID_,ATTRIBUTE_,DEFICIENT,PERCENTDEFICIENT,CRITERIA,DEFICIENTNAME FROM " + cgOMS.Prefix + "DEFICIENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Accessing DEFICIENTS table. SQL message -" + exception.Message));

                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Accessing DEFICIENTS table: " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string id = row["ID_"].ToString();
                Targets targets = new Targets(id, cgOMS.Prefix + "DEFICIENTS");

                String strAttribute = row["ATTRIBUTE_"].ToString();
                String strDeficient = row["DEFICIENT"].ToString();
                String strPercentDeficient = row["PERCENTDEFICIENT"].ToString();
                string targetName = "";
                if (row["DEFICIENTNAME"] != DBNull.Value) targetName = row["DEFICIENTNAME"].ToString();
                targets.TargetName = targetName;

                if (strDeficient == "" || strPercentDeficient == "")
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: " + strAttribute + " - DEFICIENT and/or DEFICIENT PERCENT is missing.  If a deficiency attribute target is included a DEFICIENT level and a percent Allowed DEFICIENT must be entered."));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Error: " + strAttribute + " - DEFICIENT and/or DEFICIENT PERCENT is missing");
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }

                targets.Attribute = row["ATTRIBUTE_"].ToString();

                if (!m_listAttributes.Contains(targets.Attribute))
                {
                    m_listAttributes.Add(targets.Attribute);
                }

                targets.IsAllYears = true;
                targets.IsTargets = false;

                sDeficient = row["DEFICIENT"].ToString();
                if (sDeficient == "")
                {
                    targets.IsDeficient = false;
                }
                else
                {
                    targets.IsDeficient = true;
                    targets.Deficient = double.Parse(sDeficient);
                }

                sPercentage = row["PERCENTDEFICIENT"].ToString();
                if (sPercentage == "")
                {
                    targets.IsDeficiencyPercentage = false;
                }
                else
                {
                    targets.IsDeficiencyPercentage = true;
                    targets.DeficientPercentage = double.Parse(sPercentage);
                }

                sCriteria = row["CRITERIA"].ToString();
                if (sCriteria == "")
                {
                    targets.IsAllSections = true;
                }
                else
                {
                    byte[] assemblyCriteria = null;
                    assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "DEFICIENTS", "BINARY_CRITERIA", id, assemblyCriteria);
                    if (assemblyCriteria != null && assemblyCriteria.Length > 0)
                    {
                        targets.Criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                        if (targets.Criteria.Evaluate.OriginalInput != sCriteria)
                        {
                            targets.Criteria.Evaluate = null;
                        }
                    }

                    targets.Criteria.Criteria = sCriteria;

                    if (targets.Criteria.CriteriaAttributes == null)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: " + strAttribute + " - Contains an unidentified attribute. " + row[4].ToString()));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Error: " + strAttribute + " - Contains an unidentified attribute " + row[4].ToString());
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        return false;
                    }

                    foreach (String str in targets.Criteria.CriteriaAttributes)
                    {
                        if (!m_listAttributes.Contains(str))
                        {
                            m_listAttributes.Add(str);
                        }
                    }

                    targets.IsAllSections = false;
                }

                Hashtable hashYearTargets;
                List<Targets> listTargets;
                if (targets.IsAllYears)
                {
                    if (!TargetHash.Contains("All"))
                    {
                        listTargets = new List<Targets>();
                        listTargets.Add(targets);
                        hashYearTargets = new Hashtable();
                        hashYearTargets.Add(targets.Attribute, listTargets);
                        TargetHash.Add("All", hashYearTargets);
                    }
                    else
                    {
                        hashYearTargets = (Hashtable)TargetHash["All"];
                        if (!hashYearTargets.Contains(targets.Attribute))
                        {
                            listTargets = new List<Targets>();
                            listTargets.Add(targets);
                            hashYearTargets.Add(targets.Attribute, listTargets);
                        }
                        else
                        {
                            listTargets = (List<Targets>)hashYearTargets[targets.Attribute];
                            listTargets.Add(targets);
                        }
                    }
                }
            }
            return true;
        }

        public void ApplyDeterioration(int nYear)
        {
            try
            {
                foreach (Sections section in m_listSections)
                {
                    section.ResetSectionForNextYear();
                    section.AddRemainingLifeHash(nYear);
                    //Don't apply deterioration if a SplitTreatment(cashflow) project is partially complete
                    var applyDeterioration = true;
                    foreach(var committed in section.YearCommit)
                    {
                        if(!string.IsNullOrWhiteSpace(committed.SplitTreatmentId) && nYear == committed.Year)
                        {
                            applyDeterioration = false;
                        }
                    }

                    foreach (Deteriorate deteriorate in m_listDeteriorate)
                    {
                        //This calculates the base attribute value for next year.
                        //Calculates Base Benefit IF it is the BENEFIT variable.
                        //Calculates the Base Remaining life if a deficient level is entered.
                        if(applyDeterioration) section.ApplyDeteriorate(deteriorate, nYear);
                    }

                    //Determine the controlling RSL bin and set the values of each to the proper level
                    if (SimulationMessaging.Method.IsConditionalRSL)
                    {
                        foreach (Deteriorate deteriorate in m_listDeteriorate)
                        {
                            section.NormalizeConditionalRSL(deteriorate.Attribute, nYear);
                        }
                    }

                    //Move not deteriorating/performnace variables into the new year.
                    section.ApplyNonDeteriorate(nYear, m_Investment);
                }
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Applying deterioration curves." + e.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Applying deterioration curves: " + e.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                throw e;
            }
        }

        /// <summary>
        /// Single section Deterioration
        /// </summary>
        /// <param name="nYear">Current Year</param>
        /// <param name="strSectionID">Single section sectionID</param>
        public void ApplyDeterioration(int nYear, String strSectionID)
        {
            Sections section = m_listSections.Find((delegate (Sections s) { return s.SectionID == strSectionID; }));
            section.ResetSectionForNextYear();
            section.AddRemainingLifeHash(nYear);
            foreach (Deteriorate deteriorate in m_listDeteriorate)
            {
                //This calculates the base attribute value for next year.
                //Calculates Base Benefit IF it is the BENEFIT variable.
                //Calculates the Base Remaining life if a deficient level is entered.
                section.ApplyDeteriorate(deteriorate, nYear);
            }

            //Move not deteriorating/performnace variables into the new year.
            section.ApplyNonDeteriorate(nYear, m_Investment);
        }



        public void DetermineBenefitCostIterative(int nYear)
        {
            noTreatments = m_listTreatments.Find(t => t.Treatment.ToUpper() == "NO TREATMENT");
            bool bDeficient = false;
            bool bTarget = false;
            bool bMultipleYear = false;
            if (Method.TypeAnalysis.Contains("Multi"))
            {
                bMultipleYear = true;
            }

            if (Method.TypeBudget.Contains("Deficient"))
            {
                bDeficient = true;
            }

            if (Method.TypeBudget.Contains("Target"))
            {
                bTarget = true;
            }

            if (bTarget || bDeficient)
            {
                m_listApplyTreatment.Clear();
                foreach (String key in m_hashTargetSectionID.Keys)
                {
                    List<String> listID = (List<String>)m_hashTargetSectionID[key];
                    listID.Clear();
                }
                m_hashTargetSectionID.Clear();
                m_listTargets.Clear();
            }

            bool bAscending = SimulationMessaging.GetAttributeAscending(Method.BenefitAttribute);

            float fCost;
            //Create a new  CalculateBenefit (Treatment treatmentToEvaluate, List<Deteriorate> deterioratates, List<CalculatedAttribute> calculatedAttribute, List<Consequence> noTreatmentConsequences, List<Committed> futureProjects).
            // A consequence can never be set for a calculated field.

            String sFile;
            TextWriter tw = SimulationMessaging.CreateTextWriter("bc_" + m_strSimulationID + ".txt", out sFile);
            Hashtable nextAttributeValue = null;
            SimulationMessaging.NoTreatmentRemainingLife = new Dictionary<string, string>();

            var index = 0;

            foreach (Sections section in m_listSections)
            {
                if (index % 10 == 0)
                {
                    var percentage = 100 * Convert.ToDouble(index) / Convert.ToDouble(m_listSections.Count);
                    SimulationMessaging.AddMessage(new SimulationMessage("Determining benefit/cost " + percentage.ToString("0.#") + "% complete.", true));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Determining benefit / cost " + percentage.ToString("0.#") + "% complete");
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                }

                bool continueOnCommitted = false;
                foreach(var committed in section.YearCommit)
                {
                    if(committed.Year >= nYear)
                    {
                        if(!string.IsNullOrEmpty(committed.ScheduledTreatmentId))
                        {
                            continueOnCommitted  = true; //Don't calculate benefit cost for sections with FUTURE scheduled work.
                        }

                        if(!string.IsNullOrWhiteSpace(committed.SplitTreatmentId))
                        {
                            continueOnCommitted = true; //Dont'calculate benefit cost for split treatments.
                        }
                    }
                }
                if (continueOnCommitted) continue;
                index++;
                var noTreatmentRemainLifeHash = "";
                var noTreatment = m_listTreatments.Find(t => t.Treatment.ToUpper() == "NO TREATMENT");
                var noTreatmentBenefit = new CalculateBenefit(nYear, noTreatment, m_listDeteriorate, m_listCalculatedAttribute, noTreatment.ConsequenceList, section.YearCommit, m_dictionaryCommittedConsequences, m_dictionaryCommittedEquations, m_listTreatments);
                var baseBenefit = noTreatmentBenefit.Solve(section.m_hashNextAttributeValue, out nextAttributeValue, out noTreatmentRemainLifeHash);
                SimulationMessaging.NoTreatmentRemainingLife.Add(section.SectionID, noTreatmentRemainLifeHash);

                //Loop through treatments to get rid of treatments that are going to be superseded.
                var possibleBeforeSupersede = new List<Treatments>();
                var possibleAfterSupersede = new List<Treatments>();
                foreach (Treatments treatment in m_listTreatments)
                {
                    if (treatment.Treatment.ToUpper() == "NO TREATMENT")
                    {
                        continue;
                    }
                    if (treatment.IsTreatmentCriteriaMet(section.m_hashNextAttributeValue))
                    {
                        possibleBeforeSupersede.Add(treatment);
                        possibleAfterSupersede.Add(treatment);
                    }
                }
                //At this point apply the supersede rules and remove from possibleAfterSupersede
                foreach (var before in possibleBeforeSupersede)
                {
                    foreach (var supersede in before.Supersedes)
                    {
                        if (supersede.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue))
                        {
                            possibleAfterSupersede.RemoveAll(after => after.TreatmentID == supersede.SupersedeTreatmentId.ToString());
                        }
                    }
                }

                foreach (Treatments treatment in possibleAfterSupersede)
                {
                    int nTargetDeficient = 0;

                    if (treatment.IsTreatmentCriteriaMet(section.m_hashNextAttributeValue))
                    {
                        section.NumberTreatment++;
                        //Cost includes treatment and all scheduled treatments
                        fCost = GetTreatmentCost(section, treatment, nYear, out int cumulativeCostId, out float treatmentOnlyCost, out Dictionary<string, float> scheduledCost);

                        #region benefit

                        //Find consequences
                        String strChangeHash = "";
                        //Calculate new deterioration using the new hashtable
                        double dBenefit = 0;
                        double deltaBenefit = 0;

                        Hashtable hashRL = new Hashtable();
                        string strRLHash = "";
                        var treamentBenefit = new CalculateBenefit(nYear, treatment, m_listDeteriorate, m_listCalculatedAttribute, noTreatment.ConsequenceList, section.YearCommit, m_dictionaryCommittedConsequences, m_dictionaryCommittedEquations, m_listTreatments);

                        var benefit = treamentBenefit.Solve(section.m_hashNextAttributeValue, out nextAttributeValue, out strRLHash);

                        deltaBenefit = benefit - baseBenefit;

                        if (!bAscending && !SimulationMessaging.Method.IsRemainingLife)
                        {
                            deltaBenefit = deltaBenefit * -1;
                        }

                        double dBCRatio;

                        double dWeighting = 1;
                        if (m_strWeighting != "none" && m_strWeighting != "")
                        {
                            if (section.m_hashNextAttributeValue.Contains(m_strWeighting))
                            {
                                String strValue = section.m_hashNextAttributeValue[m_strWeighting].ToString();
                                double.TryParse(strValue, out dWeighting);
                            }
                        }

                        switch (Method.TypeAnalysis)
                        {
                            case "Maximum Benefit":
                            case "Multi-year Maximum Benefit":
                                dBCRatio = deltaBenefit;
                                break;

                            case "Remaining Life/Cost":
                            case "Multi-year Remaining Life/Cost":
                                if (fCost > 0)
                                {
                                    dBCRatio = deltaBenefit / (double)fCost;
                                }
                                else
                                {
                                    dBCRatio = 0;
                                }
                                break;

                            case "Conditional RSL/Cost":
                                if (fCost > 0)
                                {
                                    dBCRatio = deltaBenefit / (double)fCost;
                                }
                                else
                                {
                                    dBCRatio = 0;
                                }
                                break;

                            case "Maximum Remaining Life":
                            case "Multi-year Maximum Life":
                                dBCRatio = deltaBenefit;
                                break;

                            case "Incremental Benefit/Cost":
                            case "Multi-year Incremental Benefit/Cost":
                            case "Prioritized Needs":
                            default:

                                if (fCost > 0)
                                {
                                    dBCRatio = deltaBenefit / (double)fCost;
                                }
                                else
                                {
                                    dBCRatio = 0;
                                }
                                break;
                        }
                        dBCRatio *= dWeighting;

                        #endregion benefit

                        //Check each section here to see if is deficient
                        int nPrevious = nTargetDeficient;
                        if (bDeficient)
                        {
                            nTargetDeficient += IsSectionDeficient(section, nYear, nextAttributeValue);
                        }

                        //Does this section impact a target. (i.e. does its section current value hash meet a target criteria;
                        if (bTarget)
                        {
                            nTargetDeficient += IsSectionTarget(section, nYear);
                        }

                        //At this point have Benefit/RL, Base Benefit/RL, Cost, ConsquenceID
                        //Calculate B/C or RL/C or both (RL*B)/C
                        //Insert in BC table with Batch  Load.
                        String strOut = "";
                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                strOut = ":" + section.SectionID + ":"
                                    + nYear.ToString() + ":"
                                    + treatment.Treatment + ":"
                                    + treatment.AnyTreatment.ToString() + ":"
                                    + treatment.SameTreatment.ToString() + ":"
                                    + treatment.Budget + ":"
                                    + fCost.ToString() + ":"
                                    + deltaBenefit.ToString() + ":"
                                    + deltaBenefit.ToString() + ":"
                                    + dBCRatio.ToString() + ":"
                                    + treatment.TreatmentID.ToString() + ":"
                                    + nTargetDeficient.ToString() + ":"
                                    + strRLHash + ":"
                                    + strChangeHash + ":"
                                    + "0";
                                tw.WriteLine(strOut);
                                break;

                            case "ORACLE":
                                strOut = section.SectionID + ":"
                                    + nYear.ToString() + ":"
                                    + treatment.Treatment + ":"
                                    + treatment.AnyTreatment.ToString() + ":"
                                    + treatment.SameTreatment.ToString() + ":"
                                    + treatment.Budget + ":"
                                    + fCost.ToString() + ":"
                                    + deltaBenefit.ToString() + ":"
                                    + deltaBenefit.ToString() + ":"
                                    + dBCRatio.ToString() + ":"
                                    + treatment.TreatmentID.ToString() + ":"
                                    + nTargetDeficient.ToString() + ":"
                                    + strRLHash + ":"
                                    + strChangeHash + ":"
                                    + "0";
                                tw.Write(strOut);
                                tw.Write("#ORACLEENDOFLINE#");
                                break;

                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                                //break;
                        }

                        AppliedTreatment applyTreatment = new AppliedTreatment();
                        applyTreatment.SectionID = section.SectionID;
                        applyTreatment.Treatment = treatment.Treatment;
                        applyTreatment.TreatmentID = treatment.TreatmentID;
                        applyTreatment.Cost = fCost;
                        applyTreatment.Any = treatment.AnyTreatment;
                        applyTreatment.Same = treatment.SameTreatment;
                        applyTreatment.Budget = treatment.Budget;
                        applyTreatment.RemainingLife = deltaBenefit;
                        applyTreatment.Benefit = dBenefit;
                        applyTreatment.BenefitCostRatio = dBCRatio;
                        applyTreatment.NumberTreatmentDeficient = nTargetDeficient;
                        applyTreatment.RemainingLifeHash = strRLHash;
                        applyTreatment.SelectionWeight = (double)nTargetDeficient * dBCRatio;
                        applyTreatment.IsExclusive = treatment.OMSIsExclusive;
                        applyTreatment.TreatmentOnlyCost = treatmentOnlyCost;
                        applyTreatment.ScheduledCost = scheduledCost;
                        m_listApplyTreatment.Add(applyTreatment);

                        if (bMultipleYear)
                        {
                            applyTreatment.Year = nYear;
                            applyTreatment.Available = true;
                            applyTreatment.ChangeHash = strChangeHash;
                            m_listMultipleYearTreatment.Add(applyTreatment);
                        }
                    }
                }
            }
            //SimulationMessaging.AddMessage(new SimulationMessage("Closing TW for BC ratios..."));
            tw.Close();
            //SimulationMessaging.AddMessage(new SimulationMessage("Closed TW for BC ratios..."));

            _spanAnalysis += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;

            String strTable = "BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID;
            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(strTable, sFile, ':');
                    break;

                case "ORACLE":
                    List<string> columnNames = new List<string>();
                    //columnNames.Add( "ID" );
                    columnNames.Add("SECTIONID");
                    columnNames.Add("YEARS");
                    columnNames.Add("TREATMENT");
                    columnNames.Add("YEARSANY");
                    columnNames.Add("YEARSSAME");
                    columnNames.Add("BUDGET");
                    columnNames.Add("COST_");
                    columnNames.Add("REMAINING_LIFE");
                    columnNames.Add("BENEFIT");
                    columnNames.Add("BC_RATIO");
                    columnNames.Add("CONSEQUENCEID");
                    columnNames.Add("DEFICIENT");
                    columnNames.Add("RLHASH");
                    columnNames.Add("CHANGEHASH");
                    DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, strTable, sFile, columnNames, ":", " \"str '#ORACLEENDOFLINE#'\"");
                    SimulationMessaging.AddMessage(new SimulationMessage("Bulk Load for BC Complete..."));
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }

            _spanReport += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;
        }

        public void DetermineBenefitCost(int nYear, String strSectionID)
        {
            bool bDeficient = false;
            bool bTarget = false;
            if (Method.TypeBudget.Contains("Deficient"))
            {
                bDeficient = true;
            }

            if (Method.TypeBudget.Contains("Target"))
            {
                bTarget = true;
            }

            if (bTarget || bDeficient)
            {
                m_listApplyTreatment.Clear();
                foreach (String key in m_hashTargetSectionID.Keys)
                {
                    List<String> listID = (List<String>)m_hashTargetSectionID[key];
                    listID.Clear();
                }
                m_hashTargetSectionID.Clear();
                m_listTargets.Clear();
            }

            float fCost;
            float fDefaultCost = 0;
            float fMostCost = 0;
            float totalCost = 0;
            Sections section = m_listSections.Find((delegate (Sections s) { return s.SectionID == strSectionID; }));

            foreach (Treatments treatment in m_listTreatments)
            {
                int nTargetDeficient = 0;
                if (treatment.Treatment.ToUpper() == "NO TREATMENT")
                {
                    noTreatments = treatment;
                    continue;
                }

                if (treatment.IsTreatmentCriteriaMet(section.m_hashNextAttributeValue))
                {
                    section.NumberTreatment++;

                    #region cost

                    //Find cost.
                    fCost = -1;
                    fDefaultCost = 0;
                    fMostCost = 0;
                    totalCost = 0;
                    foreach (Costs cost in treatment.CostList)
                    {
                        if (cost.Default)
                        {
                            if (cost._attributesEquation.Contains("LENGTH"))
                            {
                                if (!section.m_hashNextAttributeValue.Contains("LENGTH"))
                                    section.m_hashNextAttributeValue.Add("LENGTH", section.Length);
                            }
                            if (cost._attributesEquation.Contains("AREA"))
                            {
                                if (section.m_hashNextAttributeValue.Contains("AREA"))
                                {
                                    section.m_hashNextAttributeValue.Remove("AREA");
                                }
                                section.m_hashNextAttributeValue.Add("AREA", section.Area);
                            }
                            fDefaultCost = (float)cost.GetCost(section.m_hashNextAttributeValue);
                            if (Method.UseCumulativeCost)
                            {
                                totalCost += fDefaultCost;
                            }

                            //fDefaultCost = cost.Cost;
                        }
                        else
                        {
                            if (cost.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue))
                            {
                                if (fCost >= 0 && !Method.UseCumulativeCost)
                                {
                                    SimulationMessaging.AddMessage(new SimulationMessage("Muliple cost criteria met for " + treatment.Treatment + " for section " + section.Facility + " " + section.Section + ".  The more expensive (conservative)is used."));
                                    if (APICall.Equals(true))
                                    {
                                        var updateStatus = Builders<SimulationModel>.Update
                                        .Set(s => s.status, "Muliple cost criteria met for " + treatment.Treatment + " for section " + section.Facility + " " + section.Section);
                                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                                    }
                                }
                                if (cost._attributesEquation.Contains("LENGTH"))
                                {
                                    if (!section.m_hashNextAttributeValue.Contains("LENGTH"))
                                        section.m_hashNextAttributeValue.Add("LENGTH", section.Length);
                                }
                                if (cost._attributesEquation.Contains("AREA"))
                                {
                                    if (section.m_hashNextAttributeValue.Contains("AREA"))
                                    {
                                        section.m_hashNextAttributeValue.Remove("AREA");
                                    }
                                    section.m_hashNextAttributeValue.Add("AREA", section.Area);
                                }
                                fCost = (float)cost.GetCost(section.m_hashNextAttributeValue);
                                // fCost = cost.Cost;
                                if (Method.UseCumulativeCost)
                                {
                                    totalCost += fCost;
                                }
                                else
                                {
                                    if (fCost > fMostCost)
                                    {
                                        fMostCost = fCost;
                                    }
                                }
                            }
                        }
                    }

                    //If cost is not set use default cost.
                    if (Method.UseCumulativeCost)
                    {
                        fCost = totalCost;
                    }
                    else
                    {
                        if (fCost <= 0) fCost = fDefaultCost;
                        else fCost = fMostCost; //Otherwise use most expensive (conservative).
                    }
                    if (section.Area <= 0)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Warning:Facility(" + section.Facility + ")Section(" + section.Section + ") AREA is equal to zero and Benefit/Cost is infinite. Section ignored."));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Warning:Facility(" + section.Facility + ")Section(" + section.Section + ") AREA is 0 and Benefit/Cost is infinite. Section ignored");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        continue;
                    }
                    else
                    {
                        fCost = fCost / section.Area;
                    }

                    #endregion cost

                    #region benefit

                    //Find consequences
                    String strChangeHash;
                    Hashtable hash = ApplyConsequences(section.m_hashNextAttributeValue, treatment.TreatmentID, out strChangeHash, section);

                    //Calculate new deterioration using the new hashtable
                    double dBenefit = 0;
                    double deltaBenefit = 0;
                    double dRemainingLife = 100;
                    double deltaRemainingLife = 0;
                    Hashtable hashRL = new Hashtable();

                    // Need to do this loop twice. First for non-default and then default.
                    foreach (Deteriorate deteriorate in m_listDeteriorate)
                    {
                        if (deteriorate.Default) continue;

                        if (deteriorate.IsCriteriaMet(hash))
                        {
                            if (Method.IsBenefitCost && Method.BenefitAttribute == deteriorate.Attribute)
                            {
                                dBenefit = deteriorate.CalculateBenefit(hash);
                            }
                            if (Method.IsRemainingLife)
                            {
                                double dRL = 0;

                                if (deteriorate.CalculateRemainingLife(hash, hash, out dRL))
                                {
                                    if (hashRL.Contains(deteriorate.Attribute))
                                    {
                                        double dRLOld = (double)hashRL[deteriorate.Attribute];
                                        if (dRLOld > dRL)
                                        {
                                            hashRL.Remove(deteriorate.Attribute);
                                            hashRL.Add(deteriorate.Attribute, dRL);
                                            if (dRL < dRemainingLife)
                                            {
                                                dRemainingLife = dRL;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hashRL.Add(deteriorate.Attribute, dRL);
                                        if (dRL < dRemainingLife)
                                        {
                                            dRemainingLife = dRL;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Need to do this loop twice. First for non-default and then default.
                    foreach (Deteriorate deteriorate in m_listDeteriorate)
                    {
                        if (hashRL.Contains(deteriorate.Attribute)) continue;//Already calculated for a non-default
                        if (deteriorate.IsCriteriaMet(hash))
                        {
                            if (Method.IsBenefitCost && Method.BenefitAttribute == deteriorate.Attribute)
                            {
                                dBenefit = deteriorate.CalculateBenefit(hash);
                            }
                            if (Method.IsRemainingLife)
                            {
                                double dRL = 0;

                                if (deteriorate.CalculateRemainingLife(hash, hash, out dRL))
                                {
                                    if (!hashRL.Contains(deteriorate.Attribute))
                                    {
                                        hashRL.Add(deteriorate.Attribute, dRL);
                                    }
                                    else
                                    {
                                        double dRLOld = (double)hashRL[deteriorate.Attribute];
                                        if (dRL < dRLOld)
                                        {
                                            hashRL.Remove(deteriorate.Attribute);
                                            hashRL.Add(deteriorate.Attribute, dRL);
                                        }
                                    }
                                    if (dRL < dRemainingLife)
                                    {
                                        dRemainingLife = dRL;
                                    }
                                }
                            }
                        }
                    }

                    //Subtract off the base remaining life change.
                    if (dRemainingLife < 0) dRemainingLife = 0;
                    if (section.RemainingLife < 0) section.RemainingLife = 0;

                    deltaRemainingLife = dRemainingLife - section.RemainingLife;
                    deltaBenefit = dBenefit - section.BaseBenefit;

                    String strRLHash = this.CreateRemainingLifeHashString(hashRL);
                    double dBCRatio;

                    double dWeighting = 1;
                    if (m_strWeighting != "none" && m_strWeighting != "")
                    {
                        if (hash.Contains(m_strWeighting))
                        {
                            String strValue = hash[m_strWeighting].ToString();
                            double.TryParse(strValue, out dWeighting);
                        }
                    }

                    switch (Method.TypeAnalysis)
                    {
                        case "Maximum Benefit":
                        case "Multi-year Maximum Benefit":
                            dBCRatio = deltaBenefit;
                            break;

                        case "Remaining Life/Cost":
                        case "Multi-year Remaining Life/Cost":
                            if (fCost > 0)
                            {
                                dBCRatio = deltaRemainingLife / (double)fCost;
                            }
                            else
                            {
                                dBCRatio = 0;
                            }

                            break;

                        case "Maximum Remaining Life":
                        case "Multi-year Maximum Life":
                            dBCRatio = deltaRemainingLife;
                            break;

                        case "Incremental Benefit/Cost":
                        case "Multi-year Incremental Benefit/Cost":
                        case "Prioritized Needs":
                        default:

                            if (fCost > 0)
                            {
                                dBCRatio = deltaBenefit / (double)fCost;
                            }
                            else
                            {
                                dBCRatio = 0;
                            }
                            break;
                    }
                    dBCRatio *= dWeighting;

                    #endregion benefit

                    //Check each section here to see if is deficient
                    int nPrevious = nTargetDeficient;
                    if (bDeficient)
                    {
                        nTargetDeficient += IsSectionDeficient(section, nYear, hash);
                    }

                    //Does this section impact a target. (i.e. does its section current value hash meet a target criteria;
                    if (bTarget)
                    {
                        nTargetDeficient += IsSectionTarget(section, nYear); ;
                    }

                    //At this point have Benefit/RL, Base Benefit/RL, Cost, ConsquenceID
                    //Calculate B/C or RL/C or both (RL*B)/C
                    //Insert in BC table with Batch  Load.
                    //String strOut = "," + section.SectionID + ","
                    //                + nYear.ToString() + ","
                    //                + treatment.Treatment + ","
                    //                + treatment.AnyTreatment.ToString() + ","
                    //                + treatment.SameTreatment.ToString() + ","
                    //                + treatment.Budget + ","
                    //                + fCost.ToString() + ","
                    //                + deltaRemainingLife.ToString() + ","
                    //                + deltaBenefit.ToString() + ","
                    //                + dBCRatio.ToString() + ","
                    //                + treatment.TreatmentID.ToString() + ","
                    //                + nTargetDeficient.ToString() + ","
                    //                + strRLHash + ","
                    //                + strChangeHash;

                    String strInsert = "INSERT INTO BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID + " (SECTIONID,YEARS,TREATMENT,YEARSSAME,YEARSANY,BUDGET,COST_,REMAINING_LIFE,BENEFIT,BC_RATIO,CONSEQUENCEID,DEFICIENT,RLHASH,CHANGEHASH) VALUES('";
                    strInsert += section.SectionID + "','";
                    strInsert += nYear.ToString() + "','";
                    strInsert += treatment.Treatment + "','";
                    strInsert += treatment.AnyTreatment.ToString() + "','";
                    strInsert += treatment.SameTreatment.ToString() + "','";
                    strInsert += treatment.Budget + "','";
                    strInsert += fCost.ToString() + "','";
                    strInsert += deltaRemainingLife.ToString() + "','";
                    strInsert += deltaBenefit.ToString() + "','";
                    strInsert += dBCRatio.ToString() + "','";
                    strInsert += treatment.TreatmentID.ToString() + "','";
                    strInsert += nTargetDeficient.ToString() + "','";
                    strInsert += strRLHash + "','";
                    strInsert += strChangeHash + "')"; ;

                    DBMgr.ExecuteNonQuery(strInsert);

                    if (bTarget || bDeficient)
                    {
                        if (nTargetDeficient != nPrevious)
                        {
                            AppliedTreatment applyTreatment = new AppliedTreatment();
                            applyTreatment.SectionID = section.SectionID;
                            applyTreatment.Treatment = treatment.Treatment;
                            applyTreatment.TreatmentID = treatment.TreatmentID;
                            applyTreatment.Cost = fCost;
                            applyTreatment.Any = treatment.AnyTreatment;
                            applyTreatment.Same = treatment.SameTreatment;
                            applyTreatment.Budget = treatment.Budget;
                            applyTreatment.RemainingLife = dRemainingLife;
                            applyTreatment.Benefit = dBenefit;
                            applyTreatment.BenefitCostRatio = dBCRatio;
                            applyTreatment.NumberTreatmentDeficient = nTargetDeficient;
                            applyTreatment.RemainingLifeHash = strRLHash;
                            applyTreatment.SelectionWeight = (double)nTargetDeficient * dBCRatio;
                            applyTreatment.IsExclusive = treatment.OMSIsExclusive;

                            m_listApplyTreatment.Add(applyTreatment);
                        }
                    }
                }
            }
        }

        private String CreateRemainingLifeHashString(Hashtable hashRL)
        {
            String strReturn = "";
            String sValue;
            foreach (String key in hashRL.Keys)
            {
                double d = Convert.ToDouble(hashRL[key]);
                sValue = d.ToString("0.0");

                strReturn += key + "\t" + sValue + "\n";
            }
            return strReturn;
        }

        /// <summary>
        /// Store committed projects in each section. Allow for pre-shadow.
        /// </summary>
        /// <returns></returns>
        private bool FillCommittedProjects()
        {
            String strSelect = "SELECT SECTIONID,YEARS,TREATMENTNAME,YEARSAME,YEARANY,BUDGET,COST_,COMMITID";
            if (SimulationMessaging.IsOMS)
            {
                strSelect += ", OMS_IS_EXCLUSIVE, OMS_IS_NOT_ALLOWED";
            }

            strSelect += " FROM " + cgOMS.Prefix + "COMMITTED_ WHERE SIMULATIONID ='" + m_strSimulationID + "' ORDER BY SECTIONID, YEARS";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Sections section = m_listSections.Find((delegate (Sections s) { return s.SectionID == row["SECTIONID"].ToString(); }));
                if (section == null) continue;
                Committed commit = new Committed();
                commit.Consequence = new Consequences();
                commit.Year = int.Parse(row[1].ToString());
                commit.Treatment = row[2].ToString();
               
                commit.Same = int.Parse(row[3].ToString());
                commit.Any = int.Parse(row[4].ToString());
                commit.Budget = row[5].ToString();
                commit.Cost = float.Parse(row[6].ToString());
                //Store the ID to the committed project, perform the select to get the Consequence when needed.  Saves storage space.
                commit.ConsequenceID = row[7].ToString();
                commit.IsRepeat = false;
                commit.OMSIsNotAllowed = false;
                commit.OMSIsExclusive = false;

                section.AnyYear = commit.Any;
                SameTreatment sameTreatment = new SameTreatment();
                sameTreatment.strTreatment = commit.Treatment;
                sameTreatment.nYear = commit.Year + commit.Same;
                section.m_listSame.Add(sameTreatment);
                
                

                section.YearCommit.Add(commit);
            }
            
            LoadCommittedEquations(m_strSimulationID);

            return true;
        }

        private void AddRepeatActivitiesAsCommitted()
        {
            int startYear = Investment.StartYear;
            int numberYears = Investment.AnalysisPeriod;

            foreach (Treatments treatment in m_listTreatments)
            {
                if (treatment.OMSIsRepeat)
                {
                    for (int year = treatment.OMSRepeatStart; year < startYear + numberYears; year += treatment.OMSRepeatInterval)
                    {
                        Committed commit = new Committed();
                        commit.Consequence = new Consequences();
                        commit.Treatment = treatment.Treatment;
                        commit.Same = 0;
                        commit.Any = 0;
                        commit.Budget = treatment.Budget;
                        commit.OMSTreatment = treatment;
                        if (treatment.ConsequenceList.Count > 0)
                        {
                            commit.Consequence = treatment.ConsequenceList[0];
                        }
                        else
                        {
                            commit.Consequence = new Consequences();
                        }
                        commit.IsRepeat = true;
                        commit.Year = year;
                        foreach (Sections section in m_listSections)
                        {
                            Committed previous = null;
                            if (section.YearCommit != null)
                            {
                                previous = section.YearCommit.Find(delegate (Committed c) { return c.Treatment == commit.Treatment && c.Year == commit.Year; });
                            }
                            if (previous == null)
                            {
                                section.YearCommit.Add(commit);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Store committed projects for single section run
        /// </summary>
        /// <returns></returns>
        private bool FillCommittedProjects(String strSectionID)
        {
            String strSelect = "SELECT SECTIONID,YEARS,TREATMENTNAME,YEARSAME,YEARANY,BUDGET,COST_,COMMITID FROM " + cgOMS.Prefix + "COMMITTED_ WHERE SIMULATIONID ='" + m_strSimulationID + "' AND SECTIONID='" + strSectionID + "' ORDER BY YEARS";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Sections section = m_listSections.Find((delegate (Sections s) { return s.SectionID == row["SECTIONID"].ToString(); }));
                    if (section == null) continue;
                    Committed commit = new Committed();
                    commit.Consequence = new Consequences();
                    commit.Treatment = row[2].ToString();
                    commit.Same = int.Parse(row[3].ToString());
                    commit.Any = int.Parse(row[4].ToString());
                    commit.Budget = row[5].ToString();
                    commit.Cost = float.Parse(row[6].ToString());
                    //Store the ID to the committed project, perform the select to get the Consequence when needed.  Saves storage space.
                    commit.ConsequenceID = row[7].ToString();
                    commit.Year = int.Parse(row[1].ToString());
                    section.YearCommit.Add(commit);
                }
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Loading single section committed project. " + exception.Message));
                return false;
            }
            LoadCommittedEquations(m_strSimulationID, strSectionID);
            return true;
        }

        /// <summary>
        /// Determines if this section is included in a targe
        /// </summary>
        /// <param name="section"></param>
        /// <param name="nYear"></param>
        /// <returns></returns>
        private int IsSectionTarget(Sections section, int nYear)
        {
            int nTargets = 0;
            //Loop through Targets that are full all years.
            Hashtable hashTargetsValues;

            if (TargetHash.Contains("All"))
            {
                hashTargetsValues = (Hashtable)this.TargetHash["All"];
                foreach (String key in hashTargetsValues.Keys)
                {
                    List<Targets> listTarget = (List<Targets>)hashTargetsValues[key];
                    foreach (Targets target in listTarget)
                    {
                        if (target.IsTargets)
                        {
                            if (target.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue)) //This target has a deficiency criteria.
                            {
                                List<String> list;
                                if (!m_listTargets.Contains(target)) m_listTargets.Add(target);
                                if (!m_hashTargetSectionID.Contains(target.ID))
                                {
                                    list = new List<String>();
                                    m_hashTargetSectionID.Add(target.ID, list);
                                    //m_listTargets.Add(target);
                                }
                                else
                                {
                                    list = (List<String>)m_hashTargetSectionID[target.ID];
                                }
                                if (!list.Contains(section.SectionID.ToString()))
                                {
                                    list.Add(section.SectionID.ToString());
                                }
                                nTargets++;
                            }
                        }
                    }
                }
            }

            if (TargetHash.Contains(nYear.ToString()))
            {
                hashTargetsValues = (Hashtable)this.TargetHash[nYear.ToString()];
                foreach (String key in hashTargetsValues.Keys)
                {
                    List<Targets> listTarget = (List<Targets>)hashTargetsValues[key];
                    foreach (Targets target in listTarget)
                    {
                        if (target.IsTargets)
                        {
                            if (target.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue)) //This target has a deficiency criteria.
                            {
                                List<String> list;
                                if (!m_listTargets.Contains(target)) m_listTargets.Add(target);
                                if (!m_hashTargetSectionID.Contains(target.ID))
                                {
                                    list = new List<String>();
                                    m_hashTargetSectionID.Add(target.ID, list);
                                    //m_listTargets.Add(target);
                                }
                                else
                                {
                                    list = (List<String>)m_hashTargetSectionID[target.ID];
                                }
                                if (!list.Contains(section.SectionID.ToString()))
                                {
                                    list.Add(section.SectionID.ToString());
                                }
                                nTargets++;
                            }
                        }
                    }
                }
            }
            return nTargets;
        }

        /// <summary>
        /// Determines if this section is deficient
        /// </summary>
        /// <param name="section"></param>
        /// <param name="nYear"></param>
        /// <returns></returns>
        private int IsSectionDeficient(Sections section, int nYear, Hashtable hashAfterTreatment)
        {
            int nDeficient = 0;
            //Loop through Targets that are full all years.
            Hashtable hashTargetsValues;

            if (TargetHash.Contains("All"))
            {
                hashTargetsValues = (Hashtable)this.TargetHash["All"];
                foreach (String key in hashTargetsValues.Keys)
                {
                    List<Targets> listTarget = (List<Targets>)hashTargetsValues[key];
                    foreach (Targets target in listTarget)
                    {
                        if (target.IsDeficient)
                        {
                            if (target.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue)) //This target has a deficiency criteria.
                            {
                                double dValue = double.Parse(section.m_hashNextAttributeValue[target.Attribute].ToString());
                                double dValueAfterTreatment = double.Parse(hashAfterTreatment[target.Attribute].ToString());// Makes sure treatment fixes problem
                                bool bAscending = SimulationMessaging.GetAttributeAscending(target.Attribute);

                                if (bAscending && dValue < target.Deficient && dValueAfterTreatment >= target.Deficient)
                                {
                                    List<String> list;
                                    if (!m_listTargets.Contains(target)) m_listTargets.Add(target);
                                    if (!m_hashTargetSectionID.Contains(target.ID))
                                    {
                                        list = new List<String>();
                                        m_hashTargetSectionID.Add(target.ID, list);
                                        if (!m_listTargets.Contains(target))
                                        {
                                            m_listTargets.Add(target);
                                        }
                                    }
                                    else
                                    {
                                        list = (List<String>)m_hashTargetSectionID[target.ID];
                                    }
                                    if (!list.Contains(section.SectionID.ToString()))
                                    {
                                        list.Add(section.SectionID.ToString());
                                    }
                                    nDeficient++;
                                }
                                else if (!bAscending && dValue > target.Deficient && dValueAfterTreatment <= target.Deficient)
                                {
                                    List<String> list;
                                    if (!m_listTargets.Contains(target)) m_listTargets.Add(target);
                                    if (!m_hashTargetSectionID.Contains(target.ID))
                                    {
                                        list = new List<String>();
                                        m_hashTargetSectionID.Add(target.ID, list);
                                        if (!m_listTargets.Contains(target))
                                        {
                                            m_listTargets.Add(target);
                                        }
                                    }
                                    else
                                    {
                                        list = (List<String>)m_hashTargetSectionID[target.ID];
                                    }
                                    if (!list.Contains(section.SectionID.ToString()))
                                    {
                                        list.Add(section.SectionID.ToString());
                                    }
                                    nDeficient++;
                                }
                            }
                        }
                    }
                }
            }

            if (TargetHash.Contains(nYear.ToString()))
            {
                hashTargetsValues = (Hashtable)this.TargetHash[nYear.ToString()];
                foreach (String key in hashTargetsValues.Keys)
                {
                    List<Targets> listTarget = (List<Targets>)hashTargetsValues[key];
                    foreach (Targets target in listTarget)
                    {
                        if (target.IsDeficient)
                        {
                            if (target.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue)) //This target has a deficiency criteria.
                            {
                                double dValue = double.Parse(section.m_hashNextAttributeValue[target.Attribute].ToString());
                                bool bAscending = SimulationMessaging.GetAttributeAscending(target.Attribute);

                                if (bAscending && dValue < target.Deficient)
                                {
                                    List<String> list;
                                    if (!m_hashTargetSectionID.Contains(target.ID))
                                    {
                                        list = new List<String>();
                                        m_hashTargetSectionID.Add(target.ID, list);
                                    }
                                    else
                                    {
                                        list = (List<String>)m_hashTargetSectionID[target.ID];
                                    }
                                    if (!list.Contains(section.SectionID.ToString()))
                                    {
                                        list.Add(section.SectionID.ToString());
                                    }
                                    nDeficient++;
                                }
                                else if (!bAscending && dValue > target.Deficient)
                                {
                                    List<String> list;
                                    if (!m_hashTargetSectionID.Contains(target.ID))
                                    {
                                        list = new List<String>();
                                        m_hashTargetSectionID.Add(target.ID, list);
                                    }
                                    else
                                    {
                                        list = (List<String>)m_hashTargetSectionID[target.ID];
                                    }
                                    if (!list.Contains(section.SectionID.ToString()))
                                    {
                                        list.Add(section.SectionID.ToString());
                                    }
                                    nDeficient++;
                                }
                            }
                        }
                    }
                }
            }
            return nDeficient;
        }

        /// <summary>
        /// After application of Deterioration, this calculates the Network
        /// Target and deficiency base.
        /// </summary>
        /// <param name="nYear">Year for Network targets</param>
        private void DetermineTargetAndDeficient(int nYear)
        {
            foreach (Sections section in m_listSections)
            {
                foreach (String key in TargetHash.Keys)
                {
                    //Get list of targets for All years or this Year.
                    if (key == "All" || key == nYear.ToString())
                    {
                        // Key attribute Object list of targets for that variable
                        Hashtable hashTargetValues = (Hashtable)TargetHash[key];
                        foreach (String strAttribute in hashTargetValues.Keys)
                        {
                            List<Targets> listTargets = (List<Targets>)hashTargetValues[strAttribute];

                            //Now go over every target and get total area (and deficient area)
                            foreach (Targets target in listTargets)
                            {
                                //Check if for all sections (criteria blank).  If so just add to total area.
                                if (target.IsAllSections || target.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue))
                                {
                                    double dValue;
                                    if (section.Treated)//Already committed
                                    {
                                        Hashtable hashAttributeValue = (Hashtable)section.m_hashYearAttributeValues[nYear];
                                        dValue = double.Parse(hashAttributeValue[target.Attribute].ToString());
                                    }
                                    else
                                    {
                                        dValue = double.Parse(section.m_hashNextAttributeValue[target.Attribute].ToString());
                                    }

                                    target.AddArea(section.Area, nYear);
                                    if (target.IsTargets)
                                    {
                                        target.AddTargetArea(section.Area, dValue, nYear);
                                    }

                                    if (target.IsDeficient)
                                    {
                                        if (SimulationMessaging.GetAttributeAscending(target.Attribute))
                                        {
                                            //This section is deficient for this criteria
                                            if (target.Deficient > dValue)
                                            {
                                                target.AddDeficientArea(section.Area, nYear);
                                            }
                                        }
                                        else
                                        {
                                            //This section is deficient for this criteria
                                            if (target.Deficient < dValue)
                                            {
                                                target.AddDeficientArea(section.Area, nYear);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// After application of Deterioration, this calculates the Network
        /// Target and deficiency base for Single Section run.
        /// </summary>
        /// <param name="nYear">Year for Network targets</param>
        private void DetermineTargetAndDeficient(int nYear, String strSectionID)
        {
            foreach (Sections section in m_listSections)
            {
                Hashtable hashAttributeValue;
                if (section.m_hashYearAttributeValues.Contains(nYear) && section.SectionID != strSectionID)
                {
                    hashAttributeValue = (Hashtable)section.m_hashYearAttributeValues[nYear];
                }
                else
                {
                    hashAttributeValue = section.m_hashNextAttributeValue;
                }

                foreach (String key in TargetHash.Keys)
                {
                    //Get list of targets for All years or this Year.
                    if (key == "All" || key == nYear.ToString())
                    {
                        // Key attribute Object list of targets for that variable
                        Hashtable hashTargetValues = (Hashtable)TargetHash[key];
                        foreach (String strAttribute in hashTargetValues.Keys)
                        {
                            List<Targets> listTargets = (List<Targets>)hashTargetValues[strAttribute];

                            //Now go over every target and get total area (and deficient area)
                            foreach (Targets target in listTargets)
                            {
                                //Check if for all sections (criteria blank).  If so just add to total area.
                                if (target.IsAllSections || target.Criteria.IsCriteriaMet(hashAttributeValue))
                                {
                                    double dValue = double.Parse(hashAttributeValue[target.Attribute].ToString());
                                    target.AddArea(section.Area, nYear);
                                    if (target.IsTargets)
                                    {
                                        target.AddTargetArea(section.Area, dValue, nYear);
                                    }

                                    if (target.IsDeficient)
                                    {
                                        if (SimulationMessaging.GetAttributeAscending(target.Attribute))
                                        {
                                            //This section is deficient for this criteria
                                            if (target.Deficient > dValue)
                                            {
                                                target.AddDeficientArea(section.Area, nYear);
                                            }
                                        }
                                        else
                                        {
                                            //This section is deficient for this criteria
                                            if (target.Deficient < dValue)
                                            {
                                                target.AddDeficientArea(section.Area, nYear);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ApplyCommitted(int nYear)
        {
            String strBudget = "";
            float fAmount = 0;
            String sOutFile;
            TextWriter tw = SimulationMessaging.CreateTextWriter("report_" + m_strSimulationID + ".csv", out sOutFile);

            foreach (Sections section in m_listSections)
            {
                List<Committed> currentYearCommitted = section.YearCommit.FindAll(delegate (Committed c) { return c.Year == nYear; });
                currentYearCommitted.Sort(delegate (Committed c1, Committed c2) { return c2.OMSIsNotAllowed.CompareTo(c1.OMSIsNotAllowed); });
                foreach (Committed commit in currentYearCommitted)
                {
                    if (!section.IsOMSCommitAllowed(commit.Treatment, nYear)) continue;

                    String strChangeHash = "";
                    Hashtable hashOutput = null;//  = section.CommitProject(commit, out strBudget, out fAmount, m_dictionaryCommittedEquations, out strChangeHash);
                    Hashtable hashInput = (Hashtable)section.m_hashNextAttributeValue;

                    //If this is not blank or null, this committed project is a Scheduled d
                    if (!string.IsNullOrWhiteSpace(commit.ScheduledTreatmentId))
                    {
                        hashOutput = ApplyConsequences(hashInput, commit.ScheduledTreatmentId, out strChangeHash, section);
                        var scheduledTreatment =
                            m_listTreatments.Find(t => t.TreatmentID == commit.ScheduledTreatmentId);
                        if (scheduledTreatment != null)
                        {
                            //Amount includes treatment and all scheduled treatments
                            fAmount = GetTreatmentCost(section, scheduledTreatment, nYear, out int cumulativeCostId,out float treatmentOnlyCost, out Dictionary<string, float> scheduledCost) * section.Area;
                            section.AnyYear = nYear + scheduledTreatment.AnyTreatment;
                            var same = new SameTreatment();
                            same.strTreatment = scheduledTreatment.Treatment;
                            same.nYear = nYear;
                            section.m_listSame.Add(same);
                            commit.Treatment = scheduledTreatment.Treatment;
                            commit.Any = scheduledTreatment.AnyTreatment;
                        }
                    }
                    else if(!string.IsNullOrWhiteSpace(commit.SplitTreatmentId))
                    {
                        if (commit.YearSplitTreatmentComplete == nYear)
                        {
                            hashOutput = ApplyConsequences(hashInput, commit.SplitTreatmentId, out strChangeHash, section);
                        }
                        else
                        {
                            hashOutput = hashInput;
                        }
                        fAmount = commit.Cost;
                    }
                    else//Otherwise just a committed project.
                    {
                        hashOutput = ApplyCommittedConsequences(hashInput, commit, out strChangeHash);
                        fAmount = commit.Cost;
                        section.AnyYear = nYear + commit.Any;
                    }

                    strBudget = commit.Budget;
                    section.Treated = true;

                    //Apply calculated fields.
                    hashOutput = ApplyCalculatedFields(hashOutput);

                    if (section.m_hashYearAttributeValues.Contains(nYear))
                    {
                        section.m_hashYearAttributeValues.Remove(nYear);
                    }
                    section.m_hashYearAttributeValues.Add(nYear, hashOutput);

                    //Calculate remaining life and incremental BC for the new values.
                    //Calculate new deterioration using the new hashtable
                    double dBenefit = 0;
                    double dRemainingLife = 100;
                    Hashtable hashRL = new Hashtable();

                    //Hashtable hash = (Hashtable)section.m_hashYearAttributeValues[nYear];
                    foreach (Deteriorate deteriorate in m_listDeteriorate)
                    {
                        if (SimulationMessaging.Method.IsConditionalRSL)
                        {
                            if (section.NormalizedConditionalRSLs.ContainsKey(deteriorate.Attribute))
                            {
                                double normalized = section.NormalizedConditionalRSLs[deteriorate.Attribute];
                                double extension = deteriorate.CalculateExtension(hashOutput, normalized);
                                if (hashRL.Contains(deteriorate.Attribute))
                                {
                                    double dRLOld = (double)hashRL[deteriorate.Attribute];
                                    if (dRLOld > extension)
                                    {
                                        hashRL.Remove(deteriorate.Attribute);
                                        hashRL.Add(deteriorate.Attribute, extension);
                                        if (extension < dRemainingLife)
                                        {
                                            dRemainingLife = extension;
                                        }
                                    }
                                }
                                else
                                {
                                    hashRL.Add(deteriorate.Attribute, extension);
                                    if (extension < dRemainingLife)
                                    {
                                        dRemainingLife = extension;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (deteriorate.IsCriteriaMet(hashOutput))
                            {
                                bool bOutOfRange;
                                deteriorate.IterateOneYear(hashOutput, out bOutOfRange);
                                //Need to add new CalculateBenefit here.
                                if (Method.IsBenefitCost && Method.BenefitAttribute == deteriorate.Attribute)
                                {
                                    dBenefit = deteriorate.CalculateBenefit(hashOutput);
                                }

                                double dRL = 0;
                                if (deteriorate.CalculateRemainingLife(hashOutput, hashInput, out dRL))
                                {
                                    if (!hashRL.Contains(deteriorate.Attribute))
                                    {
                                        hashRL.Add(deteriorate.Attribute, dRL);
                                    }
                                    else
                                    {
                                        double dRLOld = (double)hashRL[deteriorate.Attribute];
                                        if (dRL < dRLOld)
                                        {
                                            hashRL.Remove(deteriorate.Attribute);
                                            hashRL.Add(deteriorate.Attribute, dRL);
                                        }
                                    }
                                    if (dRL < dRemainingLife)
                                    {
                                        dRemainingLife = dRL;
                                    }
                                }
                            }
                        }
                    }

                    //Removing spend budget here. Move to outside of year loop so that
                    //Committed projects budgets are accounted for BEFORE scheduled treatments and split treatments.
                    //    Investment.SpendBudget(fAmount, strBudget, nYear.ToString());

                    float fCost = fAmount / section.Area;

                    double deltaBenefit = dBenefit - section.BaseBenefit;
                    double deltaRemainingLife = dRemainingLife - section.RemainingLife;

                    String strRLHash = this.CreateRemainingLifeHashString(hashRL);
                    double dBCRatio;
                    if (Method.IsBenefitCost)
                    {
                        if (fCost > 0)
                        {
                            dBCRatio = deltaBenefit / (double)fCost;
                        }
                        else
                        {
                            dBCRatio = 0;
                        }
                    }
                    else
                    {
                        if (fCost > 0)
                        {
                            dBCRatio = deltaRemainingLife / (double)fCost;
                        }
                        else
                        {
                            dBCRatio = 0;
                        }
                    }

                    SameTreatment sameTreatment = new SameTreatment();
                    sameTreatment.strTreatment = commit.Treatment;
                    sameTreatment.nYear = nYear;
                    sameTreatment.isExclusive = commit.OMSIsExclusive;
                    sameTreatment.isNotAllowed = commit.OMSIsNotAllowed;
                    section.m_listSame.Add(sameTreatment);

                    if (SimulationMessaging.IsOMS)
                    {
                        section.OCI.UpdateApparentAge(hashOutput);
                    }
                    // No need to Update targets and update deficiency. They arcalculated after committed
                    //At this point have Benefit/RL, Base Benefit/RL, Cost, ConsquenceID
                    //Calculate B/C or RL/C or both (RL*B)/C
                    //Insert in Report table with Batch  Load.
                    string strOut = "";

                    if (string.IsNullOrWhiteSpace(commit.ScheduledTreatmentId) && string.IsNullOrWhiteSpace(commit.SplitTreatmentId))
                    {
                        strOut = section.SectionID + ":"
                                                   + nYear.ToString() + ":"
                                                   + commit.Treatment + ":"
                                                   + commit.Any.ToString() + ":"
                                                   + commit.Same.ToString() + ":"
                                                   + strBudget + ":"
                                                   + fAmount.ToString("f") + ":"
                                                   + deltaRemainingLife.ToString("f") + ":"
                                                   + deltaBenefit.ToString("f") + ":"
                                                   + dBCRatio.ToString("f") + ":"
                                                   + commit.Consequence.CommitID + ":"
                                                   + "0" + ":" //Priority
                                                   + strRLHash + ":"
                                                   + "0" + ":" //Commit order.
                                                   + "1" + ":" //Committed
                                                   + "0" + ":"
                                                   + strChangeHash + ":" //number viable treatment
                                                   + section.Area.ToString() + ":";
                    }
                    else
                    {
                        strOut = section.SectionID + ":"
                                                   + nYear.ToString() + ":"
                                                   + commit.Treatment + ":"
                                                   + commit.Any.ToString() + ":"
                                                   + commit.Same.ToString() + ":"
                                                   + strBudget + ":"
                                                   + fAmount.ToString("f") + ":"
                                                   + deltaRemainingLife.ToString("f") + ":"
                                                   + deltaBenefit.ToString("f") + ":"
                                                   + dBCRatio.ToString("f") + ":"
                                                   + "0" + ":"
                                                   + "0" + ":" //Priority
                                                   + strRLHash + ":"
                                                   + "0" + ":" //Commit order.
                                                   + "1" + ":" //Committed
                                                   + "0" + ":"
                                                   + strChangeHash + ":" //number viable treatment
                                                   + section.Area.ToString() + ":";
                    }

                    if (commit.OMSIsNotAllowed)
                    {
                        strOut += "4";//Not allowed.
                    }
                    else if (commit.IsRepeat)
                    {
                        strOut += "3"; //Committed result type.
                    }
                    else
                    {
                        strOut += "1"; //Committed result type.
                    }
                    switch (DBMgr.NativeConnectionParameters.Provider)
                    {
                        case "MSSQL":
                            // MSSQL needs the extra spot for the ID_
                            tw.WriteLine(":" + strOut);
                            break;

                        case "ORACLE":
                            tw.Write(strOut);
                            tw.Write("#ORACLEENDOFLINE#");
                            break;

                        default:
                            throw new NotImplementedException("TODO: Implement ANSI version of ApplyCommitted()");
                            //break;
                    }
                }
            }
            //Write to database
            tw.Close();

            _spanAnalysis += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(SimulationMessaging.ReportTable, sOutFile, ':');
                    break;

                case "ORACLE":
                    //throw new NotImplementedException( "TODO: Figure out columns in ApplyCommitted." );
                    List<string> columnNames = new List<string>();
                    //columnNames.Add("ID_");
                    columnNames.Add("SECTIONID");
                    columnNames.Add("YEARS");
                    columnNames.Add("TREATMENT");
                    columnNames.Add("YEARSANY");
                    columnNames.Add("YEARSSAME");
                    columnNames.Add("BUDGET");
                    columnNames.Add("COST_");
                    columnNames.Add("REMAINING_LIFE");
                    columnNames.Add("BENEFIT");
                    columnNames.Add("BC_RATIO");
                    columnNames.Add("CONSEQUENCEID");
                    columnNames.Add("PRIORITY");
                    columnNames.Add("RLHASH");
                    columnNames.Add("COMMITORDER");
                    columnNames.Add("ISCOMMITTED");
                    columnNames.Add("NUMBERTREATMENT");
                    columnNames.Add("CHANGEHASH");
                    columnNames.Add("AREA");
                    columnNames.Add("RESULT_TYPE");

                    //" \"str '#ORACLEENDOFLINE#'\""

                    DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, SimulationMessaging.ReportTable, sOutFile, columnNames, ":", " \"str '#ORACLEENDOFLINE#'\"");
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }
            _spanReport += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;
        }

        private void ApplyCommitted(int nYear, String strSectionID)
        {
            String strBudget;
            float fAmount;
            Sections section = m_listSections.Find((delegate (Sections s) { return s.SectionID == strSectionID; }));
            List<Committed> currentYearCommitted = section.YearCommit.FindAll(delegate (Committed c) { return c.Year == nYear; });
            foreach (Committed commit in currentYearCommitted)
            {
                if (!section.IsOMSCommitAllowed(commit.Treatment, nYear)) continue;
                String strChangeHash = "";
                if (section.m_hashYearAttributeValues.Contains(nYear))
                {
                    section.m_hashYearAttributeValues.Remove(nYear);
                }
                Hashtable hashOutput = section.CommitProject(commit, out strBudget, out fAmount, m_dictionaryCommittedEquations, out strChangeHash);
                //Investment.SpendBudget(fAmount, strBudget, nYear.ToString());

                Hashtable hashInput = (Hashtable)section.m_hashNextAttributeValue;
                if (SimulationMessaging.IsOMS)
                {
                    hashOutput = ApplyCommittedConsequences(hashInput, commit, out strChangeHash);
                }

                if (section.m_hashYearAttributeValues.Contains(nYear))
                {
                    section.m_hashYearAttributeValues.Remove(nYear);
                }
                section.m_hashYearAttributeValues.Add(nYear, hashOutput);

                //Calculate remaining life and incremental BC for the new values.
                //Calculate new deterioration using the new hashtable
                double dBenefit = 0;
                double dRemainingLife = 100;
                Hashtable hashRL = new Hashtable();

                //Hashtable hash = (Hashtable)section.m_hashYearAttributeValues[nYear];
                foreach (Deteriorate deteriorate in m_listDeteriorate)
                {
                    if (deteriorate.IsCriteriaMet(hashOutput))
                    {
                        bool bOutOfRange;
                        deteriorate.IterateOneYear(hashOutput, out bOutOfRange);
                        if (Method.IsBenefitCost && Method.BenefitAttribute == deteriorate.Attribute)
                        {
                            dBenefit = deteriorate.CalculateBenefit(hashOutput);
                        }

                        double dRL = 0;
                        if (deteriorate.CalculateRemainingLife(hashOutput, hashInput, out dRL))
                        {
                            if (!hashRL.Contains(deteriorate.Attribute))
                            {
                                hashRL.Add(deteriorate.Attribute, dRL);
                            }
                            else
                            {
                                double dRLOld = (double)hashRL[deteriorate.Attribute];
                                if (dRL < dRLOld)
                                {
                                    hashRL.Remove(deteriorate.Attribute);
                                    hashRL.Add(deteriorate.Attribute, dRL);
                                }
                            }

                            if (dRL < dRemainingLife)
                            {
                                dRemainingLife = dRL;
                            }
                        }
                    }
                }

                float fCost = fAmount / section.Area;

                double deltaBenefit = dBenefit - section.BaseBenefit;
                double deltaRemainingLife = dRemainingLife - section.RemainingLife;

                String strRLHash = this.CreateRemainingLifeHashString(hashRL);
                double dBCRatio;
                if (Method.IsBenefitCost)
                {
                    if (fCost > 0)
                    {
                        dBCRatio = deltaBenefit / (double)fCost;
                    }
                    else
                    {
                        dBCRatio = 0;
                    }
                }
                else
                {
                    if (fCost > 0)
                    {
                        dBCRatio = deltaRemainingLife / (double)fCost;
                    }
                    else
                    {
                        dBCRatio = 0;
                    }
                }

                // No need to Update targets and update deficiency. They are
                // calculated after committed

                //At this point have Benefit/RL, Base Benefit/RL, Cost, ConsquenceID
                //Calculate B/C or RL/C or both (RL*B)/C
                //Insert in Report table with Batch  Load.
                //String strOut = "," + section.SectionID + ","
                //                + nYear.ToString() + ","
                //                + commit.Treatment + ","
                //                + commit.Any.ToString() + ","
                //                + commit.Same.ToString() + ","
                //                + strBudget + ","
                //                + fAmount.ToString("f") + ","
                //                + deltaRemainingLife.ToString("f") + ","
                //                + deltaBenefit.ToString("f") + ","
                //                + dBCRatio.ToString("f") + ","
                //                + commit.Consequence.CommitID + ","
                //                + "0" + ","//Priority
                //                + strRLHash + ","
                //                + "0" + ","//Commit order.
                //                + "1" + "," //Committed
                //                + "0" + ","
                //                + strChangeHash + ","  //number viable treatment
                //                + section.Area.ToString();

                String strInsert = "INSERT INTO REPORT_" + m_strNetworkID + "_" + m_strSimulationID + " (SECTIONID,YEARS,TREATMENT,YEARSANY,YEARSSAME,BUDGET,COST_,REMAINING_LIFE,BENEFIT,BC_RATIO,CONSEQUENCEID,PRIORITY,RLHASH,COMMITORDER,ISCOMMITTED,NUMBERTREATMENT,CHANGEHASH,AREA) VALUES ('";
                strInsert += section.SectionID + "','";
                strInsert += nYear.ToString() + "','";
                strInsert += commit.Treatment + "','";
                strInsert += commit.Any.ToString() + "','";
                strInsert += commit.Same.ToString() + "','";
                strInsert += strBudget + "','";
                strInsert += fAmount.ToString("f") + "','";
                strInsert += deltaRemainingLife.ToString("f") + "','";
                strInsert += deltaBenefit.ToString("f") + "','";
                strInsert += dBCRatio.ToString("f") + "','";
                strInsert += commit.Consequence.CommitID + "','";
                strInsert += "0" + "','";//Priority
                strInsert += strRLHash + "','";
                strInsert += "0" + "','";//Commit order.
                strInsert += "1" + "','"; //Committed
                strInsert += "0" + "','";
                strInsert += strChangeHash + "','";  //number viable treatment
                strInsert += section.Area.ToString() + "')"; ;
                //Write to database
                DBMgr.ExecuteNonQuery(strInsert);
            }
        }

        private void SpendUntilTargetsDeficientMet(int nYear)
        {
            SpendUntilTargetsDeficientMetRoadCare(nYear);
        }

        private void SpendUntilTargetsDeficientMetRoadCare(int nYear)
        {
            //Figure out which targets need to be done first (and do them in that order).
            List<Targets> subsetTargets = new List<Targets>();
            //List<Targets> subsetTargets = GetOrderedSubsetTargets();
            //Call this SubsetTarget.

            //Cost inflation calculations
            int nInflationYear = nYear - Investment.StartYear;
            double dRate = Investment.Inflation;
            float fInflationMultiplier = (float)Math.Pow(1 + dRate, (double)nInflationYear);

            string sOutFile = "";
            string sReasonOutfile = "";
            TextWriter tw = SimulationMessaging.CreateTextWriter("report_" + m_strSimulationID + ".csv", out sOutFile);
            var reasonReportWriter = SimulationMessaging.CreateTextWriter("reason_" + m_strSimulationID + ".csv", out sReasonOutfile);

            String strSectionID = "";

            int nCommitOrder = 0;
            int benefitOrder = 0;

            bool bRemoveTreatedSection = false;
            int currentHead = 0; //Current head of AppliedTreament.  Used for SubsetTarget.

            while (m_listApplyTreatment.Count > 0)
            {
                benefitOrder++;
                //Remove all other possible treatments for this sectionID
                if (strSectionID != "" && bRemoveTreatedSection)
                {
                    m_listApplyTreatment.RemoveAll(delegate (AppliedTreatment at) { return at.SectionID == strSectionID; });
                }

                //Remove all targets and deficient that are met from
                try
                {
                    List<Targets> listRemove = CheckTargetsAndRemoveMet(nYear);
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Removing targets and deficient error:" + e.Message));
                    throw e;
                }

                //Iterate through SubsetTarget until we find a SectionID that is included.
                //Increment currentHead.

                AppliedTreatment treatment = null;
                try
                {
                    m_listApplyTreatment.Sort(delegate (AppliedTreatment t1, AppliedTreatment t2) { return t2.SelectionWeight.CompareTo(t1.SelectionWeight); });
                    //Apply first treatment on list
                    if (m_listApplyTreatment.Count == 0) continue;

                    //SubsetTarget check.
                    if (subsetTargets.Count > 0)
                    {
                        Targets subsetTarget = subsetTargets[0];
                        List<String> sectionIDs = (List<String>)m_hashTargetSectionID[subsetTarget.ID];

                        foreach (AppliedTreatment appliedTreatment in m_listApplyTreatment)
                        {
                            if (sectionIDs.Contains(appliedTreatment.SectionID))
                            {
                                break;
                            }
                            else
                            {
                                currentHead++;
                            }
                        }
                    }

                    //SubsetTarget check.
                    //if(currentHead == m_listApplyTreatment.Count)
                    // {
                    //We have iterated past end of list and target is not met.
                    //Remove SubsetTarget and set current head to 0/
                    currentHead = 0;
                    //}

                    treatment = (AppliedTreatment)m_listApplyTreatment[currentHead];
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Sorting and getting best BenefitCost treatment:" + e.Message));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Error: Sorting and getting best BenefitCost treatment: " + e.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    throw e;
                }

                strSectionID = treatment.SectionID;
                String sTreatment = treatment.Treatment;
                int nAny = treatment.Any;
                int nSame = treatment.Same;
                float fCost = treatment.Cost;
                String strMultipleBudget = treatment.Budget;
                string strBudget;
                String sRemaining = treatment.RemainingLife.ToString();
                String sBenefit = treatment.Benefit.ToString();
                String sBC = treatment.BenefitCostRatio.ToString();
                String strTreatmentID = treatment.TreatmentID;
                String sRLHash = treatment.RemainingLifeHash;
                string budgetHash = "";
                // Get the section that matches this
                Sections section = m_listSections.Find(delegate (Sections s) { return s.SectionID == strSectionID; });
                if (section == null) continue;//Should never happen.

                try
                {
                    //Check if already treated.  Check if treament is in a shadow or will cast a shadow.
                    // Check for negative BC, if its negative.  Dont spend money on it.
                    if (Convert.ToDouble(sBC) < 0)
                    {
                        //SimulationMessaging.AddMessage(new SimulationMessage("Warning! Applying treatment " + treatment.Treatment + " to section " + section.Section + " produces as negative benefit cost. Please evaluate your treatment consequence parameters. Treatment will not be suggested."));
                        //Added May 16, 2014.  If negative benefit/cost remove section from possible list go to next.
                        m_listApplyTreatment.Remove(treatment);
                        reasonReportWriter.WriteLine(MakeReasonReportLine(treatment.SectionID, sTreatment, "Negative benefit cost", "", "", nYear, 0, treatment.NumberTreatmentDeficient, benefitOrder, treatment.SelectionWeight));

                        bRemoveTreatedSection = false;
                        continue;
                    }
                    if (!section.IsTreatmentAllowed(sTreatment, nAny.ToString(), nSame.ToString(), nYear))
                    {
                        m_listApplyTreatment.Remove(treatment);
                        bRemoveTreatedSection = false;
                        reasonReportWriter.WriteLine(MakeReasonReportLine(treatment.SectionID, sTreatment, "Any/Same shadow", "", "", nYear, 0, treatment.NumberTreatmentDeficient, benefitOrder, treatment.SelectionWeight));
                        continue;
                        
                    }
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error:Checking if treatment is allowed:" + e.Message));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Error:Checking if treatment is allowed: " + e.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    throw e;
                }
                ISplitTreatmentLimit limit = null;
                float fAmount = section.Area * fCost * fInflationMultiplier;
                try
                {
                    if (strMultipleBudget == null)
                    {
                        throw new Exception("Budget is equal to null");
                    }

                    strBudget = null;
                    List<ISplitTreatmentLimit> limits = GetSplitTreatmentLimits(section.m_hashNextAttributeValue);

                    foreach (Priorities priority in m_listPriorities)
                    {
                        if (strBudget == null)
                        {
                            strBudget = Investment.IsBudgetAvailable(fAmount, strMultipleBudget, nYear.ToString(), section.m_hashNextAttributeValue, priority,limits, "", out budgetHash,out limit);
                        }
                    }
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Checking priority in targets and deficient for treatment (" + sTreatment + "):" + e.Message));
                    throw e;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(strBudget))
                    {
                        m_listApplyTreatment.Remove(treatment);
                        bRemoveTreatedSection = false;
                        reasonReportWriter.WriteLine(MakeReasonReportLine(treatment.SectionID, sTreatment, "No budget available under any priority", "", "", nYear, 0, treatment.NumberTreatmentDeficient, benefitOrder, treatment.SelectionWeight));
                        continue;
                    }
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error removing applied treatment:" + e.Message));
                    throw e;
                }

                try
                {
                    if (Method.TypeBudget == "Until Targets Met")
                    {
                        List<Targets> listNotMet = m_listTargets.FindAll(delegate (Targets t) { return t.IsTargets == true; });
                        if (listNotMet.Count == 0)
                        {
                            m_listApplyTreatment.Remove(treatment);
                            bRemoveTreatedSection = false;
                            reasonReportWriter.WriteLine(MakeReasonReportLine(treatment.SectionID, sTreatment, "All targets met which this treatment can help", strBudget, budgetHash, nYear, 0, treatment.NumberTreatmentDeficient, benefitOrder, treatment.SelectionWeight));
                            continue;
                        }
                    }

                    if (Method.TypeBudget == "Until Deficient Met")
                    {
                        List<Targets> listNotMet = m_listTargets.FindAll(delegate (Targets t) { return t.IsTargets == false; });
                        if (listNotMet.Count == 0)
                        {
                            m_listApplyTreatment.Remove(treatment);
                            reasonReportWriter.WriteLine(MakeReasonReportLine(treatment.SectionID, sTreatment, "All deficients met which this treatment can help", strBudget, budgetHash, nYear, 0, treatment.NumberTreatmentDeficient, benefitOrder, treatment.SelectionWeight));

                            bRemoveTreatedSection = false;
                            continue;
                        }
                    }

                    if (Method.TypeBudget == "Targets/Deficient Met")
                    {
                        if (m_listTargets.Count == 0)
                        {
                            m_listApplyTreatment.Remove(treatment);
                            reasonReportWriter.WriteLine(MakeReasonReportLine(treatment.SectionID, sTreatment, "All targets/deficient met which this treatment can help", strBudget, budgetHash, nYear, 0, treatment.NumberTreatmentDeficient, benefitOrder, treatment.SelectionWeight));
                            bRemoveTreatedSection = false;
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Removing Deficient and Targets from list of Targets: " + e.Message));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Error: Removing Deficient and Targets from list of Targets: " + e.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    throw e;
                }

                String strChangeHash;
                float fNewArea;
                try
                {
                    int numberTreatments = 0;
                    if(treatment.BenefitCostRatio >= 0)
                    {
                        numberTreatments = (int) Math.Ceiling(treatment.SelectionWeight / treatment.BenefitCostRatio);
                    }
                    reasonReportWriter.WriteLine(MakeReasonReportLine(treatment.SectionID, sTreatment, "Selected", strBudget, budgetHash, nYear, 0,numberTreatments , benefitOrder, treatment.SelectionWeight));

                    //Spend budget.

                    //Spend budget (accoreding to limit)
                    int splitYear = 0;
                    foreach (var percentage in limit.Percentages)
                    {
                        var year = nYear + splitYear;
                        Investment.SpendBudget(fAmount * percentage / 100, strBudget, year.ToString());
                        splitYear++;
                    }



                    //Mark treated.
                    section.Treated = true;
                    section.AnyYear = nYear + nAny;

                    SameTreatment sameTreatment = new SameTreatment();
                    sameTreatment.strTreatment = sTreatment;
                    sameTreatment.nYear = nYear + nSame;
                    section.m_listSame.Add(sameTreatment);

                    //Apply consequences.
                    Hashtable hashOutput;
                    if (limit.Percentages.Count == 1)//If project is done in one year
                    {
                        hashOutput = ApplyConsequences(section.m_hashNextAttributeValue, strTreatmentID,
                            out strChangeHash, section); // This is the attribute value pair for this year.

                        //Apply CalculatedAttribute
                        hashOutput = ApplyCalculatedFields(hashOutput);
                    }
                    else //If it takes more than one year
                    {
                        //Consequences are applied in the final year (with no deterioration from start to finish).
                        hashOutput = ApplyCalculatedFields(section.m_hashNextAttributeValue);
                        //Commit split treatment (cash flow).  
                        CommitSplitTreatment(section, nYear, treatment, strBudget, limit, fAmount);
                        strChangeHash = "";
                    }

            
                    //If treatment has Scheduled treatments.   Commit them now.
                    CommitScheduled(section, nYear, strTreatmentID, strBudget, treatment.ScheduledCost);

                    section.m_hashYearAttributeValues.Add(nYear, hashOutput);

                    float fOldArea = section.Area;
                    //Calculate new section area
                    section.CalculateArea(nYear);
                    fNewArea = section.Area;

                    //Update targets.
                    //SimulationMessaging.AddMessage(new SimulationMessage("Applying to " + section.Facility + " " + section.Section + " treatment " + treatment.Treatment));
                    UpdateTargetsAndDeficiency(nYear, fOldArea, fNewArea, section.m_hashNextAttributeValue, hashOutput);

                    //foreach (Targets target in m_listTargets)
                    //{
                    //    SimulationMessaging.AddMessage(new SimulationMessage(target.Attribute + " " + target.Target + " (" + target.CurrentTarget(nYear).ToString() + ")"));
                    //}

                    bRemoveTreatedSection = true;
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error updating Targets and deficient." + e.Message));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Error updating Targets and deficient: " + e.Message);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    throw e;
                }


                //Only report Amount spent this year.
                fAmount = fAmount * limit.Percentages[0] / 100;
                
                //Write report.
                String strOut;
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        strOut = ":";
                        strOut += section.SectionID + ":"
                                        + nYear.ToString() + ":"
                                        + sTreatment + ":"
                                        + nAny.ToString() + ":"
                                        + nSame.ToString() + ":"
                                        + strBudget + ":"
                                        + fAmount.ToString("f") + ":"
                                        + sRemaining + ":"
                                        + sBenefit + ":"
                                        + sBC + ":"
                                        + strTreatmentID + ":"
                                        + "0" + ":"//Priority
                                        + sRLHash + ":"
                                        + nCommitOrder.ToString() + ":"   //Commit order.
                                        + "0" + ":" //Committed
                                        + section.NumberTreatment.ToString() + ":" //number viable treatment
                                        + strChangeHash + ":"
                                        + fNewArea.ToString() + ":"
                                        + "0";
                        tw.WriteLine(strOut);
                        break;

                    case "ORACLE":
                        strOut = "";
                        strOut += section.SectionID + ":"
                                        + nYear.ToString() + ":"
                                        + sTreatment + ":"
                                        + nAny.ToString() + ":"
                                        + nSame.ToString() + ":"
                                        + strBudget + ":"
                                        + fAmount.ToString("f") + ":"
                                        + sRemaining + ":"
                                        + sBenefit + ":"
                                        + sBC + ":"
                                        + strTreatmentID + ":"
                                        + "0" + ":"//Priority
                                        + sRLHash + ":"
                                        + nCommitOrder.ToString() + ":"   //Commit order.
                                        + "0" + ":" //Committed
                                        + section.NumberTreatment.ToString() + ":" //number viable treatment
                                        + strChangeHash + ":"
                                        + fNewArea.ToString() + ":"
                                        + "0";

                        tw.Write(strOut);
                        tw.Write("#ORACLEENDOFLINE#");

                        break;

                    default:
                        throw new NotImplementedException("TODO: implement ANSI version of SpendUntilTargetsDeficientMet()");
                }

                nCommitOrder++;
            }
            //Loop through all SECTIONS and apply no treatment to sections without treatments
            try
            {
                foreach (Sections section in m_listSections)
                {
                    if (section.Treated) continue;
                    //Apply consequences.
                    String strChangeHash;
                    Hashtable hashAttributeValue = ApplyConsequences(section.m_hashNextAttributeValue, noTreatments.TreatmentID, out strChangeHash, section);

                    //Apply CalculatedAttribute
                    hashAttributeValue = ApplyCalculatedFields(hashAttributeValue);

                    section.m_hashYearAttributeValues.Add(nYear, hashAttributeValue);

                    Hashtable hashRL = new Hashtable();
                    double dRemainingLife = 100;
                    foreach (Deteriorate deteriorate in m_listDeteriorate)
                    {
                        if (SimulationMessaging.Method.IsConditionalRSL)
                        {
                            if (!hashRL.Contains(deteriorate.Attribute))
                            {
                                hashRL.Add(deteriorate.Attribute, 0); //Add to remain life hash
                            }
                            dRemainingLife = 0;
                        }
                        else
                        {
                            if (deteriorate.IsCriteriaMet(hashAttributeValue))
                            {
                                double dRL = 0;

                                if (deteriorate.CalculateRemainingLife(hashAttributeValue, hashAttributeValue, out dRL))
                                {
                                    if (!hashRL.Contains(deteriorate.Attribute))
                                    {
                                        hashRL.Add(deteriorate.Attribute, dRL);
                                    }
                                    else
                                    {
                                        double dRLOld = (double)hashRL[deteriorate.Attribute];
                                        if (dRL < dRLOld)
                                        {
                                            hashRL.Remove(deteriorate.Attribute);
                                            hashRL.Add(deteriorate.Attribute, dRL);
                                        }
                                    }
                                    if (dRL < dRemainingLife)
                                    {
                                        dRemainingLife = dRL;
                                    }
                                }
                            }
                        }
                    }
                    String strRLHash = this.CreateRemainingLifeHashString(hashRL);

                    //Write report.

                    String strOut;
                    switch (DBMgr.NativeConnectionParameters.Provider)
                    {
                        case "MSSQL":
                            strOut = ":";
                            strOut += section.SectionID + ":"
                                                    + nYear.ToString() + ":"
                                                    + noTreatments.Treatment + ":"
                                                    + "0" + ":"
                                                    + "0" + ":"
                                                    + "" + ":"
                                                    + "0" + ":"
                                                    + dRemainingLife.ToString("f") + ":"
                                                    + "0" + ":"
                                                    + "0" + ":"
                                                    + noTreatments.TreatmentID + ":"
                                                    + "0" + ":"//Priority
                                                    + strRLHash + ":"
                                                    + "0" + ":" //Commit order.
                                                    + "0" + ":" //bool whether committed or not
                                                    + section.NumberTreatment.ToString() + ":"
                                                    + strChangeHash + ":"
                                                    + section.Area.ToString() + ":"
                                                    + "0";
                            tw.WriteLine(strOut);

                            break;

                        case "ORACLE":
                            strOut = "";
                            strOut += section.SectionID + ":"
                                                    + nYear.ToString() + ":"
                                                    + noTreatments.Treatment + ":"
                                                    + "0" + ":"
                                                    + "0" + ":"
                                                    + "" + ":"
                                                    + "0" + ":"
                                                    + dRemainingLife.ToString("f") + ":"
                                                    + "0" + ":"
                                                    + "0" + ":"
                                                    + noTreatments.TreatmentID + ":"
                                                    + "0" + ":"//Priority
                                                    + strRLHash + ":"
                                                    + "0" + ":" //Commit order.
                                                    + "0" + ":" //bool whether committed or not
                                                    + section.NumberTreatment.ToString() + ":"
                                                    + strChangeHash + ":"
                                                    + section.Area.ToString() + ":"
                                                    + "0";
                            tw.Write(strOut);
                            tw.Write("#ORACLEENDOFLINE#");

                            break;

                        default:
                            throw new NotImplementedException("TODO: implement ANSI version of SpendUntilTargetsDeficientMet()");
                    }
                }
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Applying No Treatment to Targets and Deficient." + e.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Applying No Treatment to Targets and Deficient: " + e.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                throw e;
            }
            tw.Close();
            reasonReportWriter.Close();
            _spanAnalysis += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;
            try
            {
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        DBMgr.SQLBulkLoad(SimulationMessaging.ReportTable, sOutFile, ':');
                        break;

                    case "ORACLE":

                        //throw new NotImplementedException("TODO: Figure out columns for SpendUnitTargetDeficientMet");
                        List<string> reportColumns = DBMgr.GetTableColumns(SimulationMessaging.ReportTable);
                        reportColumns.Remove("ID_");
                        DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, SimulationMessaging.ReportTable, sOutFile, reportColumns, ":", " \"str '#ORACLEENDOFLINE#'\"");
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                        //break;
                }
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Bulk loading results from year = " + nYear.ToString() + ". " + e.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Bulk loading results from year = " + nYear.ToString() + " - " + e.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                throw e;
            }


            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(SimulationMessaging.ReasonsTable, sReasonOutfile, ',');
                    break;

                case "ORACLE":
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }


            _spanReport += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;
        }

        private void CommitScheduled(Sections section, int year, string strTreatmentId, string budget, Dictionary<string, float> scheduledCost)
        {
            var treatmentWithScheduled = m_listTreatments.Find(t => t.TreatmentID == strTreatmentId);
            if (treatmentWithScheduled != null)
            {
                foreach (var scheduled in treatmentWithScheduled.Scheduleds)
                {
                    var commit = new Committed();
                    commit.Year = year + scheduled.ScheduledYear;
                    commit.ScheduledTreatmentId = scheduled.Treatment.TreatmentID;
                    commit.Budget = budget;
                    section.YearCommit.Add(commit);
                    var key = scheduled.Treatment.TreatmentID + "|" + scheduled.ScheduledYear;
                    float amount = 0;
                    if(scheduledCost.ContainsKey(key))
                    {
                        amount = scheduledCost[key];
                    }

                    //Spend the budget for scheduled now.
                    Investment.SpendBudget(amount, budget, commit.Year.ToString());
                }
            }
        }

        /// <summary>
        /// Retrieves list of targets ordered by whether they are exclusive or
        /// subsets of another target.
        /// </summary>
        /// <returns></returns>
        private List<Targets> GetOrderedSubsetTargets()
        {
            List<Targets> subsetTargets = new List<Targets>();

            foreach (Targets target in m_listTargets)
            {
                if (m_hashTargetSectionID.Contains(target.ID))
                {
                    target.SectionCount = ((List<String>)m_hashTargetSectionID[target.ID]).Count;
                }
            }
            m_listTargets.Sort(delegate (Targets t1, Targets t2) { return t1.SectionCount.CompareTo(t2.SectionCount); });

            //Get targets that are unique.  Do them first.
            foreach (Targets right in m_listTargets)
            {
                if (!right.IsDeficient) continue;
                foreach (Targets left in m_listTargets)
                {
                    if (!left.IsDeficient) continue;
                    if (left == right) continue;

                    List<String> rightSections = (List<String>)m_hashTargetSectionID[right.ID];
                    List<String> leftSections = (List<String>)m_hashTargetSectionID[left.ID];

                    if (ContainsUniqueStrings(rightSections, leftSections))
                    {
                        subsetTargets.Add(right);
                    }
                }
            }

            //Next get targets that are subsets of larger targets.  These should be done before larger targets.
            //Only for deficient.
            foreach (Targets right in m_listTargets)
            {
                if (!right.IsDeficient) continue;
                foreach (Targets left in m_listTargets)
                {
                    if (!left.IsDeficient) continue;
                    if (left == right) continue;

                    List<String> rightSections = (List<String>)m_hashTargetSectionID[right.ID];
                    List<String> leftSections = (List<String>)m_hashTargetSectionID[left.ID];

                    if (IsProperSet(rightSections, leftSections))
                    {
                        subsetTargets.Add(right);
                    }
                }
            }

            return subsetTargets;
        }

        private bool ContainsUniqueStrings(List<String> rightSections, List<String> leftSections)
        {
            foreach (String right in rightSections)
            {
                foreach (String left in leftSections)
                {
                    if (right == left) return false;
                }
            }
            return true;
        }

        private bool IsProperSet(List<String> rightSections, List<String> leftSections)
        {
            if (rightSections.Count < leftSections.Count)
            {
                foreach (String right in rightSections)
                {
                    if (!leftSections.Contains(right)) return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private List<Targets> CheckTargetsAndRemoveMet(int nYear)
        {
            List<Targets> listRemove = new List<Targets>();
            foreach (Targets target in m_listTargets)
            {
                if (target.IsTargets)
                {
                    if (target.IsTargetMet(nYear))
                    {
                        if (!m_hashTargetSectionID.Contains(target.ID)) continue;
                        SimulationMessaging.AddMessage(new SimulationMessage(target.Attribute + " " + target.TargetName + " Target Met"));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, target.Attribute + " " + target.TargetName + " Target Met");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        RemoveMetTargets(target);
                        listRemove.Add(target);
                    }
                }
                else // Deficient
                {
                    if (target.IsDeficientMet(nYear))
                    {
                        if (!m_hashTargetSectionID.Contains(target.ID)) continue;
                        SimulationMessaging.AddMessage(new SimulationMessage(target.Attribute + " " + target.TargetName + " Deficient Target Met"));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, target.Attribute + " " + target.TargetName + " Deficient Target Met");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        RemoveMetTargets(target);
                        listRemove.Add(target);
                    }
                }
            }
            foreach (Targets target in listRemove)
            {
                m_listTargets.Remove(target);
            }

            //For OMS, if OCI Target is met for the year we get to stop.
            if (SimulationMessaging.IsOMS)
            {
                Targets target = m_listTargets.Find(delegate (Targets t) { return t.Year == nYear && t.TargetName == "OCI TARGET"; });
                if (target == null)//Means OCI Target met, so rest of targets can be ignored.
                {
                    m_listTargets.RemoveAll(delegate (Targets t) { return t.Year == nYear; });
                }
            }

            return listRemove;
        }

        /// <summary>
        /// Removes Targets from targetID listSectionID AND removes target from
        /// list of appliable treatmets
        /// </summary>
        /// <param name="target"></param>
        private void RemoveMetTargets(Targets target)
        {
            //Target is met.  Decrement each
            if (!m_hashTargetSectionID.Contains(target.ID)) return;
            List<String> listSectionID = (List<String>)m_hashTargetSectionID[target.ID];
            foreach (String strSectionID in listSectionID)
            {
                //Find each matching sectionID.
                List<AppliedTreatment> listTreatments = m_listApplyTreatment.FindAll(delegate (AppliedTreatment at) { return at.SectionID == strSectionID; });
                if (listTreatments != null)
                {
                    foreach (AppliedTreatment appliedTreatment in listTreatments)
                    {
                        if (appliedTreatment.NumberTreatmentDeficient > 0)
                        {
                            appliedTreatment.NumberTreatmentDeficient--;
                            appliedTreatment.SelectionWeight = (double)appliedTreatment.NumberTreatmentDeficient * appliedTreatment.BenefitCostRatio;
                        }
                    }
                }
            }

            //List<AppliedTreatment> listNoLongerValid = m_listApplyTreatment.FindAll(delegate(AppliedTreatment at) { return at.NumberTreatmentDeficient == 0; });

            //foreach (AppliedTreatment at in listNoLongerValid)
            //{
            //    Sections sectionNoLongerValid = m_listSections.Find(delegate(Sections s) { return s.SectionID == at.SectionID; });
            //    if (sectionNoLongerValid != null)
            //    {
            //        SimulationMessaging.AddMessage(new SimulationMessage(sectionNoLongerValid.Facility + " " + sectionNoLongerValid.Section + " treatment " + at.Treatment + " no longer valid because target " + target.TargetName + " met."));
            //    }
            //}

            m_listApplyTreatment.RemoveAll(delegate (AppliedTreatment at) { return at.NumberTreatmentDeficient == 0; });

            return;
        }

        /// <summary>
        /// Spend budget with no constraints (Best Benefit Cost)
        /// </summary>
        /// <param name="nYear"></param>
        private void SpendMultipleYearBudgetPermits()
        {
            //String strSelect;
            String sSectionID;
            String sTreatment;
            String sAny;
            String sSame;
            String sCost;
            String sConsequenceID;
            //String sOutFile;
            String sRemaining;
            String sBenefit;
            String sBC;
            String sRLHash;
            int nYear = 0;

            SimulationMessaging.AddMessage(new SimulationMessage("Beginning Multiple Year Sort. " + DateTime.Now.ToString("HH:mm:ss")));
            m_listMultipleYearTreatment.Sort(delegate (AppliedTreatment t1, AppliedTreatment t2) { return t2.BenefitCostRatio.CompareTo(t1.BenefitCostRatio); });
            SimulationMessaging.AddMessage(new SimulationMessage("End Multiple Year Sort. " + DateTime.Now.ToString("HH:mm:ss")));

            //List of Section with treatment
            // Retrieve Treatments from BC BENEFITCOST_networkid_simulationid
            // in BC ORDER desending.
            int nCommitOrder = 1;
            foreach (Priorities priority in m_listPriorities)
            {
                foreach (String strBudget in Investment.BudgetOrder)
                {
                    //MULTIBUDGET FIX

                    //List<AppliedTreatment> listTreatmentsForBudget = m_listMultipleYearTreatment.FindAll(delegate(AppliedTreatment t1) { return (t1.Budget == strBudget && t1.Available); });
                    List<AppliedTreatment> listTreatmentsForBudget = m_listMultipleYearTreatment.FindAll(delegate (AppliedTreatment t1) { return (ContainsBudget(t1.Budget, strBudget) && t1.Available); });
                    foreach (AppliedTreatment appliedTreatment in listTreatmentsForBudget)
                    {
                        if (!appliedTreatment.Available) continue;//If this treatment is not available, loop to next.
                        sSectionID = appliedTreatment.SectionID;
                        sTreatment = appliedTreatment.Treatment;
                        sAny = appliedTreatment.Any.ToString();
                        sSame = appliedTreatment.Same.ToString();
                        sCost = appliedTreatment.Cost.ToString();
                        sRemaining = appliedTreatment.RemainingLife.ToString();
                        sBenefit = appliedTreatment.Benefit.ToString();
                        sBC = appliedTreatment.BenefitCostRatio.ToString();
                        sConsequenceID = appliedTreatment.TreatmentID;
                        sRLHash = appliedTreatment.RemainingLifeHash;
                        nYear = appliedTreatment.Year;
                        String strTreatmentID = sConsequenceID;
                        string budgetHash = "";
                        if (!priority.IsAllYears)
                        {
                            if (priority.Years != nYear) continue;
                        }

                        //Cost inflation calculations
                        int nInflationYear = nYear - Investment.StartYear;
                        double dRate = Investment.Inflation;
                        float fInflationMultiplier = (float)Math.Pow(1 + dRate, (double)nInflationYear);

                        // Get the section that matches this
                        Sections section = m_listSections.Find(delegate (Sections s) { return s.SectionID == sSectionID; });
                        if (section == null) continue;//Should never happen.

                        //Check if already treated.  Check if treament is in a shadow or will cast a shadow.
                        // Check for negative BC, if its negative.  Dont spend money on it.
                        if (Convert.ToDouble(sBC) < 0)
                        {
                            //SimulationMessaging.AddMessage(new SimulationMessage("Warning! Applying treatment " + appliedTreatment.Treatment + " to section " + section.Section + " produces as negative benefit cost. Please evaluate your treatment consequence parameters. Treatment will not be suggested."));
                            continue;
                        }
                        if (!section.IsTreatmentAllowed(sTreatment, sAny, sSame, nYear)) continue;

                        float fAmount = section.Area * float.Parse(sCost) * fInflationMultiplier;
                        List<ISplitTreatmentLimit> limits = GetSplitTreatmentLimits(section.m_hashNextAttributeValue);
                        string budgetSelected = Investment.IsBudgetAvailable(fAmount, strBudget, nYear.ToString(), section.m_hashNextAttributeValue, priority, limits,"", out budgetHash, out ISplitTreatmentLimit limit);
                        if (string.IsNullOrWhiteSpace(budgetSelected)) continue;
                        appliedTreatment.Budget = budgetSelected;

                        Investment.SpendBudget(fAmount, budgetSelected, nYear.ToString());

                        //Remove all treatments from list that are for this section (
                        List<AppliedTreatment> listSections = m_listMultipleYearTreatment.FindAll(delegate (AppliedTreatment t1) { return (t1.SectionID == sSectionID); });
                        foreach (AppliedTreatment at in listSections)
                        {
                            at.Available = false;
                        }
                        //Need to delete existing.
                        String strDelete = "DELETE FROM REPORT_" + m_strNetworkID + "_" + m_strSimulationID + " WHERE YEARS >='" + nYear.ToString() + "' AND SECTIONID='" + sSectionID + "'";
                        try
                        {
                            DBMgr.ExecuteNonQuery(strDelete);
                        }
                        catch
                        {
                        }

                        for (int nYearLoop = appliedTreatment.Year; nYear < Investment.StartYear + Investment.AnalysisPeriod; nYear++)
                        {
                            //Apply Deteriorate/Performance curves.
                            ApplyDeterioration(nYear, sSectionID);
                            if (nYear == appliedTreatment.Year)
                            {
                                SimulationMessaging.AddMessage(new SimulationMessage("Applying treatment:" + sTreatment + " to Facility:" + section.Facility + " Section:" + section.Section + " for Year:" + nYear.ToString() + " at " + DateTime.Now.ToString("HH:mm:ss")));
                                if (APICall.Equals(true))
                                {
                                    var updateStatus = Builders<SimulationModel>.Update
                                    .Set(s => s.status, "Applying treatment:" + sTreatment + " to Facility:" + section.Facility + " Section:" + section.Section + " for Year:" + nYear.ToString());
                                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                                }
                                Committed singleSection = new Committed();
                                singleSection.Treatment = appliedTreatment.Treatment;
                                singleSection.ConsequenceID = appliedTreatment.TreatmentID;
                                singleSection.Any = appliedTreatment.Any;
                                singleSection.Same = appliedTreatment.Same;
                                singleSection.Benefit = appliedTreatment.Benefit.ToString();
                                singleSection.Cost = fAmount;
                                singleSection.BenefitCost = appliedTreatment.BenefitCostRatio.ToString();
                                singleSection.RemainingLife = appliedTreatment.RemainingLife.ToString(); ;
                                singleSection.RemainingLifeHash = appliedTreatment.RemainingLifeHash;
                                singleSection.Priority = priority.PriorityLevel.ToString();
                                singleSection.CommitOrder = nCommitOrder.ToString();
                                singleSection.Consequence = new Consequences();
                                singleSection.Budget = appliedTreatment.Budget;
                                singleSection.MultipleYear = true;
                                string[] changedAttributes = appliedTreatment.ChangeHash.Split('\n');
                                foreach (string attribute in changedAttributes)
                                {
                                    string[] change = attribute.Split('\t');
                                    if (!String.IsNullOrEmpty(change[0]))
                                    {
                                        AttributeChange attributeChange = new AttributeChange();
                                        attributeChange.Attribute = change[0];
                                        attributeChange.Change = change[1];
                                        singleSection.Consequence.AddAttributeChange(attributeChange);
                                    }
                                }
                                //singleSection.Consequence = treatmentSingle.Con
                                nCommitOrder++;
                                SpendSingleSection(nYear, sSectionID, singleSection, false);
                            }
                            else
                            {
                                ApplyCommitted(nYear, sSectionID);
                                SpendSingleSection(nYear, sSectionID, null, false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines if desired budget is in list of budgets.
        /// </summary>
        /// <param name="possibleBudgets"></param>
        /// <param name="budgetCurrent"></param>
        /// <returns></returns>
        private bool ContainsBudget(string possibleBudgets, string budgetCurrent)
        {
            string[] budgets = possibleBudgets.Split('|');
            foreach (string budget in budgets)
            {
                if (budget.Trim() == budgetCurrent)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateAppliedTreatments(AppliedTreatment appliedTreatment)
        {
            double updateBenefitCost = appliedTreatment.TotalBenefitCostRatio;
            for (int j = 0; j < m_listApplyTreatment.Count - 1; j++)
            {
                try
                {
                    if (m_listApplyTreatment[j].TotalBenefitCostRatio <= updateBenefitCost) //This is where this needs to updateTreatment needs to be added.
                    {
                        //Find the old location of this UpdatedTreatment and null it.
                        int found = m_listApplyTreatment.FindIndex(delegate (AppliedTreatment a) { return a == appliedTreatment; });
                        m_listApplyTreatment[found] = null;

                        //If j+1 is available = false or null, just stuff it here.
                        if (m_listApplyTreatment[j] == null || m_listApplyTreatment[j].Available == false)
                        {
                            m_listApplyTreatment[j] = appliedTreatment;
                        }
                        else//pay the penalty and insert it here (we insert so as not to reorder.
                        {
                            m_listApplyTreatment.Insert(j, appliedTreatment);
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }
        }

        private List<AppliedTreatment> GetSingleTreatmentsFromMultiple(AppliedTreatment applyTreatment)
        {
            List<AppliedTreatment> appliedTreatments = null;
            if (applyTreatment.MultipleTreatment == null)
            {
                appliedTreatments = new List<AppliedTreatment>();
            }
            else
            {
                appliedTreatments = GetSingleTreatmentsFromMultiple(applyTreatment.MultipleTreatment);
            }
            appliedTreatments.Add(applyTreatment);
            return appliedTreatments;
        }

        private void SpendBudgetPermits(int nYear, String strBudgetLimit)
        {
            SpendBudgetPermitsRoadCare(nYear, strBudgetLimit);
        }

        /// <summary>
        /// Spend budget with no constraints (Best Benefit Cost)
        /// </summary>
        /// <param name="nYear"></param>
        private void SpendBudgetPermitsRoadCare(int nYear, String strBudgetLimit)
        {
            //Cost inflation calculations
            int nInflationYear = nYear - Investment.StartYear;
            double dRate = Investment.Inflation;
            float fInflationMultiplier = (float)Math.Pow(1 + dRate, (double)nInflationYear);

            String sSectionID;
            String sTreatment;
            String sCost;
            String sConsequenceID;
            String sOutFile;
            String sReasonOutFile;
            String sRemaining;
            String sBenefit;
            String sBC;
            String sRLHash;
            string budgetHash = "";

            TextWriter tw = SimulationMessaging.CreateTextWriter("report_" + m_strSimulationID + ".csv", out sOutFile);
            var reasonReportWriter = SimulationMessaging.CreateTextWriter("reason_" + m_strSimulationID + ".csv", out sReasonOutFile);
            //List of Section with treatment
            // Retrieve Treatments from BC BENEFITCOST_networkid_simulationid
            // in BC ORDER desending.
            int nCommitOrder = 1;
            int nBenefitOrder = 0;

            if (strBudgetLimit != "None")
            {
                //Sort descending
                m_listApplyTreatment.Sort((t1, t2) => t2.BenefitCostRatio.CompareTo(t1.BenefitCostRatio));

                foreach (Priorities priority in m_listPriorities)
                {
                    if (!priority.IsAllYears)
                    {
                        if (priority.Years != nYear) continue;
                    }

                    foreach (String strBudget in Investment.BudgetOrder)
                    {
                        var treatmentsWithBudget = m_listApplyTreatment.Where(t => t.Budget.Contains(strBudget));
                        foreach (var treatment in treatmentsWithBudget)
                        {
                            nBenefitOrder++;
                            sSectionID = treatment.SectionID;
                            sTreatment = treatment.Treatment;
                            var nAny = treatment.Any;
                            var nSame = treatment.Same;
                            sCost = treatment.Cost.ToString();
                            sRemaining = treatment.RemainingLife.ToString();
                            sBenefit = treatment.Benefit.ToString();
                            sBC = treatment.BenefitCostRatio.ToString();
                            sConsequenceID = treatment.TreatmentID;
                            sRLHash = treatment.RemainingLifeHash;
                            //This makes sure the like function does not find something goofy for budgets like ("Greggs Budget", "eggs") -> That one budget it a subset of another.
                            if (!ContainsBudget(treatment.Budget, strBudget)) continue;

                            String strTreatmentID = sConsequenceID;

                            // Get the section that matches this
                            Sections section = m_listSections.Find(delegate (Sections s)
                            {
                                return s.SectionID == sSectionID;
                            });
                            if (section == null)
                                continue; //Should never happen. TODO: Add error handling.

                            //Check if already treated.  Check if treament is in a shadow or will cast a shadow.
                            // Check for negative BC, if its negative.  Dont spend money on it.
                            if (Convert.ToDouble(sBC) < 0)
                            {
                                //SimulationMessaging.AddMessage(new SimulationMessage("Warning! Applying treatment " + sTreatment + " to section " + section.Section + " produces as negative benefit cost. Please evaluate your treatment consequence parameters. Treatment will not be suggested."));
                                //May 16, 2014.  No need to remove this from list.  A continue will work properly here since iterating list.
                                //All subsequent treatments in this budget should have negative benefit, so can be ignored.
                                //Consider replacing with a break;
                                reasonReportWriter.WriteLine(MakeReasonReportLine(section.SectionID, sTreatment, "Negative benefit/cost", strBudget, "", nYear, priority.PriorityLevel, 1, nBenefitOrder, treatment.BenefitCostRatio));
                                continue;
                            }

                            if (!section.IsTreatmentAllowed(sTreatment, nAny.ToString(), nSame.ToString(), nYear))
                            {
                                reasonReportWriter.WriteLine(MakeReasonReportLine(section.SectionID, sTreatment, "Any/Same shadow", strBudget, "", nYear, priority.PriorityLevel, 1, nBenefitOrder, treatment.BenefitCostRatio));
                                continue;
                            }
                            float fAmount = treatment.TreatmentOnlyCost * fInflationMultiplier;
                            string budgetSelected = "";
                            //Checks budget and priorities
                            var limits = GetSplitTreatmentLimits(section.m_hashNextAttributeValue);
                            ISplitTreatmentLimit limit = null;
                            if (strBudgetLimit == "None") //No treatments can be spent
                            {
                                reasonReportWriter.WriteLine(MakeReasonReportLine(section.SectionID, sTreatment, "None selected for analysis budget", strBudget, "", nYear, priority.PriorityLevel, 1, nBenefitOrder, treatment.BenefitCostRatio));
                                continue;
                            }
                            else if (strBudgetLimit == "") // Check and see if money is available
                            {
                            budgetSelected = Investment.IsBudgetAvailable(fAmount, strBudget, nYear.ToString(),
                                    section.m_hashNextAttributeValue, priority, limits, strBudgetLimit, out budgetHash,out limit);
                                if (string.IsNullOrWhiteSpace(budgetSelected))
                                {
                                    reasonReportWriter.WriteLine(MakeReasonReportLine(section.SectionID, sTreatment, "Inadequate budget", strBudget, budgetHash, nYear, priority.PriorityLevel, 1, nBenefitOrder, treatment.BenefitCostRatio));
                                    continue;
                                }
                            }
                            else //if strBudgetLimit == "Unlimited" there is always budget.
                            {
                                budgetSelected = Investment.IsBudgetAvailable(fAmount, strBudget, nYear.ToString(),
                                        section.m_hashNextAttributeValue, priority, limits, strBudgetLimit, out budgetHash, out limit);

                                if (string.IsNullOrWhiteSpace(budgetSelected))
                                {
                                    reasonReportWriter.WriteLine(MakeReasonReportLine(section.SectionID, sTreatment, "Inadequate budget", strBudget, budgetHash, nYear, priority.PriorityLevel, 1, nBenefitOrder, treatment.BenefitCostRatio));
                                    continue;
                                }
                            }

                            reasonReportWriter.WriteLine(MakeReasonReportLine(section.SectionID, sTreatment, "Selected", strBudget, budgetHash, nYear, priority.PriorityLevel, 1, nBenefitOrder, treatment.BenefitCostRatio));

                            //Spend budget (accoreding to limit)
                            int splitYear = 0;
                            foreach (var percentage in limit.Percentages)
                            {
                                var year = nYear + splitYear;
                                Investment.SpendBudget(fAmount * percentage / 100, budgetSelected, year.ToString());
                                splitYear++;
                            }

                            
                            //Mark treated.
                            section.Treated = true;
                            section.AnyYear = nYear + nAny + (limit.Rank - 1);

                            SameTreatment sameTreatment = new SameTreatment();
                            sameTreatment.strTreatment = sTreatment;
                            sameTreatment.nYear = nYear + nSame + (limit.Rank - 1);
                            section.m_listSame.Add(sameTreatment);

                            //Apply consequences.
                            String strChangeHash;
                            Hashtable hashOutput;
                            if (limit.Percentages.Count == 1)//If project is done in one year
                            {
                                hashOutput = ApplyConsequences(section.m_hashNextAttributeValue, strTreatmentID,
                                    out strChangeHash, section); // This is the attribute value pair for this year.

                                //Apply CalculatedAttribute
                                hashOutput = ApplyCalculatedFields(hashOutput);
                            }
                            else //If it takes more than one year
                            {
                                //Consequences are applied in the final year (with no deterioration from start to finish).
                                hashOutput = ApplyCalculatedFields(section.m_hashNextAttributeValue);
                                //Commit split treatment (cash flow).  
                                CommitSplitTreatment(section, nYear, treatment, budgetSelected, limit, fAmount);
                                strChangeHash = "";
                            }

                            
                            //Commit scheduled projects
                            CommitScheduled(section, nYear, strTreatmentID, strBudget,treatment.ScheduledCost);

                            section.m_hashYearAttributeValues.Add(nYear, hashOutput);

                            float fOldArea = section.Area;
                            //Calculate new section area
                            section.CalculateArea(nYear);
                            float fNewArea = section.Area;

                            //Update targets.
                            UpdateTargetsAndDeficiency(nYear, fOldArea, fNewArea, section.m_hashNextAttributeValue,
                                hashOutput);

                            fAmount = fAmount * limit.Percentages[0] / 100;
                            //Write report.
                            String strOut = section.SectionID + ":"
                                                              + nYear.ToString() + ":"
                                                              + sTreatment + ":"
                                                              + nAny + ":"
                                                              + nSame + ":"
                                                              + budgetSelected + ":"
                                                              + fAmount.ToString("f") + ":"
                                                              + sRemaining + ":"
                                                              + sBenefit + ":"
                                                              + sBC + ":"
                                                              + strTreatmentID + ":"
                                                              + priority.PriorityLevel.ToString() + ":" //Priority
                                                              + sRLHash + ":"
                                                              + nCommitOrder.ToString() + ":" //Commit order.
                                                              + "0" + ":" //Committed
                                                              + section.NumberTreatment.ToString() +
                                                              ":" //number viable treatment
                                                              + strChangeHash + ":"
                                                              + fNewArea.ToString() + ":"
                                                              + "0";

                            switch (DBMgr.NativeConnectionParameters.Provider)
                            {
                                case "MSSQL":
                                    // MSSQL needs the leading spot for the ID_ column.
                                    tw.WriteLine(":" + strOut);
                                    break;

                                case "ORACLE":
                                    tw.Write(strOut);
                                    tw.Write("#ORACLEENDOFLINE#");
                                    break;

                                default:
                                    throw new NotImplementedException(
                                        "TODO: Create ANSI implementation for XXXXXXXXXXXX");
                                    //break;
                            }

                            nCommitOrder++;
                        }
                        //If spend across budgets is enabled.  Move money to the next budget.
                        if (Method.UseAcrossBudgets)
                        {
                            Investment.MoveBudgetAcross(strBudget, nYear.ToString(), priority);
                        }
                    }
                }
            }

            //Loop through all SECTIONS and apply no treatment to sections without treatments
            foreach (Sections section in m_listSections)
            {
                if (section.Treated) continue;
                //Apply consequences.
                String strChangeHash;
                Hashtable hashAttributeValue = ApplyConsequences(section.m_hashNextAttributeValue, noTreatments.TreatmentID, out strChangeHash, section);

                //Apply CalculatedAttribute
                hashAttributeValue = ApplyCalculatedFields(hashAttributeValue);

                section.m_hashYearAttributeValues.Add(nYear, hashAttributeValue);

                Hashtable hashRL = new Hashtable();
                double dRemainingLife = 100;
                //foreach (Deteriorate deteriorate in m_listDeteriorate)
                //{
                //    if (SimulationMessaging.Method.IsConditionalRSL)
                //    {
                //        if (!hashRL.Contains(deteriorate.Attribute))
                //        {
                //            hashRL.Add(deteriorate.Attribute, 0); //Add to remain life hash
                //        }
                //        dRemainingLife = 0;
                //    }
                //    else
                //    {
                //        if (deteriorate.IsCriteriaMet(hashAttributeValue))
                //        {
                //            double dRL = 0;

                //            if (deteriorate.CalculateRemainingLife(hashAttributeValue, hashAttributeValue, out dRL))
                //            {
                //                if (!hashRL.Contains(deteriorate.Attribute))
                //                {
                //                    hashRL.Add(deteriorate.Attribute, dRL);
                //                    if (dRL < dRemainingLife)
                //                    {
                //                        dRemainingLife = dRL;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                String strRLHash = SimulationMessaging.NoTreatmentRemainingLife[section.SectionID];

                //Write report.

                String strOut = section.SectionID + ":"
                                        + nYear.ToString() + ":"
                                        + noTreatments.Treatment + ":"
                                        + "0" + ":"
                                        + "0" + ":"
                                        + "" + ":"
                                        + "0" + ":"
                                        + dRemainingLife.ToString("f") + ":"
                                        + "0" + ":"
                                        + "0" + ":"
                                        + noTreatments.TreatmentID + ":"
                                        + "0" + ":"//Priority
                                        + strRLHash + ":"
                                        + "0" + ":" //Commit order.
                                        + "0" + ":" //bool whether committed or not
                                        + section.NumberTreatment.ToString() + ":"
                                        + strChangeHash + ":"
                                        + section.Area.ToString() + ":"
                                        + "0";
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        // MSSQL needs the leading spot for the ID_ column.
                        tw.WriteLine(":" + strOut);
                        break;

                    case "ORACLE":
                        tw.Write(strOut);
                        tw.Write("#ORACLEENDOFLINE#");
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                        //break;
                }
            }
            tw.Close();
            reasonReportWriter.Close();
            _spanAnalysis += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(SimulationMessaging.ReportTable, sOutFile, ':');
                    break;

                case "ORACLE":
                    //throw new NotImplementedException( "TODO: Figure out columns for SpendBudgetPermit" );
                    List<string> reportColumns = DBMgr.GetTableColumns(SimulationMessaging.ReportTable);
                    reportColumns.Remove("ID_");
                    DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, SimulationMessaging.ReportTable, sOutFile, reportColumns, ":", " \"str '#ORACLEENDOFLINE#'\"");
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(SimulationMessaging.ReasonsTable, sReasonOutFile, ',');
                    break;

                case "ORACLE":
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }

            _spanReport += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;
        }

        private void CommitSplitTreatment(Sections section, int year, AppliedTreatment treatment, string budget, ISplitTreatmentLimit limit, float amount)
        {
            for(int i = 1; i < limit.Percentages.Count; i++)
            {
                var commit = new Committed(year+i, year + limit.Percentages.Count - 1, budget,treatment.TreatmentID, amount * limit.Percentages[i] / 100, treatment.Treatment);
                section.YearCommit.Add(commit);
            }
        }



        private List<ISplitTreatmentLimit> GetSplitTreatmentLimits(Hashtable hashAttributeValue)
        {
            
            foreach(var splitTreatment in SimulationMessaging.SplitTreatments)
            {
                if(splitTreatment.Criteria.IsCriteriaMet(hashAttributeValue))
                {
                    return splitTreatment.Limits;
                }
            }
            return null;
        }

        private string MakeReasonReportLine(string sectionId, string treatmentName, string reason, string budget, string budgetHash, int year, int priority, int numberTarget, int benefitOrder, double benefitCost )
        {
            var reasonReportLine = sectionId + ","
                + year + ","
                + treatmentName + "," 
                + reason + ","
                + budget + ","
                + benefitOrder + ","
                + priority + ","
                + benefitCost + ","
                + numberTarget + ","
                + budgetHash;


            return reasonReportLine;
        }

        /// <summary>
        /// Spend budget with no constraints (Best Benefit Cost)
        /// </summary>
        /// <param name="nYear"></param>
        private void SpendSingleSection(int nYear, String sSectionID, Committed singleSection, bool bCommitted)
        {
            //Cost inflation calculations
            int nInflationYear = nYear - Investment.StartYear;
            double dRate = Investment.Inflation;
            float fInflationMultiplier = (float)Math.Pow(1 + dRate, (double)nInflationYear);

            String sTreatment;
            String sAny;
            String sSame;
            String sCost;
            String sConsequenceID;
            //String sOutFile;
            String sRemaining;
            String sBenefit;
            String sBC;
            String sRLHash;
            String strCommitted = "0";
            //List of Section with treatment
            // Retrieve Treatments from BC BENEFITCOST_networkid_simulationid
            // in BC ORDER desending.
            Sections section = m_listSections.Find(delegate (Sections s) { return s.SectionID == sSectionID; });
            if (singleSection != null)
            {
                if (singleSection.IsCommitted) strCommitted = "1";
                sTreatment = singleSection.Treatment;
                sAny = singleSection.Any.ToString();
                sSame = singleSection.Same.ToString();
                sCost = singleSection.Cost.ToString();
                sRemaining = singleSection.RemainingLife;
                sBenefit = singleSection.Benefit;
                sBC = singleSection.BenefitCost;
                sConsequenceID = singleSection.ConsequenceID;//Need to change this to an ID, which should be gotten when creating.
                sRLHash = singleSection.RemainingLifeHash;

                string strBudget = singleSection.Budget;
                if (strBudget != null)
                {
                    string[] budgets = strBudget.Split('|');
                    if (budgets.Length > 0)
                    {
                        strBudget = budgets[0].Trim();
                    }
                }

                //Spend budget.
                if (!singleSection.MultipleYear)
                {
                    float fAmount = float.Parse(sCost);
                    Investment.SpendBudget(fAmount, strBudget, nYear.ToString());
                }

                int nAny = 0;
                int.TryParse(sAny, out nAny);
                //Mark treated.
                section.Treated = true;
                section.AnyYear = nYear + nAny;

                SameTreatment sameTreatment = new SameTreatment();
                sameTreatment.strTreatment = sTreatment;
                int nSame = 0;
                int.TryParse(sSame, out nSame);
                sameTreatment.nYear = nYear + nSame;
                section.m_listSame.Add(sameTreatment);

                //Apply consequences.
                String strChangeHash;
                Hashtable hashOutput = ApplySingleSectionConsequences(section.m_hashNextAttributeValue, singleSection, out strChangeHash);// This is the attribute value pair for this year.
                if (section.m_hashYearAttributeValues.Contains(nYear))
                {
                    section.m_hashYearAttributeValues.Remove(nYear);
                }
                section.m_hashYearAttributeValues.Add(nYear, hashOutput);

                float fOldArea = section.Area;
                //Calculate new section area
                section.CalculateArea(nYear);
                float fNewArea = section.Area;
                //Update targets.
                UpdateTargetsAndDeficiency(nYear, fOldArea, fNewArea, section.m_hashNextAttributeValue, hashOutput);

                ////Insert into REPORT.
                //String strOut = "," + section.SectionID + ","
                //                        + nYear.ToString() + ","
                //                        + sTreatment + ","
                //                        + sAny + ","
                //                        + sSame + ","
                //                        + strBudget + ","
                //                        + fAmount.ToString("f") + ","
                //                        + sRemaining + ","
                //                        + sBenefit + ","
                //                        + sBC + ","
                //                        + strTreatmentID + ","
                //                        + priority.PriorityLevel.ToString() + ","//Priority
                //                        + sRLHash + ","
                //                        + "0" + ","   //Commit order.
                //                        + "0" + "," //Committed
                //                        + section.NumberTreatment.ToString() + "," //number viable treatment
                //                        + strChangeHash + ","
                //                        + fNewArea.ToString();

                String strInsert = "INSERT INTO REPORT_" + m_strNetworkID + "_" + m_strSimulationID + " (SECTIONID,YEARS,TREATMENT,YEARSANY,YEARSSAME,BUDGET,COST_,REMAINING_LIFE,BENEFIT,BC_RATIO,CONSEQUENCEID,PRIORITY,RLHASH,COMMITORDER,ISCOMMITTED,NUMBERTREATMENT,CHANGEHASH,AREA) VALUES ('";
                strInsert += sSectionID + "','";
                strInsert += nYear.ToString() + "','";
                strInsert += singleSection.Treatment.ToString() + "','";
                strInsert += singleSection.Any.ToString() + "','";
                strInsert += singleSection.Same.ToString() + "','";
                strInsert += strBudget + "','";
                strInsert += singleSection.Cost.ToString() + "','";
                strInsert += singleSection.RemainingLife + "','";
                strInsert += singleSection.Benefit + "','";
                strInsert += singleSection.BenefitCost + "','";
                strInsert += singleSection.ConsequenceID + "','";
                strInsert += singleSection.Priority + "','";//Priority
                strInsert += singleSection.RemainingLifeHash + "','";
                strInsert += singleSection.CommitOrder + "','";//Commit order.
                strInsert += strCommitted + "','"; //Committed
                strInsert += section.NumberTreatment + "','";//number viable treatment
                strInsert += strChangeHash + "','";
                strInsert += section.Area.ToString() + "')";
                //Write to database
                DBMgr.ExecuteNonQuery(strInsert);
                if (m_listAttributes.Count > 0)
                {
                    String strUpdate = "UPDATE SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID + " SET ";
                    bool bComma = false;
                    foreach (String attribute in m_listAttributes)
                    {
                        if (attribute != "AREA")
                        {
                            String strYearAttribute = "";
                            if (bComma) strYearAttribute += ",";
                            bComma = true;
                            strYearAttribute += attribute + "_" + nYear + "='" + hashOutput[attribute].ToString() + "'";
                            strUpdate += strYearAttribute;
                        }
                    }
                    strUpdate += " WHERE SECTIONID='" + sSectionID + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception exception)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error update section for YEAR=" + nYear.ToString() + ". " + exception.Message));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Error update section for YEAR=" + nYear.ToString() + ". " + exception.Message);
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                    }
                }
            }
            else
            {
                if (!section.Treated)
                {
                    //Apply consequences.
                    String strChangeHash;
                    Hashtable hashAttributeValue = ApplyConsequences(section.m_hashNextAttributeValue, noTreatments.TreatmentID, out strChangeHash, section);
                    //Apply CalculatedAttribute
                    hashAttributeValue = ApplyCalculatedFields(hashAttributeValue);

                    if (section.m_hashYearAttributeValues.Contains(nYear))
                    {
                        section.m_hashYearAttributeValues.Remove(nYear);
                    }
                    section.m_hashYearAttributeValues.Add(nYear, hashAttributeValue);

                    Hashtable hashRL = new Hashtable();
                    double dRemainingLife = 100;
                    foreach (Deteriorate deteriorate in m_listDeteriorate)
                    {
                        if (deteriorate.IsCriteriaMet(hashAttributeValue))
                        {
                            double dRL = 0;

                            if (deteriorate.CalculateRemainingLife(hashAttributeValue, hashAttributeValue, out dRL))
                            {
                                if (!hashRL.Contains(deteriorate.Attribute))
                                {
                                    hashRL.Add(deteriorate.Attribute, dRL);
                                    if (dRL < dRemainingLife)
                                    {
                                        dRemainingLife = dRL;
                                    }
                                }
                            }
                        }
                    }
                    String strRLHash = this.CreateRemainingLifeHashString(hashRL);

                    //Write report.
                    //String strOut = "," + section.SectionID + ","
                    //                    + nYear.ToString() + ","
                    //                    + noTreatments.Treatment + ","
                    //                    + "0" + ","
                    //                    + "0" + ","
                    //                    + "" + ","
                    //                    + "0" + ","
                    //                    + dRemainingLife.ToString("f") + ","
                    //                    + "0" + ","
                    //                    + "0" + ","
                    //                    + noTreatments.TreatmentID + ","
                    //                    + "0" + ","//Priority
                    //                    + strRLHash + ","
                    //                    + "0" + "," //Commit order.
                    //                    + "0" + "," //bool whether committed or not
                    //                    + section.NumberTreatment.ToString() + ","
                    //                    + strChangeHash + ","
                    //                    + section.Area.ToString();

                    String strInsert = "INSERT INTO REPORT_" + m_strNetworkID + "_" + m_strSimulationID + " (SECTIONID,YEARS,TREATMENT,YEARSANY,YEARSSAME,BUDGET,COST_,REMAINING_LIFE,BENEFIT,BC_RATIO,CONSEQUENCEID,PRIORITY,RLHASH,COMMITORDER,ISCOMMITTED,NUMBERTREATMENT,CHANGEHASH,AREA) VALUES ('";
                    strInsert += sSectionID + "','";
                    strInsert += nYear.ToString() + "','";
                    strInsert += noTreatments.Treatment + "','";
                    strInsert += 0 + "','";
                    strInsert += 0 + "','";
                    strInsert += "" + "','";
                    strInsert += "0" + "','";
                    strInsert += dRemainingLife.ToString("f") + "','";
                    strInsert += "0" + "','";
                    strInsert += "0" + "','";
                    strInsert += noTreatments.TreatmentID + "','";
                    strInsert += "0" + "','";//Priority
                    strInsert += strRLHash + "','";
                    strInsert += "0" + "','";//Commit order.
                    strInsert += "0" + "','"; //Committed
                    strInsert += section.NumberTreatment + "','";//number viable treatment
                    strInsert += strChangeHash + "','";
                    strInsert += section.Area.ToString() + "')";
                    //Write to database
                    DBMgr.ExecuteNonQuery(strInsert);
                    String strUpdate = "UPDATE SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID + " SET ";
                    bool bComma = false;
                    foreach (String attribute in m_listAttributes)
                    {
                        if (attribute != "AREA")
                        {
                            String strYearAttribute = "";
                            if (bComma) strYearAttribute += ",";
                            bComma = true;
                            strYearAttribute += attribute + "_" + nYear + "='" + hashAttributeValue[attribute].ToString() + "'";
                            strUpdate += strYearAttribute;
                        }
                    }
                    strUpdate += " WHERE SECTIONID='" + sSectionID + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception exception)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error update section for YEAR=" + nYear.ToString() + ". " + exception.Message));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Error update section for YEAR=" + nYear.ToString() + ". " + exception.Message);
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                    }
                }
            }
        }

        private Hashtable ApplyCalculatedFields(Hashtable hashInput)
        {
            var hashOutput = new Hashtable();
            foreach (var calculatedattribute in m_listCalculatedAttribute)
            {
                if (!calculatedattribute.Default) continue;
                var calculated = calculatedattribute.Calculate(hashInput);
                if (hashOutput.Contains(calculatedattribute.Attribute))
                {
                    hashOutput[calculatedattribute.Attribute] = calculated;
                }
                else
                {
                    hashOutput.Add(calculatedattribute.Attribute, calculated);
                }
            }

            foreach (var calculatedattribute in m_listCalculatedAttribute)
            {
                if (calculatedattribute.Default) continue;
                if (!calculatedattribute.IsCriteriaMet(hashInput)) continue;
                var calculated = calculatedattribute.Calculate(hashInput);
                if (hashOutput.Contains(calculatedattribute.Attribute))
                {
                    hashOutput[calculatedattribute.Attribute] = calculated;
                }
                else
                {
                    hashOutput.Add(calculatedattribute.Attribute, calculated);
                }
            }

            foreach (string key in hashInput.Keys)
            {
                if (hashOutput.ContainsKey(key)) continue;
                hashOutput.Add(key, hashInput[key]);
            }

            return hashOutput;
        }

        private Hashtable ApplyConsequences(Hashtable hashInput, String strTreatmentID, out String strChangeHash, Sections section)
        {
            strChangeHash = "";

            object sValue;
            Treatments treatment = m_listTreatments.Find((delegate (Treatments tr) { return tr.TreatmentID == strTreatmentID; }));
            Hashtable hashOutput = new Hashtable();

            Hashtable hashConsequences = new Hashtable();
            Consequences compoundConsquences = null;
            foreach (Consequences consequence in treatment.ConsequenceList)
            {
                if (consequence.CompoundTreatment != null)
                {
                    compoundConsquences = consequence;
                }
                if (consequence.Default)
                {
                    if (!hashConsequences.Contains(consequence.Attributes[0]))
                    {
                        hashConsequences.Add(consequence.Attributes[0], consequence);
                    }
                }
                else
                {
                    if (consequence.Criteria.IsCriteriaMet(hashInput))
                    {
                        if (hashConsequences.Contains(consequence.Attributes[0]))
                        {
                            Consequences consequenceIsDefault = (Consequences)hashConsequences[consequence.Attributes[0]];
                            if (consequenceIsDefault.Default == true)
                            {
                                hashConsequences.Remove(consequence.Attributes[0]);
                                hashConsequences.Add(consequence.Attributes[0], consequence);
                            }
                            else
                            {
                                SimulationMessaging.AddMessage(new SimulationMessage("Multiple valid " + consequence.Attributes[0] + " consequences for " + treatment.Treatment));
                                if (APICall.Equals(true))
                                {
                                    var updateStatus = Builders<SimulationModel>.Update
                                    .Set(s => s.status, "Multiple valid " + consequence.Attributes[0] + " consequences for " + treatment.Treatment);
                                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                                }
                            }
                        }
                        else
                        {
                            hashConsequences.Add(consequence.Attributes[0], consequence);
                        }
                    }
                }
            }

            // Get all of this years deteriorated values.
            foreach (String key in hashInput.Keys)
            {
                sValue = hashInput[key];
                if (hashConsequences.Contains(key))
                {
                    Consequences consequence = (Consequences)hashConsequences[key];
                    if (consequence.CompoundTreatment != null) continue;

                    if (consequence.IsEquation)
                    {
                        sValue = consequence.GetConsequence(hashInput);
                        String strPair = consequence.AttributeChange[0].Attribute + "\t" + consequence.Equation + "\n";
                        strChangeHash += strPair;
                    }
                    else
                    {
                        AttributeChange change = consequence.AttributeChange[0];
                        if (change != null)
                        {
                            sValue = change.ApplyChange(sValue);
                        }

                        String strPair = change.Attribute.ToString() + "\t" + change.Change.ToString() + "\n";
                        strChangeHash += strPair;
                    }
                }
                hashOutput.Add(key, sValue);
            }

            if (SimulationMessaging.IsOMS) //Calculate OCI
            {
                double nextOCI = section.CalculateOCI(hashOutput);
                hashOutput["OverallConditionIndex"] = nextOCI;
            }

            //Now apply the the maintenance.
            try
            {
                if (compoundConsquences != null)
                {
                    CompoundTreatment compoundTreatment = Simulation.CompoundTreatments.Find(delegate (CompoundTreatment ct) { return ct.CompoundTreatmentName == compoundConsquences.CompoundTreatment; });
                    Dictionary<string, double> consequenceValues = compoundTreatment.GetConsequences(hashInput);

                    foreach (String key in hashInput.Keys)
                    {
                        sValue = hashInput[key].ToString();
                        if (consequenceValues.ContainsKey(key))
                        {
                            sValue = consequenceValues[key].ToString();
                            String strPair = key + "\t" + sValue + "\n";
                            strChangeHash += strPair;
                            hashOutput.Remove(key);
                        }
                        if (!hashOutput.Contains(key))
                        {
                            hashOutput.Add(key, sValue);
                        }
                    }
                }
            }
            catch
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Compound treatment failed to properly calculate."));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Compound treatment failed to properly calculate");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
            }
            return hashOutput;
        }

        private Hashtable ApplySingleSectionConsequences(Hashtable hashInput, Committed commit, out String strChangeHash)
        {
            strChangeHash = "";
            Hashtable hashOutput = new Hashtable();
            Hashtable hashConsequences = new Hashtable();

            foreach (AttributeChange attributeChange in commit.Consequence.AttributeChange)
            {
                hashConsequences.Add(attributeChange.Attribute, attributeChange);
            }
            // Get all of this years deteriorated values.

            foreach (String key in hashInput.Keys)
            {
                object sValue = hashInput[key];

                if (hashConsequences.Contains(key))
                {
                    AttributeChange change = (AttributeChange)hashConsequences[key];

                    if (change != null && m_dictionaryCommittedEquations.ContainsKey(change.Change))
                    {
                        CommittedEquation ce = m_dictionaryCommittedEquations[change.Change];
                        if (!ce.HasErrors) sValue = ce.GetConsequence(hashInput);
                    }
                    else if (change != null)
                    {
                        sValue = change.ApplyChange(sValue);
                    }

                    String strPair = change.Attribute.ToString() + "\t" + change.Change.ToString() + "\n";
                    strChangeHash += strPair;
                }
                hashOutput.Add(key, sValue);
            }
            return hashOutput;
        }

        private Hashtable ApplyCommittedConsequences(Hashtable hashInput, Committed commit, out String strChangeHash)
        {
            Hashtable hashOutput = new Hashtable();
            strChangeHash = "";
            if (!SimulationMessaging.IsOMS)
            {
                Hashtable hashConsequences = new Hashtable();
                var committedConsequences = m_dictionaryCommittedConsequences[commit.ConsequenceID];

                foreach (AttributeChange attributeChange in committedConsequences)
                {
                    if (SimulationMessaging.AttributeMinimum.Contains(attributeChange.Attribute)) attributeChange.Minimum = SimulationMessaging.AttributeMinimum[attributeChange.Attribute].ToString();
                    if (SimulationMessaging.AttributeMaximum.Contains(attributeChange.Attribute)) attributeChange.Maximum = SimulationMessaging.AttributeMaximum[attributeChange.Attribute].ToString();
                    hashConsequences.Add(attributeChange.Attribute, attributeChange);
                }

                // Get all of this years deteriorated values.
                foreach (String key in hashInput.Keys)
                {
                    object sValue = hashInput[key];

                    if (hashConsequences.Contains(key))
                    {
                        AttributeChange change = (AttributeChange)hashConsequences[key];
                        if (change != null && m_dictionaryCommittedEquations.ContainsKey(change.Change))
                        {
                            CommittedEquation ce = m_dictionaryCommittedEquations[change.Change];
                            if (!ce.HasErrors) sValue = ce.GetConsequence(hashInput);
                        }
                        else
                        {
                            if (change != null)
                            {
                                sValue = change.ApplyChange(sValue);
                            }
                        }
                        String strPair = change.Attribute.ToString() + "\t" + change.Change.ToString() + "\n";
                        strChangeHash += strPair;
                    }
                    hashOutput.Add(key, sValue);
                }
            }
            return hashOutput;
        }

        /// <summary>
        /// Updates targets and deficiency when applying a treatment.
        /// </summary>
        /// <param name="nYear">Year of treatment</param>
        /// <param name="fAreaOld">Area before treatment</param>
        /// <param name="fAreaNew">Area after treatment (new lane)</param>
        /// <param name="hashOldAttributeValue">Old attribute values</param>
        /// <param name="hashNewAttributeValue">New attribute values</param>
        private void UpdateTargetsAndDeficiency(int nYear, float fAreaOld, float fAreaNew, Hashtable hashOldAttributeValue, Hashtable hashNewAttributeValue)
        {
            foreach (String key in TargetHash.Keys)
            {
                if (key != "All" && key != nYear.ToString()) continue;

                Hashtable hashTargetValues = (Hashtable)TargetHash[key];
                foreach (String strAttribute in hashTargetValues.Keys)
                {
                    List<Targets> listTargets = (List<Targets>)hashTargetValues[strAttribute];

                    //Now go over every target and get total area (and deficient area)
                    //Each of these list is for a specific attribute
                    foreach (Targets target in listTargets)
                    {
                        // Remove old hash with old area.

                        if (target.IsAllSections || target.Criteria.IsCriteriaMet(hashOldAttributeValue))
                        {
                            target.SubtractArea(fAreaOld, nYear);
                            if (target.IsTargets)
                            {
                                double dValue = double.Parse(hashOldAttributeValue[target.Attribute].ToString());
                                target.SubtractTargetArea(fAreaOld, dValue, nYear);
                            }

                            if (target.IsDeficient)
                            {
                                double dValue = double.Parse(hashOldAttributeValue[target.Attribute].ToString());
                                if (SimulationMessaging.GetAttributeAscending(target.Attribute))
                                {
                                    //This section is deficient for this criteria
                                    if (target.Deficient > dValue)
                                    {
                                        target.SubtractDeficientArea(fAreaOld, nYear);
                                    }
                                }
                                else
                                {
                                    //This section is deficient for this criteria
                                    if (target.Deficient < dValue)
                                    {
                                        target.SubtractDeficientArea(fAreaOld, nYear);
                                    }
                                }
                            }
                        }

                        //Add new values for new area
                        if (target.IsAllSections || target.Criteria.IsCriteriaMet(hashNewAttributeValue))
                        {
                            target.AddArea(fAreaNew, nYear);
                            if (target.IsTargets)
                            {
                                double dValue = double.Parse(hashNewAttributeValue[target.Attribute].ToString());
                                target.AddTargetArea(fAreaNew, dValue, nYear);
                            }

                            if (target.IsDeficient)
                            {
                                double dValue = double.Parse(hashNewAttributeValue[target.Attribute].ToString());
                                if (SimulationMessaging.GetAttributeAscending(target.Attribute))
                                {
                                    //This section is deficient for this criteria
                                    if (target.Deficient > dValue)
                                    {
                                        target.AddDeficientArea(fAreaNew, nYear);
                                    }
                                }
                                else
                                {
                                    //This section is deficient for this criteria
                                    if (target.Deficient < dValue)
                                    {
                                        target.AddDeficientArea(fAreaNew, nYear);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReportTargetDeficient(int nYear)
        {
            String sAttribute = "";
            String sCriteria = "";
            String sYear = nYear.ToString();
            String sTargetGoal = "";
            String sTarget = "";
            String sDeficiency = "";
            String sDeficiencyGoal = "";
            double dArea = 0;
            double dTarget = 0;
            double dDeficient = 0;
            String sOutFile;

            TextWriter tw = SimulationMessaging.CreateTextWriter("target_" + m_strSimulationID + ".csv", out sOutFile);

            foreach (String key in TargetHash.Keys)
            {
                if (key == "All" || key == nYear.ToString())
                {
                    Hashtable hashAttributeTargets = (Hashtable)TargetHash[key];
                    foreach (String attribute in hashAttributeTargets.Keys)
                    {
                        List<Targets> listTargets = (List<Targets>)hashAttributeTargets[attribute];
                        foreach (Targets target in listTargets)
                        {
                            sAttribute = target.Attribute;
                            sCriteria = target.Criteria.Criteria;

                            if (target.m_hashYearSumArea.Contains(nYear))
                            {
                                dArea = (double)target.m_hashYearSumArea[nYear];
                            }

                            if (target.IsTargets)
                            {
                                sTargetGoal = target.Target.ToString();
                                if (target.m_hashYearTargetArea.Contains(nYear))
                                {
                                    dTarget = (double)target.m_hashYearTargetArea[nYear];
                                }
                                if (dArea != 0)
                                {
                                    dTarget = dTarget / dArea;
                                    sTarget = dTarget.ToString("0.00");
                                }
                                sDeficiency = null;
                                sDeficiencyGoal = null;
                            }

                            if (target.IsDeficient)
                            {
                                sDeficiencyGoal = target.DeficientPercentage.ToString();

                                if (target.m_hashYearDeficientArea.Contains(nYear))
                                {
                                    dDeficient = (double)target.m_hashYearDeficientArea[nYear];
                                }
                                else
                                {
                                    dDeficient = 0;
                                }
                                if (dArea != 0)
                                {
                                    dDeficient = dDeficient / dArea;
                                    sDeficiency = dDeficient.ToString("0.0000");
                                }
                            }
                            String strOut;
                            if (target.IsDeficient)
                                strOut = "," + target.ID.ToString() + "," + sYear + "," + sDeficiency + "," + dArea.ToString() + ",1";
                            else
                                strOut = "," + target.ID.ToString() + "," + sYear + "," + sTarget + "," + dArea.ToString() + ",0";
                            tw.WriteLine(strOut);
                        }
                    }
                }
            }
            tw.Close();

            _spanAnalysis += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;

            SimulationMessaging.TargetTable = cgOMS.Prefix + "TARGET_" + m_strNetworkID + "_" + m_strSimulationID;
            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(SimulationMessaging.TargetTable, sOutFile, ',');
                    break;

                case "ORACLE":
                    //throw new NotImplementedException( "TODO: Figure out tables for ReportTargetDeficient" );
                    List<string> columnNames = new List<string>();
                    columnNames.Add("ID_");
                    columnNames.Add("TARGETID");
                    columnNames.Add("YEARS");
                    columnNames.Add("TARGETMET");
                    columnNames.Add("AREA");
                    columnNames.Add("ISDEFICIENT");

                    DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, SimulationMessaging.TargetTable, sOutFile, columnNames, ",");
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                    //break;
            }

            _spanReport += DateTime.Now - _dateTimeLast;
            _dateTimeLast = DateTime.Now;
        }

        private void LoadCommittedEquations(string simualationID)
        {
            LoadCommittedEquations(simualationID, null);
        }

        private void LoadCommittedEquations(string simulationID, string sectionID)
        {
            m_dictionaryCommittedEquations = new Dictionary<string, CommittedEquation>();
            SimulationMessaging.AddMessage(new SimulationMessage("Compile Committed Project Consequence equations at " + DateTime.Now.ToString("HH:mm:ss")));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Compile Committed Project Consequence equations");
                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }
            string query = "SELECT A.ID_, A.CHANGE_ FROM " + cgOMS.Prefix + "COMMIT_CONSEQUENCES A INNER JOIN " + cgOMS.Prefix + "COMMITTED_ B ON A.COMMITID= B.COMMITID WHERE B.SIMULATIONID='" + simulationID + "' AND A.CHANGE_ LIKE '%]%' ORDER BY A.ID_";
            if (sectionID != null)
            {
                query += " AND B.SECTIONID='" + sectionID + "'";
            }
            DataSet ds = DBMgr.ExecuteQuery(query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string equation = row["CHANGE_"].ToString();
                string id = row["ID_"].ToString();
                if (!m_dictionaryCommittedEquations.ContainsKey(equation))
                {
                    CommittedEquation ce = new CommittedEquation(equation, id);
                    m_dictionaryCommittedEquations.Add(equation, ce);
                    foreach (string attribute in ce.CommittedAttributes)
                    {
                        if (!m_listAttributes.Contains(attribute))
                        {
                            m_listAttributes.Add(attribute);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If OCI weights are included this is an OMS database. Fill
        /// ConditionCategoryWeight and IsOMS appropriately.
        /// </summary>
        private void FillOCIWeights()
        {
            SimulationMessaging.ConditionCategoryWeight = null;
            //Always read OCI Weights to allow OCI performance curves in stand alone RoadCare
            //SimulationMessaging.IsOMS = false;
            if (DBMgr.IsTableInDatabase(cgOMS.Prefix + "OCI_WEIGHTS"))
            {
                string query = "SELECT OCIID,CONDITION_CATEGORY, WEIGHT,CRITERIA FROM " + cgOMS.Prefix + "OCI_WEIGHTS WHERE SIMULATIONID='" + m_strSimulationID + "' ORDER BY CRITERIA";
                DataSet ds = DBMgr.ExecuteQuery(query);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (SimulationMessaging.ConditionCategoryWeight == null)
                    {
                        SimulationMessaging.IsOMS = true;
                        SimulationMessaging.ConditionCategoryWeight = new List<OCIWeight>();
                    }
                    string conditionCategory = row["CONDITION_CATEGORY"].ToString();
                    int weight = Convert.ToInt32(row["WEIGHT"]);
                    String ociID = Convert.ToString(row["OCIID"]);
                    string criteria = null;
                    if (row["CRITERIA"] != DBNull.Value) criteria = row["CRITERIA"].ToString();
                    OCIWeight ociWeight = new OCIWeight(conditionCategory, weight, criteria, ociID);
                    SimulationMessaging.ConditionCategoryWeight.Add(ociWeight);
                }
            }
        }

        public void GetSectionFingerprint(Hashtable attributeValues)
        {
            //List<bool> fingerprint = new List<bool>();

            //foreach (Deteriorate deteriorate in m_listDeteriorate)
            //{
            //    fingerprint.Add(deteriorate.IsCriteriaMet(null));
            //}

            //foreach (Treatments treatment in m_listTreatments)
            //{
            //    fingerprint.Add(treatment.IsTreatmentCriteriaMet(null));
            //    foreach (Costs cost in treatment.CostList)
            //    {
            //        fingerprint.Add(cost.Criteria.IsCriteriaMet(attributeObjects));
            //    }
            //    foreach (Consequences consquence in treatment.ConsequenceList)
            //    {
            //        fingerprint.Add(consquence.Criteria.IsCriteriaMet(attributeObjects));
            //    }
            //}
            //return null;
        }

        private void FillMultipleTreatmentsPerSection(Sections section, Hashtable hashBeforeTreatment, int year)
        {
            bool isDeficient = false;//Is a deficient analysis being performed.
            bool isTarget = false;// Is a targets analysis being performed
            bool isIncludeExclusive = false;//Should we include exclusive treatments.  Only on first pass.

            if (Method.TypeBudget.Contains("Deficient")) isDeficient = true;
            if (Method.TypeBudget.Contains("Target")) isTarget = true;

            AppliedTreatment prerequisiteTreatment = null;
            List<string> previouslyAppliedIDs = null;
            if (section.AppliedTreatments == null)
            {
                section.AppliedTreatments = new List<AppliedTreatment>();
                isIncludeExclusive = true;
            }
            else
            {
                int count = 0;
                count = section.AppliedTreatments.Count;
                if (count > 0)
                {
                    prerequisiteTreatment = section.AppliedTreatments[count - 1];
                    previouslyAppliedIDs = GetPreviouslyApplied(section.AppliedTreatments[count - 1]);
                }
            }

            string changeHash;
            double benefit;
            int remainingLife;
            int numberTargetDeficient = 0;

            foreach (Treatments treatment in m_listTreatments)
            {
                if (treatment.Treatment.ToUpper() == "NO TREATMENT") continue;
                if (treatment.OMSIsExclusive && !isIncludeExclusive) continue;
                if (treatment.OMSIsRepeat) continue; //Never include repeats in feasible treatments.
                if (previouslyAppliedIDs != null && previouslyAppliedIDs.Contains(treatment.TreatmentID)) continue;//This has already been added.  Cannot be added twice.
                if (treatment.IsTreatmentCriteriaMet(hashBeforeTreatment))
                {
                    //System.Diagnostics.Debug.WriteLine(treatment.Treatment);
                    float cost = GetTreatmentCost(section, treatment,year, out int cumulativeCostId,out float treatmentOnlyCost,out Dictionary<string,float> scheduledCost);
                    Hashtable hashAfterTreatment = ApplyConsequences(hashBeforeTreatment, treatment.TreatmentID, out changeHash, section);
                    section.OCI.GetBenefitAndRemainingLife(hashBeforeTreatment, hashAfterTreatment, out benefit, out remainingLife, 30);

                    //Does this treatment help fix a deficiency
                    if (isDeficient) numberTargetDeficient += IsSectionDeficient(section, year, hashAfterTreatment);

                    //Does this section impact a target. (i.e. does its section current value hash meet a target criteria;
                    if (isTarget) numberTargetDeficient += IsSectionTarget(section, year); ;

                    AppliedTreatment appliedTreatment = MakeAppliedTreatment(section, treatment, cost, benefit, (double)remainingLife, numberTargetDeficient);
                    appliedTreatment.MultipleTreatment = prerequisiteTreatment;

                    section.AppliedTreatments.Add(appliedTreatment);
                    if (!treatment.OMSIsExclusive)//Exclusive treatments cannot have extra treatments included.
                    {
                        FillMultipleTreatmentsPerSection(section, hashAfterTreatment, year);//Need to return total benefit cost.
                    }
                }
            }
        }

        private List<string> GetPreviouslyApplied(AppliedTreatment previousTreatment)
        {
            List<string> listID = null;
            if (previousTreatment == null)
            {
                listID = new List<string>();
            }
            else
            {
                AppliedTreatment grandParentTreatment = previousTreatment.MultipleTreatment;
                listID = GetPreviouslyApplied(grandParentTreatment);
                listID.Add(previousTreatment.TreatmentID);
            }
            return listID;
        }

        private AppliedTreatment MakeAppliedTreatment(Sections section, Treatments treatment, float cost, double benefit, double remainingLife, int targetDeficient)
        {
            AppliedTreatment applyTreatment = new AppliedTreatment();
            applyTreatment.SectionID = section.SectionID;
            applyTreatment.Treatment = treatment.Treatment;
            applyTreatment.TreatmentID = treatment.TreatmentID;
            applyTreatment.Cost = cost;
            applyTreatment.Any = treatment.AnyTreatment;
            applyTreatment.Same = treatment.SameTreatment;
            applyTreatment.Budget = treatment.Budget;
            applyTreatment.RemainingLife = remainingLife;
            applyTreatment.Benefit = benefit;

            if (cost > 0) applyTreatment.BenefitCostRatio = benefit / cost;
            else applyTreatment.BenefitCostRatio = 0;

            applyTreatment.NumberTreatmentDeficient = targetDeficient;
            applyTreatment.RemainingLifeHash = "OverallConditionIndex\t" + remainingLife.ToString() + "\n";
            applyTreatment.SelectionWeight = (double)targetDeficient * applyTreatment.BenefitCostRatio;
            applyTreatment.Available = true;
            applyTreatment.IsExclusive = treatment.OMSIsExclusive;
            return applyTreatment;
        }

        private float GetTreatmentCost(Sections section, Treatments treatment, int year, out int cumulativeCostId, out float treatmentOnlyCost, out Dictionary<string,float> scheduledCosts)
        {
            #region cost

            //Find cost.
            float fCost = -1;
            float fDefaultCost = 0;
            float fMostCost = 0;
            float totalCost = 0;
            cumulativeCostId = 0;
            scheduledCosts = new Dictionary<string, float>();
            foreach (Costs cost in treatment.CostList)
            {
                if (cost.Default)
                {
                    if (cost._attributesEquation.Contains("LENGTH"))
                    {
                        if (!section.m_hashNextAttributeValue.Contains("LENGTH"))
                            section.m_hashNextAttributeValue.Add("LENGTH", section.Length);
                    }
                    if (cost._attributesEquation.Contains("AREA"))
                    {
                        if (section.m_hashNextAttributeValue.Contains("AREA"))
                        {
                            section.m_hashNextAttributeValue.Remove("AREA");
                        }
                        section.m_hashNextAttributeValue.Add("AREA", section.Area);
                    }
                    fDefaultCost = (float)cost.GetCost(section.m_hashNextAttributeValue);
                    totalCost += fDefaultCost;
                }
                else
                {
                    if (cost.Criteria.IsCriteriaMet(section.m_hashNextAttributeValue))
                    {
                        if (fCost >= 0 && !Method.UseCumulativeCost)
                        {
                            SimulationMessaging.AddMessage(new SimulationMessage("Muliple cost criteria met for " + treatment.Treatment + " for section " + section.Facility + " " + section.Section + ".  The more expensive (conservative)is used."));
                            if (APICall.Equals(true))
                            {
                                var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, "Muliple cost criteria met for " + treatment.Treatment + " for section " + section.Facility + " " + section.Section);
                                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                            }
                        }
                        if (cost._attributesEquation.Contains("LENGTH"))
                        {
                            if (!section.m_hashNextAttributeValue.Contains("LENGTH"))
                                section.m_hashNextAttributeValue.Add("LENGTH", section.Length);
                        }
                        if (cost._attributesEquation.Contains("AREA"))
                        {
                            if (section.m_hashNextAttributeValue.Contains("AREA"))
                            {
                                section.m_hashNextAttributeValue.Remove("AREA");
                            }
                            section.m_hashNextAttributeValue.Add("AREA", section.Area);
                        }
                        fCost = (float)cost.GetCost(section.m_hashNextAttributeValue);
                        totalCost += fCost;

                        // fCost = cost.Cost;
                        if (fCost > fMostCost)
                        {
                            fMostCost = fCost;
                        }
                    }
                }
            }

            if (Method.UseCumulativeCost)
            {
                fCost = totalCost;
            }
            else
            {
                if (fCost <= 0) fCost = fDefaultCost;
                else fCost = fMostCost; //Otherwise use most expensive (conservative).
            }

            if (section.Area <= 0)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Warning:Facility(" + section.Facility + ")Section(" + section.Section + ") AREA is equal to zero and Benefit/Cost is infinite. Section ignored."));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Warning:Facility(" + section.Facility + ")Section(" + section.Section + ") AREA is zero and Benefit/Cost is infinite. Section ignored");

                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                treatmentOnlyCost = 0;
                return 0;
            }
            else
            {
                treatmentOnlyCost = fCost;
                fCost = fCost / section.Area;
            }

            //Now recursively call Scheduled treatments
            foreach (var scheduled in treatment.Scheduleds)
            {
                fCost += GetTreatmentCost(section, scheduled.Treatment, year, out int dummy, out float scheduledCost, out Dictionary<string, float> recursedScheduledCost);
                int scheduledYear = year + scheduled.ScheduledYear;
                var key = scheduled.Treatment.TreatmentID + "|" + scheduledYear;
                if(!scheduledCosts.ContainsKey(key))
                {
                    scheduledCosts.Add(key, scheduledCost);
                }
                foreach(var recursedKey in recursedScheduledCost.Keys)
                {
                    if(!scheduledCosts.ContainsKey(recursedKey))
                    {
                        scheduledCosts.Add(recursedKey, recursedScheduledCost[recursedKey]);
                    }
                }
            }

            #endregion cost

            return fCost;
        }

        #region OMS UPDATE SIMULATION

        public void UpdateSimulation()
        {
            SimulationMessaging.AddMessage(new SimulationMessage("Beginning simulation update." + DateTime.Now.ToLongTimeString()));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Beginning simulation update");

                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }
            if (this.ActionOMS != "Commit")
            {
                SetSimulationTables();
                FillOCIWeights();//This checks if this is an OMS analysis (and fills ConditionCategoryWeight if it is).
                SimulationMessaging.LoadAttributes(m_strSimulationID);
                if (!GetSimulationMethod()) return;
                GetSimulationAttributes();
                List<ReportOMS> omsReports = GetExistingSectionTreatments();
                ApplyOMSActions(omsReports);
                RecalculateSectionFromReport(omsReports);
            }
            else
            {
                CommitOMSActivity(false);
            }
            SimulationMessaging.AddMessage(new SimulationMessage("Complete simulation update." + DateTime.Now.ToLongTimeString()));
            if (APICall.Equals(true))
            {
                var updateStatus = Builders<SimulationModel>.Update
                .Set(s => s.status, "Completed simulation update");

                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
            }
            Thread.Sleep(1000);
        }

        private void SetSimulationTables()
        {
            SimulationMessaging.SimulationID = m_strSimulationID;
            SimulationMessaging.ReportTable = cgOMS.Prefix + "REPORT_1_" + m_strSimulationID;
            SimulationMessaging.SimulationTable = cgOMS.Prefix + "SIMULATION_1_" + m_strSimulationID;
            SimulationMessaging.BenefitCostTable = cgOMS.Prefix + "BENEFITCOST_1_" + m_strSimulationID;
            SimulationMessaging.TargetTable = cgOMS.Prefix + "TARGETS_1_" + m_strSimulationID;
        }

        private List<ReportOMS> GetExistingSectionTreatments()
        {
            string reportTable = cgOMS.Prefix + "REPORT_" + m_strNetworkID + "_" + m_strSimulationID;
            List<ReportOMS> omsReports = new List<ReportOMS>();
            string select = "SELECT SECTIONID, YEARS, TREATMENT, YEARSANY, YEARSSAME, BUDGET,COST_,COMMITORDER, CHANGEHASH, AREA, RESULT_TYPE FROM " + reportTable + " WHERE SECTIONID='" + m_strSectionID + "' ORDER BY YEARS";
            DataSet ds = DBMgr.ExecuteQuery(select);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ReportOMS omsReport = new ReportOMS();
                omsReport.SectionID = row["SECTIONID"].ToString();
                omsReport.Year = Convert.ToInt32(row["YEARS"]);
                omsReport.Treatment = row["TREATMENT"].ToString();
                omsReport.YearsAny = Convert.ToInt32(row["YEARSANY"]);
                omsReport.YearsSame = Convert.ToInt32(row["YEARSSAME"]);
                if (row["BUDGET"] != DBNull.Value) omsReport.Budget = row["BUDGET"].ToString();
                omsReport.Cost = Convert.ToDouble(row["COST_"]);
                omsReport.CommitOrder = Convert.ToInt32(row["COMMITORDER"]);
                if (row["CHANGEHASH"] != DBNull.Value) omsReport.ChangeHash = row["CHANGEHASH"].ToString();
                omsReport.Area = Convert.ToDouble(row["AREA"]);
                omsReport.ResultType = Convert.ToInt32(row["RESULT_TYPE"]);

                omsReports.Add(omsReport);
            }
            //AddUnassignedFeasible(omsReports);
            //REMOVE ALL OF THIS SECTION FROM REPORT_. WILL REBUILD
            string deleteSectionID = "DELETE FROM " + reportTable + " WHERE SECTIONID='" + m_strSectionID + "'";
            DBMgr.ExecuteNonQuery(deleteSectionID);

            return omsReports;
        }

        private void AddUnassignedFeasible(List<ReportOMS> omsReports)
        {
            string benefitTable = cgOMS.Prefix + "BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID;
            string select = "SELECT DISTINCT YEARS, TREATMENT,BUDGET FROM " + benefitTable + " WHERE SECTIONID='" + m_strSectionID + "' AND OMS_IGNORE <>'1'";
            DataSet ds = DBMgr.ExecuteQuery(select);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string treatment = row["TREATMENT"].ToString();
                int year = Convert.ToInt32(row["YEARS"]);
                string budget = row["BUDGET"].ToString();
                ReportOMS omsReport = omsReports.Find(delegate (ReportOMS r) { return r.Treatment == treatment && r.Year == year; });
                if (omsReport == null)
                {
                    omsReport = new ReportOMS();
                    omsReport.Treatment = treatment;
                    omsReport.Year = year;
                    omsReport.Budget = budget;
                    omsReport.ResultType = 2;
                    omsReports.Add(omsReport);
                }
            }
        }

        private void ApplyOMSActions(List<ReportOMS> omsReports)
        {
            ReportOMS omsReport = null;

            AddIgnoreToBenefitCost(omsReports);

            switch (this.ActionOMS)
            {
                case "Delete":
                    omsReports.RemoveAll(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS; });
                    DeleteCommittedOMSActivity();
                    break;

                case "Commit":
                    DeleteCommittedOMSActivity();
                    omsReport = omsReports.Find(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS; });
                    if (omsReport != null)
                    {
                        omsReport.ResultType = 1;
                    }
                    break;

                case "Add":
                    DeleteCommittedOMSActivity();
                    omsReports.RemoveAll(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS; }); //Remove any treatments with this name.
                    omsReports.RemoveAll(delegate (ReportOMS r) { return r.Treatment == "No Treatment" && r.Year == this.YearOMS; }); //Remove any "No Treatment" activities.
                    omsReport = new ReportOMS();
                    omsReport.Treatment = this.TreatmentOMS;
                    omsReport.ResultType = 5;
                    omsReport.Year = this.YearOMS;
                    omsReports.Add(omsReport);
                    break;

                case "AddDoNotAllow":
                    DeleteCommittedOMSActivity();
                    CommitOMSActivity(true);
                    omsReports.RemoveAll(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS; });
                    omsReport = new ReportOMS();
                    omsReport.Treatment = this.TreatmentOMS;
                    omsReport.ResultType = 4;
                    omsReport.Year = this.YearOMS;
                    omsReports.Add(omsReport);
                    break;

                case "Move":
                    DeleteCommittedOMSActivity();
                    omsReports.RemoveAll(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == Convert.ToInt32(this.ValueOMS); });//Remove treatments from where this is going.
                    omsReport = omsReports.Find(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS; });
                    omsReport.ResultType = 5;
                    omsReport.Year = Convert.ToInt32(this.ValueOMS);//Changing the year of the source effectivily moves it
                    break;

                case "DoNotAllow":
                    DeleteCommittedOMSActivity();
                    omsReport = omsReports.Find(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS; });
                    omsReport.ResultType = 4;
                    break;

                case "ChangeBudget":
                    omsReport = omsReports.Find(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS; });
                    omsReport.Budget = this.ValueOMS;
                    break;
            }
        }

        private void AddIgnoreToBenefitCost(List<ReportOMS> omsReports)
        {
            ReportOMS omsReport = omsReports.Find(delegate (ReportOMS r) { return r.Treatment == this.TreatmentOMS && r.Year == this.YearOMS && r.ResultType == 2; });
            if (omsReport != null)
            {
                string benefitTable = cgOMS.Prefix + "BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID;
                string update = "UPDATE " + benefitTable + " SET OMS_IGNORE='1' WHERE SECTIONID='" + m_strSectionID + "' AND TREATMENT='" + this.TreatmentOMS + "' AND YEARS='" + this.YearOMS.ToString() + "'";
                DBMgr.ExecuteNonQuery(update);
            }
        }

        private void RecalculateSectionFromReport(List<ReportOMS> omsReports)
        {
            Hashtable hashRollForward = GetRolledForward();
            Sections section = new Sections(m_strSectionID);

            foreach (OCIWeight weight in SimulationMessaging.ConditionCategoryWeight)
            {
                if (weight.Evaluate.IsCriteriaMet(hashRollForward))
                {
                    section.OCI = new OverallConditionIndex(weight.Criteria);
                }
            }

            section.m_hashYearAttributeValues.Add(0, hashRollForward);
            section.m_hashYearAttributeValues.Add(this.Investment.StartYear, hashRollForward);
            section.CalculateArea(0);//Calculate area from roll forward.
            m_listSections.Add(section);
            m_dictionaryCommittedEquations = new Dictionary<string, CommittedEquation>();
            for (int year = this.Investment.StartYear; year < this.Investment.StartYear + this.Investment.AnalysisPeriod; year++)
            {
                ApplyDeterioration(year);
            }
            UpdateSimulationTable(section.m_hashYearAttributeValues);
            UpdateTargetsTable();
        }

        private void UpdateSimulationTable(Hashtable hashYearAttributeValues)
        {
            string update = "UPDATE " + cgOMS.Prefix + "SIMULATION_1_" + m_strSimulationID + " SET ";
            bool bFirstUpdate = true;
            for (int year = m_Investment.StartYear; year < m_Investment.StartYear + m_Investment.AnalysisPeriod; year++)
            {
                Hashtable hashAttribute = (Hashtable)hashYearAttributeValues[year];
                foreach (string attribute in m_listAttributes)
                {
                    if (!bFirstUpdate)
                    {
                        update += ",";
                    }
                    bFirstUpdate = false;
                    update += " [" + attribute + "_" + year + "]='" + hashAttribute[attribute] + "'";
                }
            }
            update += " WHERE SECTIONID='" + m_strSectionID + "'";
            DBMgr.ExecuteNonQuery(update);
        }

        private void UpdateTargetsTable()
        {
            Dictionary<string, double> sectionArea = new Dictionary<string, double>();
            Dictionary<string, Dictionary<int, double>> sectionYearOCI = new Dictionary<string, Dictionary<int, double>>();
            string simulationTable = cgOMS.Prefix + "SIMULATION_1_" + m_strSimulationID;
            int firstYear = m_Investment.StartYear;
            string select = "SELECT SECTIONID";
            //Loop years
            for (int year = m_Investment.StartYear; year < m_Investment.StartYear + m_Investment.AnalysisPeriod; year++)
            {
                select += ",";
                select += "OverallConditionIndex_" + year.ToString();
            }
            select += " FROM " + simulationTable + " ORDER BY SECTIONID";

            DataSet ds = DBMgr.ExecuteQuery(select);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Dictionary<int, double> yearOCI = new Dictionary<int, double>();
                for (int year = m_Investment.StartYear; year < m_Investment.StartYear + m_Investment.AnalysisPeriod; year++)
                {
                    double oci = Convert.ToDouble(row["OverallConditionIndex_" + year.ToString()]);
                    yearOCI.Add(year, oci);
                }
                string sectionID = row["SECTIONID"].ToString();
                sectionYearOCI.Add(sectionID, yearOCI);
            }
            //Get dictionary of areas
            string reportTable = cgOMS.Prefix + "REPORT_1_" + m_strSimulationID;
            select = "SELECT DISTINCT SECTIONID,  AREA FROM " + reportTable + " WHERE YEARS='" + m_Investment.StartYear.ToString() + "' ORDER BY SECTIONID";

            DataSet dsArea = DBMgr.ExecuteQuery(select);
            foreach (DataRow row in dsArea.Tables[0].Rows)
            {
                string sectionID = row["SECTIONID"].ToString();
                double area = Convert.ToDouble(row["AREA"]);
                sectionArea.Add(sectionID, area);
            }

            for (int year = m_Investment.StartYear; year < m_Investment.StartYear + m_Investment.AnalysisPeriod; year++)
            {
                double sumArea = 0;
                double sumOCIArea = 0;
                foreach (string key in sectionYearOCI.Keys)
                {
                    Dictionary<int, double> yearOCI = sectionYearOCI[key];
                    if (sectionArea.ContainsKey(key) && yearOCI.ContainsKey(year))
                    {
                        sumArea += sectionArea[key];
                        sumOCIArea += sectionArea[key] * yearOCI[year];
                    }
                }
                double ociTarget = sumOCIArea / sumArea;
                //Update target table
                string oci = ociTarget.ToString();

                if (TargetHash != null)
                {
                    if (TargetHash.ContainsKey(year.ToString()))
                    {
                        Hashtable hashYearTargets = (Hashtable)TargetHash[year.ToString()];
                        if (hashYearTargets.ContainsKey("OverallConditionIndex"))
                        {
                            List<Targets> targets = (List<Targets>)hashYearTargets["OverallConditionIndex"];
                            if (targets != null && targets.Count > 0)
                            {
                                string update = "UPDATE " + cgOMS.Prefix + "TARGET_" + m_strNetworkID + "_" + m_strSimulationID + " SET TARGETMET='" + oci.ToString() + "' WHERE TARGETID='" + targets[0].ID + "'";
                                DBMgr.ExecuteNonQuery(update);
                            }
                        }
                    }
                }
            }
        }

        private Hashtable GetRolledForward()
        {
            Hashtable hashRollForward = new Hashtable();
            string select = "SELECT ";
            foreach (string attribute in m_listAttributes)
            {
                if (select.Length > 7) select += ",";
                select += "[" + attribute + "_0]";
            }
            select += " FROM " + cgOMS.Prefix + "SIMULATION_1_" + m_strSimulationID + " WHERE SECTIONID='" + m_strSectionID + "'";
            DataSet ds = DBMgr.ExecuteQuery(select);
            DataRow row = ds.Tables[0].Rows[0];

            foreach (string attribute in m_listAttributes)
            {
                string type = SimulationMessaging.GetAttributeType(attribute);
                object value = row[attribute + "_0"];
                if (value != DBNull.Value)
                {
                    if (type == "NUMBER")
                    {
                        value = Convert.ToDouble(value);
                    }
                    else if (SimulationMessaging.GetAttributeType(attribute) == "DATETIME")
                    {
                        value = Convert.ToDateTime(value);
                    }
                    else
                    {
                        value = value.ToString();
                    }
                }
                else
                {
                    value = null;
                }
                hashRollForward.Add(attribute, value);
            }

            return hashRollForward;
        }

        private bool RemoveYearFromTargetsOMS(int nYear)
        {
            String strDelete = "DELETE FROM TARGET_" + m_strNetworkID + "_" + m_strSimulationID + " WHERE YEARS >='" + nYear.ToString() + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Deleting year from TARGET table. " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Error: Deleting year from TARGET table: " + exception.Message);

                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }
            return true;
        }

        private void ApplyOMSReport(int nYear, List<ReportOMS> omsReports, Sections section)
        {
            String strBudget;
            float fAmount;
            String sOutFile;
            //RemoveYearFromTargetsOMS(nYear); //Remove previous targets
            List<Committed> currentYearCommitted = new List<Committed>();
            Dictionary<Committed, ReportOMS> committedOMSReport = new Dictionary<Committed, ReportOMS>();
            List<int> resultOrder = new List<int> { 4, 1, 3, 5, 0 };//(0) Recommended, (1) Committed, (2) Feasible, (3) Repeat, (4) Do not allow , (5) Added
            foreach (ReportOMS omsReport in omsReports)
            {
                foreach (int resultType in resultOrder)
                {
                    if (omsReport.Year == nYear && omsReport.ResultType == resultType)
                    {
                        Treatments treatment = m_listTreatments.Find(delegate (Treatments t) { return t.Treatment == omsReport.Treatment; });
                        //Create a committed from OMSReport
                        Committed commit = new Committed();
                        commit.Consequence = new Consequences();
                        commit.Treatment = omsReport.Treatment;
                        commit.Same = omsReport.YearsSame;
                        commit.Any = omsReport.YearsAny;
                        commit.Budget = omsReport.Budget;
                        commit.ResultType = omsReport.ResultType;
                        commit.Cost = (float)omsReport.Cost;
                        commit.CommitOrder = omsReport.CommitOrder.ToString();
                        //Store the ID to the committed project, perform the select to get the Consequence when needed.  Saves storage space.
                        commit.ConsequenceID = null;
                        if (resultType == 3) commit.IsRepeat = true;
                        else commit.IsRepeat = false;
                        commit.Year = omsReport.Year;

                        if (resultType == 4) commit.OMSIsNotAllowed = true;
                        else commit.OMSIsNotAllowed = false;

                        commit.OMSIsExclusive = treatment.OMSIsExclusive;
                        commit.OMSTreatment = treatment;
                        commit.Any = commit.OMSTreatment.AnyTreatment;
                        commit.Same = commit.OMSTreatment.SameTreatment;
                        commit.Budget = commit.OMSTreatment.Budget;

                        currentYearCommitted.Add(commit);
                        committedOMSReport.Add(commit, omsReport);
                    }
                }
            }

            if (currentYearCommitted.Count == 0)
            {
                Treatments treatment = m_listTreatments.Find(delegate (Treatments t) { return t.Treatment == "No Treatment"; });
                Committed commit = new Committed();
                commit.Consequence = new Consequences();
                commit.Treatment = "No Treatment";
                commit.OMSTreatment = treatment;
                commit.Same = 0;
                commit.Any = 0;
                commit.Budget = null;
                commit.ResultType = 0;
                commit.Cost = (float)0;
                commit.Year = nYear;
                commit.IsRepeat = false;
                commit.IsCommitted = false;
                commit.OMSIsExclusive = false;
                commit.OMSIsNotAllowed = false;
                currentYearCommitted.Add(commit);
            }

            TextWriter tw = SimulationMessaging.CreateTextWriter("report_" + m_strSimulationID + ".csv", out sOutFile);
            foreach (Committed commit in currentYearCommitted)
            {
                if (currentYearCommitted.Count > 1 && commit.Treatment == "No Treatment") continue;

                if (!section.IsOMSCommitAllowed(commit.Treatment, nYear)) continue;

                String strChangeHash = "";
                Hashtable hashOutput = section.CommitProject(commit, out strBudget, out fAmount, m_dictionaryCommittedEquations, out strChangeHash);
                section.OCI.UpdateApparentAge(hashOutput);

                Hashtable hashInput = (Hashtable)section.m_hashNextAttributeValue;

                if (section.m_hashYearAttributeValues.Contains(nYear))
                {
                    section.m_hashYearAttributeValues.Remove(nYear);
                }
                section.m_hashYearAttributeValues.Add(nYear, hashOutput);

                //Calculate remaining life and incremental BC for the new values.
                //Calculate new deterioration using the new hashtable
                double dBenefit = 0;
                double dRemainingLife = 100;
                Hashtable hashRL = new Hashtable();

                //Hashtable hash = (Hashtable)section.m_hashYearAttributeValues[nYear];
                foreach (Deteriorate deteriorate in m_listDeteriorate)
                {
                    if (deteriorate.IsCriteriaMet(hashOutput))
                    {
                        bool bOutOfRange;
                        deteriorate.IterateOneYear(hashOutput, out bOutOfRange);
                        if (Method.IsBenefitCost && Method.BenefitAttribute == deteriorate.Attribute)
                        {
                            dBenefit = deteriorate.CalculateBenefit(hashOutput);
                        }
                        double dRL = 0;
                        if (deteriorate.CalculateRemainingLife(hashOutput, hashInput, out dRL))
                        {
                            if (!hashRL.Contains(deteriorate.Attribute))
                            {
                                hashRL.Add(deteriorate.Attribute, dRL);
                            }
                            else
                            {
                                double dRLOld = (double)hashRL[deteriorate.Attribute];
                                if (dRL < dRLOld)
                                {
                                    hashRL.Remove(deteriorate.Attribute);
                                    hashRL.Add(deteriorate.Attribute, dRL);
                                }
                            }
                            if (dRL < dRemainingLife)
                            {
                                dRemainingLife = dRL;
                            }
                        }
                    }
                }

                Investment.SpendBudget(fAmount, strBudget, nYear.ToString());

                float fCost = fAmount / section.Area;

                double deltaBenefit = dBenefit - section.BaseBenefit;
                double deltaRemainingLife = dRemainingLife - section.RemainingLife;

                String strRLHash = this.CreateRemainingLifeHashString(hashRL);
                double dBCRatio;
                if (Method.IsBenefitCost)
                {
                    if (fCost > 0)
                    {
                        dBCRatio = deltaBenefit / (double)fCost;
                    }
                    else
                    {
                        dBCRatio = 0;
                    }
                }
                else
                {
                    if (fCost > 0)
                    {
                        dBCRatio = deltaRemainingLife / (double)fCost;
                    }
                    else
                    {
                        dBCRatio = 0;
                    }
                }

                SameTreatment sameTreatment = new SameTreatment();
                sameTreatment.strTreatment = commit.Treatment;
                sameTreatment.nYear = nYear;
                sameTreatment.isExclusive = commit.OMSIsExclusive;
                sameTreatment.isNotAllowed = commit.OMSIsNotAllowed;
                section.m_listSame.Add(sameTreatment);

                // No need to Update targets and update deficiency. They arcalculated after committed
                //At this point have Benefit/RL, Base Benefit/RL, Cost, ConsquenceID
                //Calculate B/C or RL/C or both (RL*B)/C
                //Insert in Report table with Batch  Load.
                String strOut = "," + section.SectionID + ","
                                + nYear.ToString() + ","
                                + commit.Treatment + ","
                                + commit.Any.ToString() + ","
                                + commit.Same.ToString() + ","
                                + strBudget + ","
                                + fAmount.ToString("f") + ","
                                + deltaRemainingLife.ToString("f") + ","
                                + deltaBenefit.ToString("f") + ","
                                + dBCRatio.ToString("f") + ","
                                + commit.Consequence.CommitID + ","
                                + "0" + ","//Priority
                                + strRLHash + ","
                                + commit.CommitOrder + ","//Commit order.
                                + "1" + "," //Committed
                                + "0" + ","
                                + strChangeHash + ","  //number viable treatment
                                + section.Area.ToString() + ","
                                + commit.ResultType;
                //if (commit.OMSIsNotAllowed)
                //{
                //    strOut += "4";//Not allowed.
                //}
                //else if (commit.IsRepeat)
                //{
                //    strOut += "3"; //Committed result type.
                //}
                //else
                //{
                //    strOut += "1"; //Committed result type.
                //}
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        tw.WriteLine(strOut);
                        break;

                    default:
                        throw new NotImplementedException("TODO: Implement ANSI version of ApplyCommitted()");
                        //break;
                }
            }
        }


        private void CommitOMSActivity(bool isNotAllowed)
        {
            int notAllowed = 0;
            if (isNotAllowed) notAllowed = 1;

            string insert = "INSERT INTO " + cgOMS.Prefix + "COMMITTED_ (SIMULATIONID,SECTIONID,YEARS,TREATMENTNAME,OMS_IS_NOT_ALLOWED) VALUES ('" + m_strSimulationID + "','" + m_strSectionID + "','" + this.YearOMS.ToString() + "','" + this.TreatmentOMS + "','" + notAllowed.ToString() + "')";
            DBMgr.ExecuteNonQuery(insert);

            string update = "UPDATE " + cgOMS.Prefix + "REPORT_1_" + m_strSimulationID + " SET RESULT_TYPE='1' WHERE SECTIONID='" + m_strSectionID + "' AND YEARS='" + this.YearOMS + "'";
            DBMgr.ExecuteNonQuery(update);
        }

        private void DeleteCommittedOMSActivity()
        {
            //Delete if already committed or committing a second time.
            string delete = "DELETE FROM " + cgOMS.Prefix + "COMMITTED_ WHERE SIMULATIONID='" + m_strSimulationID + "' AND SECTIONID='" + m_strSectionID + "' AND YEARS='" + this.YearOMS + "'";
            DBMgr.ExecuteNonQuery(delete);
        }

        #endregion OMS UPDATE SIMULATION
    }
}