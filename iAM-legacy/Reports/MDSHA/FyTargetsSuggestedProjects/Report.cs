namespace Reports.MDSHA.FyTargetsSuggestedProjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using DatabaseManager;
    using OfficeOpenXml;

    public class Report : ReportBase
    {
        public const string GenericTitle =
            "FY Targets & Suggested Projects Report";

        private const string TitleTemplate =
            "FY{0} Targets & Suggested Projects Report";

        private static readonly string QueryTemplate =
            #region MDSHA SQL Query: Composite Format String, Verbatim Literal
            new FormattedSql(@"
--include attributes LAST_YEAR, SKID_RUT_LIST, GLOBAL_ROUTE_ID
SELECT projlist.SECTIONID ,
  projlist.AADT_VMT_{3} ,
  projlist.""FISCAL YEAR"" ,
  projlist.DISTRICT ,
  projlist.SHOP ,
  projlist.COUNTY ,
  projlist.FACILITY ,
  projlist.SECTION ,
  projlist.ROUTE ,
  projlist.RNUM ,
  projlist.RSUFF ,
  projlist.BEGIN_STATION ,
  projlist.END_STATION ,
  projlist.DIRECTION ,
  projlist.BEGIN_LIMIT ,
  projlist.END_LIMIT ,
  projlist.""NO. OF LANES"" ,
  projlist.""TOTAL LANE MILES"" ,
  projlist.DIVIDED ,
  projlist.ROAD_CLASS ,
  projlist.ROAD_TYPE ,
  projlist.FUNCTIONAL_CLASS ,
  projlist.PAVEMENT_TYPE ,
  projlist.AVG_IRI_{4} ,
  projlist.R_IRI_{4} ,
  projlist.CI_{4} ,
  projlist.CI_FUNCTIONAL_{4} ,
  projlist.CI_STRUCTURAL_{4} ,
  projlist.AVG_RUT_{4} ,
  projlist.SPAD_SKID_NUMBER_{4} ,
  projlist.AVG_IRI_{3} ,
  projlist.CI_{3} ,
  projlist.CI_FUNCTIONAL_{3} ,
  projlist.CI_STRUCTURAL_{3} ,
  projlist.AVG_RUT_{3} ,
  projlist.SPADJ_SKID_NUMBER_{3} ,
  projlist.AVG_IRI_{2} ,
  projlist.CI_{2} ,
  projlist.CI_FUNCTIONAL_{2} ,
  projlist.CI_STRUCTURAL_{2} ,
  projlist.AVG_RUT_{2} ,
  projlist.SPADJ_SKID_NUMBER_{2} ,
  projlist.TREATMENT ,
  projlist.BUDGET ,
  projlist.""TREATMENT LIFE"" ,
  projlist.LANE_MILE_YEARS ,
  CASE
    WHEN prersl.iri_rsl_{3}<0
    THEN 0
    WHEN prersl.iri_rsl_{3}>50
    THEN 50
    ELSE prersl.iri_rsl_{3}
  END iri_rsl_{3} ,
  CASE
    WHEN prersl.fci_rsl_{3}<0
    THEN 0
    WHEN prersl.fci_rsl_{3}>50
    THEN 50
    ELSE prersl.fci_rsl_{3}
  END fci_rsl_{3} ,
  CASE
    WHEN prersl.sci_rsl_{3}<0
    THEN 0
    WHEN prersl.sci_rsl_{3}>50
    THEN 50
    ELSE prersl.sci_rsl_{3}
  END sci_rsl_{3} ,
  CASE
    WHEN prersl.rut_rsl_{3}<0
    THEN 0
    WHEN prersl.rut_rsl_{3}>50
    THEN 50
    ELSE prersl.rut_rsl_{3}
  END rut_rsl_{3} ,
  CASE
    WHEN prersl.skid_rsl_{3}<0
    THEN 0
    WHEN prersl.skid_rsl_{3}>50
    THEN 50
    ELSE prersl.skid_rsl_{3}
  END skid_rsl_{3} ,
  CASE
    WHEN postrsl.iri_rsl_{2}<0
    THEN 0
    WHEN postrsl.iri_rsl_{2}>50
    THEN 50
    ELSE postrsl.iri_rsl_{2}
  END iri_rsl_{2} ,
  CASE
    WHEN postrsl.fci_rsl_{2}<0
    THEN 0
    WHEN postrsl.fci_rsl_{2}>50
    THEN 50
    ELSE postrsl.fci_rsl_{2}
  END fci_rsl_{2} ,
  CASE
    WHEN postrsl.sci_rsl_{2}<0
    THEN 0
    WHEN postrsl.sci_rsl_{2}>50
    THEN 50
    ELSE postrsl.sci_rsl_{2}
  END sci_rsl_{2} ,
  CASE
    WHEN postrsl.rut_rsl_{2}<0
    THEN 0
    WHEN postrsl.rut_rsl_{2}>50
    THEN 50
    ELSE postrsl.rut_rsl_{2}
  END rut_rsl_{2} ,
  CASE
    WHEN postrsl.skid_rsl_{2}<0
    THEN 0
    WHEN postrsl.skid_rsl_{2}>50
    THEN 50
    ELSE postrsl.skid_rsl_{2}
  END skid_rsl_{2} ,
  PROJLIST.SKID_RUT_LIST ,
  PROJLIST.GLOBAL_ROUTE_ID ,
  PROJLIST.LAST_YEAR
FROM
  (SELECT REP.SECTIONID,
    SIM.AADT_VMT_{3},
    REP.YEARS AS ""FISCAL YEAR"",
    TO_NUMBER(seg.DISTRICT) DISTRICT,
    SHOP,
    seg.COUNTY,
    sec.FACILITY,
    sec.SECTION,
    seg.ROUTE,
    seg.RNUM,
    seg.RSUFF,
    --SEG.SKID_RUT_LIST,
    '' AS SKID_RUT_LIST,
    seg.LAST_YEAR,
    sec.BEGIN_STATION,
    sec.END_STATION,
    sec.DIRECTION,
    seg.BEGIN_LIMIT,
    seg.END_LIMIT,
    seg.TOTAL_LANES   AS ""NO. OF LANES"",
    ROUND(SEC.AREA,2) AS ""TOTAL LANE MILES"",
    DIVIDED,
    CASE
      WHEN Upper(sim.ROAD_CLASS_{2}) = 'U'
      THEN 'URBAN'
      ELSE 'RURAL'
    END AS ROAD_CLASS,
    CASE
      WHEN Upper(seg.ROUTE) = 'IS'
      THEN 'IS'
      ELSE 'NON-IS'
    END                       AS ROAD_TYPE,
    TO_NUMBER(seg.FUNC_CLASS) AS ""FUNCTIONAL_CLASS"",
    seg.PAVEMENT_TYPE,
    SEG.AVG_IRI AVG_IRI_{4},
    SEG.R_IRI R_IRI_{4},
    SEG.CI CI_{4},
    SEG.CI_FUNCTIONAL CI_FUNCTIONAL_{4},
    SEG.CI_STRUCTURAL CI_STRUCTURAL_{4},
    SEG.AVG_RUT AVG_RUT_{4},
    SEG.SPADJ_SKID_NUMBER SPAD_SKID_NUMBER_{4},
    SEG.GLOBAL_ROUTE_ID,
    SIM.AVG_IRI_{3},
    SIM.CI_FUNCTIONAL_{3}*0.25+SIM.CI_STRUCTURAL_{3}*0.75 CI_{3},
    SIM.CI_FUNCTIONAL_{3},
    SIM.CI_STRUCTURAL_{3},
    SIM.AVG_RUT_{3},
    SIM.SPADJ_SKID_NUMBER_{3},
    SIM.AVG_IRI_{2},
    SIM.CI_FUNCTIONAL_{2}*0.25+SIM.CI_STRUCTURAL_{2}*0.75 CI_{2},
    SIM.CI_FUNCTIONAL_{2},
    SIM.CI_STRUCTURAL_{2},
    SIM.AVG_RUT_{2},
    SIM.SPADJ_SKID_NUMBER_{2},
    rep.TREATMENT,
    ROUND(rep.COST_,0)                   AS BUDGET,
    ROUND(rep.REMAINING_LIFE,1)          AS ""TREATMENT LIFE"",
    ROUND(rep.remaining_life*sec.area,1) AS ""LANE_MILE_YEARS""
  FROM SECTION_{0} SEC
  JOIN SEGMENT_{0}_NS0 SEG
  ON (SEC.SECTIONID = SEG.SECTIONID)
  JOIN SIMULATION_{0}_{1} SIM
  ON (SEG.SECTIONID=SIM.SECTIONID)
  JOIN REPORT_{0}_{1} REP
  ON (SEC.SECTIONID         =REP.SECTIONID)
  WHERE rep.YEARS           = {2}
  AND Upper(rep.TREATMENT) != 'NO TREATMENT'
  GROUP BY REP.SECTIONID,
    SIM.AADT_VMT_{3},
    rep.YEARS,
    seg.DISTRICT,
    SHOP,
    seg.COUNTY,
    sec.FACILITY,
    sec.SECTION,
    SEG.ROUTE,
    SEG.RNUM,
    SEG.RSUFF,
    SEC.BEGIN_STATION,
    sec.END_STATION,
    sec.DIRECTION,
    seg.BEGIN_LIMIT,
    seg.END_LIMIT,
    --seg.SKID_RUT_LIST,
    seg.LAST_YEAR,
    seg.TOTAL_LANES,
    sim.ROAD_CLASS_{2},
    seg.PAVEMENT_TYPE,
    seg.FUNC_CLASS,
    sim.AADT_{2},
    seg.LAST_TREATMENT,
    rep.TREATMENT,
    SEG.AVG_IRI,
    SEG.R_IRI,
    SEG.CI,
    SEG.CI_FUNCTIONAL,
    SEG.CI_STRUCTURAL,
    SEG.AVG_RUT,
    SEG.SPADJ_SKID_NUMBER,
    SEG.GLOBAL_ROUTE_ID,
    SIM.AVG_IRI_{3},
    SIM.AVG_IRI_{2},
    SIM.CI_FUNCTIONAL_{3},
    SIM.CI_FUNCTIONAL_{2},
    SIM.CI_STRUCTURAL_{3},
    SIM.CI_STRUCTURAL_{2},
    SIM.AVG_RUT_{3},
    SIM.AVG_RUT_{2},
    SIM.SPADJ_SKID_NUMBER_{3},
    SIM.SPADJ_SKID_NUMBER_{2},
    rep.RLHASH,
    DIVIDED,
    rep.REMAINING_LIFE,
    rep.COST_,
    SEC.AREA
  ) projlist,-- Main project list is from this subquery
  (SELECT sectionid,
    rlhash ,
    CASE
      WHEN regexp_instr (RLHASH, 'IRI',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'IRI',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'IRI',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'IRI',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'IRI',1),2))-1
    END IRI_RSL_{3} ,
    CASE
      WHEN regexp_instr (RLHASH, 'SKID',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'SKID',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'SKID',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'SKID',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'SKID',1),2))-1
    END SKID_RSL_{3} ,
    CASE
      WHEN regexp_instr (RLHASH, 'STRUCTURAL',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'STRUCTURAL',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'STRUCTURAL',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'STRUCTURAL',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'STRUCTURAL',1),2))-1
    END SCI_RSL_{3} ,
    CASE
      WHEN regexp_instr (RLHASH, 'RUT',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'RUT',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'RUT',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'RUT',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'RUT',1),2))-1
    END RUT_RSL_{3} ,
    CASE
      WHEN regexp_instr (RLHASH, 'FUNCTIONAL',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'FUNCTIONAL',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'FUNCTIONAL',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'FUNCTIONAL',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'FUNCTIONAL',1),2))-1
    END FCI_RSL_{3}
  FROM report_{0}_{1}
  WHERE YEARS = {3}
  ) prersl,-- pre rsl values are from this subquery
  (SELECT sectionid,
    rlhash ,
    CASE
      WHEN regexp_instr (RLHASH, 'IRI',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'IRI',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'IRI',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'IRI',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'IRI',1),2))
    END IRI_RSL_{2} ,
    CASE
      WHEN regexp_instr (RLHASH, 'SKID',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'SKID',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'SKID',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'SKID',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'SKID',1),2))
    END SKID_RSL_{2} ,
    CASE
      WHEN regexp_instr (RLHASH, 'STRUCTURAL',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'STRUCTURAL',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'STRUCTURAL',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'STRUCTURAL',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'STRUCTURAL',1),2))
    END SCI_RSL_{2} ,
    CASE
      WHEN regexp_instr (RLHASH, 'RUT',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'RUT',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'RUT',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'RUT',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'RUT',1),2))
    END RUT_RSL_{2} ,
    CASE
      WHEN regexp_instr (RLHASH, 'FUNCTIONAL',1)= 0
      THEN NULL
      WHEN (regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'FUNCTIONAL',1),1) IS NULL
      AND regexp_substr(rlhash,'[[:digit:]]+',regexp_instr (RLHASH, 'FUNCTIONAL',1),2)   IS NULL)
      THEN 0
      ELSE TO_NUMBER(REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'FUNCTIONAL',1),1)
        ||'.'
        || REGEXP_SUBSTR(RLHASH,'[[:digit:]]+',REGEXP_INSTR (RLHASH, 'FUNCTIONAL',1),2))
    END FCI_RSL_{2}
  FROM report_{0}_{1}
  WHERE YEARS = {2}
  ) postrsl -- post rsl values are from this subquery
WHERE prersl.sectionid =postrsl.sectionid
AND projlist.sectionid =prersl.sectionid
ORDER BY DISTRICT,
  SHOP,
  TREATMENT
");
            #endregion

        private static readonly Color BlueA = Color.FromArgb(0, 176, 240);

        private static readonly Color GreenB = Color.FromArgb(0, 176, 80);

        private static readonly Color YellowC = Color.FromArgb(255, 255, 0);

        private static readonly Color OrangeD = Color.FromArgb(255, 192, 0);

        private static readonly Color RedE = Color.FromArgb(255, 0, 0);

        private static readonly Color MaroonF = Color.FromArgb(192, 0, 0);

        private readonly string Query;

        private DataTable Data;

        public Report(
            string networkId,
            string simulationId,
            string simulation,
            string[] analysisYears,
            int selectedYearIndex)
        {
            this.NetworkId = networkId;
            this.SimulationId = simulationId;
            this.Simulation = simulation;

            var selectedFiscalYear = analysisYears[selectedYearIndex];
            var previousFiscalYear = selectedFiscalYear.SubtractOne();

            var firstYearIndex = analysisYears.Length - 1;
            var latestConditionDataYear =
                analysisYears[firstYearIndex].SubtractOne();

            this.Query = string.Format(
                Report.QueryTemplate,
                networkId,
                simulationId,
                selectedFiscalYear,
                previousFiscalYear,
                latestConditionDataYear);

            var title = string.Format(TitleTemplate, selectedFiscalYear);
            this.OutputFileBaseName =
                string.Format("{0} - {1}", title, simulation);
        }

        public override string Title
        {
            get { return GenericTitle; }
        }

        public string TemplateFileName { private get; set; }

        /// <summary>
        ///     Prepares report template to be filled in by retrieving required
        ///     data and opening the report's output package.
        /// </summary>
        /// <exception cref="DbException">
        ///     the retrieval of this report's required data fails; see Remarks
        /// </exception>
        /// <remarks>
        ///     Callers of the <see cref="ReportBase.Generate"/> template method
        ///     (of which this method is a component) should be prepared to
        ///     catch the potential DbException from this method, since it will
        ///     often simply indicate that the user selected a simulation to
        ///     which this report's data query cannot apply due to the
        ///     simulation's data not having the expected schema.
        /// </remarks>
        protected override void Open()
        {
            var queryResult = DBMgr.ExecuteQuery(this.Query);
            this.Data = queryResult.Tables[0];

            // If no exception was thrown by the data retrieval above,
            // then output preparation happens next.
            byte[] templateBytes;
            if (this.TemplateFileName == null)
            {
                templateBytes =
                    Resources
                    .FY17_Targets_Suggested_Projects_Report_Template_09_03_14;

                this.OutputFileExtension = "xlsm";
            }
            else
            {
                var templateFile = new FileInfo(this.TemplateFileName);
                templateBytes = File.ReadAllBytes(templateFile.FullName);

                this.OutputFileExtension = templateFile.Extension.Substring(1);
            }

            var templateStream = new MemoryStream(templateBytes);

            var outputFile = this.GetNewOutputFile();
            var outputStream = outputFile.Open(FileMode.Create);

            try
            {
                this.OutputPackage =
                    new ExcelPackage(outputStream, templateStream);
            }
            catch (Exception e) // That's right; EPPlus only throws Exception...
            {
                throw new ReportGenerationException(
                    "Excel output package could not be created: " + e.Message,
                    e);
            }
        }

        protected override void Fill()
        {
            try
            {
                // Grab the second sheet and fill it in
                var sheet =
                    this.OutputPackage.Workbook.Worksheets["Suggested Projects"];

                sheet.Cells["CB3"].LoadFromDataTable(this.Data, true);

                var numRows = this.Data.Rows.Count;
                if (numRows != 0)
                {
                    // Set the formulas and styles for all input and output rows
                    var addrTopOutputRow = new ExcelAddress("A4:CA4");
                    var addrTopInputRow = new ExcelAddress("CB4:EH4");

                    var fromRow = addrTopOutputRow.Start.Row;
                    var toRow = fromRow + numRows - 1;

                    var fromColOut = addrTopOutputRow.Start.Column;
                    var toColOut = addrTopOutputRow.End.Column;

                    var fromColIn = addrTopInputRow.Start.Column;
                    var toColIn = addrTopInputRow.End.Column;

                    if (numRows > 1)
                    {
                        for (int c = fromColOut; c <= toColOut; ++c)
                        {
                            var top = sheet.Cells[fromRow, c];
                            var rest = sheet.Cells[fromRow + 1, c, toRow, c];

                            // Seems to be a bug in EPPlus when setting a null
                            // or blank formula to a multi-cell range, where the
                            // resulting shared (blank) formula is broken and
                            // gets repaired/removed when opened in Excel. See
                            // https://epplus.codeplex.com/workitem/15056
                            if (!string.IsNullOrWhiteSpace(top.FormulaR1C1))
                            {
                                rest.FormulaR1C1 = top.FormulaR1C1;
                            }

                            rest.StyleID = top.StyleID;
                        }

                        for (int c = fromColIn; c <= toColIn; ++c)
                        {
                            var top = sheet.Cells[fromRow, c];
                            var rest = sheet.Cells[fromRow + 1, c, toRow, c];

                            rest.StyleID = top.StyleID;
                        }
                    }

                    // Set the six color-code conditional formatting rules for
                    // the output rows
                    var cf = sheet.ConditionalFormatting;
                    var addrFullOutput =
                        new ExcelAddress(fromRow, fromColOut, toRow, toColOut);

                    var ruleA = cf.AddExpression(addrFullOutput);
                    ruleA.Formula = @"$BG4=""A""";
                    ruleA.Style.Fill.BackgroundColor.Color = BlueA;
                    ruleA.Style.Font.Color.Color = Color.Black;

                    var ruleB = cf.AddExpression(addrFullOutput);
                    ruleB.Formula = @"$BG4=""B""";
                    ruleB.Style.Fill.BackgroundColor.Color = GreenB;
                    ruleB.Style.Font.Color.Color = Color.Black;

                    var ruleC = cf.AddExpression(addrFullOutput);
                    ruleC.Formula = @"$BG4=""C""";
                    ruleC.Style.Fill.BackgroundColor.Color = YellowC;
                    ruleC.Style.Font.Color.Color = Color.Black;

                    var ruleD = cf.AddExpression(addrFullOutput);
                    ruleD.Formula = @"$BG4=""D""";
                    ruleD.Style.Fill.BackgroundColor.Color = OrangeD;
                    ruleD.Style.Font.Color.Color = Color.Black;

                    var ruleE = cf.AddExpression(addrFullOutput);
                    ruleE.Formula = @"$BG4=""E""";
                    ruleE.Style.Fill.BackgroundColor.Color = RedE;
                    ruleE.Style.Font.Color.Color = Color.White;

                    var ruleF = cf.AddExpression(addrFullOutput);
                    ruleF.Formula = @"$BG4=""F""";
                    ruleF.Style.Fill.BackgroundColor.Color = MaroonF;
                    ruleF.Style.Font.Color.Color = Color.White;
                }
            }
            catch (Exception e) // EPPlus only throws Exception...
            {
                throw new ReportGenerationException(
                    "Excel output package was created, but report content" +
                    " could not be filled: " + e.Message,
                    e);
            }
        }

        protected override void Save()
        {
            if (this.OutputPackage != null)
            {
                this.OutputPackage.Save();
                this.OutputPackage.Stream.Dispose();
            }
        }

        public class Builder
        {
            private readonly MetaData Tags;

            private readonly List<string> AnalysisYears_;

            public string[] AnalysisYears
            {
                get
                {
                    return this.AnalysisYears_.ToArray();
                }
            }

            public Builder(
                string networkId,
                string simulationId,
                string simulation)
            {
                this.Tags = new MetaData();

                this.Tags.NetworkId = networkId;
                this.Tags.SimulationId = simulationId;
                this.Tags.Simulation = simulation;

                this.AnalysisYears_ = Utilities.GetAnalysisYears(simulationId);
                this.AnalysisYears_.Reverse();
            }

            public Report Build(int selectedYearIndex)
            {
                return new Report(
                    this.Tags.NetworkId,
                    this.Tags.SimulationId,
                    this.Tags.Simulation,
                    this.AnalysisYears,
                    selectedYearIndex);
            }
        }
    }
}
