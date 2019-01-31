using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.Services
{
    public class NetworkRepository : INetwork
    {
        private readonly BridgeCareContext db;

        public NetworkRepository(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IQueryable<Network> filteredColumns;

        public object Logger { get; private set; }

        public IQueryable<Network> GetAllNetworks()
        {
            try
            {
                filteredColumns = from contextTable in db.NETWORKS
                                  select new Network
                                  {
                                      NetworkId = contextTable.NETWORKID,
                                      NetworkName = contextTable.NETWORK_NAME
                                  };
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Networks");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Some error has occured while running query agains NETWORKS table");
            }

            return filteredColumns;
        }
    }
}