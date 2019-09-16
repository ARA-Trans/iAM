using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class ValueStore
    {
        string _value;


        ErrorStore _error;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public ErrorStore Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public ValueStore(string value, ErrorStore error)
        {
            _value = value;
            _error = error;
        }
    }
}
