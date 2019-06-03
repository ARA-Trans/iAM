using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;


namespace BridgeCare.Models
{
    [DataContract]
    public class TreatmentModel : CrudModel
    {
        [DataMember(Name = "id")]
        public int TreatmentId { get; set; }

        [DataMember(Name = "treatmentLibraryId")]
        public int SimulationId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "feasibility")]
        public FeasibilityModel Feasilbility { get; set; }

        [DataMember(Name = "costs")]
        public List<CostModel> Costs { get; set; }

        [DataMember(Name = "consequences")]
        public List<ConsequenceModel> Consequences { get; set; }

        [DataMember(Name = "budgets")]
        public List<string> Budgets { get; set; }

        // TODO check what is this
        /*[DataMember(Name = "description")]
        public string Description { get; set; }*/
        

        [IgnoreDataMember]
        public int BeforeAny { get; set; }
        [IgnoreDataMember]
        public int BeforeSame { get; set; }
        [IgnoreDataMember]
        public string Budget { get; set; }

        [IgnoreDataMember]
        public string OMS_IS_EXCLUSIVE { get; set; }
        [IgnoreDataMember]
        public string OMS_IS_REPEAT { get; set; }
        [IgnoreDataMember]
        public string OMS_REPEAT_START { get; set; }
        [IgnoreDataMember]
        public string OMS_REPEAT_INTERVAL { get; set; }

        [IgnoreDataMember]
        public List<FeasibilityModel> Feasibilities { get; set; }    

        /// <summary>
        /// The conversion from DB comma delimited format to json array format
        /// </summary>
        public void SetBudgets()
        {
            Budgets = Budget != null ? Budget.Split(',').ToList<string>() : new List<string>();
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