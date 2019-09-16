using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class YearInvestmentStore
    {
        int _year = -1;
        string _budget;
        double _amount = 0;

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }
        
        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public YearInvestmentStore()
        {
        }

        public YearInvestmentStore(int year, string budget, double amount)
        {
            _year = year;
            _budget = budget;
            _amount = amount;
        }

        public override string ToString()
        {
            return _budget + "(" + _year.ToString() + ") $" + _amount.ToString("f2"); ;
        }
    }
}
