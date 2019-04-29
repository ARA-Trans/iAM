using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IInventoryItemDetailModelGenerator
    {
        InventoryItemDetailModel MakeInventoryItemDetailModel(InventoryModel inventoryModel);
    }
}
