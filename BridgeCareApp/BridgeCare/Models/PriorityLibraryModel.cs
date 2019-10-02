using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class PriorityLibraryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PriorityModel> Priorities { get; set; }

        public PriorityLibraryModel()
        {
            Priorities = new List<PriorityModel>();
        }

        public PriorityLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            Description = entity.COMMENTS;
            Priorities = entity.PRIORITIES.Any()
                ? entity.PRIORITIES.Select(p => new PriorityModel(p)).ToList()
                : new List<PriorityModel>();
        }
    }
}