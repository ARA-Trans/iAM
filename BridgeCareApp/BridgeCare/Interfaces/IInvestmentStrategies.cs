using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IInvestmentStrategies
    {
        IQueryable<InvestmentStrategyModel> GetInvestmentStrategies(NetworkModel data, BridgeCareContext db);
    }
}