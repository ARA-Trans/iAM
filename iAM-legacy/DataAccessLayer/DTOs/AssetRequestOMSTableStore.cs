using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Stores the a table request from DecisionEngine for necessary simulation data.
    /// </summary>
    public class AssetRequestOMSTableStore
    {
        string _tableName;
        string _primaryKeyColumn;
        string _foreignKeyColumn;
        string _foreignKeyTable;
        List<AssetRequestOMSColumnStore> _columns;

        /// <summary>
        /// Name of OMS table for which data is requested.
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// List of column names and OMS ObjectUserIDHierarchy for wich data is requested.
        /// </summary>
        public List<AssetRequestOMSColumnStore> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public string PrimaryKeyColumn
        {
            get { return _primaryKeyColumn; }
            set { _primaryKeyColumn = value; }
        }

        public string ForeignKeyColumn
        {
            get { return _foreignKeyColumn; }
            set { _foreignKeyColumn = value; }
        }

        public string ForeignKeyTable
        {
            get { return _foreignKeyTable; }
            set { _foreignKeyTable = value; }
        }

        /// <summary>
        /// Empty constructor for serialization.
        /// </summary>
        public AssetRequestOMSTableStore()
        {
        }

        /// <summary>
        /// Constructor for OMS table request.
        /// </summary>
        /// <param name="tableName">OMS table name</param>
        public AssetRequestOMSTableStore(string tableName)
        {
            _tableName = tableName;
            _columns = new List<AssetRequestOMSColumnStore>();
        }

        public override string ToString()
        {
            string value = _tableName + "(";
            bool isFirstColumn = true;
            foreach (AssetRequestOMSColumnStore column in _columns)
            {
                if(!isFirstColumn)value += ",";
                value += column.ColumnName;
                isFirstColumn = false;
            }
            value += ")";

            return value;
        }
   }
}
