using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class NumericAttribute : Attribute
    {
        public NumericAttribute(string name,
                                AttributeConnection attributeConnection,
                                double defaultValue,
                                double maximum,
                                double minimum) : base(name, attributeConnection)
        {
            DefaultValue = defaultValue;
            Maximum = maximum;
            Minimum = minimum;
        }

        public double DefaultValue { get; }
        public double Maximum { get; }
        public double Minimum { get; }
    }
}
