using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IAttributeRepo
    {
        List<AttributeModel> GetAttributes(BridgeCareContext db);
        AttributeSelectValuesResult GetAttributeSelectValues(int networkId, string attribute, BridgeCareContext db);
    }
}
