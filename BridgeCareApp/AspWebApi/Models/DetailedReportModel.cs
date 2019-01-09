using AspWebApi.Controllers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
                var reportData = bc.Database.SqlQuery<DetailedReportModel>(getReport);
                dataForExcel = reportData.ToList();
            }
            return dataForExcel;
        }

        public IQueryable<int> GetYearsData(ReportData data)
        {
            var yearsForBudget = db.YEARLYINVESTMENTs.Where(_ => _.SIMULATIONID == data.SimulationId)
                                                     .OrderBy(year => year.YEAR_)
                                                     .Select(p => p.YEAR_).Distinct();

            return yearsForBudget;
        }
    }
    public class ReportData
    {
        public int NetworkId;
        public int SimulationId;
    }
}