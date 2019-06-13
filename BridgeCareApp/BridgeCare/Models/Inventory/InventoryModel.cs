using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class InventoryModel
    {
        public InventoryModel()
        {
            InventoryItems = new List<InventoryItemModel>();
            InventoryNbiLoadRatings = new List<InventoryNbiLoadRatingModel>();
        }

        public List<InventoryItemModel> InventoryItems { get; set; }

        public List<InventoryNbiLoadRatingModel> InventoryNbiLoadRatings { get; set; }

        public void AddModel(InventoryItemModel item, string value)
        {
            item.DisplayValue = value;
            InventoryItems.Add(item);
        }
    }
}