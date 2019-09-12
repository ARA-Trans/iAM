using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IInventory
    {
        InventoryModel GetInventoryByBMSId(string bmsId, BridgeCareContext db);

        InventoryModel GetInventoryByBRKey(int brKey, BridgeCareContext db);

        List<InventorySelectionModel> GetInventorySelectionModels(BridgeCareContext db);
    }
}