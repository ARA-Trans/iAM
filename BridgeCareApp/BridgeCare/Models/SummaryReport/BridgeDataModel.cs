namespace BridgeCare.Models
{
    public class BridgeDataModel
    {
        //Below data fetched from table PennDot_Report_A
        public string BridgeID { get; set; }

        public int BRKey { get; set; }

        public string District { get; set; }

        public string DeckArea { get; set; }

        public string NHS { get; set; }

        public string BPN { get; set; }

        public string FunctionalClass { get; set; }

        public string YearBuilt { get; set; }

        public string StructureLength { get; set; }

        public string StructureType { get; set; }
        public string PlanningPartner { get; set; }
        public string Posted { get; set; }
        public string AdtTotal { get; set; }
        public int P3 { get; set; }

        //Below data fetched from table PENNDOT_BRIDGE_DATA
        public string BridgeFamily { get; set; }

        public string Age { get; set; }

        public string BridgeCulvert { get; set; }

        //Computed
        public string ADTOverTenThousand { get; set; }

        //Below data fetched from table SD_RISK
        public double RiskScore { get; set; }
    }
}
