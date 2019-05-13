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
            FeasibilityId = -1;
            Criteria = "";
        }
        [DataMember(Name = "id")]
        public int FeasibilityId { get; set; }

        [DataMember(Name = "treatmentId")]
        public int TreatmentId { get; set; }

        [DataMember(Name = "criteria")]
        public string Criteria { get; set; }

        [DataMember(Name = "yearsBeforeAny")]
        public int BeforeAny { get; set; }

        [DataMember(Name = "yearsBeforeSame")]
        public int BeforeSame { get; set; }

        public void Agregate(FeasibilityModel model)
        {
            if (FeasibilityId <= 0) 
                FeasibilityId= model.FeasibilityId;

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