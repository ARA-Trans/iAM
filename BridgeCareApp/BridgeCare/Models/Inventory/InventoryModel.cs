using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class InventoryModel
    {
        public InventoryModel()
        {
            InventoryItems = new List<InventoryItemModel>();      
        }

        public List<InventoryItemModel> InventoryItems { get; set; }  
        
        public void AddModel(InventoryItemModel item,string value)
        {
            item.DisplayValue = value;
            InventoryItems.Add(item);
        }
    }
}