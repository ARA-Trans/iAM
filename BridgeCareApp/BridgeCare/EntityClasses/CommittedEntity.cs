using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{

    [Table("COMMITTED_")]
    public class CommittedEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CommittedEntity()
        {
            COMMIT_CONSEQUENCES = new List<CommitConsequencesEntity>();
        }
       
        public CommittedEntity(CommittedProjectModel model)
        {
            SIMULATIONID = model.SimulationId;
            SECTIONID = model.SectionId;
            YEARS = model.Years;
            TREATMENTNAME = model.TreatmentName;
            YEARSAME = model.YearSame;
            YEARANY = model.YearAny;
            BUDGET = model.Budget;
            COST_ = model.Cost;
            COMMIT_CONSEQUENCES = model.CommitConsequences.Any()
                ? model.CommitConsequences.Select(cc => new CommitConsequencesEntity(cc)).ToList()
                : new List<CommitConsequencesEntity>();
        }

        [Key]
        public int COMMITID { get; set; }
        public int SIMULATIONID { get; set; }
        public int SECTIONID { get; set; }
        public int YEARS { get; set; }
        public string TREATMENTNAME { get; set; }
        public int? YEARSAME { get; set; }
        public int? YEARANY { get; set; }
        public string BUDGET { get; set; }
        public double? COST_ { get; set; }
        public string OMS_IS_EXCLUSIVE { get; set; }
        public string OMS_IS_NOT_ALLOWED { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommitConsequencesEntity> COMMIT_CONSEQUENCES { get; set; }
        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }
    }
}
