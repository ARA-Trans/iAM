using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class TreatmentModel
    {
        public string Treatment { get; set; }
        public int BeforeAny { get; set; }
        public int BeforeSame { get; set; }
        public string Budget { get; set; }
        public string Description { get; set; }
        public string OMS_IS_EXCLUSIVE { get; set; }
        public string OMS_IS_REPEAT { get; set; }
        public string OMS_REPEAT_START { get; set; }
        public string OMS_REPEAT_INTERVAL { get; set; }
    }
}