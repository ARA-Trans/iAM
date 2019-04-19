using BridgeCare.Models;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Models
{
    public class InventoryItemModel
    {
        string DataTableName { get; set; }
        string ViewName { get; set; }

        dynamic Value { get; set; }
    }
}