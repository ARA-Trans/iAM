using AspWebApi.ApplicationLogs;
using AspWebApi.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;

namespace AspWebApi.Models
{
    public class DetailedReportModel
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

        private List<DetailedReportModel> dataForExcel = new List<DetailedReportModel>();
        private IQueryable<int> yearsForBudget;
        private BridgeCareEntities db = new BridgeCareEntities();

        public List<DetailedReportModel> GetDetailedReportData(ReportData data)
        {
            string getReport =
                "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years " +
                    " FROM Report_" + data.NetworkId
                    + "_" + data.SimulationId + " Rpt WITH (NOLOCK) INNER JOIN Section_" + data.NetworkId + " Sec WITH (NOLOCK) " +
                    " ON Rpt.SectionID = Sec.SectionID " +
                    "ORDER BY Facility, Section, Years";

            using (var bc = new BridgeCareEntities())
            {
                try
                {
                    var reportData = bc.Database.SqlQuery<DetailedReportModel>(getReport);
                    dataForExcel = reportData.ToList();
                }
                catch(SqlException ex)
                {
                    if (ex.Number == -2 || ex.Number == 11)
                    {
                        Logger.Error("The server has timed out. Please try after some time", "GetDetailedReportData(ReportData data)");
                        throw new TimeoutException("The server has timed out. Please try after some time");
                    }
                    if(ex.Number == 208)
                    {
                        Logger.Error("Network or simulation table does not exist in the database", "GetDetailedReportData(ReportData data)");
                        throw new InvalidOperationException("Network or simulation table does not exist in the database");
                    }
                }
            }
            return dataForExcel;
        }

        public IQueryable<int> GetYearsData(ReportData data)
        {
            try
            {
                yearsForBudget = db.YEARLYINVESTMENTs.Where(_ => _.SIMULATIONID == data.SimulationId)
                                                     .OrderBy(year => year.YEAR_)
                                                     .Select(p => p.YEAR_).Distinct();
            }
            catch (SqlException ex)
            {
                if (ex.Number == -2 || ex.Number == 11)
                {
                    Logger.Error("The server has timed out. Please try after some time", "CreateExcelReport(ReportData data)");
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    Logger.Error("Years data does not exist in the database", "CreateExcelReport(ReportData data)");
                    throw new InvalidOperationException("Years data does not exist in the database");
                }
            }
            return yearsForBudget;
        }
    }
    public class ReportData
    {
        public int NetworkId;
        public int SimulationId;
    }
}