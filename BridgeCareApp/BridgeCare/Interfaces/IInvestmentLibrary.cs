using BridgeCare.Models;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Interfaces
{
    public interface IInvestmentLibrary
    {
        InvestmentLibraryModel GetAnySimulationInvestmentLibrary(int selectedScenarioId, BridgeCareContext db);
        InvestmentLibraryModel GetPermittedSimulationInvestmentLibrary(int selectedScenarioId, BridgeCareContext db, string username);

        InvestmentLibraryModel SaveAnySimulationInvestmentLibrary(InvestmentLibraryModel data, BridgeCareContext db);
        InvestmentLibraryModel SavePermittedSimulationInvestmentLibrary(InvestmentLibraryModel data, BridgeCareContext db, string username);
    }
}
