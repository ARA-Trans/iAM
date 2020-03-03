using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class SimulationUserModel : CrudModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public bool CanModify { get; set; }

        public SimulationUserModel() { }

        public SimulationUserModel(SimulationUserEntity simulationUserEntity)
        {
            Id = simulationUserEntity.ID_.ToString();
            Username = simulationUserEntity.USERNAME;
            CanModify = simulationUserEntity.CAN_MODIFY;
        }

        public void UpdateSimulationUser(SimulationUserEntity entity)
        {
            entity.USERNAME = Username;
            entity.CAN_MODIFY = CanModify;
        }
    }
}
