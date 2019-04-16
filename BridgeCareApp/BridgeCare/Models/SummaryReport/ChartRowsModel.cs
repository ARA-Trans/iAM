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

        public int NHSBridgeCountPercentRow { get; set; }
        
        public int NHSBridgeDeckAreaPercentRow { get; set; }
    }
}