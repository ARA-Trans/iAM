using System;
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
        public ICollection<PriorityFundEntity> PRIORITYFUNDS { get; set; } = new List<PriorityFundEntity>();

        public PriorityEntity() { }

        public PriorityEntity(int simulationId, List<string> budgets)
        {
            SIMULATIONID = simulationId;
            PRIORITYLEVEL = 1;
            CRITERIA = "";
            PRIORITYFUNDS = budgets.Select(budget => new PriorityFundEntity(budget)).ToList();
            YEARS = DateTime.Now.Year;
        }

        public PriorityEntity(int simulationId, PriorityModel model)
        {
            SIMULATIONID = simulationId;
            PRIORITYLEVEL = model.PriorityLevel;
            CRITERIA = model.Criteria;
            YEARS = model.Year;

            if (model.PriorityFunds.Any())
                model.PriorityFunds.ForEach(priorityFund =>
                {
                    PRIORITYFUNDS.Add(new PriorityFundEntity(priorityFund));
                });
        }

        public static void DeleteEntry(PriorityEntity entity, BridgeCareContext db)
        {
            if (entity.PRIORITYFUNDS.Any())
                entity.PRIORITYFUNDS.ToList()
                    .ForEach(pfEntity => PriorityFundEntity.DeleteEntry(pfEntity, db));

            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}