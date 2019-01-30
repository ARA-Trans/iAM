using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface ICostDetails
    {
        double Cost { get; set; }
        int Years { get; set; }
        string Budget { get; set; }
    }
}
