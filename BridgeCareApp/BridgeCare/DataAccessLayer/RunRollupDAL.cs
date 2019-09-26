using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Properties;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BridgeCare.DataAccessLayer
{
    public class RunRollupDAL : IRunRollup
    {
        public void SetLastRunDate(int networkId, BridgeCareContext db)
        {
            throw new NotImplementedException();
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
                var start = new RollupSegmentation.RollupSegmentation(data.NetworkName, data.NetworkId.ToString(), true, mongoConnection);
                start.strNetwork = data.NetworkName;

                Thread rollUpandSimulation = new Thread(new ThreadStart(start.DoRollup));
                rollUpandSimulation.Start();
                return Task.FromResult("Rolling up...");
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