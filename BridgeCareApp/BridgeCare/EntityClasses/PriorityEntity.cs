using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("PRIORITY")]
    public class PriorityEntity
    {
        [Key]
        public int PRIORITYID { get; set; }
        public int SIMULATIONID { get; set; }
        public int? PRIORITYLEVEL { get; set; }
        public string CRITERIA { get; set; }
        public byte?[] BINARY_CRITERIA { get; set; }
        public int? YEARS { get; set; }

        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }
        public ICollection<PriorityFundEntity> PRIORITYFUNDS { get; set; }

        public PriorityEntity() { }

        public PriorityEntity(int simulationId, PriorityModel priorityModel)
        {
            SIMULATIONID = simulationId;
            PRIORITYLEVEL = priorityModel.PriorityLevel;
            CRITERIA = priorityModel.Criteria;
            YEARS = priorityModel.Year;

            if (priorityModel.PriorityFunds.Any())
            {
                priorityModel.PriorityFunds.ForEach(priorityFund =>
                {
                    PRIORITYFUNDS.Add(new PriorityFundEntity(priorityFund));
                });
            }
        }

        public static void DeleteEntry(PriorityEntity priorityEntity, BridgeCareContext db)
        {
            if (priorityEntity.PRIORITYFUNDS.Any())
            {
                priorityEntity.PRIORITYFUNDS.ToList()
                    .ForEach(priorityFundEntity => db.Entry(priorityFundEntity).State = EntityState.Deleted);
            }

            db.Entry(priorityEntity).State = EntityState.Deleted;
        }
    }
}