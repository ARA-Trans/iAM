using System.Data;

namespace BridgeCare.Models
{
    public class DeficientResult
    {
        public DataTable Deficients { get; set; } = new DataTable();
        public CellAddress Address = new CellAddress();
    }
}