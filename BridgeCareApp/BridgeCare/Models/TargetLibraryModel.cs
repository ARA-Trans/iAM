using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class TargetLibraryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TargetModel> Targets { get; set; }

        public TargetLibraryModel()
        {
            Targets = new List<TargetModel>();
        }

        public TargetLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            Description = entity.COMMENTS;
            Targets = entity.TARGETS.Any()
                ? entity.TARGETS.Select(t => new TargetModel(t)).ToList()
                : new List<TargetModel>();
        }
    }
}