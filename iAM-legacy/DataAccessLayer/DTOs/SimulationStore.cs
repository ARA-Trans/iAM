using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using DataAccessLayer.DTOs;

namespace DataAccessLayer.DTOs
{
    [DataContract]
    public class SimulationStore
    {
        private string _simulationID;   //Simulations
        private string _simulationName;  //Simulations
        private int _years; //Count of YEARBUDGETS
        private bool _useTarget; //Simulations
        private string _targetOCI; //Simulations
        private string _currentOCI; //OMS database
        private string _asset;//Simulations
        private int _totalAssets;//OMS DATABASE
        private string _scopeDescription;//Simulations
        private string _lastRunDate;//Simulations
        private string _lastAuthor;//Simulations
        private double _runTime;//Simulations
        private int _assetsYearMinute = 10000;
        private string _jurisdiction;
        private string _simulationArea;
        private DateTime _startDate;
        private string _analysisType;
        private string _budgetName;
        private string _assetTypeOID;
        private List<string> _budgetOrder;
        private string _budgetConstraint;
        private double _inflationRate;
        private ErrorStore _error;

        // private string _deficientOCI;
        // private string _deficientPercentage;


        List<YearBudgetStore> _yearBudgetList;//YEARBUDGETS
        List<CategoryBudgetStore> _categoryBudgetList;//CATEGORYBUDGETS
        List<NetworkConditionStore> _networkConditionList;//TARGET_

        #region Properties
        
        [DataMember(Order = 0)]
        public string SimulationID
        {
            get { return _simulationID; }
            set { _simulationID = value; }
        }


        [DataMember(Order=1)]
        public string SimulationName
        {
            get { return _simulationName; }
            set { _simulationName = value; }
        }

        [DataMember(Order = 2)]
        public int Years
        {
            get { return _years; }
            set { _years = value; }
        }
       
        [DataMember(Order = 3)]
        public bool UseTarget
        {
            get { return _useTarget; }
            set { _useTarget = value; }
        }

        [DataMember(Order = 4)]
        public string TargetOCI
        {
            get { return _targetOCI; }
            set { _targetOCI = value; }
        }

        [DataMember(Order = 5)]
        public string CurrentOCI
        {
            get { return _currentOCI; }
            set { _currentOCI = value; }
        }

        [DataMember(Order = 6)]
        public string PercentImprovement
        {
            get
            {
                if (_networkConditionList == null) return "n/a";
                if (_networkConditionList.Count <= 1) return "n/a";
                NetworkConditionStore start = _networkConditionList[0];
                NetworkConditionStore end = _networkConditionList[_networkConditionList.Count - 1];
                if (start.GetAverage() != double.NaN && end.GetAverage() != double.NaN)
                {
                    if (start.GetAverage() == 0 && end.GetAverage() != 0) return "Infinte";
                    if (start.GetAverage() == 0 && end.GetAverage() == 0) return "n/a";
                    double difference = end.GetAverage() - start.GetAverage();
                    double percent = 100 * difference / start.GetAverage();
                    return percent.ToString("f1") + "%";
                }
                else
                {
                    return "n/a";
                }
            }
            set { }
        }

        [DataMember(Order = 7)]
        public double TotalBudget
        {
            get
            { 
                if(_yearBudgetList == null) return 0;
                double totalBuget = 0;
                foreach (YearBudgetStore yearBudget in _yearBudgetList)
                {
                    double amount = 0;
                    double.TryParse(yearBudget.Target, out amount);
                    totalBuget += amount;
                }
                return totalBuget;
            }
            set {  }
        }

        [DataMember(Order = 8)]
        public string Asset
        {
            get { return _asset; }
            set { _asset = value; }
        }

        [DataMember(Order = 9)]
        public int TotalAssets
        {
            get { return _totalAssets; }
            set { _totalAssets = value; }
        }

        [DataMember(Order = 10)]
        public string ScopeDescription
        {
            get { return _scopeDescription; }
            set { _scopeDescription = value; }
        }

        [DataMember(Order = 11)]
        public string LastRunDate
        {
            get { return _lastRunDate; }
            set { _lastRunDate = value; }
        }

        [DataMember(Order = 12)]
        public string LastAuthor
        {
            get { return _lastAuthor; }
            set { _lastAuthor = value; }
        }

        [DataMember(Order = 13)]
        public string EstimatedRunTime
        {
            get
            {
                if (_runTime != 0)
                {
                    DateTime now = DateTime.Now;
                    DateTime end = now.AddMinutes(_runTime/60);
                    TimeSpan span = end - now;
                    return span.ToString();
                }
                else
                {
                    int nTotalRun = _totalAssets * _years / _assetsYearMinute;
                    DateTime now = DateTime.Now;
                    DateTime end = now.AddMinutes(nTotalRun);
                    TimeSpan span = end - now;
                    return span.ToString();
                }
            }
            set {
                _runTime = Convert.ToDouble(value);
            }
        }

        [DataMember(Order = 14)]
        public bool HasRun
        {
            get
            {
                if (_networkConditionList == null) return false;
                if (_networkConditionList.Count > 1) return true;
                else return false;
            }
            set { }
        }
        
        [DataMember(Order = 15)]
        public string Jurisdiction
        {
            get { return _jurisdiction; }
            set { _jurisdiction = value; }
        }
        
        [DataMember(Order = 16)]
        public string SimulationArea
        {
            get { return _simulationArea; }
            set { _simulationArea = value; }
        }

        [DataMember(Order = 16)]
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        [DataMember(Order = 17)]
        public string AnalysisType
        {
            get { return _analysisType; }
            set { _analysisType = value; }
        }

        [DataMember(Order = 18)]
        public string BudgetName
        {
            get { return _budgetName; }
            set { _budgetName = value; }
        }

        [DataMember(Order = 19)]
        public string AssetTypeOID
        {
            get { return _assetTypeOID; }
            set { _assetTypeOID = value; }
        }

        [DataMember(Order = 20)]
        public List<string> BudgetOrder
        {
            get { return _budgetOrder; }
            set { _budgetOrder = value; }
        }

        [DataMember(Order = 21)]
        public string BudgetConstraint
        {
            get { return _budgetConstraint; }
            set { _budgetConstraint = value; }
        }


        [DataMember(Order = 22)]
        public double InflationRate
        {
            get { return _inflationRate; }
            set { _inflationRate = value; }
        }

        [DataMember(Order = 23)]
        public ErrorStore Error
        {
            get { return _error; }
            set { _error = value; }
        }


        [DataMember(Order = 100)]
        public List<YearBudgetStore> YearBudgets
        {
            get { return _yearBudgetList; }
            set { _yearBudgetList = value; }
        }

        [DataMember(Order = 101)]
        public List<CategoryBudgetStore> CategoryBudgets
        {
            get { return _categoryBudgetList; }
            set { _categoryBudgetList = value; }
        }

        [DataMember(Order = 102)]
        public List<NetworkConditionStore> NetworkConditions
        {
            get { return _networkConditionList; }
            set { _networkConditionList = value; }
        }
        #endregion

        public SimulationStore(string simulationName, string simulationID)
        {
            _simulationName = simulationName;
            _simulationID = simulationID;


        }

        public SimulationStore(SqlDataReader dr)
        {
            _simulationName = dr["Simulation"].ToString();
        }

        public SimulationStore()
        {
            
        }
    }
}
