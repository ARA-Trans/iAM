namespace BridgeCare.Models
{
    public class LabelValue
    {
        public LabelValue(string label, string value)
        {
            Label = label;
            Value = value;
        }

        string Label { get; set; }

        string Value { get; set; }
    }
}