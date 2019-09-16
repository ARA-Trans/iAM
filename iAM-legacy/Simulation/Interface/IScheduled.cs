using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation.Interface
{
    interface IScheduled
    {
        int ScheduledId { get; }
        int ScheduledYear { get; }
        Treatments Treatment { get; }
        string  Budget { get; set; }
    }
}
