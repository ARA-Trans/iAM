using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer
{
    public class ErrorStore
    {
        string _errorNumber = "0";
        string _logNumber;
        string _description = "SUCCESS";
        string _verbose;

        public string ErrorNumber
        {
            get { return _errorNumber; }
            set { _errorNumber = value; }
        }

        public string LogNumber
        {
            get { return _logNumber; }
            set { _logNumber = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Verbose
        {
            get { return _verbose; }
            set { _verbose = value; }
        }

    }
}
