using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Interface
{
    public interface ISplitTreatmentLimit
    {
        int Rank { get; }
        float Amount { get; }
        List<float> Percentages { get; }
    }
}
