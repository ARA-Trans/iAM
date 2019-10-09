using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("SIMULATIONS")]
    public class SimulationEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

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
        public virtual ICollection<RemainingLifeLimitsEntity> REMAINING_LIFE_LIMITS { get; set; }
        public virtual ICollection<CriteriaDrivenBudgetsEntity> CriteriaDrivenBudgets { get; set; }

        public SimulationEntity()
        {
            YEARLYINVESTMENTS = new HashSet<YearlyInvestmentEntity>();
        }

        public SimulationEntity(CreateSimulationDataModel model)
        {
            NETWORKID = model.NetworkId;
            SIMULATION = model.Name;
            DATE_CREATED = DateTime.Now;
            ANALYSIS = "Incremental Benefit/Cost";
            BUDGET_CONSTRAINT = "As Budget Permits";
            WEIGHTING = "none";
            COMMITTED_START = DateTime.Now.Year;
            COMMITTED_PERIOD = 1;

            YEARLYINVESTMENTS = new List<YearlyInvestmentEntity>
            {
                new YearlyInvestmentEntity
                {
                    YEAR_ = DateTime.Now.Year,
                    BUDGETNAME = "Rehabilitation",
                    AMOUNT = 5000000
                },
                new YearlyInvestmentEntity
                {
                    YEAR_ = DateTime.Now.Year,
                    BUDGETNAME = "Maintenance",
                    AMOUNT = 5000000
                },
                new YearlyInvestmentEntity
                {
                    YEAR_ = DateTime.Now.Year,
                    BUDGETNAME = "Construction",
                    AMOUNT = 5000000
                }
            };

            TREATMENTS = new List<TreatmentsEntity>
            {
                new TreatmentsEntity()
                {
                    TREATMENT = "No Treatment",
                    BEFOREANY = 1,
                    BEFORESAME = 1,
                    BUDGET = "Construction,Maintenance,Rehabilitation",
                    DESCRIPTION = "Default Treatment",
                    OMS_IS_EXCLUSIVE = null,
                    OMS_IS_REPEAT = null,
                    OMS_REPEAT_START = null,
                    OMS_REPEAT_INTERVAL = null,
                    FEASIBILITIES = new List<FeasibilityEntity>
                    {
                        new FeasibilityEntity
                        {
                            CRITERIA = ""
                        }
                    },
                    CONSEQUENCES = new List<ConsequencesEntity>
                    {
                        new ConsequencesEntity
                        {
                            ATTRIBUTE_ = "AGE",
                            CHANGE_ = "+1"
                        }
                    }
                }
            };

            INVESTMENTS = new InvestmentsEntity()
            {
                FIRSTYEAR = DateTime.Now.Year,
                NUMBERYEARS = 1,
                INFLATIONRATE = 0,
                DISCOUNTRATE = 0,
                BUDGETORDER = "Rehabilitation,Maintenance,Construction"
            };

            CriteriaDrivenBudgets = new List<CriteriaDrivenBudgetsEntity>
            {
                new CriteriaDrivenBudgetsEntity
                {
                    BUDGET_NAME = "Maintenance",
                    CRITERIA = ""
                },
                new CriteriaDrivenBudgetsEntity
                {
                    BUDGET_NAME = "Rehabilitation",
                    CRITERIA = ""
                },
                new CriteriaDrivenBudgetsEntity
                {
                    BUDGET_NAME = "Construction",
                    CRITERIA = ""
                }
            };
        }
    }
}