using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class Target
    {
        public DataTable Targets { get; set; } = new DataTable();
        public Dictionary<int, List<int>> GreenColorFill = new Dictionary<int, List<int>>();
        public ExcelFillCoral CoralColorFill = new ExcelFillCoral();
    }
}