using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class AttributesByYear : IAttributesByYear
    {
        private readonly BridgeCareContext db;

        public AttributesByYear(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<AttributeByYearModel> GetProjectedAttributes(SimulatedSegmentIdsModel segmentAddressModel, BridgeCareContext db)
        {
            var projectedAttributesAndValues = new List<AttributeByYearModel>();
            try
            {
                var selectStatement = string.Format("SELECT * FROM SIMULATION_{0}_{1} WHERE SectionID={2}",
                segmentAddressModel.NetworkId, segmentAddressModel.SimulationId, segmentAddressModel.SectionId);
                projectedAttributesAndValues = GeneralAttributeValueQuery(selectStatement);
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
                var selectStatement = String.Format(
                    "SELECT * FROM SEGMENT_{0}_NS0 WHERE SectionID={1}",
                    sectionModel.NetworkId, sectionModel.SectionId);

                historicalAttributes = GeneralAttributeValueQuery(selectStatement);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "HistoricalAttributes");
            }
            return historicalAttributes;
        }

        public List<AttributeByYearModel> GeneralAttributeValueQuery(string selectStatement)
        {
            try
            {
                DataTable queryReturnValues = UtilityFunctions.NonEntitySQLQuery(selectStatement, db);
                return DataTableToAttributeList(queryReturnValues);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Query:" + selectStatement);
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