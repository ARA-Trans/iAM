using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Tables that make up the OMS reply to the Decision Engine.
    /// </summary>
    public class AssetReplyOMSTableStore
    {
        string _omsTable;
        DateTime _dateLastEntry;
        List<AssetReplyOMSColumnStore> _columns;
        List<string> _lookupFields;


        /// <summary>
        /// Date of last entry for this row.
        /// </summary>
        public DateTime DateLastEntry
        {
            get { return _dateLastEntry; }
            set { _dateLastEntry = value; }
        }
    
        /// <summary>
        /// The OMS table for this data.
        /// </summary>
        public string OmsTable
        {
            get { return _omsTable; }
            set { _omsTable = value; }
        }

        /// <summary>
        /// List of OMS columns in this OMS table.
        /// </summary>
        public List<AssetReplyOMSColumnStore> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        /// <summary>
        /// List of fields that are looked up due to data in this table
        /// </summary>
        public List<string> LookupFields
        {
            get { return _lookupFields; }
            set { _lookupFields = value; }
        }



        public AssetReplyOMSTableStore()
        {
        }
        /// <summary>
        /// The reply from a single omsTable for a single asset.   Multiple columns returned.
        /// </summary>
        /// <param name="omsTable">Name of the omsTable</param>
        /// <param name="dateLastEntry">Last Entry Date for this asset row in omsTable</param>
        /// <param name="columns">List of columns returned</param>
        public AssetReplyOMSTableStore(string omsTable, DateTime dateLastEntry)
        {
            _omsTable = omsTable;
            _dateLastEntry = dateLastEntry;
            _columns = new List<AssetReplyOMSColumnStore>();
            _lookupFields = new List<string>();
        }

        public override string ToString()
        {
            string value = _omsTable + " from " + _dateLastEntry.ToString() + "(";
            bool isFirstColumn = true;
            foreach (AssetReplyOMSColumnStore column in _columns)
            {
                if (!isFirstColumn) value += ",";
                value += column.ToString();
                isFirstColumn = false;
            }
            value += ")";
            return  value;
        }
    }
}
