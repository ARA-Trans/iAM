using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DatabaseManager;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Reports.MDSHA.BudgetLmPerFcPerCond
{
    /// <summary>
    ///     Represents all information necessary to generate the "Budget and LM
    ///     per FC per Condition" report type.
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
        public const string GenericTitle = "Budget & LM per FC per Condition";

        private static readonly string SubQueryTemplate =
            new FormattedSql(@"
  SELECT REPORT_{0}_{1}.YEARS                    AS YEAR,
    TO_NUMBER(SIMULATION_{0}_{1}.FUNC_CLASS_{2}) AS FUNCTIONAL_CLASS,
    SIMULATION_{0}_{1}.CONDITION_IRI_{2}         AS CONDITION,
    SUM(REPORT_{0}_{1}.COST_)                    AS BUDGET,
    SUM(SEGMENT_{0}_NS0.LANE_MILES)              AS LANE_MILES,
    (
    CASE
      WHEN SIMULATION_{0}_{1}.CONDITION_IRI_{2} = 'VERY GOOD'
      THEN 1
      WHEN SIMULATION_{0}_{1}.CONDITION_IRI_{2} = 'GOOD'
      THEN 2
      WHEN SIMULATION_{0}_{1}.CONDITION_IRI_{2} = 'FAIR'
      THEN 3
      WHEN SIMULATION_{0}_{1}.CONDITION_IRI_{2} = 'MEDIOCRE'
      THEN 4
      ELSE 5
    END) AS CONDITION_ORDER
  FROM REPORT_{0}_{1}
  INNER JOIN SIMULATION_{0}_{1}
  ON SIMULATION_{0}_{1}.SECTIONID = REPORT_{0}_{1}.SECTIONID
  INNER JOIN SECTION_{0}
  ON SECTION_{0}.SECTIONID = REPORT_{0}_{1}.SECTIONID
  INNER JOIN SEGMENT_{0}_NS0
  ON SEGMENT_{0}_NS0.SECTIONID = REPORT_{0}_{1}.SECTIONID
  WHERE REPORT_{0}_{1}.COST_  > 0
  AND REPORT_{0}_{1}.YEARS    = {2}
  GROUP BY REPORT_{0}_{1}.YEARS,
    SIMULATION_{0}_{1}.CONDITION_IRI_{2},
    SIMULATION_{0}_{1}.FUNC_CLASS_{2}
");

        private static readonly string MainQueryTemplate =
            new FormattedSql(@"
SELECT YEAR,
  FUNCTIONAL_CLASS,
  CONDITION,
  BUDGET,
  LANE_MILES
FROM
  (
  {0}

  ORDER BY 1,
    2,
    6
  )
");

        private const string TableCaptionTemplate =
            "FY {0}-{1}: Budget and Lane miles per Functional Class per Condition";

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
        ///     Create a new "Budget and LM per FC per Condition" report given
        ///     the appropriate network/simulation information.
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

            var subQueryCompound = string.Join(
                " UNION ",
                Utilities.GetAnalysisYears(simulationId)
                .Select(year =>
                    string.Format(
                        Report.SubQueryTemplate,
                        networkId,
                        simulationId,
                        year)));

            this.Query =
                string.Format(Report.MainQueryTemplate, subQueryCompound);
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
        ///     Retrieves the report data first (to fail fast if this net/sim
        ///     cannot be used with this report). Prepares the working OOXML
        ///     package and output file for writing.
        /// </summary>
        protected override void Open()
        {
            var queryResult = DBMgr.ExecuteQuery(this.Query);
            this.Data = queryResult.Tables[0];

            this.OutputFileExtension = "xlsx";
            var outputFile = this.GetNewOutputFile();
            var outputStream = outputFile.Open(FileMode.Create);

            var templateBytes = Resources.BlfcReportTemplate;
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
                this.Data.Columns.Add("FcDesc", typeof(string));
            fcDescriptionCol.SetOrdinal(2);
            foreach (var r in this.Data.AsEnumerable())
            {
                string description;
                description =
                    Report.FcDescriptions.TryGetValue(
                        r.Field<decimal>("FUNCTIONAL_CLASS"),
                        out description) ?
                    description :
                    string.Empty;

                r.SetField<string>(fcDescriptionCol, description);
            }

            var years =
                (from r in this.Data.AsEnumerable()
                 select r.Field<decimal>("YEAR"))
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
                group r by r.Field<decimal>("YEAR") into g
                select g.Count();

            var accumulatedOffset = 0;
            foreach (var groupSize in orderedYearGroupSizes)
            {
                range.Offset(accumulatedOffset, 0, groupSize, 1).Merge = true;
                accumulatedOffset += groupSize;
            }

            range.Offset(0, 4, accumulatedOffset, 1)
                .Style.Numberformat.Format = "$* #,##0";
            range.Offset(0, 5, accumulatedOffset, 1)
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
