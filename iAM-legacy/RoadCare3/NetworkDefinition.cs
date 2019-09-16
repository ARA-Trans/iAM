using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadCare3
{
    public class NetworkDefinition
    {
        string _netDefName;

        public string NetDefName
        {
            get { return _netDefName; }
        }
        string _isNative;

        public string IsNative
        {
            get { return _isNative; }
        }
        string _selStatement;

        public string SelStatement
        {
            get { return _selStatement; }
        }
        string _connString;

        public string ConnString
        {
            get { return _connString; }
        }
        string _tableName;

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public NetworkDefinition(string netDefName, string isNative, string selStatement, string connString, string tableName)
        {
            _netDefName = netDefName;
            _isNative = isNative;
            _selStatement = selStatement;
            _connString = connString;
            _tableName = tableName;
        }
    }
}
