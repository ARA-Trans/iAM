using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
            // check if new priority has priority funds
            if (priorityModel.PriorityFunds.Any())
            {
                // create new priority funds for new priority
                PRIORITYFUNDS = new List<PriorityFundEntity>();
                priorityModel.PriorityFunds.ForEach(priorityFund =>
                {
                    PRIORITYFUNDS.Add(new PriorityFundEntity(priorityFund));
                });
            }
        }
    }
}