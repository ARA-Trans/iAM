using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class DeficientResult
    {
        public int TargetID { get; set; }
        public int Years { get; set; }
        public double TargetMet { get; set; }
        public bool IsDeficient { get; set; }
    }
}