using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    [DataContract]
    public class FeasibilityModel
    {
        public FeasibilityModel()
        {
            FeasibilityId = null;
            Criteria = "";
        }
        [DataMember(Name = "id")]
        public string FeasibilityId { get; set; }

        [DataMember(Name = "criteria")]
        public string Criteria { get; set; }

        [DataMember(Name = "yearsBeforeAny")]
        public int BeforeAny { get; set; }

        [DataMember(Name = "yearsBeforeSame")]
        public int BeforeSame { get; set; }

        public void Aggregate(FeasibilityModel model)
        {
            if (FeasibilityId == null)
            {
              FeasibilityId = model.FeasibilityId;
            }

            if (Criteria.Length <= 0)
            {
                Criteria = model.Criteria;
            }
            else
            {
                Criteria += " AND " + model.Criteria;
            }
        }
    }
}