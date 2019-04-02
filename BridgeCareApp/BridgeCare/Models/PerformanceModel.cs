using System;

namespace BridgeCare.Models
{
    public class PerformanceModel
    {
       
        public string Attribute { get; set; }
        public string EquationName { get; set; }
        public string Criteria { get; set; }
        public string Equation { get; set; }
        public Nullable<bool> Shift { get; set; }
        public byte[] BinaryEquation { get; set; }
        public byte[] BinaryCriteria { get; set; }
        public Nullable<bool> Piecwise { get; set; }
        public Nullable<bool> IsFunction { get; set; }
    }
}