using BridgeCareCodeFirst.EntityClasses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCareCodeFirst.Models
{
    public class DetailedReportData
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

        private IQueryable<int> yearsForBudget;
        private IQueryable<DetailedReportData> RawQueryForData = null;

        public IQueryable<int> GetYearsData(ReportData data, BridgeCareContext db)
        {
            try
            {
                yearsForBudget = db.YEARLYINVESTMENTs.AsNoTracking().Where(_ => _.SIMULATIONID == data.SimulationId)
                                                             .OrderBy(year => year.YEAR_)
                                                             .Select(p => p.YEAR_).Distinct();
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
            return yearsForBudget;
        }

        public IQueryable<DetailedReportData> GetDataForReport(ReportData data, BridgeCareContext db)
        {
            string getReport =
                "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years " +
                    " FROM Report_" + data.NetworkId
                    + "_" + data.SimulationId + " Rpt WITH (NOLOCK) INNER JOIN Section_" + data.NetworkId + " Sec WITH (NOLOCK) " +
                    " ON Rpt.SectionID = Sec.SectionID " +
                    "Order By Facility, Section, Years";

            try
            {
                //BridgeCareContext bc = new BridgeCareContext();
                //SqlParameter param1 = new SqlParameter("@ReportTableName", "Report_" + data.NetworkId + "_" + data.SimulationId);
                //SqlParameter param2 = new SqlParameter("@ReportSection", "Section_" + data.NetworkId);
                //var commandExecutionStatus = bc.Database.ExecuteSqlCommand("iam_GetReportData @ReportTableName, @ReportSection", param1, param2);
                //bc.Dispose();
                //RawQueryForData = db.DetailedReportModels.AsNoTracking().AsQueryable();
                //var test = DetailedReportModels1.ToList();
               RawQueryForData = db.Database.SqlQuery<DetailedReportData>(getReport).AsQueryable();
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
    }
}