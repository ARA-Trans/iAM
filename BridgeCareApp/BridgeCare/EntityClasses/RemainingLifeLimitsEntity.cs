using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("REMAINING_LIFE_LIMITS")]
    public class RemainingLifeLimitsEntity
    {
        [Key]
        public int REMAINING_LIFE_ID { get; set; }
        public int SIMULATION_ID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public double REMAINING_LIFE_LIMIT { get; set; }
        public string CRITERIA { get; set; }
        [ForeignKey("SIMULATION_ID")]
        public virtual SimulationEntity SIMULATION { get; set; }

        public RemainingLifeLimitsEntity() { }

        public RemainingLifeLimitsEntity(int simulationId, RemainingLifeLimitModel remainingLifeLimitModel)
        {
            SIMULATION_ID = simulationId;
            ATTRIBUTE_ = remainingLifeLimitModel.Attribute;
            REMAINING_LIFE_LIMIT = remainingLifeLimitModel.Limit;
            CRITERIA = remainingLifeLimitModel.Criteria;
        }

        public static void DeleteEntry(RemainingLifeLimitsEntity remainingLifeLimit, BridgeCareContext db)
        {
            db.Entry(remainingLifeLimit).State = EntityState.Deleted;
        }
    }
}