using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using DatabaseManager;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace BridgeCare.DataAccessLayer
{
    public class RunSimulation : IRunSimulation
    {
        public Task<string> Start(SimulationModel data)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString;
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");

                var start = new RollupSegmentation.RollupSegmentation(data.SimulationName, data.NetworkName,
                    data.SimulationId.ToString(), data.NetworkId.ToString(), true);
                start.strNetwork = data.NetworkName;

                Thread rollUpandSimulation = new Thread(new ThreadStart(start.DoRollup));
                rollUpandSimulation.Start();
                return Task.FromResult("Simulation running...");
            }
            catch (Exception ex)
            {
                DBMgr.CloseConnection();
                HandleException.GeneralError(ex);
                return Task.FromResult("Simulation failed");
            }
        }
    }
}