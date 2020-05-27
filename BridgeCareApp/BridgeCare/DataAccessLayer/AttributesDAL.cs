using System;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class AttributesDAL : IAttributeRepo
    {
        /// <summary>
        /// Fetches all attributes data
        /// Throws a RowNotInTableException if no attributes are found
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>AttributeModel list</returns>
        public List<AttributeModel> GetAttributes(BridgeCareContext db)
        {
            if (!db.Attributes.Any())
            {
                var log = log4net.LogManager.GetLogger(typeof(AttributesDAL));
                log.Error("No attribute data could be found.");
                throw new RowNotInTableException("No attribute data could be found.");
            }

            return db.Attributes.ToList().Select(a => new AttributeModel(a)).ToList();
        }

        public AttributeSelectValuesResult GetAttributeSelectValues(int networkId, string attribute, BridgeCareContext db)
        {
            if (!db.NETWORKS.Any(network => network.NETWORKID == networkId))
                throw new RowNotInTableException($"Could not find network with id {networkId}");

            var validAttributes = GetAttributes(db).Select(attr => attr.Name);
            if (!validAttributes.Contains(attribute))
                throw new RowNotInTableException($"{attribute} is not a valid attribute");

            var countsQuery = $"SELECT COUNT(DISTINCT({attribute})) FROM SEGMENT_{networkId}_NS0";
            var rawQueryForCounts = db.Database.SqlQuery<int>(countsQuery).AsQueryable();
            var result = rawQueryForCounts.ToList()[0];

            if (result == 0)
                return new AttributeSelectValuesResult(new List<string>(), $"No values found for attribute {attribute}; use text input");

            if (result > 100)
                return new AttributeSelectValuesResult(new List<string>(), $"Number of values for attribute {attribute} exceeds 100; use text input");

            var dataQuery = $"SELECT DISTINCT(CAST({attribute} AS VARCHAR(255))) FROM SEGMENT_{networkId}_NS0";
            var rawQueryForData = db.Database.SqlQuery<string>(dataQuery).AsQueryable();

            return new AttributeSelectValuesResult(rawQueryForData.ToList(), "Success");
        }
    }
}
