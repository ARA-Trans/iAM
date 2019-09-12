using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class ParameterizedCriteriaData
    {
        public string ParameterizedPredicatesString { get; set; }
        public List<SqlParameter> SqlParameters { get; set; }

        public ParameterizedCriteriaData() { }

        public ParameterizedCriteriaData(string parameterizedPredicatesString, List<SqlParameter> sqlParameters)
        {
            ParameterizedPredicatesString = parameterizedPredicatesString;
            SqlParameters = sqlParameters;
        }
    }
}