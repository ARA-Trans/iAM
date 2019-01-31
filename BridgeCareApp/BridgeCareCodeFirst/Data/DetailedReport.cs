using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Data
{
    public class DetailedReport : IDetailedReport
    {
        [Column(TypeName = "VARCHAR")]
        public string Facility { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string Section { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string Treatment { get; set; }

        public int NumberTreatment { get; set; }
        public bool IsCommitted { get; set; }
        public int Years { get; set; }

        private readonly BridgeCareContext db;

        public DetailedReport(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Required by entity framework
        public DetailedReport() { }

        public List<YearlyData> GetYearsData(SimulationResult data)
        {
            var yearsForBudget = new List<YearlyData>();
            try
            {
                yearsForBudget = db.YEARLYINVESTMENTs.AsNoTracking().Where(_ => _.SIMULATIONID == data.SimulationId)
                                                              .OrderBy(year => year.YEAR_)
                                                              .Select(p => new YearlyData
                                                              {
                                                                  Year = p.YEAR_,
                                                                  Amount = p.AMOUNT,
                                                                  BudgetName = p.BUDGETNAME
                                                              }).ToList();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Years");
            }
            return yearsForBudget;
        }

        public IQueryable<DetailedReport> GetDataForReport(SimulationResult data, BridgeCareContext dbContext)
        {
            IQueryable<DetailedReport> rawQueryForData = null;
            var select =
                    "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years " +
                        " FROM Report_" + data.NetworkId
                        + "_" + data.SimulationId + " Rpt WITH (NOLOCK) INNER JOIN Section_" + data.NetworkId + " Sec WITH (NOLOCK) " +
                        " ON Rpt.SectionID = Sec.SectionID " +
                        "Order By Facility, Section, Years";

            try
            {
                rawQueryForData = dbContext.Database.SqlQuery<DetailedReport>(select).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Report_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForData;
        }
    }
}