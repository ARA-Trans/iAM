using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class AttributesByYearDAL : IAttributesByYear
    {
        private readonly BridgeCareContext db;

        public AttributesByYearDAL(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<AttributeByYearModel> GetProjectedAttributes(SimulatedSegmentIdsModel segmentAddressModel, BridgeCareContext db)
        {
            var projectedAttributesAndValues = new List<AttributeByYearModel>();
            try
            {
                var query = $"SELECT * FROM SIMULATION_{segmentAddressModel.NetworkId}_{segmentAddressModel.SimulationId} WHERE SectionID = @sectionId";
                var sectionIdParameter = new SqlParameter()
                {
                    ParameterName = "@sectionId",
                    Value = segmentAddressModel.SectionId
                };
                projectedAttributesAndValues = GeneralAttributeValueQuery(query, sectionIdParameter);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "ProjectedAttributes");
            }
            return projectedAttributesAndValues;
        }

        public List<AttributeByYearModel> GetHistoricalAttributes(SectionModel sectionModel, BridgeCareContext db)
        {
            var historicalAttributes = new List<AttributeByYearModel>();
            try
            {
                var query = $"SELECT * FROM SEGMENT_{sectionModel.NetworkId}_NS0 WHERE SectionID = @sectionId";
                var sectionIdParameter = new SqlParameter()
                {
                    ParameterName = "@sectionId",
                    Value = sectionModel.SectionId
                };
                historicalAttributes = GeneralAttributeValueQuery(query, sectionIdParameter);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "HistoricalAttributes");
            }
            return historicalAttributes;
        }

        public List<AttributeByYearModel> GeneralAttributeValueQuery(string query, SqlParameter sectionIdParameter)
        {
            try
            {
                // open db connection
                var connection = new SqlConnection(db.Database.Connection.ConnectionString);
                connection.Open();
                // create sql command with parameterized query and connection
                var sqlCommand = new SqlCommand(query, connection);
                // add the sectionId parameter
                sqlCommand.Parameters.Add(sectionIdParameter);
                // create a data table from the reader
                var queryReturnValues = new DataTable();
                // create data reader from sql command
                var dataReader = sqlCommand.ExecuteReader();
                // use the data reader to load the data table
                queryReturnValues.Load(dataReader);
                // close the data reader
                dataReader.Close();
                // close the connection
                connection.Close();
                // create the attributes list and return
                return DataTableToAttributeList(queryReturnValues);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Query:" + query);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return new List<AttributeByYearModel>();
        }

        private bool IsNumber(string str) => int.TryParse(str, out _);

        //check for year
        private bool IsIamYear(string inputString) => (IsNumber(inputString) && inputString.Length == 4);

        private bool IsAttributeYearNameProfile(string columnName, DataTable AttributesDataTable)
        {
            string value = AttributesDataTable.Rows[0][columnName].ToString();

            string[] tokens = columnName.Split('_');
            return (IsIamYear(tokens.Last()) && value.Length > 0);
        }

        public List<AttributeByYearModel> DataTableToAttributeList(DataTable AttributesDataTable)
        {
            List<AttributeByYearModel> returnAttributeList = new List<AttributeByYearModel>();

            foreach (DataColumn column in AttributesDataTable.Columns)
            {
                if (AttributesDataTable.Rows.Count <= 0)
                    break;

                if (IsAttributeYearNameProfile(column.ColumnName, AttributesDataTable))
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