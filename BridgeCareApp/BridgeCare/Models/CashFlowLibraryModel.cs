using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class CashFlowLibraryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SplitTreatmentModel> SplitTreatments { get; set; }

        public CashFlowLibraryModel()
        {
            SplitTreatments = new List<SplitTreatmentModel>();
        }

        public CashFlowLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            Description = entity.COMMENTS;
            SplitTreatments = entity.SPLIT_TREATMENTS.Any()
                ? entity.SPLIT_TREATMENTS.Select(st => new SplitTreatmentModel(st)).ToList()
                : new List<SplitTreatmentModel>();
        }
    }
}