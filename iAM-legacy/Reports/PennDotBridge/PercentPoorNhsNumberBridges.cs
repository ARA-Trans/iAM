using System;
using Microsoft.Office.Interop.Excel;

namespace Reports.PennDotBridge
{
    internal class PercentPoorNhsNumberBridges :PennDotBaseReport, IReportActions
    {
        public PercentPoorNhsNumberBridges(string networkId, string simulationId, _Worksheet oSheet,BridgeAnalysis analysis)
            : base(networkId, simulationId, oSheet,analysis)
        {

        }

        public void CreateReport()
        {
            Report.SheetPageSetup(Sheet, "% Poor NHS # Bridges", 50d, 20d, 10d, "", DateTime.Now.ToLongDateString(), "Page &P", 1);

            WriteHeader("Percent Poor NHS Number Bridges");
            WriteAnalysisInformationFooter("A26");
            WriteAnalysisYears("Category");
            WriteAnalysisData("% Poor");
            CreateScatter("Percent Poor NHS Number Bridges","","%",1);
        }


        private void WriteAnalysisData(string columnHeader)
        {
            var oData = new object[1, NumberYears + 1];
            oData[0, 0] = columnHeader;
            var poorValues = Analysis.GetPercentagePoorNhs();
            for (int i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = poorValues[i];
            }
            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, "A4", false, true);

            var oR = Sheet.get_Range("A4", Report.GetCellAtOffset("A4", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";


        }
    }
}
