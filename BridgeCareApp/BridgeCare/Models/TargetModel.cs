using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class TargetModel
    {
        public DataTable Targets { get; set; } = new DataTable();
        public CellAddress Address = new CellAddress();
    }
}