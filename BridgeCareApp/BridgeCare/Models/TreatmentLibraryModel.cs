using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class TreatmentLibraryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TreatmentModel> Treatments { get; set; }

        public TreatmentLibraryModel() { }

        public TreatmentLibraryModel(SimulationEntity simulationEntity)
        {
            Id = simulationEntity.SIMULATIONID.ToString();
            Name = simulationEntity.SIMULATION;
            Description = simulationEntity.COMMENTS;
            Treatments = simulationEntity.TREATMENTS.Any()
                ? simulationEntity.TREATMENTS.Select(t => new TreatmentModel(t)).ToList()
                : new List<TreatmentModel>();
        }
    }
}