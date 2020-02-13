using System;
using System.ComponentModel.DataAnnotations;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class SimulationModel
    {
        [Required]
        public int simulationId { get; set; }
        public string simulationName { get; set; }
        public string networkName { get; set; }
        [Required]
        public int networkId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastRun { get; set; }

        public string status { get; set; }

        public SimulationModel() { }

        public SimulationModel(SimulationEntity entity)
        {
            simulationId = entity.SIMULATIONID;
            simulationName = entity.SIMULATION;
            networkId = entity.NETWORKID ?? 0;
            Created = entity.DATE_CREATED;
            LastRun = entity.DATE_LAST_RUN ?? DateTime.Now;
            networkName = entity.NETWORK.NETWORK_NAME;
        }
    }
}
