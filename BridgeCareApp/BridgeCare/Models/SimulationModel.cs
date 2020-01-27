using System;
using System.ComponentModel.DataAnnotations;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class SimulationModel
    {
        [Required]
        public int SimulationId { get; set; }
        public string SimulationName { get; set; }
        public string NetworkName { get; set; }
        public string Owner { get; set; }
        public string Creator { get; set; }
        [Required]
        public int NetworkId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastRun { get; set; }

        public SimulationModel() { }

        public SimulationModel(SimulationEntity entity)
        {
            SimulationId = entity.SIMULATIONID;
            SimulationName = entity.SIMULATION;
            Owner = entity.OWNER;
            Creator = entity.CREATOR;
            NetworkId = entity.NETWORKID ?? 0;
            Created = entity.DATE_CREATED;
            LastRun = entity.DATE_LAST_RUN ?? DateTime.Now;
            NetworkName = entity.NETWORK.NETWORK_NAME;
        }
    }
}
