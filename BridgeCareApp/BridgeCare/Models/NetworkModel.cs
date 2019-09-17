using System.ComponentModel.DataAnnotations;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class NetworkModel
    {
        public int NetworkId { get; set; }
        public string NetworkName { get; set; }

        public NetworkModel() { }

        public NetworkModel(NetworkEntity networkEntity)
        {
            NetworkId = networkEntity.NETWORKID;
            NetworkName = networkEntity.NETWORK_NAME;
        }
    }
}