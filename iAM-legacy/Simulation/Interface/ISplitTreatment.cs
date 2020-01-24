using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Interface
{
    public interface ISplitTreatment
    {
        string Id { get; }
        string Description { get; }
        Criterias Criteria { get; }
        List<ISplitTreatmentLimit> Limits { get; }
    }
}
