using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AspWebApi.Models
{
    public class SimulationModel
    {
        public int SimulationId { get; set; }
        public string SimulationName { get; set; }
        public int NetworkId { get; set; }
    }
}