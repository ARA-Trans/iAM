using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype.Attributes
{
    public class TextAttribute : AttributeDatum<string>
    {
        public TextAttribute(string value, Location location) : base(value, location)
        {
        }
    }
}
