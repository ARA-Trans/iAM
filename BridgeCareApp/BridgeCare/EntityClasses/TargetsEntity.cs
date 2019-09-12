using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("TARGETS")]
    public class TargetsEntity
    {
        [Key]
        public int ID_ { get; set; }
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        [ForeignKey("ATTRIBUTE")]
        public string ATTRIBUTE_ { get; set; }
        public int? YEARS { get; set; }
        public double? TARGETMEAN { get; set; }
        public string TARGETNAME { get; set; }
        public string CRITERIA { get; set; }
        public byte?[] BINARY_CRITERIA { get; set; }

        public virtual SimulationEntity SIMULATION { get; set; }
        public virtual AttributesEntity ATTRIBUTE { get; set; }

        public TargetsEntity() { }

        public TargetsEntity(int simulationId, TargetModel targetModel)
        {
            SIMULATIONID = simulationId;
            ATTRIBUTE_ = targetModel.Attribute;
            YEARS = targetModel.Year;
            TARGETMEAN = targetModel.TargetMean;
            TARGETNAME = targetModel.Name;
            CRITERIA = targetModel.Criteria;
        }

        public static void DeleteEntry(TargetsEntity target, BridgeCareContext db)
        {
            db.Entry(target).State = EntityState.Deleted;
        }
    }
}