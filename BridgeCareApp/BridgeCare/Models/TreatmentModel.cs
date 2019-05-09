using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class TreatmentModel
    {
        public string Treatment { get; set; }
        [IgnoreDataMember]
        public int BeforeAny { get; set; }
        [IgnoreDataMember]
        public int BeforeSame { get; set; }
        [IgnoreDataMember]
        public string Budget { get; set; }
        [IgnoreDataMember]
        public string Description { get; set; }
        public string OMS_IS_EXCLUSIVE { get; set; }
        public string OMS_IS_REPEAT { get; set; }
        public string OMS_REPEAT_START { get; set; }
        public string OMS_REPEAT_INTERVAL { get; set; }
    }
}