using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public List<SimulationUserModel> Users { get; set; }

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
            Users = entity.USERS.Select(userEntity => new SimulationUserModel(userEntity)).ToList();
        }
    }
}
