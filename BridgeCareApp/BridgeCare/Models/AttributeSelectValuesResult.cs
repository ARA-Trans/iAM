﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class AttributeSelectValuesResult
    {
        public string Attribute { get; set; }
        public List<string> Values { get; set; }
        public string ResultMessage { get; set; }

        public AttributeSelectValuesResult() { }

        public AttributeSelectValuesResult(string attribute, List<string> values, string resultMessage)
        {
            Attribute = attribute;
            Values = values;
            ResultMessage = resultMessage;
        }
    }
}
