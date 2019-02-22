using BridgeCare.Interfaces;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class AttributeYearlyValueModel
    {
        public AttributeYearlyValueModel() { }
        public AttributeYearlyValueModel(int setyear,string setvalue)
        {
            year = setyear;
            value = setvalue;
        }
        public int year { get; set; }
        public string value { get; set; }
    }
}