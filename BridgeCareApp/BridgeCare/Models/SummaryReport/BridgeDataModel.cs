namespace BridgeCare.Models
{
    public class BridgeDataModel
    {
        //Below data fetched from table PennDot_Report_A
        public string BridgeID { get; set; }

        public int BRKey { get; set; }

        public string District { get; set; }

        public double DeckArea { get; set; }

        public string NHS { get; set; }

        public string BPN { get; set; }

        public string FunctionalClass { get; set; }

        public int YearBuilt { get; set; }

        public int StructureLength { get; set; }

        public string StructureType { get; set; }
        public string PlanningPartner { get; set; }
        public string Posted { get; set; }
        public int AdtTotal { get; set; }
        public int P3 { get; set; }
        public int ParallelBridge { get; set; }
        public double RiskScore { get; set; }

        //Below data fetched from table PENNDOT_BRIDGE_DATA
        public int BridgeFamily { get; set; }

        public int Age { get; set; }

        public string BridgeCulvert { get; set; }

        //Computed
        public string ADTOverTenThousand { get; set; }
    }
}
