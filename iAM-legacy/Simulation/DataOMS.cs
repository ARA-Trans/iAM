using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class DataOMS
    {
        private object _value;
        private DateTime _date;

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public DataOMS(object value, DateTime date)
        {
            _value = value;
            _date = date;
        }
    }
}
