using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DatabaseManager;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Reports.MDSHA.IriConditionPerFc
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
        public const string GenericTitle = "IRI Condition per FC";

        private const string SubQueryTemplateAscending =
@"
    SELECT D.FC AS FC2,
      SUM(D.AREA1) AS AREA2
    FROM
      (SELECT A.SECTIONID,
        A.FUNC_CLASS AS FC,
        A.LANE_MILES AS AREA1,
        C.AVG_IRI_{2} AS IRI
      FROM SEGMENT_{0}_NS0 A,
        REPORT_{0}_{1} B,
        SIMULATION_{0}_{1} C
      WHERE A.SECTIONID  = B.SECTIONID
      AND C.SECTIONID    = B.SECTIONID
      AND B.YEARS        = {2}
      AND C.AVG_IRI_{2}  > {3}
      AND C.AVG_IRI_{2} <= {4}
      ) D
    GROUP BY D.FC
";

        private const string SubQueryTemplateDescending =
@"
    SELECT D.FC AS FC2,
      SUM(D.AREA1) AS AREA2
    FROM
      (SELECT A.SECTIONID,
        A.FUNC_CLASS AS FC,
        A.LANE_MILES AS AREA1,
        C.AVG_IRI_{2} AS IRI
      FROM SEGMENT_{0}_NS0 A,
        REPORT_{0}_{1} B,
        SIMULATION_{0}_{1} C
      WHERE A.SECTIONID  = B.SECTIONID
      AND C.SECTIONID    = B.SECTIONID
      AND B.YEARS        = {2}
      AND C.AVG_IRI_{2}  < {3}
      AND C.AVG_IRI_{2} >= {4}
      ) D
    GROUP BY D.FC
";
        private const string MainQueryTemplate =
@"
SELECT FC AS ""FUNC CLASS"",
  CASE
    WHEN VERY_GOOD IS NOT NULL
    THEN VERY_GOOD
    ELSE 0
  END AS ""VERY GOOD"",
  CASE
    WHEN GOOD IS NOT NULL
    THEN GOOD
    ELSE 0
  END AS GOOD,
  CASE
    WHEN FAIR IS NOT NULL
    THEN FAIR
    ELSE 0
  END AS FAIR,
  CASE
    WHEN MEDIOCRE IS NOT NULL
    THEN MEDIOCRE
    ELSE 0
  END AS MEDIOCRE,
  CASE
    WHEN POOR IS NOT NULL
    THEN POOR
    ELSE 0
  END AS POOR
FROM
  (SELECT CAST(E.FC1 AS INT) AS FC ,
    ROUND(F.AREA2, 1) AS VERY_GOOD ,
    ROUND(G.AREA2, 1) AS GOOD ,
    ROUND(H.AREA2, 1) AS FAIR ,
    ROUND(I.AREA2, 1) AS MEDIOCRE ,
    ROUND(J.AREA2, 1) AS POOR
  FROM
    (
    SELECT DISTINCT FUNC_CLASS AS FC1 FROM SEGMENT_{0}_NS0 WHERE FUNC_CLASS > 0
    ) E
  LEFT OUTER JOIN
    (
    {1}
    ) F
  ON E.FC1 = F.FC2
  LEFT OUTER JOIN
    (
    {2}
    ) G
  ON E.FC1 = G.FC2
  LEFT OUTER JOIN
    (
    {3}
    ) H
  ON E.FC1 = H.FC2
  LEFT OUTER JOIN
    (
    {4}
    ) I
  ON E.FC1 = I.FC2
  LEFT OUTER JOIN
    (
    {5}
    ) J
  ON E.FC1 = J.FC2
  ORDER BY FC
  )
";

        private static readonly Dictionary<decimal, double>
            DefaultFcPercentageCutoffs = new Dictionary<decimal, double>()
            {
                {1, .95},
                {2, .90},
                {6, .80},
                {7, .80},
                {8, .75},
                {9, .70},
                {11, .95},
                {12, .90},
                {14, .85},
                {16, .80},
                {17, .75},
                {19, .70},
            };

        private static readonly Color AraBlue = Color.FromArgb(0, 46, 108);

        private static readonly Color MdshaFailRatePink =
            Color.FromArgb(215, 143, 141);

        private static readonly Color MdshaPassRateGreen =
            Color.FromArgb(190, 227, 149);

        private readonly List<string> Years;

        private readonly List<DataTable> Data;

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

            this.Years = Utilities.GetAnalysisYears(simulationId);

            this.OutputFileBaseName =
                string.Format("{0} - {1}", Title, simulation);

            this.Data = new List<DataTable>();
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
            var levelBounds = Utilities
                .GetDefaultAttributeBounds(this.SimulationId)["AVG_IRI"];

            // There must be exactly five non-null level bounds, one for each
            // MDSHA condition.
            if (levelBounds.Any(b => b == null))
            {
                throw new ReportGenerationException(string.Format(
                    "Error: To generate the \"{0}\" report, AVG_IRI must have" +
                    " all level bounds defined.",
                    Report.GenericTitle));
            }

            double initBound;
            string subqueryTemplate;
            if (levelBounds.IsSortedBy((x, y) => x.Value.CompareTo(y.Value)))
            {
                initBound = float.MinValue;
                subqueryTemplate = Report.SubQueryTemplateAscending;
            }
            else if (levelBounds.IsSortedBy((x, y) => y.Value.CompareTo(x.Value)))
            {
                initBound = float.MaxValue;
                subqueryTemplate = Report.SubQueryTemplateAscending;
            }
            else
            {
                throw new ReportGenerationException(
                    "Error: The defined level bounds for AVG_IRI in the" +
                    " database must be ordered either ascending or descending.");
            }

            foreach (var year in this.Years)
            {
                var prevBound = initBound;
                var objectsToFormatMainQuery = new LinkedList<string>(
                    levelBounds.Select(bound =>
                    {
                        var subquery = string.Format(
                            subqueryTemplate,
                            this.NetworkId,
                            this.SimulationId,
                            year,
                            prevBound,
                            bound.Value);

                        prevBound = bound.Value;
                        return subquery;
                    }));
                objectsToFormatMainQuery.AddFirst(this.NetworkId);

                var query = string.Format(
                    Report.MainQueryTemplate,
                    objectsToFormatMainQuery.ToArray());

                var queryResult = DBMgr.ExecuteQuery(query);
                this.Data.Add(queryResult.Tables[0]);
            }

            // Data retrieval complete; proceed with output preparation.
            this.OutputFileExtension = "xlsx";
            this.OutputPackage = new ExcelPackage(this.GetNewOutputFile());
        }

        /// <summary>
        ///     Loads the report's data into the working package and formats the
        ///     package content.
        /// </summary>
        /// <remarks>
        ///     This fill logic takes the approach of first finding all range
        ///     addresses that need to be formatted/filled/etc, then applies to
        ///     them all at once. According to an EPPlus master branch
        ///     contributor, this is the most efficient way to use EPPlus
        ///     because it avoids the creation (and overhead) of as many EPPlus
        ///     range objects as possible. (Apparently, these range objects are
        ///     expensive to create and dispose.)
        /// </remarks>
        protected override void Fill()
        {
            var sheet = 
                this.OutputPackage.Workbook
                .Worksheets.Add("IRI Condition per FC");

            // Ranges to be formatted
            var rangesBold = new ExcelAddressSequenceBuilder();
            var rangesBorder = new ExcelAddressSequenceBuilder();
            var rangesCenter = new ExcelAddressSequenceBuilder();
            var rangesFillSolid = new ExcelAddressSequenceBuilder();
            var rangesColorAraBlue = new ExcelAddressSequenceBuilder();
            var rangesColorLightBlue = new ExcelAddressSequenceBuilder();
            var rangesColorYellow = new ExcelAddressSequenceBuilder();
            var rangesNumFmtWhole = new ExcelAddressSequenceBuilder();
            var rangesNumFmtWholePct = new ExcelAddressSequenceBuilder();
            var rangesNumFmtTenthPct = new ExcelAddressSequenceBuilder();
            var rangesNumFmtHundredthPct = new ExcelAddressSequenceBuilder();

            // Ranges to apply the conditional formatting rules
            var rangesAccRateCondColor = new ExcelAddressSequenceBuilder();

            // Ranges to have formulas
            var rangesFcValues = new ExcelAddressSequenceBuilder();
            var rangesDataHeaders = new ExcelAddressSequenceBuilder();
            var rangesYears = new ExcelAddressSequenceBuilder();
            var rangesPctValues = new ExcelAddressSequenceBuilder();
            var rangesFcTotals = new ExcelAddressSequenceBuilder();
            var rangesPctTotals = new ExcelAddressSequenceBuilder();
            var rangesTargetLm = new ExcelAddressSequenceBuilder();

            // Ranges to have set values
            var rangesTotalLabel = new ExcelAddressSequenceBuilder();
            var rangesAccRateLabel = new ExcelAddressSequenceBuilder();
            var rangesFcHeader = new ExcelAddressSequenceBuilder();
            var rangesTargetLmHeader = new ExcelAddressSequenceBuilder();
            var rangesTargetPctHeader = new ExcelAddressSequenceBuilder();

            // Gather all respective ranges for the above lists
            var numCols = this.Data[0].Columns.Count;

            var firstOriginCol = 1;
            var fcFirstLmCol = firstOriginCol + 1;
            var fcLmTotalsCol = firstOriginCol + numCols;
            var fcLastLmCol = fcLmTotalsCol - 1;

            var secondOriginCol = fcLmTotalsCol + 2;
            var fcFirstPctCol = secondOriginCol + 1;
            var fcPctAccRateCol = secondOriginCol + numCols;
            var fcLastPctCol = fcPctAccRateCol - 1;

            var thirdOriginCol = fcPctAccRateCol + 2;
            var targetLmCol = thirdOriginCol + 1;
            var targetPctCol = thirdOriginCol + 2;

            var currentOriginRow = 1;
            var loadInfos =
                this.Years.Zip(this.Data, (year, data) =>
                {
                    var numRows = data.Rows.Count;

                    var headerRow = currentOriginRow + 1;
                    var firstDataRow = headerRow + 1;
                    var lastDataRow = headerRow + numRows;
                    var condTotalsRow = lastDataRow + 1;

                    // Create the addresses for the range components of the
                    // first tabular area
                    var firstOrigin = ExcelCellBase.GetAddress(
                        currentOriginRow, firstOriginCol);

                    var firstDataHeaders = ExcelCellBase.GetAddress(
                        headerRow, firstOriginCol,
                        headerRow, fcLastLmCol);

                    var firstAggHeader = ExcelCellBase.GetAddress(
                        headerRow, fcLmTotalsCol);

                    var firstFcValues = ExcelCellBase.GetAddress(
                        firstDataRow, firstOriginCol,
                        lastDataRow, firstOriginCol);

                    var firstDataRange = ExcelCellBase.GetAddress(
                        firstDataRow, fcFirstLmCol,
                        lastDataRow, fcLastLmCol);

                    var firstAggValues = ExcelCellBase.GetAddress(
                        firstDataRow, fcLmTotalsCol,
                        lastDataRow, fcLmTotalsCol);

                    var firstCondTotals = ExcelCellBase.GetAddress(
                        condTotalsRow, fcFirstLmCol,
                        condTotalsRow, fcLastLmCol);

                    var firstCondAgg = ExcelCellBase.GetAddress(
                        condTotalsRow, fcLmTotalsCol);

                    // Create the addresses for the range components of the
                    // second tabular area
                    var secondOrigin = ExcelCellBase.GetAddress(
                        currentOriginRow, secondOriginCol);

                    var secondDataHeaders = ExcelCellBase.GetAddress(
                        headerRow, secondOriginCol,
                        headerRow, fcLastPctCol);

                    var secondAggHeader = ExcelCellBase.GetAddress(
                        headerRow, fcPctAccRateCol);

                    var secondFcValues = ExcelCellBase.GetAddress(
                        firstDataRow, secondOriginCol,
                        lastDataRow, secondOriginCol);

                    var secondDataRange = ExcelCellBase.GetAddress(
                        firstDataRow, fcFirstPctCol,
                        lastDataRow, fcLastPctCol);

                    var secondAggValues = ExcelCellBase.GetAddress(
                        firstDataRow, fcPctAccRateCol,
                        lastDataRow, fcPctAccRateCol);

                    var secondCondAverages = ExcelCellBase.GetAddress(
                        condTotalsRow, fcFirstPctCol,
                        condTotalsRow, fcLastPctCol);

                    var secondCondAgg = ExcelCellBase.GetAddress(
                        condTotalsRow, fcPctAccRateCol);

                    // Create the addresses for the range components of the
                    // third tabular area
                    var thirdHeaderFc = ExcelCellBase.GetAddress(
                        headerRow, thirdOriginCol);

                    var thirdHeaderLm = ExcelCellBase.GetAddress(
                        headerRow, targetLmCol);

                    var thirdHeaderPct = ExcelCellBase.GetAddress(
                        headerRow, targetPctCol);

                    var thirdFcValues = ExcelCellBase.GetAddress(
                        firstDataRow, thirdOriginCol,
                        lastDataRow, thirdOriginCol);

                    var thirdTargetLms = ExcelCellBase.GetAddress(
                        firstDataRow, targetLmCol,
                        lastDataRow, targetLmCol);

                    var thirdTargetPcts = ExcelCellBase.GetAddress(
                        firstDataRow, targetPctCol,
                        lastDataRow, targetPctCol);

                    // Add the appropriate range addresses to each range address
                    // collection for FORMATTING
                    rangesBold
                        .Append(firstOrigin)
                        .Append(firstDataHeaders)
                        .Append(firstAggHeader)
                        .Append(firstFcValues)
                        .Append(firstAggValues)
                        .Append(firstCondTotals)
                        .Append(firstCondAgg)
                        .Append(secondOrigin)
                        .Append(secondDataHeaders)
                        .Append(secondAggHeader)
                        .Append(secondFcValues)
                        .Append(secondAggValues)
                        .Append(secondCondAverages)
                        .Append(secondCondAgg)
                        .Append(thirdHeaderFc)
                        .Append(thirdHeaderLm)
                        .Append(thirdHeaderPct)
                        .Append(thirdFcValues)
                        .Append(thirdTargetLms)
                        .Append(thirdTargetPcts);

                    rangesBorder
                        .Append(firstOrigin)
                        .Append(firstDataHeaders)
                        .Append(firstAggHeader)
                        .Append(firstFcValues)
                        .Append(firstDataRange)
                        .Append(firstAggValues)
                        .Append(firstCondTotals)
                        .Append(firstCondAgg)
                        .Append(secondOrigin)
                        .Append(secondDataHeaders)
                        .Append(secondAggHeader)
                        .Append(secondFcValues)
                        .Append(secondDataRange)
                        .Append(secondAggValues)
                        .Append(secondCondAverages)
                        .Append(secondCondAgg)
                        .Append(thirdHeaderFc)
                        .Append(thirdHeaderLm)
                        .Append(thirdHeaderPct)
                        .Append(thirdFcValues)
                        .Append(thirdTargetLms)
                        .Append(thirdTargetPcts);

                    rangesCenter
                        .Append(firstOrigin)
                        .Append(firstDataHeaders)
                        .Append(firstAggHeader)
                        .Append(firstFcValues)
                        .Append(secondOrigin)
                        .Append(secondDataHeaders)
                        .Append(secondAggHeader)
                        .Append(secondFcValues)
                        .Append(thirdHeaderFc)
                        .Append(thirdHeaderLm)
                        .Append(thirdHeaderPct)
                        .Append(thirdFcValues);

                    rangesColorAraBlue
                        .Append(firstOrigin)
                        .Append(firstDataHeaders)
                        .Append(firstAggHeader)
                        .Append(secondOrigin)
                        .Append(secondDataHeaders)
                        .Append(secondAggHeader);

                    rangesColorLightBlue
                        .Append(thirdHeaderFc)
                        .Append(thirdHeaderLm)
                        .Append(thirdHeaderPct);

                    rangesColorYellow
                        .Append(secondCondAgg)
                        .Append(thirdFcValues)
                        .Append(thirdTargetLms)
                        .Append(thirdTargetPcts);

                    rangesFillSolid
                        .Append(rangesColorAraBlue.ToString())
                        .Append(rangesColorLightBlue.ToString())
                        .Append(rangesColorYellow.ToString());

                    rangesNumFmtWhole
                        .Append(firstDataRange)
                        .Append(firstAggValues)
                        .Append(firstCondTotals)
                        .Append(firstCondAgg)
                        .Append(thirdTargetLms);

                    rangesNumFmtWholePct
                        .Append(thirdTargetPcts);

                    rangesNumFmtTenthPct
                        .Append(secondDataRange)
                        .Append(secondCondAverages)
                        .Append(secondCondAgg);

                    rangesNumFmtHundredthPct
                        .Append(secondAggValues);

                    // Add the appropriate range addresses to each range address
                    // collection for FORMULAS
                    rangesFcValues
                        .Append(secondFcValues)
                        .Append(thirdFcValues);

                    rangesDataHeaders
                        .Append(secondDataHeaders);

                    rangesYears
                        .Append(secondOrigin);

                    rangesPctValues
                        .Append(secondDataRange);

                    var rangesColSums = new ExcelAddressSequenceBuilder()
                        .Append(firstCondTotals)
                        .Append(firstCondAgg);

                    var rangesPctAvgs = new ExcelAddressSequenceBuilder()
                        .Append(secondCondAverages)
                        .Append(secondCondAgg);

                    rangesFcTotals
                        .Append(firstAggValues);

                    rangesPctTotals
                        .Append(secondAggValues);

                    rangesTargetLm
                        .Append(thirdTargetLms);

                    // Add the appropriate range addresses to each range address
                    // collection for VALUES
                    rangesTotalLabel
                        .Append(firstAggHeader);

                    rangesAccRateLabel
                        .Append(secondAggHeader);

                    rangesFcHeader
                        .Append(thirdHeaderFc);

                    rangesTargetLmHeader
                        .Append(thirdHeaderLm);

                    rangesTargetPctHeader
                        .Append(thirdHeaderPct);

                    // Add the appropriate range addresses to each range address
                    // collection for CONDITIONAL FORMATTING
                    rangesAccRateCondColor
                        .Append(secondAggValues);

                    // Advance the current origin row
                    currentOriginRow = condTotalsRow + 2;

                    // Create (and return from the Select) info about loading
                    // this data table
                    return new SubTableLoadInfo()
                    {
                        Year = int.Parse(year),

                        SectionOrigin = firstOrigin,

                        Data = data,

                        DataOrigin = ExcelCellBase.GetAddress(
                            headerRow, firstOriginCol),

                        TargetPctValues =
                            data.AsEnumerable().Select(r =>
                            {
                                var fc = r.Field<decimal>("FUNC CLASS");

                                double targetPct;
                                Report.DefaultFcPercentageCutoffs
                                    .TryGetValue(fc, out targetPct);

                                return new object[] { targetPct };
                            }),

                        TargetPctValuesOrigin = ExcelCellBase.GetAddress(
                            firstDataRow, targetPctCol),

                        CondLmSumFormula =
                            string.Format("SUM(R[-{0}]C:R[-1]C)", numRows),

                        CondLmSumAddress = rangesColSums,

                        CondPctAvgFormula =
                            string.Format("AVERAGE(R[-{0}]C:R[-1]C)", numRows),

                        CondPctAvgAddress = rangesPctAvgs,
                    };
                })
                // *** ESSENTIAL *** Forces immediate LINQ query execution so
                //     that the address sequence variables are built in time for
                //     use in the very next section of code.
                .ToArray(); // <<< ESSENTIAL, SEE ABOVE ^^^

            // Apply formatting/formulas/values to collected ranges
            // --- Formatting
            sheet.Cells[rangesBold]
                .Style.Font.Bold = true;

            sheet.Cells[rangesBorder]
                .Style.Border.BorderAll(ExcelBorderStyle.Thin);

            sheet.Cells[rangesCenter]
                .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[rangesFillSolid]
                .Style.Fill.PatternType = ExcelFillStyle.Solid;

            var whiteOnBlueCells = sheet.Cells[rangesColorAraBlue];
            whiteOnBlueCells
                .Style.Fill.BackgroundColor.SetColor(Report.AraBlue);
            whiteOnBlueCells
                .Style.Font.Color.SetColor(Color.White);

            sheet.Cells[rangesColorLightBlue]
                .Style.Fill.BackgroundColor.SetColor(Color.CornflowerBlue);

            sheet.Cells[rangesColorYellow]
                .Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            sheet.Cells[rangesNumFmtWhole]
                .Style.Numberformat.Format = "0";

            sheet.Cells[rangesNumFmtWholePct]
                .Style.Numberformat.Format = "0%";

            sheet.Cells[rangesNumFmtTenthPct]
                .Style.Numberformat.Format = "0.0%";

            sheet.Cells[rangesNumFmtHundredthPct]
                .Style.Numberformat.Format = "0.00%";

            // --- Conditional Formatting
            var cfAddress = new ExcelAddress(rangesAccRateCondColor);
            var cfTargetPctCell = ExcelCellBase.GetAddress(3, targetPctCol);

            var cfPassRate = sheet.ConditionalFormatting
                .AddGreaterThanOrEqual(cfAddress);

            cfPassRate.Priority = 1;
            cfPassRate.Formula = cfTargetPctCell;
            cfPassRate.StopIfTrue = true;
            cfPassRate.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cfPassRate.Style.Fill.BackgroundColor.Color =
                Report.MdshaPassRateGreen;

            var cfFailRate = sheet.ConditionalFormatting
                .AddLessThan(cfAddress);

            cfFailRate.Priority = 2;
            cfFailRate.Formula = cfTargetPctCell;
            cfFailRate.StopIfTrue = true;
            cfFailRate.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cfFailRate.Style.Fill.BackgroundColor.Color =
                Report.MdshaFailRatePink;

            // --- Formulas
            sheet.Cells[rangesFcValues]
                .FormulaR1C1 = "RC1";

            sheet.Cells[rangesDataHeaders]
                .FormulaR1C1 = string.Format("RC[-{0}]", numCols + 2);

            sheet.Cells[rangesYears]
                .FormulaR1C1 = string.Format("RC[-{0}]", numCols + 2);

            sheet.Cells[rangesPctValues]
                .FormulaR1C1 =
                    string.Format("RC[-{0}]/RC{1}", numCols + 2, numCols + 1);

            sheet.Cells[rangesFcTotals]
                .FormulaR1C1 =
                    string.Format("SUM(RC[-{0}]:RC[-1])", numCols - 1);

            sheet.Cells[rangesPctTotals]
                .FormulaR1C1 =
                    string.Format("SUM(RC[-{0}]:RC[-3])", numCols - 1);

            sheet.Cells[rangesTargetLm]
                .FormulaR1C1 = string.Format("RC{0}*RC[1]", numCols + 1);

            // --- Values
            sheet.Cells[rangesTotalLabel]
                .Value = "TOTAL";

            sheet.Cells[rangesAccRateLabel]
                .Value = "Acceptable Rate";

            sheet.Cells[rangesFcHeader]
                .Value = "FC";

            sheet.Cells[rangesTargetLmHeader]
                .Value = "LM";

            sheet.Cells[rangesTargetPctHeader]
                .Value = "Target";

            // Load subtable/targetPct table pairs
            foreach (var load in loadInfos)
            {
                sheet.Cells[load.SectionOrigin]
                    .Value = load.Year;

                sheet.Cells[load.DataOrigin]
                    .LoadFromDataTable(load.Data, true);

                sheet.Cells[load.TargetPctValuesOrigin]
                    .LoadFromArrays(load.TargetPctValues);

                sheet.Cells[load.CondLmSumAddress]
                    .FormulaR1C1 = load.CondLmSumFormula;

                sheet.Cells[load.CondPctAvgAddress]
                    .FormulaR1C1 = load.CondPctAvgFormula;
            }

            // Auto-fit all columns
            sheet.Cells.AutoFitColumns();

            // Take the auto widths from the first table and apply them to the
            // second table (because the uncalculated formulas of the second
            // table do not provide a way for the auto-fitter to determine
            // width)
            for (int c = 1; c <= numCols; ++c)
            {
                sheet.Column(c + numCols + 2).Width = sheet.Column(c).Width;
            }

            // Hide the columns for Target LM/Pct per FC table (third table)
            sheet.Column(thirdOriginCol).Hidden = true;
            sheet.Column(targetLmCol).Hidden = true;
            sheet.Column(targetPctCol).Hidden = true;
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

        /// <summary>
        ///     A record containing the info needed to properly load a subtable
        ///     section in this report.
        /// </summary>
        /// <remarks>
        ///     A good place to have a C# 6 record, methinks (if records
        ///     actually make it through committee).
        /// </remarks>
        private class SubTableLoadInfo
        {
            public int Year;

            public string SectionOrigin;

            public DataTable Data;

            public string DataOrigin;

            public IEnumerable<object[]> TargetPctValues;

            public string TargetPctValuesOrigin;

            public string CondLmSumFormula;

            public string CondLmSumAddress;

            public string CondPctAvgFormula;

            public string CondPctAvgAddress;
        }

        /// <summary>
        ///     Abstracts a tiny bit of dirty work when appending new addresses
        ///     to an existing Excel address sequence and when generating the
        ///     full address sequence string.
        /// </summary>
        /// <remarks>
        ///     Note that there is no input validation for the Append method,
        ///     but since this class is private this isn't urgently necessary.
        /// </remarks>
        private class ExcelAddressSequenceBuilder
        {
            private readonly StringBuilder StringBuilder;

            public ExcelAddressSequenceBuilder(StringBuilder stringBuilder)
            {
                this.StringBuilder = stringBuilder;
            }

            public ExcelAddressSequenceBuilder() : this(new StringBuilder())
            {
            }

            public ExcelAddressSequenceBuilder Append(string address)
            {
                this.StringBuilder.Append(address).Append(',');
                return this;
            }

            public override string ToString()
            {
                return
                    this.StringBuilder
                    .ToString(0, this.StringBuilder.Length - 1);
            }

            public static implicit operator string(ExcelAddressSequenceBuilder o)
            {
                return o.ToString();
            }
        }
    }
}
