using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Services
{
    public class Network : INetwork
    {
        private readonly BridgeCareContext db;

        public Network(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IQueryable<NetworkModel> filteredColumns;

        public IQueryable<NetworkModel> GetAllNetworks()
        {
            try
            {
                filteredColumns = from contextTable in db.NETWORKS
                                  select new NetworkModel
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