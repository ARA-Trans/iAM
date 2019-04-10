using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class CostModel
    {
        public int CostId { get; set; }
        [IgnoreDataMember]
        public int TreatmentId { get; set; }
        public string Cost { get; set; }
        public string Unit { get; set; }
        public string Criteria { get; set; }
        public Nullable<bool> IsFunction { get; set; }
    }
}