using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DatabaseManager;
using OfficeOpenXml;
using Reports.MDSHA;

namespace Reports.Lexington
{
    public class BenefitCostRatioReport : ReportBase
    {
        public const string GenericTitle = "Benefit-Cost Ratio";

        private static readonly string QueryTemplate = new FormattedSql(@"
SELECT main.FACILITY
    ,main.SECTION
    {2}
    ,main.AREA
    ,bc.YEARS
    ,bc.TREATMENT
    ,bc.COST_
    ,bc.BC_RATIO
FROM [SECTION_{0}] main
LEFT OUTER JOIN [SEGMENT_{0}_NS0] attrib ON main.SECTIONID = attrib.SECTIONID
LEFT OUTER JOIN [REPORT_{0}_{1}] bc ON main.SECTIONID = bc.SECTIONID
WHERE YEARS IS NOT NULL
ORDER BY Facility
    ,SECTION
    ,YEARS
");

        private readonly string Query;

        private DataTable Data;

        public BenefitCostRatioReport(string networkId, string simulationId, string simulation)
        {
            OutputFileBaseName = string.Format("{0} - {1}", Title, simulation);

            var attributes = string.Join(string.Empty,
                DBMgr.GetTableColumns($"SEGMENT_{networkId}_NS0")
                .Where(c => !Regex.IsMatch(c, @"_\d\d\d\d$"))
                .Select(c => $",attrib.{c}"));

            Query = string.Format(QueryTemplate, networkId, simulationId, attributes);
        }

        public override string Title => GenericTitle;

        protected override void Fill()
        {
            var sheet = OutputPackage.Workbook.Worksheets[1];
            sheet.Cells.LoadFromDataTable(Data, true);
            sheet.Cells[1, 1, 1, Data.Columns.Count].AutoFilter = true;
            sheet.Cells.AutoFitColumns();
        }

        protected override void Open()
        {
            Data = DBMgr.ExecuteQuery(Query).Tables[0];

            OutputFileExtension = "xlsx";
            var outputStream = GetNewOutputFile().Open(FileMode.Create);

            var templateStream =
                new MemoryStream(Resources.Benefit_Cost_Ratio_Report_Template);

            OutputPackage = new ExcelPackage(outputStream, templateStream);
        }

        protected override void Save()
        {
            OutputPackage?.Save();
            OutputPackage?.Stream.Dispose();
        }
    }
}
