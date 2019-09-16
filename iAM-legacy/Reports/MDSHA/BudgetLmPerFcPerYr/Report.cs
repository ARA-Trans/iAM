using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DatabaseManager;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Reports.MDSHA.BudgetLmPerFcPerYr
{
    /// <summary>
    ///     Represents all information necessary to generate the "Budget and LM
    ///     per FC per Year" report type.
    /// </summary>
    public class Report : ReportBase
    {
        /// <summary>
        ///     This is the generic title of this type of report.
        /// </summary>
        /// <remarks>
        ///     This is a const string so that it can be used in a switch
        ///     statement that discerns report types based on name. As of this
        ///     writing, it is not yet used that way (though previous reports
        ///     were, so here we are, just in case).
        /// </remarks>
        public const string GenericTitle = "Budget & LM per FC per Year";

        private static readonly string QueryTemplate =
            new FormattedSql(@"
SELECT YEARS                      AS ""Year"",
  CAST(FUNC_CLASS AS INT)         AS ""FC"",
  SUM(COST_)                      AS ""Budget"",
  SUM(SEGMENT_{0}_NS0.LANE_MILES) AS ""Treatment LM""
FROM REPORT_{0}_{1}
JOIN SIMULATION_{0}_{1}
ON SIMULATION_{0}_{1}.SECTIONID = REPORT_{0}_{1}.SECTIONID
JOIN SECTION_{0}
ON SECTION_{0}.SECTIONID = REPORT_{0}_{1}.SECTIONID
JOIN SEGMENT_{0}_NS0
ON SEGMENT_{0}_NS0.SECTIONID = REPORT_{0}_{1}.SECTIONID
WHERE COST_                   > 0
GROUP BY YEARS,
  FUNC_CLASS
ORDER BY 1,
  2,
  3
");

        private const string TableCaptionTemplate =
            "FY {0}-{1}: Budget and Lane miles per Functional Class";

        private static readonly Dictionary<decimal, string> FcDescriptions =
            new Dictionary<decimal, string>()
            {
                {1, "Rural Interstate"},
                {2, "Rural Other Principal Arterial"},
                {6, "Rural Minor Arterial"},
                {7, "Rural Major Collector"},
                {8, "Rural Minor Collector - No Federal Aid"},
                {9, "Rural Local - No Federal Aid"},
                {11, "Urban Interstate"},
                {12, "Urban Other Freeways and Expressways"},
                {14, "Urban Other Principal Arterial"},
                {16, "Urban Minor Arterial"},
                {17, "Urban Collector"},
                {19, "Urban Local - No Federal Aid"}
            };

        private readonly string Query;

        private DataTable Data;

        /// <summary>
        ///     Create a new "Budget and LM per FC per Year" report given the
        ///     appropriate network/simulation information.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="simulationId"></param>
        /// <param name="simulation"></param>
        public Report(string networkId, string simulationId, string simulation)
        {
            this.NetworkId = networkId;
            this.SimulationId = simulationId;
            this.Simulation = simulation;

            this.OutputFileBaseName =
                string.Format("{0} - {1}", Title, simulation);

            this.Query =
                string.Format(Report.QueryTemplate, networkId, simulationId);
        }

        /// <summary>
        ///     Returns the title of this specific report instance.
        /// </summary>
        /// <remarks>
        ///     This is virtual so that the base class can use it while the
        ///     subclasses define it.
        /// </remarks>
        public override string Title
        {
            get { return GenericTitle; }
        }

        /// <summary>
        ///     Gets the report's data (to fail fast if this report cannot be
        ///     used with the given net/sim). Prepares the working OOXML package
        ///     and output file for writing.
        /// </summary>
        protected override void Open()
        {
            var queryResult = DBMgr.ExecuteQuery(this.Query);
            this.Data = queryResult.Tables[0];

            this.OutputFileExtension = "xlsx";
            var outputFile = this.GetNewOutputFile();
            var outputStream = outputFile.Open(FileMode.Create);

            var templateBytes = Resources.BlfyReportTemplate;
            var templateStream = new MemoryStream(templateBytes);

            this.OutputPackage =
                new ExcelPackage(outputStream, templateStream);
        }

        /// <summary>
        ///     Prepares the report's data, loads it into the working package,
        ///     and formats the package content.
        /// </summary>
        protected override void Fill()
        {
            var fcDescriptionCol =
                this.Data.Columns
                .Add("Functional Class (Description)", typeof(string));

            fcDescriptionCol.SetOrdinal(2);
            foreach (var r in this.Data.AsEnumerable())
            {
                string description;
                description =
                    Report.FcDescriptions.TryGetValue(
                        r.Field<decimal>("FC"),
                        out description) ?
                    description :
                    string.Empty;

                r.SetField<string>(fcDescriptionCol, description);
            }

            var years =
                (from r in this.Data.AsEnumerable()
                 select r.Field<decimal>("Year"))
                .Distinct()
                .OrderBy(y => y);

            var tableCaption = string.Format(
                Report.TableCaptionTemplate,
                years.First(),
                years.Last());

            var sheet = this.OutputPackage.Workbook.Worksheets[1];

            sheet.Cells["A1"].Value = tableCaption;
            var range = sheet.Cells["A3"].LoadFromDataTable(this.Data, false);

            var orderedYearGroupSizes =
                from r in this.Data.AsEnumerable()
                group r by r.Field<decimal>("Year") into g
                select g.Count();

            var accumulatedOffset = 0;
            foreach (var groupSize in orderedYearGroupSizes)
            {
                range.Offset(accumulatedOffset, 0, groupSize, 1).Merge = true;
                accumulatedOffset += groupSize;
            }

            range.Offset(0, 3, accumulatedOffset, 1)
                .Style.Numberformat.Format = "$* #,##0";
            range.Offset(0, 4, accumulatedOffset, 1)
                .Style.Numberformat.Format = "#,##0.0";

            range.Offset(0, 0, accumulatedOffset, 2)
                .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            range.Offset(0, 0, accumulatedOffset, 1)
                .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            range.Style.Border.BorderAll(ExcelBorderStyle.Thin);

            sheet.Cells.AutoFitColumns();
        }

        /// <summary>
        ///     Saves the working package to the prepared output. Closes the
        ///     output stream.
        /// </summary>
        protected override void Save()
        {
            if (this.OutputPackage != null)
            {
                this.OutputPackage.Save();
                this.OutputPackage.Stream.Dispose();
            }
        }
    }
}
