using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class DeficientResult
    {
        public DataTable Deficients { get; set; } = new DataTable();
        public CellAddress Address = new CellAddress();
    }
}