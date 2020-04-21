using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models.SummaryReport.ParametersTAB
{
    public class ParametersModel
    {
        public NHSModel nHSModel { get; set; } = new NHSModel();
        public List<string> BPNValues { get; } = new List<string>();
        public List<string> Status { get; } = new List<string>();
        public int P3 { get; set; }
        public string LengthBetween8and20 { get; set; } = "N";
        public string LengthGreaterThan20 { get; set; } = "N";
        public List<string> OwnerCode { get; } = new List<string>();
        public List<string> FunctionalClass { get; } = new List<string>();
    }
}
