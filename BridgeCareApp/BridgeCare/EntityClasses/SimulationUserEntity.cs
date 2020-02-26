using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("SIMULATION_USERS")]
    public class SimulationUserEntity
    {
        [Key]
        public int ID_ { get; set; }
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        public string USERNAME { get; set; }
        public bool CAN_MODIFY { get; set; }

        public virtual SimulationEntity SIMULATION { get; set; }

        public SimulationUserEntity() { }

        public SimulationUserEntity(int simulationId, SimulationUserModel simulationUserModel)
        {
            SIMULATIONID = simulationId;
            USERNAME = simulationUserModel.Username;
            CAN_MODIFY = simulationUserModel.CanModify;
        }

        public static void DeleteEntry(SimulationUserEntity simulationUserEntity, BridgeCareContext db)
        {
            db.Entry(simulationUserEntity).State = EntityState.Deleted;
        }
    }
}
