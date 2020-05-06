using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype.Attributes
{
    public class NumericAttribute : AttributeDatum<double>
    {
        public NumericAttribute(double maximum, double minimum, double value, Location location) : base(value, location)
        {
            Maximum = maximum;
            Minimum = minimum;
        }

        public double Maximum { get; }
        public double Minimum { get; }
        
    }
}
