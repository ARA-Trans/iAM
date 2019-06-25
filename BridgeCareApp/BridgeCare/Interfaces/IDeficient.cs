using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficient
    {
        List<DeficientModel> GetDeficients(int simulationId, BridgeCareContext db);
        List<DeficientModel> SaveDeficients(int simulationId, List<DeficientModel> data, BridgeCareContext db);
    }
}