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
            var rawQueryForData = Enumerable.Empty<AttributeNameModel>();
            try
            {
                rawQueryForData = db.Database.SqlQuery<AttributeNameModel>(select);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "ATTRIBUTE_ from ATTRIBUTES_ SQL Select Error_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForData.AsQueryable();
        }
    }
}