using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class OperatingRatingInventoryRatingGrouping
    {
        public List<OperatingRatingInventoryRatingRow> RatingRows { get; set; }

        public LabelValue MinRatioLegalLoad { get; set; }
    }
}