using System.Collections.Generic;
using System.Linq;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class PerformanceLibraryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PerformanceLibraryEquationModel> Equations { get; set; }

        public PerformanceLibraryModel()
        {
            Equations = new List<PerformanceLibraryEquationModel>();
        }

        public PerformanceLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            Description = entity.COMMENTS;
            Equations = entity.PERFORMANCES.Any()
                ? entity.PERFORMANCES.Select(p => new PerformanceLibraryEquationModel(p)).ToList()
                : new List<PerformanceLibraryEquationModel>();
        }
    }
}