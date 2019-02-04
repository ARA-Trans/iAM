using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class CellAddress
    {
        public List<(int row, int column)> Cells = new List<(int row, int column)>();
    }
}