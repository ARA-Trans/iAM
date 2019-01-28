using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Models
{
    public class DetailedReport
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

        public List<YearlyData> GetYearsData(SimulationResult data)
        {
            BridgeCareContext db = new BridgeCareContext();
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
                db.Dispose();
                if (ex.Number == -2 || ex.Number == 11)
                {
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    throw new InvalidOperationException("Years data does not exist in the database");
                }
            }
            db.Dispose();
            return yearsForBudget;
        }

        public IQueryable<DetailedReport> GetDataForReport(SimulationResult data, BridgeCareContext db)
        {
            IQueryable<DetailedReport> RawQueryForData = null;
            string getReport =
                    "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years " +
                        " FROM Report_" + data.NetworkId
                        + "_" + data.SimulationId + " Rpt WITH (NOLOCK) INNER JOIN Section_" + data.NetworkId + " Sec WITH (NOLOCK) " +
                        " ON Rpt.SectionID = Sec.SectionID " +
                        "Order By Facility, Section, Years";

            try
            {
                RawQueryForData = db.Database.SqlQuery<DetailedReport>(getReport).AsQueryable();
            }
            catch (SqlException ex)
            {
                db.Dispose();
                if (ex.Number == -2 || ex.Number == 11)
                {
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    throw new InvalidOperationException("Network or simulation table does not exist in the database");
                }
            }
            catch (OutOfMemoryException)
            {
                db.Dispose();
                throw new OutOfMemoryException("The server is out of memory. Please try after some time");
            }
            return RawQueryForData;
        }

        public class YearlyData
        {
            public int Year { get; set; }
            public double? Amount { get; set; }
            public string BudgetName { get; set; }
        }
    }
}