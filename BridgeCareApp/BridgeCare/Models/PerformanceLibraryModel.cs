using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class PerformanceLibraryModel
    {
        public PerformanceLibraryModel()
        {
            Equations = new List<PerformanceLibraryEquationModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PerformanceLibraryEquationModel> Equations { get; set; }
    }
}