using System;

namespace BridgeCare.Models
{
    public class PerformanceLibraryEquation
    {
        public int PerformanceId { get; set; }
        public string Attribute { get; set; }
        public string EquationName { get; set; }
        public string Criteria { get; set; }
        public string Equation { get; set; }
        public Nullable<bool> Shift { get; set; }  
        public Nullable<bool> Piecewise { get; set; }
        public Nullable<bool> IsFunction { get; set; }
    }
}