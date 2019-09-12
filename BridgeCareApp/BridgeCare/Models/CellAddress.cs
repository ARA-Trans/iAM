using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class CellAddress
    {
        public List<(int row, int column)> Cells = new List<(int row, int column)>();
    }
}