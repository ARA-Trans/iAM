using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class PriorityLibraryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PriorityModel> Priorities { get; set; }

        public PriorityLibraryModel()
        {
            Priorities = new List<PriorityModel>();
        }

        public PriorityLibraryModel(SimulationEntity simulation, List<PriorityModel> priorities)
        {
            Id = simulation.SIMULATIONID;
            Name = simulation.SIMULATION;
            Description = simulation.COMMENTS;
            Priorities = priorities;
        }
    }
}