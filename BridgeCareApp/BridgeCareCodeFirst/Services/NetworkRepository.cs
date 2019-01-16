using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BridgeCareCodeFirst.Models;

namespace BridgeCareCodeFirst.Services
{
    public class NetworkRepository
    {
        private BridgeCareContext db = new BridgeCareContext();

        private IQueryable<NetworkModel> filteredColumns;

        public object Logger { get; private set; }

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
                if (ex.Number == -2 || ex.Number == 11)
                {
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    throw new InvalidOperationException("Networks does not exist in the database");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Some error has occured while running query agains NETWORKS table");
            }

            return filteredColumns;
        }

        public NetworkModel GetSelectedNetwork(int id)
        {
            var filterNetwork = db.NETWORKS.Where(_ => _.NETWORKID == id)
                .Select(p => new NetworkModel
                {
                    NetworkId = p.NETWORKID,
                    NetworkName = p.NETWORK_NAME
                }).First();

            return filterNetwork;
        }
    }
}