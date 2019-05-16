using BridgeCare.Models;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Interfaces
{
    public interface IInvestmentLibrary
    {
        IQueryable<InvestmentLibraryModel> GetInvestmentLibrary(int simulationId, BridgeCareContext db);

        void SetInvestmentStrategies(InvestmentLibraryModel data, BridgeCareContext db);
    }
}