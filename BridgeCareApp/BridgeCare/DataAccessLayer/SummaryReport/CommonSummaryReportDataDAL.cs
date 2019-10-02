using System;
using System.Linq;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer.SummaryReport
{
    public class CommonSummaryReportDataDAL : ICommonSummaryReportData
    {
        private readonly BridgeCareContext db;

        public CommonSummaryReportDataDAL(BridgeCareContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// Fetches a simulation's yearly investments data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>SimulationYearsModel</returns>
        public SimulationYearsModel GetSimulationYearsData(int id)
        {
            return new SimulationYearsModel
            {
                SimulationID = id,
                Years = db.YearlyInvestments
                    .Where(y => y.SIMULATIONID == id)
                    .Select(y => y.YEAR_).Distinct().ToList()
            };
        }
    }
}