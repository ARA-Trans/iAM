using System.ComponentModel.DataAnnotations;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class NetworkModel
    {
        public int NetworkId { get; set; }
        public string NetworkName { get; set; }

        public NetworkModel() { }

        public NetworkModel(NetworkEntity entity)
        {
            NetworkId = entity.NETWORKID;
            NetworkName = entity.NETWORK_NAME;
        }
    }
}