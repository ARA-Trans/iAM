using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class ConsequenceModel
    {
        public int ConsequenceId { get; set; }
        public string Attribute_ { get; set; }
        public string Change { get; set; }
        public string Criteria { get; set; }
        public string Equation { get; set; }
        public Nullable<bool> IsFunction { get; set; }
    }
}