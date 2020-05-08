namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class NumericAttribute : AttributeDatum<double>
    {
        public NumericAttribute(Attribute attribute, double maximum, double minimum, double _default, double value, Location location) : base(attribute, value, _default, location)
        {
            Maximum = maximum;
            Minimum = minimum;
        }

        public double Maximum { get; }

        public double Minimum { get; }
    }
}
