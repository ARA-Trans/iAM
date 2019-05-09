using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IInventory
    {
        InventoryModel GetInventoryByBMSId(string bmsId, BridgeCareContext db);

        InventoryModel GetInventoryByBRKey(int brKey, BridgeCareContext db);
    }
}