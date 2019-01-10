using AspWebApi.ApplicationLogs;
using AspWebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace AspWebApi.Services
{
    public class NetworkRepository
    {
        private BridgeCareEntities db = new BridgeCareEntities();

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
            catch(SqlException ex)
            {
                if (ex.Number == -2 || ex.Number == 11)
                {
                    Logger.Error("The server has timed out. Please try after some time", "GetAllNetworks()");
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    Logger.Error("Networks does not exist in the database", "GetAllNetworks()");
                    throw new InvalidOperationException("Networks does not exist in the database");
                }
            }
            catch(Exception)
            {
                Logger.Error("Some error has occured while running query agains NETWORKS table", "GetAllNetworks()");
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