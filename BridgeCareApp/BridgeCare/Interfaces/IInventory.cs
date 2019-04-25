using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IInventory
    {
        InventoryModel GetInventory(SectionModel data, BridgeCareContext db);
    }
}