using BridgeCare;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BridgeCare.Models.DetailedReport;

namespace BridgeCareCodeFirst.Interfaces
{
    public interface IDetailedReport
    {
        List<YearlyData> GetYearsData(SimulationResult data);
        IQueryable<DetailedReport> GetDataForReport(SimulationResult data, BridgeCareContext db);
    }
}
