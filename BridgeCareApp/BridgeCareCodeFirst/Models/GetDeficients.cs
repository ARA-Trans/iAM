using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class GetDeficients
    {
        public DataTable Deficients { get; set; } = new DataTable();
        public Dictionary<int, List<int>> DeficientColorFill = new Dictionary<int, List<int>>();
    }
}