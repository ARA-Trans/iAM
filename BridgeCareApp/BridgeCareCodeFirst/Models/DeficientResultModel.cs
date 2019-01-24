using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCareCodeFirst.Models
{
    public class DeficientResultModel
    {
        public int TargetID { get; set; }
        public int Years { get; set; }
        public double TargetMet { get; set; }
        public bool IsDeficient { get; set; }
    }
}