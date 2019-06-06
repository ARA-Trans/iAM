using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class Attributes : IAttributeNames
    {
        public Attributes()
        {
        }

        public IQueryable<AttributeModel> GetAttributes(BridgeCareContext db)
        {
            var select = ("SELECT ATTRIBUTE_ as Name, TYPE_ as Type FROM Attributes_");
            var rawQueryForData = Enumerable.Empty<AttributeModel>();
            try
            {
                rawQueryForData = db.Database.SqlQuery<AttributeModel>(select);
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