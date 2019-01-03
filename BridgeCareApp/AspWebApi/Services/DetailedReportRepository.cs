using AspWebApi.Controllers;
using AspWebApi.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;

namespace AspWebApi.Services
{
    public class DetailedReportRepository
    {
        private BridgeCareEntities db = new BridgeCareEntities();

        public void GetDetailedReportData(ReportData data)
        {
            List<DetailedReportModel> dataForExcel = new List<DetailedReportModel>();

            string getReport = "SELECT * FROM ( " +
                "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years, " +
                    " ROW_NUMBER() OVER (ORDER BY Facility, Section, Years) as RowNumber " +
                    " FROM Report_" + data.NetworkId
                    + "_" + data.SimulationId + " Rpt WITH (NOLOCK) INNER JOIN Section_" + data.NetworkId + " Sec WITH (NOLOCK) " +
                    " ON Rpt.SectionID = Sec.SectionID "
                    + ") as innerTable ";

            string budgetYears = "SELECT DISTINCT Year_ FROM YearlyInvestment WHERE simulationID = " + data.SimulationId + " ORDER BY Year_";

            var yearsForBudget = db.YEARLYINVESTMENTs.Where(_ => _.SIMULATIONID == data.SimulationId)
                                                     .OrderBy(year => year.YEAR_)
                                                     .Select(p => p.YEAR_).Distinct();

            var totalYears = yearsForBudget.Count();

            using (var bc = new BridgeCareEntities())
            {
                var reportData = bc.Database.SqlQuery<DetailedReportModel>(getReport);
                dataForExcel = reportData.ToList();
            }

            //using (ExcelPackage excelPackage = new ExcelPackage())
            //{
            //    //create a WorkSheet
            //    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
            //    worksheet.Cells["A1:F1"].LoadFromText("Facility, Section, Treatment, NumberTreatment, IsCommitted, Years");
            //    worksheet.Cells["A2"].LoadFromCollection(dataForExcel);
            //    excelPackage.SaveAs(new System.IO.FileInfo(@"C:\Users\sbhardwaj\Downloads\New.xlsx"));
            //}
        }
    }
}