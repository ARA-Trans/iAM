using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class RemainingLifeLimitLibraryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RemainingLifeLimitModel> RemainingLifeLimits { get; set; }

        public RemainingLifeLimitLibraryModel()
        {
            RemainingLifeLimits = new List<RemainingLifeLimitModel>();
        }

        public RemainingLifeLimitLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            Description = entity.COMMENTS;
            RemainingLifeLimits = entity.REMAINING_LIFE_LIMITS.Any()
                ? entity.REMAINING_LIFE_LIMITS.Select(rll => new RemainingLifeLimitModel(rll)).ToList()
                : new List<RemainingLifeLimitModel>();
        }
    }
}