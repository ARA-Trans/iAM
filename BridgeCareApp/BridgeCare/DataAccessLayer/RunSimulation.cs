using BridgeCare.Interfaces;
using BridgeCare.Models;
using DatabaseManager;
using System.Configuration;
using System.Threading;

namespace BridgeCare.DataAccessLayer
{
    public class RunSimulation : IRunSimulation
    {
        public void Start(SimulationModel data)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString;
            DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");
            var testData = new Simulation.Simulation(data.SimulationName, data.NetworkName,
                data.SimulationId.ToString(), data.NetworkId.ToString());
            var simulationThread = new Thread(new ThreadStart(testData.CompileSimulation));
            simulationThread.Start();
        }
    }
}