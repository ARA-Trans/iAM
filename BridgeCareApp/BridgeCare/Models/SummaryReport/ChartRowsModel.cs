using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class ChartRowsModel
    {
        public int TotalBridgeCountSectionYearsRow { get; set; }

        public int TotalDeckAreaSectionYearsRow { get; set; }

        public int TotalPoorBridgesCountSectionYearsRow { get; set; }

        public int TotalPoorBridgesDeckAreaSectionYearsRow { get; set; }

        public int NHSBridgeCountSectionYearsRow { get; set; }

        public int NonNHSBridgeCountSectionYearsRow { get; set; }

        public int NHSBridgeCountPercentSectionYearsRow { get; set; }

        public int NonNHSBridgeCountPercentSectionYearsRow { get; set; }

        public int NHSDeckAreaSectionYearsRow { get; set; }

        public int NHSBridgeDeckAreaPercentSectionYearsRow { get; set; }

        public int NonNHSDeckAreaYearsRow { get; set; }

        public int NonNHSDeckAreaPercentSectionYearsRow { get; set; }

        public int TotalBridgeCountPercentYearsRow { get; set; }

        public int TotalDeckAreaPercentYearsRow { get; set; }

        public int TotalPoorDeckAreaByBPNSectionYearsRow { get; set; }

        public int TotalBridgePostedCountByBPNYearsRow { get; set; }

        public int TotalPostedBridgeDeckAreaByBPNYearsRow { get; internal set; }

        public int TotalClosedBridgeCountByBPNYearsRow { get; internal set; }

        public int TotalClosedBridgeDeckAreaByBPNYearsRow { get; internal set; }

        public int TotalPostedAndClosedByBPNYearsRow { get; internal set; }

        public int TotalCashNeededByBPNYearsRow { get; internal set; }
    }
}
