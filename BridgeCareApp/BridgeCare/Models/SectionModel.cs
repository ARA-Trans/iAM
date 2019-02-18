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
        public string Facility { get; set; }
        public string Section { get; set; }

        public int NetworkID { get; set; }
        public string NetworkName { get; set; }
        //isnt this needed, data is valid only for a specific network
        //can be broken easily
        //public NetworkModel Network { get; set; }

    }
}