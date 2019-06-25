namespace BridgeCare.Models
{
    public class DeficientReportModel
    {
        public int TargetID { get; set; }
        public int Years { get; set; }
        public double TargetMet { get; set; }
        public bool IsDeficient { get; set; }
    }
}