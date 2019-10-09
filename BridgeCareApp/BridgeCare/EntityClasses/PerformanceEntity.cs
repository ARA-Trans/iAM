using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("PERFORMANCE")]
    public class PerformanceEntity
    {
        [Key]
        public int PERFORMANCEID { get; set; }
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string EQUATIONNAME { get; set; }
        public string CRITERIA { get; set; }
        public string EQUATION { get; set; }
        public bool? SHIFT { get; set; }
        public byte?[] BINARY_EQUATION { get; set; }
        public byte?[] BINARY_CRITERIA { get; set; }
        public bool? PIECEWISE { get; set; }
        public bool? ISFUNCTION { get; set; }

        [ForeignKey("ATTRIBUTE_")]
        public virtual AttributesEntity ATTRIBUTE { get; set; }
        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }

        public PerformanceEntity() { }

        public PerformanceEntity(int simulationId, PerformanceLibraryEquationModel performanceModel)
        {
            SIMULATIONID = simulationId;
            ATTRIBUTE_ = performanceModel.Attribute;
            EQUATIONNAME = performanceModel.EquationName;
            CRITERIA = performanceModel.Criteria;
            EQUATION = performanceModel.Equation;
            SHIFT = performanceModel.Shift ?? false;
            PIECEWISE = performanceModel.Piecewise ?? false;
            ISFUNCTION = performanceModel.IsFunction ?? false;
        }

        public static void DeleteEntry(PerformanceEntity performanceEntity, BridgeCareContext db)
        {
            db.Entry(performanceEntity).State = EntityState.Deleted;
        }
    }
}