using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class InventoryItemDetailModel
    {
        public int SimulationId { get; set; }

        public string Label { get; set; } // Currently not used in UI

        public string Name { get; set; } // Currently not used in UI

        public List<LabelValue> Location { get; set; }
        public List<LabelValue> AgeAndService { get; set; }
        public List<LabelValue> Management { get; set; }
        public List<LabelValue> DeckInformation { get; set; }
        public List<LabelValue> SpanInformation { get; set; }
        public List<LabelValue> NbiLoadRating { get; set; }
        public List<LabelValue> Posting { get; set; }
        public List<LabelValue> RoadwayInfo { get; set; }

        public List<ConditionDuration> CurrentConditionDuration { get; set; }
        public List<ConditionDuration> PreviousConditionDuration { get; set; }

        public RiskScores RiskScores { get; set; }

        public OperatingRatingInventoryRatingGrouping OperatingRatingInventoryRatingGrouping { get; set; }
    }
}