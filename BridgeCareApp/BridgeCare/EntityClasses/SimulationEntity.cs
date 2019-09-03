using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("SIMULATIONS")]
    public class SimulationEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SimulationEntity()
        {
            YEARLYINVESTMENTS = new HashSet<YearlyInvestmentEntity>();
        }

        [Key]
        public int SIMULATIONID { get; set; }
        public int? NETWORKID { get; set; }
        public DateTime? DATE_CREATED { get; set; }
        public DateTime? DATE_LAST_RUN { get; set; }
        [StringLength(50)]
        public string ANALYSIS { get; set; }
        [StringLength(50)]
        public string BUDGET_CONSTRAINT { get; set; }
        [StringLength(50)]
        public string WEIGHTING { get; set; }
        public int COMMITTED_START { get; set; }
        public int COMMITTED_PERIOD { get; set; }
        public double BENEFIT_LIMIT { get; set; }
        public string JURISDICTION { get; set; }
        public string SIMULATION_VARIABLES { get; set; }
        public string BENEFIT_VARIABLE { get; set; }
        [StringLength(8000)]
        public string COMMENTS { get; set; }
        [StringLength(50)]
        public string SIMULATION { get; set; }

        public virtual NetworkEntity NETWORK { get; set; }
        public virtual InvestmentsEntity INVESTMENTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YearlyInvestmentEntity> YEARLYINVESTMENTS { get; set; }
        public virtual ICollection<TreatmentsEntity> TREATMENTS { get; set; }
        public virtual ICollection<PerformanceEntity> PERFORMANCES { get; set; }
        public virtual ICollection<PrioritizedNeedEntity> PRIORITIZEDNEEDS { get; set; }
        public virtual ICollection<PriorityEntity> PRIORITIES { get; set; }
        public virtual ICollection<DeficientsEntity> DEFICIENTS { get; set; }
        public virtual ICollection<TargetDeficientEntity> TARGET_DEFICIENTS { get; set; }
        public virtual ICollection<TargetsEntity> TARGETS { get; set; }                     
        public virtual ICollection<CommittedEntity> COMMITTEDPROJECTS { get; set; }
        public virtual ICollection<CriteriaDrivenBudgetsEntity> CriteriaDrivenBudgets { get; set; }
    }
}