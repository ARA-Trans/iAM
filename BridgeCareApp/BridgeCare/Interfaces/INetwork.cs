using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface INetwork
    {
        IQueryable<NetworkModel> GetAllNetworks();
    }
}