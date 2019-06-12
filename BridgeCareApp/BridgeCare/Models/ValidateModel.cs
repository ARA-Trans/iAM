using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class ValidateModel
    {
        public string equation { get; set; }
        public bool isFunction { get; set; }
        public bool isPiecewise { get; set; }
    }
}