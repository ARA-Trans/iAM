using BridgeCare.DataAccessLayer;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IDetailedReport
    {
        List<YearlyDataModel> GetYearsData(SimulationModel data);

        IQueryable<DetailedReportDAL> GetRawQuery(SimulationModel data, BridgeCareContext db);
    }
}