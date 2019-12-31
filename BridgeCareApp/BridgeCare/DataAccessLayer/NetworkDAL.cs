using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Services
{
    public class NetworkDAL : INetwork
    {
        /// <summary>
        /// Fetches all networks data
        /// Throws a RowNotInTableException if no networks are found
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>NetworkModel list</returns>
        public List<NetworkModel> GetAllNetworks(BridgeCareContext db)
        {
            if (!db.NETWORKS.Any())
            {
                var log = log4net.LogManager.GetLogger(typeof(NetworkDAL));
                log.Error("No network data could be found.");
                throw new RowNotInTableException("No network data could be found.");
            }

            return db.NETWORKS.ToList().Select(n => new NetworkModel(n)).ToList();
        }
    }
}