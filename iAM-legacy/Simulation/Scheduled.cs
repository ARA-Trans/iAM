using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simulation.Interface;

namespace Simulation
{
    internal class Scheduled:IScheduled
    {
        public int ScheduledId { get; }
        public int ScheduledYear { get; }
        public Treatments Treatment { get; }

        public string Budget { get; set; }


        public Scheduled(int scheduleId, Treatments treatment, int scheduledYear)
        {
            ScheduledId = scheduleId;
            Treatment = treatment;
            ScheduledYear = scheduledYear;
        }

    }
}
