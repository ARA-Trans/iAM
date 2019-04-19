using BridgeCare.Models;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Interfaces
{
    public interface IInventory
    {
        IQueryable<InventoryModel> GetInventory(SectionModel data, BridgeCareContext db);
    }
}
