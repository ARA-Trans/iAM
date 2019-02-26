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
    public class AttributeByYear : IAttributeByYear
    {
        private readonly BridgeCareContext db;

        public AttributeByYear(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<AttributeByYearModel> GetProjectedAttributes(SimulatedSegmentAddressModel segmentAddressModel, BridgeCareContext db)
        {
            if (segmentAddressModel == null)
                return new List<AttributeByYearModel>();

            var selectStatement = String.Format("SELECT * FROM SIMULATION_{0}_{1} WHERE SectionID={2}",
                segmentAddressModel.NetworkId, segmentAddressModel.SimulationId, segmentAddressModel.SectionId);

            return GeneralAttributeValueQuery(selectStatement);
        }

        public List<AttributeByYearModel> GetHistoricalAttributes(SectionModel sectionModel, BridgeCareContext db)
        {
            if (sectionModel == null)
                return new List<AttributeByYearModel>();

            var selectStatement = String.Format(
                "SELECT * FROM SEGMENT_{0}_NS0 WHERE SectionID={1}",
                sectionModel.NetworkId, sectionModel.SectionId);

            return GeneralAttributeValueQuery(selectStatement);
        }

        public List<AttributeByYearModel> GeneralAttributeValueQuery(string selectStatement)
        {
            DataTable queryReturnValues = null;

            try
            {
                queryReturnValues = UtilityFunctions.NonEntitySQLQuery(selectStatement, db);
                List<AttributeByYearModel> attributesByYear = DataTableToAttributeList(queryReturnValues);
                return attributesByYear;
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

        public List<AttributeByYearModel> DataTableToAttributeList(DataTable dt)
        {
            List<AttributeByYearModel> returnAttributeList = new List<AttributeByYearModel>();

            foreach (DataColumn column in dt.Columns)
            {
                if (dt.Rows.Count <= 0)
                    break;

                string value = dt.Rows[0][column.ColumnName].ToString();

                string[] tokens = column.ColumnName.Split('_');
                if (UtilityFunctions.IsIamYear(tokens.Last()) && value.Length > 0)
                {
                    //true means this is an attribute_year column, subtract 5 digits, 4 for year and one '_'
                    string name = column.ColumnName.Substring(0, column.ColumnName.Length - 5);
                    AttributeYearlyValueModel attributeYearValue = new AttributeYearlyValueModel();

                    attributeYearValue.Year = Convert.ToInt32(tokens.Last());
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