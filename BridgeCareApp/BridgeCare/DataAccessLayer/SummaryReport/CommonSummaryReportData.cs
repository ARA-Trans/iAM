using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models.SummaryReport;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer.SummaryReport
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
                    Years = dbContext.YEARLYINVESTMENTs.Where(y => y.SIMULATIONID == simulationId).Select(y => y.YEAR_).ToList()
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