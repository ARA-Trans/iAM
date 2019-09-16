using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation.Interface
{
    interface ISupersede
    {
        int SupersedeId { get; }
        int SupersedeTreatmentId { get; }
        Criterias Criteria { get; }
    }
}
