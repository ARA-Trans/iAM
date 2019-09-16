using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class AssetReplyOMSLookupTable
    {
        string _table;
        Dictionary<string, Dictionary<string, string>> _lookupData;

        
        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public Dictionary<string, Dictionary<string, string>> LookupData
        {
            get { return _lookupData; }
            set { _lookupData = value; }
        }
        
        
        public AssetReplyOMSLookupTable(string table)
        {
            _table = table;
            _lookupData = new Dictionary<string, Dictionary<string, string>>();
        }

    }
}
