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
    }
}