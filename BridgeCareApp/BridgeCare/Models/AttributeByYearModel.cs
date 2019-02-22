using BridgeCare.Interfaces;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class AttributeByYearModel
    {
        public AttributeByYearModel()
        {
            yearlyvalues = new List<AttributeYearlyValueModel>();
        }
        public string name { get; set; }
        public List<AttributeYearlyValueModel> yearlyvalues { get; set; }

    }
}