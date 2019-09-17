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
        public List<NetworkModel> GetAllNetworks(BridgeCareContext db)
        {
            if (!db.NETWORKS.Any())
                throw new RowNotInTableException("No 'Network' data was found.");

            return db.NETWORKS.ToList().Select(network => new NetworkModel(network)).ToList();
        }
    }
}