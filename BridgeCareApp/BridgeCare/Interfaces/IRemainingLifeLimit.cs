using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IRemainingLifeLimit
    {
        RemainingLifeLimitLibraryModel GetScenarioRemainingLifeLimitLibrary(int selectedScenarioId,
            BridgeCareContext db);

        RemainingLifeLimitLibraryModel SaveScenarioRemainingLifeLimitLibrary(RemainingLifeLimitLibraryModel data,
            BridgeCareContext db);
    }
}