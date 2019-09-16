using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class ValuesStringStore
    {
        List<string> _values;
        ErrorStore _error;

        public ErrorStore Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public List<string> Values
        {
            get { return _values; }
            set { _values = value; }
        }
    }
}
