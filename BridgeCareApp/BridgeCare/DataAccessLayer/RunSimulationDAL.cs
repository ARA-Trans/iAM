using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Properties;
using DatabaseManager;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BridgeCare.DataAccessLayer
{
    public class RunSimulationDAL : IRunSimulation
    {
        public void SetLastRunDate(int simulationId, BridgeCareContext db)
        {
            try
            {
                var result = db.Simulations.SingleOrDefault(b => b.SIMULATIONID == simulationId);
                result.DATE_LAST_RUN = DateTime.Now;
                db.SaveChanges();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Update Simulation Run Date");
            }
        }

        public Task<string> Start(SimulationModel data)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString;
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");
                var mongoConnection = "";
#if DEBUG
                mongoConnection = Settings.Default.MongoDBDevConnectionString;
#else
                mongoConnection = Settings.Default.MongoDBProdConnectionString;
#endif
                var simulation = new Simulation.Simulation(data.SimulationName, data.NetworkName, data.SimulationId, data.NetworkId, mongoConnection);

                Thread simulationThread = new Thread(new ParameterizedThreadStart(simulation.CompileSimulation));

                simulationThread.Start(true);
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