﻿using AspWebApi.ApplicationLogs;
using AspWebApi.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace AspWebApi.Models
{
    public class DetailedReportModel
    {
        //[Column(TypeName = "VARCHAR")]
        public string Facility { get; set; }

        //[Column(TypeName = "VARCHAR")]
        public string Section { get; set; }

        //[Column(TypeName = "VARCHAR")]
        public string Treatment { get; set; }

        public int NumberTreatment { get; set; }
        public bool IsCommitted { get; set; }
        public int Years { get; set; }

        private IQueryable<int> yearsForBudget;
        private DbRawSqlQuery<DetailedReportModel> RawQueryForData = null;

        public IQueryable<int> GetYearsData(ReportData data)
        {
            BridgeCareEntities db = new BridgeCareEntities();
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

        public DbRawSqlQuery<DetailedReportModel> GetDataForReport(ReportData data)
        {
            string getReport =
                "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years " +
                    " FROM Report_" + data.NetworkId
                    + "_" + data.SimulationId + " Rpt WITH (NOLOCK) INNER JOIN Section_" + data.NetworkId + " Sec WITH (NOLOCK) " +
                    " ON Rpt.SectionID = Sec.SectionID " +
                    "Order By Facility, Section, Years" ;
            BridgeCareEntities db = new BridgeCareEntities();

            try
            {
                //bc.Configuration.LazyLoadingEnabled = true;
                //SqlParameter param1 = new SqlParameter("@ReportTableName", "Report_" + data.NetworkId + "_" + data.SimulationId);
                //SqlParameter param2 = new SqlParameter("@ReportSection" ,"Section_" + data.NetworkId);

                RawQueryForData = db.Database.SqlQuery<DetailedReportModel>(getReport);
                //var RawQueryForData1 = bc.Database.SqlQuery<iam_GetReportData_Result>("iam_GetReportData @ReportTableName, @ReportSection", param1, param2);
                //var RawQueryForData1 = bc.iam_GetReportData("Report_" + data.NetworkId + "_" + data.SimulationId, "Section_" + data.NetworkId).Count();
            }
            catch (SqlException ex)
            {
                db.Dispose();
                if (ex.Number == -2 || ex.Number == 11)
                {
                    Logger.Error("The server has timed out. Please try after some time", "GetDetailedReportData(ReportData data)");
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    Logger.Error("Network or simulation table does not exist in the database", "GetDetailedReportData(ReportData data)");
                    throw new InvalidOperationException("Network or simulation table does not exist in the database");
                }
            }
            catch (OutOfMemoryException)
            {
                db.Dispose();
                Logger.Error("The server is out of memory. Please try after some time", "GetDetailedReportData(ReportData data)");
                throw new OutOfMemoryException("The server is out of memory. Please try after some time");
            }
            return RawQueryForData;
        }
    }
}