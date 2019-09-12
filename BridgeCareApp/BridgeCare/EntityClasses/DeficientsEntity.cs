using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("DEFICIENTS")]
    public class DeficientsEntity
    {
        [Key]
        public int ID_ { get; set; }
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string DEFICIENTNAME { get; set; }
        public double? DEFICIENT { get; set; }
        public double? PERCENTDEFICIENT { get; set; }
        public string CRITERIA { get; set; }
        public byte?[] BINARY_CRITERIA { get; set; }

        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }

        public DeficientsEntity() { }

        public DeficientsEntity(int simulationId, DeficientModel deficientModel)
        {
            SIMULATIONID = simulationId;
            ATTRIBUTE_ = deficientModel.Attribute;
            DEFICIENTNAME = deficientModel.Name;
            DEFICIENT = deficientModel.Deficient ?? 0;
            PERCENTDEFICIENT = deficientModel.PercentDeficient ?? 0;
            CRITERIA = deficientModel.Criteria;
        }

        public static void DeleteEntry(DeficientsEntity deficient, BridgeCareContext db)
        {
            db.Entry(deficient).State = EntityState.Deleted;
        }
    }
}