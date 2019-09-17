using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class AttributesDAL : IAttributeRepo
    {
        public List<AttributeModel> GetAttributes(BridgeCareContext db)
        {
            if (!db.Attributes.Any())
                throw new RowNotInTableException("No 'Attribute' data was found.");

            return db.Attributes.ToList().Select(attribute => new AttributeModel(attribute)).ToList();
        }
    }
}