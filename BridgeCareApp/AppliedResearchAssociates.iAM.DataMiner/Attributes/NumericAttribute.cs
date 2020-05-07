using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class NumericAttribute : AttributeDatum<double>
    {
        public NumericAttribute(Attribute attribute, double maximum, double minimum, double value, Location location) : base(attribute, value, location)
        {
            Maximum = maximum;
            Minimum = minimum;
        }

        public double Maximum { get; }
        public double Minimum { get; }

    }
}
