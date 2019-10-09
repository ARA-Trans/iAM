namespace BridgeCare.Models
{
    public class InventoryItemModel
    {
        public string ColumnName { get; set; }
        public string Id { get; set; }
        public string ViewName { get; set; }
        public string DisplayValue { get; set; }

        public InventoryItemModel() { }

        public InventoryItemModel(string columnName, string id, string viewName)
        {
            ColumnName = columnName;
            Id = id;
            ViewName = viewName;
            DisplayValue = "-";
        }
    }
}