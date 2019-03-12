namespace BridgeCare.Models
{
    public class DetailReportModel
    {
        public string Treatment { get; set; }
        public bool IsCommitted { get; set; }
        public int NumberTreatment { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
    }
}