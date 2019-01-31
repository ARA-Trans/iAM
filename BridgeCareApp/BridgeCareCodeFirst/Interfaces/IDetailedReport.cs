using BridgeCare.Data;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IDetailedReport
    {
        List<YearlyData> GetYearsData(SimulationResult data);
        IQueryable<DetailedReport> GetDataForReport(SimulationResult data, BridgeCareContext db);
    }
}
