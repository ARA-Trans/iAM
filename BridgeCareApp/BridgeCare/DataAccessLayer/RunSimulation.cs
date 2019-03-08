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
                var start = new Simulation.Simulation(data.SimulationName, data.NetworkName,
                    data.SimulationId.ToString(), data.NetworkId.ToString());
                Thread simulationThread = new Thread(new ParameterizedThreadStart(start.CompileSimulation));
                simulationThread.Start(true);
                if (!simulationThread.IsAlive)
                {
                    DBMgr.CloseConnection();
                }
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