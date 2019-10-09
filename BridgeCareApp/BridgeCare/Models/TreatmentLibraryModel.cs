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

        public TreatmentLibraryModel()
        {
            Treatments = new List<TreatmentModel>();
        }

        public TreatmentLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            Description = entity.COMMENTS;
            Treatments = entity.TREATMENTS.Any()
                ? entity.TREATMENTS.Select(t => new TreatmentModel(t)).ToList()
                : new List<TreatmentModel>();
        }
    }
}