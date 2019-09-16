using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DatabaseManager;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace Reports.MDSHA.LmDmVmtCondition
{
    public class Report : ReportBase
    {
        public const string GenericTitle = "LM DM VMT Condition";

        private const string DataOrigin = "A3";

        public static readonly string[] Conditions = new[]
        {
            "VERY GOOD",
            "GOOD",
            "FAIR",
            "MEDIOCRE",
            "POOR"
        };

        private static readonly string[] FieldsObtainableFromSegmentTable = new[]
        {
            "DISTRICT",
            "COUNTY",
            "SHOP",
            "FUNC_CLASS",
            "TOTAL_LANES",
            "AADT_VMT"
        };

        private readonly List<int> AnalysisYears;

        private readonly List<string> FieldsToObtainFromSegmentTable;

        private readonly string SimulationTableName;

        private readonly string SegmentTableName;

        private readonly string SectionTableName;

        private AttributeIndex[] AttributeIndexesToFill;

        private string[] SourceAttributesToFill;

        private string[] SourceAttributesToQuery;

        private HashSet<string> DataFieldNames;

        private string Query;

        private IEnumerable<ConditionRecord> ConditionReportData;

        private IEnumerable<AverageIndexRecord> AverageIndexReportData;

        private Profile CustomProfile;

        /// <summary>
        ///     Constructs and validates most of what's necessary to build this
        ///     report. The custom profile must be set (with SetCustomProfile)
        ///     before calling Generate (inherited from ReportBase), though!
        ///     There's nothing in the design yet that enforces this, however,
        ///     so this note is in the summary!
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="network"></param>
        /// <param name="simulationId"></param>
        /// <param name="simulation"></param>
        public Report(
            string networkId,
            string network,
            string simulationId,
            string simulation)
        {
            this.NetworkId = networkId;
            this.Network = network;
            this.SimulationId = simulationId;
            this.Simulation = simulation;

            this.SegmentTableName = string.Format("SEGMENT_{0}_NS0", networkId);
            this.SectionTableName = string.Format("SECTION_{0}", networkId);
            this.SimulationTableName =
                string.Format("SIMULATION_{0}_{1}", networkId, simulationId);

            // Track any necessary fields missing from the simulation table that
            // then need to be retrieved from the segment table.
            var simulationColumns =
                DBMgr.GetTableColumns(this.SimulationTableName);

            this.FieldsToObtainFromSegmentTable =
                Report.FieldsObtainableFromSegmentTable
                .Where(f => !simulationColumns.Any(c => c.StartsWith(f + "_")))
                .ToList();

            // Now, check to make sure the segment table has the fields we need;
            // if it doesn't, error out...
            var segmentColumns = DBMgr.GetTableColumns(this.SegmentTableName);
            if (!this.FieldsToObtainFromSegmentTable.All(segmentColumns.Contains))
            {
                throw new ReportGenerationException(
                    "[ERROR] To generate this report, the selected simulation" +
                    " or network must have all of the following fields available: " +
                    string.Join(", ", Report.FieldsObtainableFromSegmentTable));
            }

            // Initialize the output file name and the set of analysis years
            this.OutputFileBaseName =
                string.Format("{0} - {1}", GenericTitle, simulation);

            this.AnalysisYears =
                Utilities.GetAnalysisYears(simulationId)
                .Select(y => int.Parse(y)).ToList();
        }

        /// <summary>
        ///     Gets the title of this report.
        /// </summary>
        public override string Title
        {
            get { return GenericTitle; }
        }

        /// <summary>
        ///     Allows external setting of the "custom index settings" profile
        ///     from the UI form that configures this report.
        /// </summary>
        public void SetCustomProfile(Profile profile)
        {
            this.CustomProfile = profile;

            this.SourceAttributesToQuery =
                profile.AttributeIndexes
                .Select(i => i.SourceAttribute)
                .ToArray();

            this.AttributeIndexesToFill =
                profile.AttributeIndexes
                .Where(i => i.Enabled)
                .ToArray();

            this.SourceAttributesToFill =
                this.AttributeIndexesToFill
                .Select(i => i.SourceAttribute)
                .ToArray();
        }

        /// <summary>
        ///     Retrieves the data and, if that went without exception, prepares
        ///     input and output.
        /// </summary>
        /// <exception cref="DbException">
        ///     report's configuration is invalid against the database schema
        /// </exception>
        protected override void Open()
        {
            // Get the correctly formed data
            this.InitializeTabularData();

            // Prepare the output file
            this.OutputFileExtension = "xlsm";
            var outputFile = this.GetNewOutputFile();
            var outputStream = outputFile.Open(FileMode.Create);

            var templateBytes = Resources.LdvcReportsTemplate;
            var templateStream = new MemoryStream(templateBytes);

            this.OutputPackage = new ExcelPackage(outputStream, templateStream);
        }

        /// <summary>
        ///     Formats the filled-in parts of the report template and then
        ///     loads the data.
        /// </summary>
        protected override void Fill()
        {
            this.FillConditionData();
            this.FillAverageIndexData();

            // Formulas sheet
            var sheet = this.OutputPackage.Workbook.Worksheets["Formulas"];
            sheet.Cells["B1"].Value = this.AnalysisYears[0];
            sheet.Cells["B3"].Value = this.AnalysisYears.Count;
            sheet.Cells.AutoFitColumns();

            // Lookup sheet
            sheet = this.OutputPackage.Workbook.Worksheets["Lookup"];
            sheet.Cells["H3"].LoadFromArrays(
                this.SourceAttributesToFill
                .Concat(
                    this.CustomProfile.DerivedIndexes
                    .Where(i => i.Enabled)
                    .Select(i => i.IndexName))
                .Select(s => new[] { s }));
            sheet.Cells["O3"].LoadFromArrays(
                this.CustomProfile.AllIndexes
                .Where(i => i.Enabled)
                .Select(i => new[] { i.IndexName })
                .ToArray());
            sheet.Cells.AutoFitColumns();
        }

        /// <summary>
        ///     Saves the report package.
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
        ///     Fills the count data table in the report worksheet. Basically
        ///     just a subroutine to help keep the main Fill method clean.
        /// </summary>
        /// <remarks>
        ///     Note that the "magic" numeric literals in this implementation
        ///     are derived from the template workbook used as input.
        /// </remarks>
        private void FillConditionData()
        {
            var sheet =
                this.OutputPackage.Workbook.Worksheets["LM_DM_VMT_Condition"];

            var numRows = this.ConditionReportData.Count();
            var numCols = ConditionRecord.Width;

            var range =
                sheet.Cells[Report.DataOrigin].Offset(0, 0, numRows, numCols);

            var table = sheet.Tables.Add(
                range.Offset(-1, 0, numRows + 1, numCols),
                "LM_DM_VMT_Table");

            table.TableStyle = TableStyles.Custom;
            table.StyleName = "TableStyleMedium9MDSHA";

            range.Offset(0, 1, numRows, 1).Style.HorizontalAlignment =
                ExcelHorizontalAlignment.Right;

            range.Offset(0, 7, numRows, 2).Style.Numberformat.Format = "0.0";
            range.Offset(0, 9, numRows, 1).Style.Numberformat.Format = "0";

            range.LoadFromCollection(this.ConditionReportData);

            sheet.Cells.AutoFitColumns();

            sheet.View.FreezePanes(3, 1);
        }

        /// <summary>
        ///     Fills the average index data table in the report worksheet.
        ///     Basically just a subroutine to help keep the main Fill method
        ///     clean.
        /// </summary>
        /// <remarks>
        ///     Note that the "magic" numeric literals in this implementation
        ///     are derived from the template workbook used as input.
        /// </remarks>
        private void FillAverageIndexData()
        {
            var sheet =
                this.OutputPackage.Workbook
                .Worksheets["Avg_Condition_per_LM_DM_VMT"];

            var numRows = this.AverageIndexReportData.Count();
            var numCols = AverageIndexRecord.Width;

            var range =
                sheet.Cells[Report.DataOrigin].Offset(0, 0, numRows, numCols);

            var table = sheet.Tables.Add(
                range.Offset(-1, 0, numRows + 1, numCols),
                "Avg_Condition_per_Table");

            table.TableStyle = TableStyles.Custom;
            table.StyleName = "TableStyleMedium9MDSHA";

            range.Offset(0, 1, numRows, 1).Style.HorizontalAlignment =
                ExcelHorizontalAlignment.Right;

            range.Offset(0, 6, numRows, 3).Style.Numberformat.Format = "0.00";
            range.Offset(0, 9, numRows, 2).Style.Numberformat.Format = "0.0";
            range.Offset(0, 11, numRows, 1).Style.Numberformat.Format = "0";

            range.LoadFromCollection(this.AverageIndexReportData);

            sheet.Cells.AutoFitColumns();

            sheet.View.FreezePanes(3, 1);
        }

        /// <summary>
        ///     Retrieve and store the separate record sets for the two
        ///     sub-reports in this report.
        /// </summary>
        private void InitializeTabularData()
        {
            this.BuildQueryString();

            // Retrieve the raw data
            var queryResult = DBMgr.ExecuteQuery(this.Query);
            var rawData = queryResult.Tables[0];

            // Transform the raw data into a basic form for later
            // report-specific aggregation
            var basicData = new List<BasicRecord>();

            var districtWithoutYear = this.DataFieldNames.Contains("DISTRICT");
            var countyWithoutYear = this.DataFieldNames.Contains("COUNTY");
            var shopWithoutYear = this.DataFieldNames.Contains("SHOP");
            var funcClassWithoutYear = this.DataFieldNames.Contains("FUNC_CLASS");
            var totalLanesWithoutYear = this.DataFieldNames.Contains("TOTAL_LANES");
            var aadtVmtWithoutYear = this.DataFieldNames.Contains("AADT_VMT");

            foreach (var year in this.AnalysisYears)
            {
                var districtField =
                    districtWithoutYear ? "DISTRICT" : "DISTRICT_" + year;
                var countyField =
                    countyWithoutYear ? "COUNTY" : "COUNTY_" + year;
                var shopField =
                    shopWithoutYear ? "SHOP" : "SHOP_" + year;
                var funcClassField =
                    funcClassWithoutYear ? "FUNC_CLASS" : "FUNC_CLASS_" + year;
                var totalLanesField =
                    totalLanesWithoutYear ? "TOTAL_LANES" : "TOTAL_LANES_" + year;
                var aadtVmtField =
                    aadtVmtWithoutYear ? "AADT_VMT" : "AADT_VMT_" + year;

                foreach (var row in rawData.AsEnumerable())
                {
                    var district = row.Field<decimal>(districtField);
                    var county = row.Field<string>(countyField);
                    var shop = row.Field<string>(shopField);
                    var funcClass = row.Field<decimal>(funcClassField);

                    var dm = row.Field<double>("LENGTH");
                    var lm = dm * row.Field<double>(totalLanesField);
                    var vmt = dm * row.Field<double>(aadtVmtField);

                    var variables = new Dictionary<string, double>();
                    var variablesLm = new Dictionary<string, double>();
                    var variablesDm = new Dictionary<string, double>();
                    var variablesVmt = new Dictionary<string, double>();

                    foreach (var measureName in this.SourceAttributesToQuery)
                    {
                        variables[measureName] =
                            row.Field<double>(measureName + "_" + year);

                        variablesLm[measureName] = lm;
                        variablesDm[measureName] = dm;
                        variablesVmt[measureName] = vmt;
                    }

                    foreach (var measureIndex in this.AttributeIndexesToFill)
                    {
                        var measureName = measureIndex.SourceAttribute;
                        var measureValue = variables[measureName];

                        var indexAndCondition =
                            this.CustomProfile
                            .AttributeFunctions[measureName](measureValue);

                        var indexName = measureIndex.IndexName;
                        var indexValue = indexAndCondition.Item1;
                        var condition = indexAndCondition.Item2;

                        variables[indexName] = indexValue;

                        variablesLm[indexName] = lm;
                        variablesDm[indexName] = dm;
                        variablesVmt[indexName] = vmt;

                        basicData.Add(new BasicRecord(
                            year,
                            district,
                            county,
                            shop,
                            funcClass,
                            lm,
                            dm,
                            vmt,
                            measureName,
                            measureValue,
                            condition));

                        basicData.Add(new BasicRecord(
                            year,
                            district,
                            county,
                            shop,
                            funcClass,
                            lm,
                            dm,
                            vmt,
                            indexName,
                            indexValue));
                    }

                    foreach (var derivedIndex in
                        this.CustomProfile.DerivedIndexes
                        .Where(index => index.Enabled))
                    {
                        basicData.Add(new BasicRecord(
                            year,
                            district,
                            county,
                            shop,
                            funcClass,
                            derivedIndex.Compute(variablesLm),
                            derivedIndex.Compute(variablesDm),
                            derivedIndex.Compute(variablesVmt),
                            derivedIndex.IndexName,
                            derivedIndex.Compute(variables)));
                    }
                }
            }

            // Create dummy records to represent empty condition buckets (to
            // make it easier to aggregate in the next steps)
            var dummyRecords =
                from r in basicData
                where r.Condition != null
                group r by new
                {
                    r.Year,
                    r.District,
                    r.County,
                    r.Shop,
                    r.FuncClass,
                    r.Name,
                } into g
                from c in Report.Conditions
                select new BasicRecord(
                    g.Key.Year,
                    g.Key.District,
                    g.Key.County,
                    g.Key.Shop,
                    g.Key.FuncClass,
                    0,
                    0,
                    0,
                    g.Key.Name,
                    0,
                    c);

            // Aggregate the condition report data for the measures
            var conditionReportMeasureData =
                from r in basicData.Concat(dummyRecords)
                where r.Condition != null
                group r by new
                {
                    r.Year,
                    r.District,
                    r.County,
                    r.Shop,
                    r.FuncClass,
                    r.Name,
                    r.Condition,
                } into g
                select new ConditionRecord(
                    g.Key.Year,
                    g.Key.District,
                    g.Key.County,
                    g.Key.Shop,
                    g.Key.FuncClass,
                    g.Select(r => r.Lm).Sum(),
                    g.Select(r => r.Dm).Sum(),
                    g.Select(r => r.Vmt).Sum(),
                    g.Key.Name,
                    g.Key.Condition);

            // Aggregate the condition report data for the derived indices
            var indexFromMeasure =
                this.AttributeIndexesToFill
                .ToDictionary(a => a.SourceAttribute, a => a.IndexName);

            var conditionReportIndexData =
                from r in conditionReportMeasureData
                group r by new
                {
                    r.Year,
                    r.District,
                    r.County,
                    r.Shop,
                    r.FuncClass,
                    r.Condition,
                } into g
                let gg = g.Where(r => indexFromMeasure.ContainsKey(r.Name))
                let varLm =
                    g.ToDictionary(r => r.Name, r => r.Lm).Concat(
                    gg.ToDictionary(r => indexFromMeasure[r.Name], r => r.Lm))
                    .ToDictionary(kv => kv.Key, kv => kv.Value)
                let varDm =
                    g.ToDictionary(r => r.Name, r => r.Dm).Concat(
                    gg.ToDictionary(r => indexFromMeasure[r.Name], r => r.Dm))
                    .ToDictionary(kv => kv.Key, kv => kv.Value)
                let varVmt =
                    g.ToDictionary(r => r.Name, r => r.Vmt).Concat(
                    gg.ToDictionary(r => indexFromMeasure[r.Name], r => r.Vmt))
                    .ToDictionary(kv => kv.Key, kv => kv.Value)
                from d in this.CustomProfile.DerivedIndexes
                where d.Enabled
                select new ConditionRecord(
                    g.Key.Year,
                    g.Key.District,
                    g.Key.County,
                    g.Key.Shop,
                    g.Key.FuncClass,
                    d.Compute(varLm),
                    d.Compute(varDm),
                    d.Compute(varVmt),
                    d.IndexName,
                    g.Key.Condition);

            // Put them together and eliminate any all-zero records (which may
            // otherwise be present due to having intermediately added the dummy
            // records above)
            this.ConditionReportData =
                from r in
                    conditionReportMeasureData
                    .Concat(conditionReportIndexData)
                where r.Lm > 0 || r.Dm > 0 || r.Vmt > 0
                orderby
                    r.District,
                    r.County,
                    r.Shop,
                    r.FuncClass,
                    r.Name,
                    r.Year,
                    Array.IndexOf(Report.Conditions, r.Condition)
                select r;

            // Aggregate the other report sheet's data
            this.AverageIndexReportData =
                from r in basicData
                where r.Condition == null
                group r by new
                {
                    r.Year,
                    r.District,
                    r.County,
                    r.Shop,
                    r.FuncClass,
                    r.Name,
                } into g
                orderby
                    g.Key.District,
                    g.Key.County,
                    g.Key.Shop,
                    g.Key.FuncClass,
                    g.Key.Name,
                    g.Key.Year
                let lmSum = g.Select(r => r.Lm).Sum()
                let dmSum = g.Select(r => r.Dm).Sum()
                let vmtSum = g.Select(r => r.Vmt).Sum()
                select new AverageIndexRecord(
                    g.Key.Year,
                    g.Key.District,
                    g.Key.County,
                    g.Key.Shop,
                    g.Key.FuncClass,
                    lmSum,
                    dmSum,
                    vmtSum,
                    g.Key.Name,
                    g.Select(r => r.Value * r.Lm).Sum() / lmSum,
                    g.Select(r => r.Value * r.Dm).Sum() / dmSum,
                    g.Select(r => r.Value * r.Vmt).Sum() / vmtSum);
        }

        /// <summary>
        ///     Build query from constructor-validated fields and user-specified
        ///     attributes.
        /// </summary>
        private void BuildQueryString()
        {
            var fieldsToObtainFromSimulationTable =
                Report.FieldsObtainableFromSegmentTable
                .Except(this.FieldsToObtainFromSegmentTable)
                .Concat(this.SourceAttributesToQuery);

            var simulationFieldsByYear = Utilities.FlatOuter(
                (year, field) => field + "_" + year,
                this.AnalysisYears,
                fieldsToObtainFromSimulationTable);

            var fieldNames =
                this.FieldsToObtainFromSegmentTable
                .Concat(simulationFieldsByYear)
                .ToList();

            var nullGuards = string.Join(
                " AND ",
                fieldNames.Select(f => f + " IS NOT NULL"));

            // Make the server do the string-to-number conversions for district
            // and functional class
            fieldNames =
                fieldNames
                .Select(f =>
                    f.StartsWith("DISTRICT") || f.StartsWith("FUNC_CLASS") ?
                    string.Format("CAST({0} AS INT) AS {0}", f) :
                    f)
                .ToList();

            var lengthProjection =
                "CAST(ABS(BEGIN_STATION - END_STATION) AS FLOAT) AS LENGTH";
            fieldNames.Add(lengthProjection);

            var projectedFields = string.Join(", ", fieldNames);

            var projectionSource = string.Format(
                "{0} JOIN {1} ON {0}.SECTIONID = {1}.SECTIONID",
                this.SimulationTableName,
                this.SectionTableName);

            if (this.FieldsToObtainFromSegmentTable.Any())
            {
                projectionSource += string.Format(
                    " JOIN {1} ON {0}.SECTIONID = {1}.SECTIONID",
                    this.SimulationTableName,
                    this.SegmentTableName);
            }

            this.Query = string.Format(
                "SELECT {0} FROM {1} WHERE {2}",
                projectedFields,
                projectionSource,
                nullGuards);

            this.DataFieldNames = new HashSet<string>(fieldNames);
        }

        private class BasicRecord
        {
            public readonly int Year;
            public readonly decimal District;
            public readonly string County;
            public readonly string Shop;
            public readonly decimal FuncClass;
            public readonly double Lm;
            public readonly double Dm;
            public readonly double Vmt;
            public readonly string Name;
            public readonly double Value;
            public readonly string Condition;

            public BasicRecord(
                int year,
                decimal district,
                string county,
                string shop,
                decimal funcClass,
                double lm,
                double dm,
                double vmt,
                string name,
                double value,
                string condition = null)
            {
                this.Year = year;
                this.District = district;
                this.County = county;
                this.Shop = shop;
                this.FuncClass = funcClass;
                this.Lm = lm;
                this.Dm = dm;
                this.Vmt = vmt;
                this.Name = name;
                this.Value = value;
                this.Condition = condition;
            }
        }

        private class ConditionRecord
        {
            public const int Width = 10;

            public decimal District { get; private set; }
            public string County { get; private set; }
            public string Shop { get; private set; }
            public decimal FuncClass { get; private set; }
            public string Name { get; private set; }
            public int Year { get; private set; }
            public string Condition { get; private set; }
            public double Lm { get; private set; }
            public double Dm { get; private set; }
            public double Vmt { get; private set; }

            public ConditionRecord(
                int year,
                decimal district,
                string county,
                string shop,
                decimal funcClass,
                double lm,
                double dm,
                double vmt,
                string name,
                string condition)
            {
                this.Year = year;
                this.District = district;
                this.County = county;
                this.Shop = shop;
                this.FuncClass = funcClass;
                this.Lm = lm;
                this.Dm = dm;
                this.Vmt = vmt;
                this.Name = name;
                this.Condition = condition;
            }
        }

        private class AverageIndexRecord
        {
            public const int Width = 12;

            public decimal District { get; private set; }
            public string County { get; private set; }
            public string Shop { get; private set; }
            public decimal FuncClass { get; private set; }
            public string Name { get; private set; }
            public int Year { get; private set; }
            public double LmAverageIndex { get; private set; }
            public double DmAverageIndex { get; private set; }
            public double VmtAverageIndex { get; private set; }
            public double Lm { get; private set; }
            public double Dm { get; private set; }
            public double Vmt { get; private set; }

            public AverageIndexRecord(
                int year,
                decimal district,
                string county,
                string shop,
                decimal funcClass,
                double lm,
                double dm,
                double vmt,
                string name,
                double lmAverageIndex,
                double dmAverageIndex,
                double vmtAverageIndex)
            {
                this.Year = year;
                this.District = district;
                this.County = county;
                this.Shop = shop;
                this.FuncClass = funcClass;
                this.Lm = lm;
                this.Dm = dm;
                this.Vmt = vmt;
                this.Name = name;
                this.LmAverageIndex = lmAverageIndex;
                this.DmAverageIndex = dmAverageIndex;
                this.VmtAverageIndex = vmtAverageIndex;
            }
        }
    }
}
