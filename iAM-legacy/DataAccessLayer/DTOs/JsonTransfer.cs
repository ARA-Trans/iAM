using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class JsonTransfer
    {
        string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public JsonTransfer()
        {
        }

    }
}
