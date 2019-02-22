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
    public class AttributeNames : IAttributeNames
    {
        public AttributeNames() { }

        public IQueryable<AttributeNameModel> GetAttributeNames(BridgeCareContext db)
        {
            IQueryable<AttributeNameModel> rawQueryForData = null;

            var select = ("SELECT ATTRIBUTE_ as Name FROM Attributes_");

            try
            {
                rawQueryForData = db.Database.SqlQuery<AttributeNameModel>(select).AsQueryable();

                //foreach (AttributeNameModel g in rawQueryForData)
                //{
                //    string name = g.Attribute_;
                //}
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "ATTRIBUTE_ from ATTRIBUTES_ SQL Select Error_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForData;
        }
    }
}