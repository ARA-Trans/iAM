using BridgeCare.Interfaces;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class SectionModel
    {
        public int SectionId { get; set; }
        public string BridgeID { get; set; }
        public string BridgeKey { get; set; }
        public int NetworkId { get; set; }
    }
}