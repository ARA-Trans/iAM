using System.Data;

namespace BridgeCare.Models
{
    public class TargetReportModel
    {
        public DataTable Targets { get; set; } = new DataTable();
        public CellAddress Address = new CellAddress();
    }
}