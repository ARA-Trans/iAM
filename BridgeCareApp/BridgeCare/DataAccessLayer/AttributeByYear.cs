using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
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

        public List<AttributeByYearModel> GetProjectedAttributes(int SimulationId, int NetworkId, int SectionId, BridgeCareContext db)
        {
            DataTable dt = null;
            var select = String.Format("SELECT * FROM SIMULATION_{0}_{1} WHERE SectionID={2}", NetworkId, SimulationId, SectionId);

            try
            {
                dt = db.NonEntitySQLQuery(select);
                List<AttributeByYearModel> returnvals = DataTableToAttributeList(dt);
                return returnvals;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Query:" + select);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return null;
        }

        public List<AttributeByYearModel> GetHistoricalAttributes(int NetworkId, int sectionID, BridgeCareContext db)
        {
            DataTable dt = null;
            var select = String.Format(
                "SELECT * FROM SEGMENT_{0}_NS0 WHERE SectionID={1}",
                NetworkId, sectionID);

            try
            {
                dt = db.NonEntitySQLQuery(select);
                List<AttributeByYearModel> returnvals = DataTableToAttributeList(dt);
                return returnvals;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Query:" + select);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return null;
        }

        public List<AttributeByYearModel> DataTableToAttributeList(DataTable dt)
        {
            List<AttributeByYearModel> returnValues = new List<AttributeByYearModel>();

            foreach (DataColumn col in dt.Columns)
            {
                string value = dt.Rows[0][col.ColumnName].ToString();

                string[] tokens = col.ColumnName.Split('_');
                if (UtilityFunctions.IsIamYear(tokens.Last()) && value.Length > 0)
                {
                    //yes means this is an attribute_year column, subtract 5 digits, 4 for year and one '_'

                    string name = col.ColumnName.Substring(0, col.ColumnName.Length - 5);
                    AttributeYearlyValueModel yvm = new AttributeYearlyValueModel();

                    yvm.year = Convert.ToInt32(tokens.Last());
                    yvm.value = value;

                    AttributeByYearModel temp = IndexofName(returnValues, name);
                    if (temp == null)
                    {
                        temp = new AttributeByYearModel();
                        temp.name = name;
                    }

                    temp.yearlyvalues.Add(yvm);
                    returnValues.Add(temp);
                }


            }
            return returnValues;
        }
        // return reference to array element where name  matches findname
        // encapsulated here as brute force version , could utilize map instead
        public AttributeByYearModel IndexofName(List<AttributeByYearModel> lam, string findname)
        {
            foreach (AttributeByYearModel aym in lam)
            {
                if (aym.name == findname)
                    return aym;
            }
            return null;

        }
    }
}