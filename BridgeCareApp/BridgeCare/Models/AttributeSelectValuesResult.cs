using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class AttributeSelectValuesResult
    {
        public List<string> Values { get; set; }
        public string ResultMessage { get; set; }

        public AttributeSelectValuesResult() { }

        public AttributeSelectValuesResult(List<string> values, string resultMessage)
        {
            Values = values;
            ResultMessage = resultMessage;
        }
    }
}
