namespace BridgeCare.Models
{
    public class InventoryItemModel
    {
        public InventoryItemModel(string columnKey, string AlphaNumericId, string IdName)
        {
            ColumnName = columnKey;
            Id = AlphaNumericId;
            ViewName = IdName;
            DisplayValue = "-";
        }

        // It is added to fix instantiation issue with same values.
        public InventoryItemModel()
        {
        }

        public string ColumnName { get; set; }
        public string Id { get; set; }
        public string ViewName { get; set; }

        public string DisplayValue { get; set; }
    }
}