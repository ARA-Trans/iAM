using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficient
    {
        List<DeficientModel> GetDeficients(int scenarioId, BridgeCareContext db);
        List<DeficientModel> SaveDeficients(int scenarioId, List<DeficientModel> data, BridgeCareContext db);
    }
}