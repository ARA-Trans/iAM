using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    // Top level model not as interface to db but for producing json
    [DataContract]
    public class TreatmentsModel
    {
        //simulationID or libraryID
        [DataMember(Name = "id")]
        public int SimulationId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "treatement")]
        public List<TreatmentScenarioModel> Treatments { get; set; }
    }
}