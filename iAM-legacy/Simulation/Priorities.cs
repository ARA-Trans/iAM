using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DatabaseManager;
using System.Collections;
using MongoDB.Driver;
using static Simulation.Simulation;

namespace Simulation
{
    public class Priorities
    {
        private int m_nPriorityLevel;
        private int m_nYears;
        private String m_strPriorityID;
        private bool m_bAllSections;
        private bool m_bAllYears = true;
        private Criterias criteria;
        private Hashtable m_hashBudgetPercent = new Hashtable();

        private string _table;
        private string _column;

        public Hashtable BudgetPercent
        {
            get { return m_hashBudgetPercent; }
            set { m_hashBudgetPercent = value; }
        }


        public bool IsAllSections
        {
            get { return m_bAllSections; }
            set { m_bAllSections = value; }
        }

        public bool IsAllYears
        {
            get { return m_bAllYears; }
            set { m_bAllYears = value; }
        }
        
        
        public int PriorityLevel
        {
            get { return m_nPriorityLevel; }
            set { m_nPriorityLevel = value; }
        }


        public String PriorityID
        {
            get { return m_strPriorityID; }
            set { m_strPriorityID = value; }
        }

        public Criterias Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }
        public int Years
        {
            get 
            {
                
                return m_nYears; 
            }
            set {
                m_bAllYears = false;
                m_nYears = value; }
        }

        public Priorities(string id)
        {
            m_strPriorityID = id;
            _table = "PRIORITY";
            _column = "BINARY_CRITERIA";
            criteria = new Criterias(_table, _column, id);
        }

        public bool LoadBudgetPercentages(List<String> listBudgets, object APICall,
            IMongoCollection<SimulationModel> Simulations, string m_strSimulationID)
        {
            //Cleanup unused Budgets
            String strDelete = "DELETE FROM PRIORITYFUND WHERE PRIORITYID='" + this.PriorityID + "'";
            
            if(listBudgets.Count > 0)strDelete += " AND (";
            int nBudget = 0;
            foreach (String sBudget in listBudgets)
            {
                if( nBudget > 0) strDelete += " AND ";
                strDelete += "BUDGET<>'" + sBudget + "'";
                nBudget++;
            }
            if (listBudgets.Count > 0) strDelete += ")";

            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception except)
            {
                if (APICall.Equals(true))
                {
                    var updateStatus = Builders<SimulationModel>.Update
                    .Set(s => s.status, $"Fatal Error: Error removing non-used budget priorities");
                    Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                }
                SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Error removing non-used budget priorities. " + except.Message));
                return false;

            }

            if (SimulationMessaging.IsOMS)
            {
                foreach (string budget in listBudgets)
                {
                    m_hashBudgetPercent.Add(budget, 100f);
                }
            }
            else
            {
                String strSelect = "SELECT BUDGET, FUNDING FROM PRIORITYFUND WHERE PRIORITYID='" + this.PriorityID + "'";
                DataSet ds;
                try
                {
                    ds = DBMgr.ExecuteQuery(strSelect);
                    if (ds.Tables[0].Rows.Count != listBudgets.Count)
                    {
                        if (APICall.Equals(true))
                        {
                            var updateStatus = Builders<SimulationModel>.Update
                            .Set(s => s.status, $"Error: Each budget must have a funding level for each priority level");
                            Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                        }
                        SimulationMessaging.AddMessage(new SimulationMessage("Fatal Error: Each budget must have a funding level for each priority level."));
                        return false;
                    }
                }
                catch
                {
                    if (APICall.Equals(true))
                    {
                        var updateStatus = Builders<SimulationModel>.Update
                        .Set(s => s.status, $"Error: Filling Priority budgets and funding");
                        Simulations.UpdateOne(s => s.simulationId == Convert.ToInt32(m_strSimulationID), updateStatus);
                    }
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Filling Priority budgets and funding"));
                    return false;
                }

                String strBudget;
                String strFunding;
                float fFunding;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    strBudget = row[0].ToString();
                    fFunding = 100;
                    strFunding = row[1].ToString();
                    float.TryParse(strFunding, out fFunding);
                    m_hashBudgetPercent.Add(strBudget, fFunding);

                }
            }
            return true;
        }
    
    
    }
}
