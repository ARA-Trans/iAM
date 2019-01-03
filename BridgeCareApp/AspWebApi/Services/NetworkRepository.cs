using AspWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspWebApi.Services
{
    public class NetworkRepository
    {
        private BridgeCareEntities db = new BridgeCareEntities();
        public IQueryable<NetworkModel> GetAllNetworks()
        {
            var filteredColumns = from contextTable in db.NETWORKS
                                  select new NetworkModel
                                  {
                                      NetworkId = contextTable.NETWORKID,
                                      NetworkName = contextTable.NETWORK_NAME
                                  };
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