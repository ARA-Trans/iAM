using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BridgeCare.Interfaces
{
    public interface IRunRollup
    {
        void SetLastRunDate(int networkId, BridgeCareContext db);

        Task<string> Start(SimulationModel data);
    }
}