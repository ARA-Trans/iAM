using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class YearBudgetAmountStore
    {
        int _year;
        string _budget;
        double _amount;

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

    }
}
