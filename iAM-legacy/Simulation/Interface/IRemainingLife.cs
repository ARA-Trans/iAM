using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation.Interface
{
    public interface IRemainingLife
    {
        int RemainingLifeId { get; }
        string Attribute { get; }
        double RemainingLifeLimit { get; }
        Criterias Criteria { get; }
    }
}
