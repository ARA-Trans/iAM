using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class PerformanceLibraryModel
    {
        public PerformanceLibraryModel()
        {
            Equations = new List<PerformanceLibraryEquation>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PerformanceLibraryEquation> Equations { get; set; }
    }
}