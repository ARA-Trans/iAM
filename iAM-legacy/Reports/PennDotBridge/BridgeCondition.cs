using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reports.PennDotBridge
{
    public class BridgeCondition
    {
        public string SectionId { get; }
        public string BusinessPlanNetwork { get; }
        public double DeckArea { get; }
        public string BridgeType { get; }
        public List<StruturalDeficientSummary> AnnualStructuralDeficient { get; }



        public BridgeCondition(DataRow row, int startYear, int numberYear)
        {
            AnnualStructuralDeficient = new List<StruturalDeficientSummary>();
            SectionId = row["SECTIONID"].ToString();
            BusinessPlanNetwork = row["BUS_PLAN_NETWORK_0"].ToString();
            DeckArea = Convert.ToDouble(row["DECK_AREA_0"]);
            BridgeType = row["BRIDGE_TYPE_0"].ToString();

            for (var year = startYear; year < startYear + numberYear; year++)
            {
                var sub = Convert.ToDouble(row["SUB_SEEDED_" + year]);
                var sup = Convert.ToDouble(row["SUP_SEEDED_" + year]);
                var deck = Convert.ToDouble(row["DECK_SEEDED_" + year]);
                var culv = Convert.ToDouble(row["CULV_SEEDED_" + year]);
                AnnualStructuralDeficient.Add(new StruturalDeficientSummary(year,BridgeType,sub,sup,deck,culv));
            }
            var sub0 = Convert.ToDouble(row["SUB_SEEDED_0"]);
            var sup0 = Convert.ToDouble(row["SUP_SEEDED_0"]);
            var deck0 = Convert.ToDouble(row["DECK_SEEDED_0"]);
            var culv0 = Convert.ToDouble(row["CULV_SEEDED_0"]);
            AnnualStructuralDeficient.Add(new StruturalDeficientSummary(0, BridgeType, sub0, sup0, deck0, culv0));





        }



    }

}
