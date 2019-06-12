using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("COMMITTED_")]
    public class COMMITTED_PROJECT
    {
        public COMMITTED_PROJECT(int simulationId, int sectionId, int years, string treatmentName, int yearSame, int yearAny, string budget, double cost_)
        {
            SIMULATIONID = simulationId;
            SECTIONID = sectionId;
            YEARS = years;
            TREATMENTNAME = treatmentName;
            YEARSAME = yearSame;
            YEARANY = yearAny;
            BUDGET = budget;
            COST_ = cost_;            
        }

        [Key]                
        public int COMMITID { get; set; }

        public int SIMULATIONID { get; set; }

        public int SECTIONID { get; set; }

        public int YEARS { get; set; }

        public string TREATMENTNAME { get; set; }

        public int YEARSAME { get; set; }

        public int YEARANY { get; set; }

        public string BUDGET { get; set; }

        public double COST_ { get; set; }

        public string OMS_IS_EXCLUSIVE { get; set; }

        public string OMS_IS_NOT_ALLOWED { get; set; }

        public virtual ICollection<COMMIT_CONSEQUENCES> CommitConsequences { get; set; }
    }
}