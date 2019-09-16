using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class WorkPlanStore
    {
        string _sessionID;
        string _orderByField;
        int _numberAssets;
        int _startYear;
        int _numberYear;
        List<WorkPlanYearStore> _years;
        ErrorStore error;



        public string SessionID
        {
            get { return _sessionID; }
            set { _sessionID = value; }
        }

        public string OrderByField
        {
            get { return _orderByField; }
            set { _orderByField = value; }
        }

        public int NumberAssets
        {
            get { return _numberAssets; }
            set { _numberAssets = value; }
        }

        public int StartYear
        {
            get { return _startYear; }
            set { _startYear = value; }
        }


        public int NumberYear
        {
            get { return _numberYear; }
            set { _numberYear = value; }
        }

        public List<WorkPlanYearStore> Years
        {
            get { return _years; }
            set { _years = value; }
        }

        public ErrorStore Error
        {
            get { return error; }
            set { error = value; }
        }

        public WorkPlanStore()
        {
        }

        public WorkPlanStore(SessionStore session)
        {
            _sessionID = session.SessionID;
            _numberAssets = session.Simulation.TotalAssets;
            _numberYear = session.Simulation.Years;
            _startYear = session.Simulation.StartDate.Year;
            _orderByField = session.OrderByField;
            List<string>treatmentsPerYear = session.TreatmentsPerYear;
            
            _years = new List<WorkPlanYearStore>();
            for (int i = 0; i < _numberYear; i++)
            {
                WorkPlanYearStore year = new WorkPlanYearStore();
                year.Index = (i+1).ToString();
                if (i < session.Simulation.YearBudgets.Count)
                {
                    year.Year = session.Simulation.YearBudgets[i].Key;
                    year.Spent = session.Simulation.YearBudgets[i].Spent;
                    year.Budget = session.Simulation.YearBudgets[i].Target;
                }

                if (i < treatmentsPerYear.Count)
                {
                    year.NumberTreatments = treatmentsPerYear[i];
                }

                _years.Add(year);
            }
        }
    }
}
