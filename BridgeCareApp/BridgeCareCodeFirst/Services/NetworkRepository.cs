using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.Services
{
    public class NetworkRepository : INetwork
    {
        private BridgeCareContext db = new BridgeCareContext();

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
    }
}