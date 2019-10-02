using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class DeficientLibraryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DeficientModel> Deficients { get; set; }

        public DeficientLibraryModel()
        {
            Deficients = new List<DeficientModel>();
        }

        public DeficientLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            Description = entity.COMMENTS;
            Deficients = entity.DEFICIENTS.Any()
                ? entity.DEFICIENTS.Select(d => new DeficientModel(d)).ToList()
                : new List<DeficientModel>();
        }
    }
}