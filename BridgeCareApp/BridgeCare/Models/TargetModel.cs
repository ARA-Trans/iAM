using System.Data;

namespace BridgeCare.Models
{
    public class TargetModel
    {
        public DataTable Targets { get; set; } = new DataTable();
        public CellAddress Address = new CellAddress();
    }
}