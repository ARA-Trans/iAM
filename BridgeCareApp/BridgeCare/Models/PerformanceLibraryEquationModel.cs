using System;

namespace BridgeCare.Models
{
    public class PerformanceLibraryEquationModel
    {
        public int Id { get; set; }
        // This is SimulationId for Performance
        public int PerformanceLibraryId { get; set; }
        public string Attribute { get; set; }
        public string EquationName { get; set; }
        public string Criteria { get; set; }
        public string Equation { get; set; }
        public Nullable<bool> Shift { get; set; }  
        public Nullable<bool> Piecewise { get; set; }
        public Nullable<bool> IsFunction { get; set; }
    }
}