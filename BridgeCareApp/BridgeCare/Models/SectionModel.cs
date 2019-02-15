using BridgeCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class SectionModel
    {
        public int SectionId { get; set; }
        public Char[] BridgeKey { get; set; }
        public Char[] BridgeiId { get; set; }

        //isnt this needed, data is valid only for a specific network
        //can be broken easily
        //public NetworkModel Network { get; set; }
    }
}