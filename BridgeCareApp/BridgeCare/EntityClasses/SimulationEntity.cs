using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BridgeCare.Models;
using System.Linq;

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
        public string CREATOR { get; set; }
        public string OWNER { get; set; }

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
        public virtual ICollection<CriteriaDrivenBudgetEntity> CriteriaDrivenBudgets { get; set; }
        public virtual ICollection<SplitTreatmentEntity> SPLIT_TREATMENTS { get; set; }
        public virtual ICollection<SimulationUserEntity> USERS { get; set; }

        public bool UserCanModify(string username)
        {
            bool userIsOwner = username == this.OWNER;
            bool simulationIsSharedWithUser = USERS.Any(
                user => (user.USERNAME == username || user.USERNAME == null) && user.CAN_MODIFY
            );
            return userIsOwner || simulationIsSharedWithUser;
        }

        public bool UserCanRead(string username)
        {
            bool userIsOwner = username == this.OWNER;
            bool simulationIsSharedWithUser = USERS.Any(
                user => user.USERNAME == username || user.USERNAME == null
            );
            return userIsOwner || simulationIsSharedWithUser;
        }

        public SimulationEntity()
        {
            YEARLYINVESTMENTS = new HashSet<YearlyInvestmentEntity>();
            USERS = new List<SimulationUserEntity>();
        }

        public SimulationEntity(CreateSimulationDataModel model)
        {
            NETWORKID = model.NetworkId;
            SIMULATION = model.Name;
            OWNER = model.Owner;
            CREATOR = model.Creator;
            DATE_CREATED = DateTime.Now;
            ANALYSIS = "Incremental Benefit/Cost";
            BUDGET_CONSTRAINT = "As Budget Permits";
            WEIGHTING = "none";
            COMMITTED_START = DateTime.Now.Year;
            COMMITTED_PERIOD = 1;

            PRIORITIES = new List<PriorityEntity>
            {
                new PriorityEntity
                {
                    PRIORITYLEVEL = 1,
                    YEARS = null,
                    CRITERIA = "",
                    PRIORITYFUNDS = new List<PriorityFundEntity>
                    {
                        new PriorityFundEntity {BUDGET = "Rehabilitation", FUNDING = 100},
                        new PriorityFundEntity {BUDGET = "Maintenance", FUNDING = 100},
                        new PriorityFundEntity {BUDGET = "Construction", FUNDING = 100}
                    }
                }
            };

            CriteriaDrivenBudgets = new List<CriteriaDrivenBudgetEntity>
            {
                new CriteriaDrivenBudgetEntity {BUDGET_NAME = "Rehabilitation"},
                new CriteriaDrivenBudgetEntity {BUDGET_NAME = "Maintenance"},
                new CriteriaDrivenBudgetEntity {BUDGET_NAME = "Construction"}
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

            CriteriaDrivenBudgets = new List<CriteriaDrivenBudgetEntity>
            {
                new CriteriaDrivenBudgetEntity
                {
                    BUDGET_NAME = "Maintenance",
                    CRITERIA = ""
                },
                new CriteriaDrivenBudgetEntity
                {
                    BUDGET_NAME = "Rehabilitation",
                    CRITERIA = ""
                },
                new CriteriaDrivenBudgetEntity
                {
                    BUDGET_NAME = "Construction",
                    CRITERIA = ""
                }
            };

            USERS = new List<SimulationUserEntity>();
        }
    }
}
