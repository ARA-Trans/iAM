using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class EditSimulationStore
    {
        string _simulationID;
        string _simulationName;
        int _numberAssets;
        string _estimatedRunTime;
        string _assetType;
        string _scopeName;
        string _scopeFilter;
        string _optimizationType;
        string _budgetName;
        string _startDate;
        int _numberYears;
        string _assetTypeOID;
        int _totalBudget;
        double _inflationRate;
        List<string> _budgetOrder;
        List<YearBudgetAmountStore> _yearBudgetAmounts;
        List<ActivityStore> _activities;
        List<TargetStore> _targets;
        ErrorStore _error;



        public string SimulationID
        {
            get { return _simulationID; }
            set { _simulationID = value; }
        }

        public string SimulationName
        {
            get { return _simulationName; }
            set { _simulationName = value; }
        }

        public int NumberAssets
        {
            get { return _numberAssets; }
            set { _numberAssets = value; }
        }

        public string EstimatedRunTime
        {
            get { return _estimatedRunTime; }
            set { _estimatedRunTime = value; }
        }


        public string AssetType
        {
            get { return _assetType; }
            set { _assetType = value; }
        }

        public string ScopeName
        {
            get { return _scopeName; }
            set { _scopeName = value; }
        }

        public string ScopeFilter
        {
            get { return _scopeFilter; }
            set { _scopeFilter = value; }
        }

        public string OptimizationType
        {
            get { return _optimizationType; }
            set { _optimizationType = value; }
        }

        public string BudgetName
        {
            get { return _budgetName; }
            set { _budgetName = value; }
        }

        public string StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public int NumberYears
        {
            get { return _numberYears; }
            set { _numberYears = value; }
        }

        public ErrorStore Error
        {
            get { return _error; }
            set { _error = value; }
        }


        public List<string> BudgetOrder
        {
            get { return _budgetOrder; }
            set { _budgetOrder = value; }
        }

        public List<YearBudgetAmountStore> YearBudgetAmounts
        {
            get { return _yearBudgetAmounts; }
            set { _yearBudgetAmounts = value; }
        }

        public List<ActivityStore> Activities
        {
            get { return _activities; }
            set { _activities = value; }
        }

        public List<TargetStore> Targets
        {
            get { return _targets; }
            set { _targets = value; }
        }


        public EditSimulationStore()
        {
        }

        public string AssetTypeOID
        {
            get { return _assetTypeOID; }
            set { _assetTypeOID = value; }
        }

        public int TotalBudget
        {
            get { return _totalBudget; }
            set { _totalBudget = value; }
        }

        public double InflationRate
        {
            get { return _inflationRate; }
            set { _inflationRate = value; }
        }

        public EditSimulationStore(SimulationStore simulation)
        {
            _simulationID = simulation.SimulationID;
            _simulationName = simulation.SimulationName;
            _numberAssets = simulation.TotalAssets;
            _numberYears = simulation.Years;
            _estimatedRunTime = simulation.EstimatedRunTime;
            _assetType = simulation.Asset;
            _scopeFilter = simulation.Jurisdiction;
            _scopeName = simulation.ScopeDescription;
            _startDate = simulation.StartDate.ToShortDateString();
            _budgetName = simulation.BudgetName;
            _yearBudgetAmounts = SelectScenario.GetBudgetsByYear(simulation.SimulationID);
            _assetTypeOID = simulation.AssetTypeOID;
            _totalBudget = 0;
            _budgetOrder = simulation.BudgetOrder;
            _inflationRate = simulation.InflationRate;

            foreach (YearBudgetAmountStore yba in _yearBudgetAmounts)
            {
                _totalBudget += (int)yba.Amount;
            }
            _activities = SelectScenario.GetActivities(simulation.SimulationID,false);
            _targets = SelectScenario.GetEditSimulationTargets(simulation.SimulationID);

            //Analysis type is a combination of AnalysisType and Budget Contraint
            // Budget Target = As Budget Permits
            // OCI Target
            // Until OCI or Budget Met
            // Unlimited
            _optimizationType = simulation.BudgetConstraint;
        }
    }
}
