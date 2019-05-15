using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    [DataContract]
    public class CostModel
    {
        [DataMember(Name = "Id")]
        public int CostId { get; set; }
        [DataMember(Name = "treatmentId")]
        public int TreatmentId { get; set; }
        [DataMember(Name = "criteria")]
        public string Criteria { get; set; }
        [DataMember(Name = "isFunction")]
        public Nullable<bool> IsFunction { get; set; }
        [DataMember(Name = "equation")]
        public string Cost { get; set; }
        [IgnoreDataMember]
        public string Unit { get; set; }

    }
}