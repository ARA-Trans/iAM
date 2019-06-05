using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IAttributeNames
    {
        IQueryable<AttributeModel> GetAttributes(BridgeCareContext db);
    }
}