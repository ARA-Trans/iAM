﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BridgeCare.Models
{
    // Top level model not as interface to db but for producing json
    [DataContract]
    public class TreatmentLibraryModel
    {
        //simulationID or libraryID
        [DataMember(Name = "id")]
        public int SimulationId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "treatements")]
        public List<TreatmentModel> Treatments { get; set; }
    }
}