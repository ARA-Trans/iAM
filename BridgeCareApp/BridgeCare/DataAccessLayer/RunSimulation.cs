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
        public string Start(SimulationModel data)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString;
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");
                var start = new Simulation.Simulation(data.SimulationName, data.NetworkName,
                    data.SimulationId.ToString(), data.NetworkId.ToString());
                start.CompileSimulation();
                return "Simulation completed successfully";
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
                return "Simulation failed";
            }
        }
    }
}