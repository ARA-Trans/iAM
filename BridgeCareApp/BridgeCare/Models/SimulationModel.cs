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
        public int simulationId { get; set; }
        public string simulationName { get; set; }
        public string networkName { get; set; }
        public string Owner { get; set; }
        public string Creator { get; set; }
        [Required]
        public int networkId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastRun { get; set; }
        public List<SimulationUserModel> Users { get; set; }

        public string status { get; set; }

        public SimulationModel() { }

        public SimulationModel(SimulationEntity entity)
        {
            Owner = entity.OWNER;
            Creator = entity.CREATOR;
            Users = entity.USERS.Select(userEntity => new SimulationUserModel(userEntity)).ToList();
            simulationId = entity.SIMULATIONID;
            simulationName = entity.SIMULATION;
            networkId = entity.NETWORKID ?? 0;
            Created = entity.DATE_CREATED;
            LastRun = entity.DATE_LAST_RUN ?? DateTime.Now;
            networkName = entity.NETWORK?.NETWORK_NAME;
        }
    }
}
