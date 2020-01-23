using System;
using System.Collections.Generic;
using DatabaseManager;
using System.Data;
using System.Collections;
using RoadCareGlobalOperations;
using System.IO;
using Simulation.Interface;
using MongoDB.Driver;
using static Simulation.Simulation;

namespace Simulation
{
    /// <summary>
    /// Stores information for a single RoadCare Treatment include feasibility, cost and consequences.
    /// </summary>
    public class Treatments
    {
        private String _treatment;
        private String _budget;
        private String _treatmentID;
        private int _anyTreatment;
        private int _sameTreatment;

        private List<Criterias> _criterias = new List<Criterias>();
        private List<Costs> _costs = new List<Costs>();
        private List<Consequences> _consequences = new List<Consequences>();
        private List<String> _attributes = new List<String>();
        private List<IScheduled> _scheduleds = new List<IScheduled>();
        private List<ISupersede> _supersedes = new List<ISupersede>();
        private List<string> m_listBudget = null;

        //Values for caching dll's
        private string _table;
        private string _column;
        private string _id;

        private bool _isExclusive;

        private bool _omsIsRepeat;
        private int _omsRepeatStart;
        private int _omsRepeatInterval;

        /// <summary>
        /// TreatmentID in which Treatment resides.
        /// </summary>
        public String TreatmentID
        {
            get { return _treatmentID; }
            set { _treatmentID = value; }

        }

        /// <summary>
        /// Treatment name.  Unique per Treatment set.
        /// </summary>
        public String Treatment
        {
            get { return _treatment; }
            set { _treatment = value; }

        }
        /// <summary>
        /// User defined budget category. Unlimited.
        /// </summary>
        public String Budget
        {
            get { return _budget; }
            set 
            {
                _budget = value.Replace(',','|');
                m_listBudget = new List<string>();
                string[] budgets = _budget.Split('|');
                foreach (string budget in budgets)
                {
                    m_listBudget.Add(budget.Trim());
                }
            }
        }

        public List<string> BudgetList
        {
            get { return m_listBudget; }
        }


        /// <summary>
        /// Integer number of years before any treatment can occcur.
        /// </summary>
        public int AnyTreatment
        {
            get { return _anyTreatment; }
            set { _anyTreatment = value; }
        }

        /// <summary>
        /// Integer number of years before the same treatment can occur.
        /// </summary>
        public int SameTreatment
        {
            get { return _sameTreatment; }
            set { _sameTreatment = value; }
        }

        /// <summary>
        /// List of Criteria which will allow this treatment to occur.
        /// </summary>
        public List<Criterias> CriteriaList
        {
            get { return _criterias; }

        }

        /// <summary>
        /// List of Costs which go with this Treatment
        /// </summary>
		public List<Costs> CostList
		{
			get
			{
				return _costs;
			}
		}


        /// <summary>
        /// List of Consequences which go with this Treatment
        /// </summary>
        public List<Consequences> ConsequenceList
        {
            get { return _consequences; }
        }

        public bool OMSIsExclusive
        {
            get { return _isExclusive; }
            set { _isExclusive = value; }
        }

        public bool OMSIsRepeat
        {
            get { return _omsIsRepeat; }
            set { _omsIsRepeat = value; }
        }

        public int OMSRepeatInterval
        {
            get { return _omsRepeatInterval; }
            set { _omsRepeatInterval = value; }
        }

        public int OMSRepeatStart
        {
            get { return _omsRepeatStart; }
            set { _omsRepeatStart = value; }
        }

        /// <summary>
        /// Add a new Criteria to the list of criterias which allow this treatment.
        /// </summary>
        /// <param name="criteria"></param>
        public void AddCriteria(Criterias criteria)
        {
            _criterias.Add(criteria);
        }

        /// <summary>
        /// Add a new Costs object to this treatment
        /// </summary>
        /// <param name="cost"></param>
        public void AddCost(Costs cost)
        {
            _costs.Add(cost);
        }


        /// <summary>
        /// Add a list of Consequences for this treatment
        /// </summary>
        /// <param name="consequence"></param>
        public void AddConsequence(Consequences consequence)
        {
            _consequences.Add(consequence);
        }

        /// <summary>
        /// List of Attributes associated with this treatment
        /// </summary>
        /// <param name="consequence"></param>
        public List<String> Attributes
        {
            get{return _attributes;}
        }

        public List<IScheduled> Scheduleds
        {
            get { return _scheduleds; }
        }

        public List<ISupersede> Supersedes
        {
            get { return _supersedes; }
        }


        /// <summary>
        /// Loads all feasibility criteria for this treatment in for this treatment ID.
        /// </summary>
        public bool LoadFeasibility(object APICall, IMongoCollection<SimulationModel> Simulations, string m_strSimulationID)
        {
            String strSelect = "SELECT CRITERIA,BINARY_CRITERIA,FEASIBILITYID FROM " + cgOMS.Prefix + "FEASIBILITY WHERE TREATMENTID='" + this.TreatmentID + "'";
            _table = cgOMS.Prefix + "FEASIBILITY";
            _column = "BINARY_CRITERIA";
            
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch(Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Opening Treatment FEASIBILITY table.  SQL Message - " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Fatal Error: Opening Treatment FEASIBILITY table.");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }


            foreach (DataRow row in ds.Tables[0].Rows)
            {
                _id = row["FEASIBILITYID"].ToString();
                Criterias criteria = new Criterias(_table,_column,_id);
                byte[] assemblyCriteria = null;
                string currentCriteria = ""; 
                if(row["CRITERIA"] != DBNull.Value) currentCriteria =   Simulation.ConvertOMSAttribute(row["CRITERIA"].ToString());
                assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(_table, _column, _id, assemblyCriteria);
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

                criteria.Criteria = Simulation.ConvertOMSAttribute(row["CRITERIA"].ToString());

                foreach (String str in criteria.Errors)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Treatment feasibility criteria for Treatment " + _treatment + ":" + str));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Error: Treatment feasibility criteria for Treatment " + _treatment + ":" + str);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                }

                this.CriteriaList.Add(criteria);
                foreach (String str in criteria.CriteriaAttributes)
                {
                    if (!SimulationMessaging.IsAttribute(str))
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error: " + str + " which is used by the Feasibility criteria is not present in the database."));
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, "Error: " + str + " which is used by the Feasibility criteria is not present in the database");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                    }
                    if (!_attributes.Contains(str))
                    {
                        _attributes.Add(str);
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Load all cost information associated with this treatment.
        /// </summary>
        public bool LoadCost(object APICall, IMongoCollection<SimulationModel> Simulations, string m_strSimulationID)
        {
            Costs cost;
            String select = "SELECT COST_,UNIT,CRITERIA,BINARY_CRITERIA,ISFUNCTION, COSTID FROM " + cgOMS.Prefix + "COSTS WHERE TREATMENTID='" + this.TreatmentID + "'";
            if (SimulationMessaging.IsOMS)
            {
                select = "SELECT COST_,UNIT,CRITERIA,BINARY_CRITERIA,NULL AS ISFUNCTION, COSTID FROM " + cgOMS.Prefix + "COSTS WHERE TREATMENTID='" + this.TreatmentID + "'";
            }
            DataSet ds;
            try
            {
				//SimulationMessaging.AddMessage("DEBUGGING: Attempting cost select...");
                ds = DBMgr.ExecuteQuery(select);
				//SimulationMessaging.AddMessage("DEBUGGING: Cost select successful.");
			}
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Opening COSTS table.  SQL message - " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Fatal Error: Opening COSTS table.  SQL message - " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }


            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string id = row["COSTID"].ToString();
                cost = new Costs(id);
               
                if (row["CRITERIA"].ToString().Trim().Length  == 0)
                {
                    cost.Default = true;
                }
                else
                {
                    cost.Default = false;
                    string criteria = row["CRITERIA"].ToString();
                    byte[] assemblyCriteria = null;
                    assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "COSTS", "BINARY_CRITERIA", cost.CostID, assemblyCriteria);
                    if (assemblyCriteria != null && assemblyCriteria.Length > 0)
                    {
						cost.Criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                        if (cost.Criteria.Evaluate.OriginalInput != criteria)
                        {
                            cost.Criteria.Evaluate = null;
                        }
					}

                    cost.Criteria.Criteria = criteria;
                    foreach (String str in cost.Criteria.CriteriaAttributes)
                    {
                        if (!SimulationMessaging.IsAttribute(str))
                        {
                            SimulationMessaging.AddMessage(new SimulationMessage("Error: " + str + " which is used by the Cost criteria is not present in the database."));
                            if (APICall.Equals(true))
                            {
                                var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, "Error: " + str + " which is used by the Cost criteria is not present in the database");
                                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                            }
                        }
                        if (!_attributes.Contains(str))
                        {
                            _attributes.Add(str);
                        }
                    }
                }


                byte[] assemblyCost = null;
                //objectValue = row["BINARY_COST"];
                //if (objectValue != System.DBNull.Value)
                //{
                //    assemblyCost = (byte[])row["BINARY_COST"];
                //}
                String strCost = row["COST_"].ToString();
                assemblyCost = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "COSTS", "BINARY_EQUATION", cost.CostID, assemblyCost);
                if (assemblyCost != null && assemblyCost.Length > 0)
                {
                    cost.Calculate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCost);
                    if (cost.Calculate.OriginalInput != strCost)
                    {
                        cost.Calculate = null;
                    }
                }
                
                if (strCost.Trim() == "")
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Cost equation is blank for treatment - " + _treatment));
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, "Fatal Error: Cost equation is blank for treatment - " + _treatment);
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    return false;
                }
 
                bool isFunction = false;
                object function = row["ISFUNCTION"];
                if(row["ISFUNCTION"] != DBNull.Value)
                {
                    isFunction = Convert.ToBoolean(row["ISFUNCTION"]);
                }
                if (isFunction)
                {
                    cost.SetFunction(row["COST_"].ToString());
                }
                else
                {
                    cost.Equation = row["COST_"].ToString();
                }

                foreach (String str in cost._attributesEquation)
                {
                    if (str != "AREA" && str != "LENGTH")
                    {
                        if (!SimulationMessaging.IsAttribute(str))
                        {
                            SimulationMessaging.AddMessage(new SimulationMessage("Error: " + str + " which is used by the Cost equation is not present in the database."));
                            if (APICall.Equals(true))
                            {
                                var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, "Error: " + str + " which is used by the Cost equation is not present in the database");
                                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                            }
                        }
                        if (!_attributes.Contains(str))
                        {

                            _attributes.Add(str);
                        }
                    }
                }
                if (!cost.IsCompoundTreatment)
                {
                    if (cost._calculate.m_listError.Count > 0)
                    {
                        foreach (String str in cost.Calculate.m_listError)
                        {
                            SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Cost equation:" + str));
                            if (APICall.Equals(true))
                            {
                                var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, "Fatal Error: Cost equation:" + str);
                                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                            }
                            return false;
                        }
                    }
                }
                _costs.Add(cost);
            }
            return true;
        }
        /// <summary>
        /// Load all consequence information associated with treat. 
        /// </summary>
        public bool LoadConsequences(object APICall, IMongoCollection<SimulationModel> Simulations, string m_strSimulationID)
        {
            Consequences consequence;
            String select = "SELECT ATTRIBUTE_,CHANGE_,CRITERIA,EQUATION,ISFUNCTION, CONSEQUENCEID FROM " + cgOMS.Prefix + "CONSEQUENCES WHERE TREATMENTID='" + _treatmentID + "'";

            if(SimulationMessaging.IsOMS)
            {
                select = "SELECT ATTRIBUTE_,CHANGE_,CRITERIA,EQUATION,NULL AS ISFUNCTION, CONSEQUENCEID FROM " + cgOMS.Prefix + "CONSEQUENCES WHERE TREATMENTID='" + _treatmentID + "'";
            }

            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(select);
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Opening CONSEQUENCES table. SQL Message - " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Fatal Error: Opening CONSEQUENCES table. SQL Message - " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }

            var consequenceCount = 0;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string id = row["CONSEQUENCEID"].ToString();
                consequence = new Consequences(id);
                String strAttribute = row["ATTRIBUTE_"].ToString();
                String strChange = row["CHANGE_"].ToString();
                String strCriteria = row["CRITERIA"].ToString();
                String strEquation = row["EQUATION"].ToString();
                consequence.TreatmentID = this.TreatmentID;
                consequence.Treatment = this.Treatment;

                SimulationMessaging.AddMessage(new SimulationMessage("Compiling treatment consequence " + consequenceCount));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Compiling treatment consequence " + consequenceCount);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                consequenceCount++;


                if (strCriteria.Trim().Length == 0)
                {
                    consequence.Default = true;
                }
                else
                {
                    consequence.Default = false;

                    byte[] assemblyCriteria = null;
                    assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "CONSEQUENCES", "BINARY_CRITERIA", id, assemblyCriteria);
                    if (assemblyCriteria != null && assemblyCriteria.Length > 0)
                    {
                        consequence.Criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                        if (consequence.Criteria.Evaluate.OriginalInput != strCriteria)
                        {
                            consequence.Criteria.Evaluate = null;
                        }
                    }
                    
                    consequence.Criteria.Criteria = strCriteria;
                    //Get attributes from consequence criteria
                    foreach (String str in consequence.Criteria.CriteriaAttributes)
                    {
                        if (!SimulationMessaging.IsAttribute(str))
                        {
                            SimulationMessaging.AddMessage(new SimulationMessage("Error: " + str + " which is used by the Consequence criteria is not present in the database."));
                            if (APICall.Equals(true))
                            {
                                var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, "Error: " + str + " which is used by the Consequence criteria is not present in the database");
                                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                            }
                        }
                        if (!_attributes.Contains(str))
                        {
                            _attributes.Add(str);
                        }
                    }
                }


                if (string.IsNullOrWhiteSpace(strEquation.Trim()))
                {
                    consequence.IsEquation = false; ;
                }
                else
                {
                    byte[] assembly = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "CONSEQUENCES", "BINARY_EQUATION", id, null);
                    if (assembly != null && assembly.Length > 0)
                    {
                        consequence._calculate= (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assembly);
                        if (consequence._calculate.OriginalInput != strEquation)
                        {
                            consequence._calculate = null;
                        }
                    }

                    bool isFunction = false;
                    if(row["ISFUNCTION"] != DBNull.Value)
                    {
                        isFunction = Convert.ToBoolean(row["ISFUNCTION"]);
                    }

                    if (isFunction)
                    {
                        consequence.SetFunction(strEquation);
                    }
                    else
                    {
                        consequence.Equation = strEquation;
                    }
                    //Get attributes from consequence criteria
                    foreach (string attribute in consequence._attributesEquation)
                    {
                        if (!SimulationMessaging.IsAttribute(attribute))
                        {
                            SimulationMessaging.AddMessage(new SimulationMessage("Error: " + attribute + " which is used by the Consequence criteria is not present in the database."));
                            if (APICall.Equals(true))
                            {
                                var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, "Error: " + attribute + " which is used by the Consequence criteria is not present in the database");
                                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                            }
                        }
                        if (!_attributes.Contains(attribute))
                        {
                            _attributes.Add(attribute);
                        }
                    }
                }
                
                consequence.LoadAttributeChange(strAttribute,strChange);
                ConsequenceList.Add(consequence);
                
                // Get attributes from Attribute change
                foreach (String str in consequence.Attributes)
                {
                    if (!_attributes.Contains(str))
                    {
                        _attributes.Add(str);
                    }
                }
            }
            return true;
        }

        public bool LoadSupersedes(object APICall, IMongoCollection<SimulationModel> Simulations, string m_strSimulationID)
        {


            try
            {
                var select = "SELECT SUPERSEDE_ID, SUPERSEDE_TREATMENT_ID, CRITERIA FROM SUPERSEDES WHERE TREATMENT_ID='" + this.TreatmentID + "'";
                DataSet ds = DBMgr.ExecuteQuery(select);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var supersedeId = int.Parse(row["SUPERSEDE_ID"].ToString());
                    var supersedeTreatmentId = int.Parse(row["SUPERSEDE_TREATMENT_ID"].ToString());
                    var criteria = "";

                    if(row["CRITERIA"] != DBNull.Value)    criteria =row["CRITERIA"].ToString();

                    Supersedes.Add(new Supersede(supersedeId, supersedeTreatmentId,criteria));
                }


                foreach (var supersede in Supersedes)
                {
                    foreach (String str in supersede.Criteria.CriteriaAttributes)
                    {
                        if (!SimulationMessaging.IsAttribute(str))
                        {
                            SimulationMessaging.AddMessage(new SimulationMessage("Error: " + str + " which is used by the Supersede criteria is not present in the database."));
                            if (APICall.Equals(true))
                            {
                                var updateStatus = Builders<SimulationModel>.Update
                                .Set(s => s.status, "Error: " + str + " which is used by the Supersede criteria is not present in the database");
                                Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                            }
                        }
                        if (!_attributes.Contains(str))
                        {
                            _attributes.Add(str);
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(
                    new SimulationMessage("Fatal Error: Opening SUPERSEDE table. SQL Message - " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Fatal Error: Opening SUPERSEDE table. SQL Message - " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }
            return true;
        }

        public bool LoadScheduled(List<Treatments> availableTreatments, object APICall, IMongoCollection<SimulationModel> Simulations, string m_strSimulationID)
        {
            var select = "SELECT SCHEDULEDID, SCHEDULEDTREATMENTID, SCHEDULEDYEAR FROM SCHEDULED WHERE TREATMENTID='" + this.TreatmentID + "'";

            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(select);


                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var scheduledId = int.Parse(row["SCHEDULEDID"].ToString());
                    var scheduledTreatmentId = row["SCHEDULEDTREATMENTID"].ToString();
                    var scheduledYear = int.Parse(row["SCHEDULEDYEAR"].ToString());
                    var scheduledTreatment = availableTreatments.Find(t => t.TreatmentID == scheduledTreatmentId);

                    if (scheduledTreatment != null)
                    {
                        _scheduleds.Add(new Scheduled(scheduledId, scheduledTreatment, scheduledYear));
                    }

                }
            }
            catch (Exception exception)
            {
                SimulationMessaging.AddMessage(
                    new SimulationMessage("Fatal Error: Opening SCHEDULED table. SQL Message - " + exception.Message));
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, "Fatal Error: Opening SCHEDULED table. SQL Message - " + exception.Message);
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                return false;
            }
            return true;
        }
        
        public bool IsTreatmentCriteriaMet(Hashtable hashAttributeValue)
        {
			bool criteriaMet = false;

			if( CriteriaList.Count > 0 )
			{
				//Loop through all criteria that may cause treatment to happen.
				foreach( Criterias criteria in CriteriaList )
				{
					if( criteria.IsCriteriaMet( hashAttributeValue ) )
					{
						criteriaMet = true;
						break;
					}
				}
			}
			else
			{
				criteriaMet = true;
			}

			return criteriaMet;
        }

        public override string ToString()
        {
            return _treatment + "(" + _treatmentID + ")";
        }
    }
}