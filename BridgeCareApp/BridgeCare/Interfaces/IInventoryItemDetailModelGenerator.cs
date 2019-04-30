using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IInventoryItemDetailModelGenerator
    {
        InventoryItemDetailModel MakeInventoryItemDetailModel(SectionModel sectionModel, InventoryModel inventoryModel);
    }
}
