using System;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class SimulationModel
    {
        [Required]
        public int SimulationId { get; set; }
        public string SimulationName { get; set; }
        public string NetworkName { get; set; }
        [Required]
        public int NetworkId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastRun { get; set; }

        public SimulationModel() { }

        public SimulationModel(SimulationEntity simulationEntity)
        {
            SimulationId = simulationEntity.SIMULATIONID;
            SimulationName = simulationEntity.SIMULATION;
            NetworkId = simulationEntity.NETWORKID ?? 0;
            Created = simulationEntity.DATE_CREATED;
            LastRun = simulationEntity.DATE_LAST_RUN ?? DateTime.Now;
            NetworkName = simulationEntity.NETWORK.NETWORK_NAME;
        }
    }
}