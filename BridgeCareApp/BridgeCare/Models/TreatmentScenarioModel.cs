using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;


namespace BridgeCare.Models
{
    public class TreatmentScenarioModel
    {
        public int TreatementId { get; set; }
        public int SimulationId { get; set; }

        public string Treatment { get; set; }
        [IgnoreDataMember]
        public int BeforeAny { get; set; }
        [IgnoreDataMember]
        public int BeforeSame { get; set; }
        [IgnoreDataMember]
        public string Budget { get; set; }
 
        public string Description { get; set; }

        public List<string> Budgets { get; set; }

        public string OMS_IS_EXCLUSIVE { get; set; }
        public string OMS_IS_REPEAT { get; set; }
        public string OMS_REPEAT_START { get; set; }
        public string OMS_REPEAT_INTERVAL { get; set; }

        public List<CostModel> Cost { get; set; }
        public FeasibilityModel Feasilbility { get; set; }

        [IgnoreDataMember]
        public List<FeasibilityModel> Feasibilities { get; set; }

        public List<ConsequenceModel> Consequences { get; set; }

        /// <summary>
        /// The conversion from DB comma delimited format to json array format
        /// </summary>
        public void SetBudgets()
        {
            Budgets = Budget.Split(',').ToList<string>();
        }

        /// <summary>
        /// The concatination of all the budgets to one comma delimited string
        /// </summary>
        public string GetBudgets()
        {
            return string.Join(",", Budgets);
        }
    }
}