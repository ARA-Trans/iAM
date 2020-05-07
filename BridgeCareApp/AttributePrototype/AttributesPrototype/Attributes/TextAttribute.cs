using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype.Attributes
{
    public class TextAttribute : AttributeDatum<string>
    {
        public TextAttribute(Attribute attribute, string value, Location location) : base(attribute, value, location)
        {
        }
    }
}
