using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IAttributeNames
    {
        IQueryable<AttributeNameModel> GetAttributeNames(BridgeCareContext db);
    }
}