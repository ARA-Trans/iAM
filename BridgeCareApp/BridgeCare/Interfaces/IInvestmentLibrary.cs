using BridgeCare.Models;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Interfaces
{
    public interface IInvestmentLibrary
    {
        InvestmentLibraryModel GetScenarioInvestmentLibrary(int simulationId, BridgeCareContext db);

        InvestmentLibraryModel SaveScenarioInvestmentLibrary(InvestmentLibraryModel data, BridgeCareContext db);
    }
}