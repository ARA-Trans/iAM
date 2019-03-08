using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class NetworkModel
    {
        [Range(1, int.MaxValue)]
        public int NetworkId { get; set; }

        public string NetworkName { get; set; }
    }
}