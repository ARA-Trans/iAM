using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class InvestmentStore
    {
        int _firstYear = DateTime.Now.AddYears(1).Year;
        int _numberYear = 5;
        double _inflationRate = 0;
        double _discountRate = 0;
        string _budgetOrder;
        DateTime _startDate;
        List<YearInvestmentStore> _listYearInvestment;
        List<string> _listBudget;
        string _budgetName;


        public int FirstYear
        {
            get { return _firstYear; }
            set { _firstYear = value; }
        }

        public int NumberYear
        {
            get { return _numberYear; }
            set { _numberYear = value; }
        }

        public double InflationRate
        {
            get { return _inflationRate; }
            set { _inflationRate = value; }
        }

        public double DiscountRate
        {
            get { return _discountRate; }
            set { _discountRate = value; }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public string BudgetOrder
        {
            get { return _budgetOrder; }
            set 
            {
                if (_listBudget == null) _listBudget = new List<string>();
                else _listBudget.Clear();
                _budgetOrder = value;

                if (!string.IsNullOrWhiteSpace(_budgetOrder))
                {
                    string[] values = _budgetOrder.Split('|');
                    foreach (string budget in values)
                    {
                        _listBudget.Add(budget);
                    }
                }
            }
        }

        public List<string> Budgets
        {
            get { return _listBudget; }
        }


        public List<YearInvestmentStore> YearInvestment
        {
            get { return _listYearInvestment; }
            set { _listYearInvestment = value; }
        }


        public string BudgetName
        {
            get { return _budgetName; }
            set { _budgetName = value; }
        }

        public InvestmentStore()
        {

        }

        public InvestmentStore(int firstYear, int numberYear, double inflationRate, double discountRate, DateTime startDate, string budgetOrder, string budgetName)
        {
            _firstYear = firstYear;
            _numberYear = numberYear;
            _inflationRate = inflationRate;
            _discountRate = discountRate;
            this.BudgetOrder = budgetOrder;
            _startDate = startDate;
            _budgetName = budgetName;
        }

        public int AddInvestmentYear(YearInvestmentStore yearInvestment)
        {
            if (_listYearInvestment == null) _listYearInvestment = new List<YearInvestmentStore>();
            _listYearInvestment.Add(yearInvestment);
            return _listYearInvestment.Count;
        }

        public List<YearBudgetStore> GetYearBudget(out double allBudgetTotal)
        {
            List<YearBudgetStore> listYearBudget = new List<YearBudgetStore>();
            allBudgetTotal = 0;
            for (int i = this.FirstYear; i < this.FirstYear + this.NumberYear; i++)
            {
                List<YearInvestmentStore> listYear = this.YearInvestment.FindAll(delegate(YearInvestmentStore yis) { return yis.Year == i; });
                double total = 0;
                foreach (YearInvestmentStore yis in listYear)
                {
                    total += yis.Amount;
                    allBudgetTotal = yis.Amount;
                }
                YearBudgetStore store = new YearBudgetStore(i.ToString(), total.ToString("f2"),null);
                listYearBudget.Add(store);
            }
            return listYearBudget;
        }
    }
}
