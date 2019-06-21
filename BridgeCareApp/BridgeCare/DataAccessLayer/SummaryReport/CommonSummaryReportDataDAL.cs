using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class CommonSummaryReportDataDAL : ICommonSummaryReportData
    {
        private readonly BridgeCareContext dbContext;

        public CommonSummaryReportDataDAL(BridgeCareContext bridgeCareContext)
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
                    Years = dbContext.YearlyInvestments.Where(y => y.SIMULATIONID == simulationId).Select(y => y.YEAR_).Distinct().ToList()
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