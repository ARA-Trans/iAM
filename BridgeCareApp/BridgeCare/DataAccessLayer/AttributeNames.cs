using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class AttributeNames : IAttributeNames
    {
        public AttributeNames()
        {
        }

        public IQueryable<AttributeNameModel> GetAttributeNames(BridgeCareContext db)
        {
            var select = ("SELECT ATTRIBUTE_ as Name FROM Attributes_");

            try
            {
                var rawQueryForData = db.Database.SqlQuery<AttributeNameModel>(select).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "ATTRIBUTE_ from ATTRIBUTES_ SQL Select Error_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return EmptyResult.AsQueryable<AttributeNameModel>();

        }
    }
}