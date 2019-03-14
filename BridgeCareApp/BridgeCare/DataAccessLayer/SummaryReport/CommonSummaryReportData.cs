using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class CommonSummaryReportData : ICommonSummaryReportData
    {
        private readonly BridgeCareContext dbContext;

        public CommonSummaryReportData(BridgeCareContext bridgeCareContext)
        {
            dbContext = bridgeCareContext ?? throw new ArgumentNullException(nameof(bridgeCareContext));
        }

        public SimulationYearsModel GetSimulationYearsData(int simulationId)
        {
            try
            {
                return new SimulationYearsModel
                {
                    SimulationID = simulationId,
                    Years = dbContext.YEARLYINVESTMENTs.Where(y => y.SIMULATIONID == simulationId).Select(y => y.YEAR_).Distinct().ToList()
                };
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Years");
            }
            return new SimulationYearsModel();
        }
    }
}