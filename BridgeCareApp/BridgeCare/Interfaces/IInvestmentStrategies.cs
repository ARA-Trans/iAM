using BridgeCare.Models;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Interfaces
{
    public interface IInvestmentStrategies
    {
        IQueryable<InvestmentStrategyModel> GetInvestmentStrategies(NetworkModel data, BridgeCareContext db);

        void SetInvestmentStrategies(InvestmentStrategyModel data, BridgeCareContext db);
    }
}