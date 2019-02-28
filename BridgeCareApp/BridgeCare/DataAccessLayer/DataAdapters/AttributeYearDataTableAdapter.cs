using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer.DataAdapters
{
    /// <summary>
    /// Populate list of AttributeByYearModel
    /// </summary>
    public class AttributeYearDataTable
    {
        private DataTable AttributesDataTable;
        private List<AttributeByYearModel> TheAttributeYearList;

        public AttributeYearDataTable(DataTable tableWithAttributes)
        {
            AttributesDataTable = tableWithAttributes ?? throw new ArgumentNullException(nameof(tableWithAttributes));

            TheAttributeYearList =  DataTableToAttributeList(tableWithAttributes);
        }

        public List<AttributeByYearModel> Fetch() => TheAttributeYearList;


        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        //check for year
        private bool IsIamYear(string inputString) => (IsDigitsOnly(inputString) && inputString.Length == 4);
        

        private bool IsAttributeYearNameProfile(string columnName)
        {
            string value = AttributesDataTable.Rows[0][columnName].ToString();

            string[] tokens = columnName.Split('_');
            return (IsIamYear(tokens.Last()) && value.Length > 0);           
        }

        public List<AttributeByYearModel> DataTableToAttributeList(DataTable dt)
        {
            List<AttributeByYearModel> returnAttributeList = new List<AttributeByYearModel>();

            foreach (DataColumn column in dt.Columns)
            {
                if (dt.Rows.Count <= 0)
                    break;

               if ( IsAttributeYearNameProfile(column.ColumnName))
                {
                    //true means this is an attribute_year column, subtract 5 digits, 4 for year and one '_'
                    string name = column.ColumnName.Substring(0, column.ColumnName.Length - 5);
                    AttributeYearlyValueModel attributeYearValue = new AttributeYearlyValueModel();

                    string[] tokens = column.ColumnName.Split('_');
                    attributeYearValue.Year = Convert.ToInt32(tokens.Last());
                    string value = AttributesDataTable.Rows[0][column.ColumnName].ToString();
                    attributeYearValue.Value = value;

                    AttributeByYearModel attributeInList = returnAttributeList.Find(_ => _.Name == name);
                    //if the list does not have the attribute already add it
                    if (attributeInList == null)
                    {
                        attributeInList = new AttributeByYearModel();
                        attributeInList.Name = name;
                    }

                    attributeInList.YearlyValues.Add(attributeYearValue);
                    returnAttributeList.Add(attributeInList);
                }
            }
            return returnAttributeList;
        }
    }
}
