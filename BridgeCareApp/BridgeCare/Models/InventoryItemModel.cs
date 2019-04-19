namespace BridgeCare.Models
{
    public class InventoryItemModel
    {
        public InventoryItemModel(string columnKey, string AlphaNumericId, string IdName)
        {
            ColumnName = columnKey;
            Id = AlphaNumericId;
            ViewName = IdName;
        }

        public string ColumnName { get; set; }
        public string Id { get; set; }
        public string ViewName { get; set; }

        public dynamic Value { get; set; }
    }
}