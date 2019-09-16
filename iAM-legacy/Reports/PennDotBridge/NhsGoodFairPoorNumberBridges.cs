using System;
using Microsoft.Office.Interop.Excel;

namespace Reports.PennDotBridge
{
    internal class NhsGoodFairPoorNumberBridges:PennDotBaseReport,IReportActions
    {
        public NhsGoodFairPoorNumberBridges(string networkId, string simulationId, _Worksheet oSheet,BridgeAnalysis analysis)
            : base(networkId, simulationId, oSheet,analysis)
        {

        }
        public void CreateReport()
        {
            const string title = "NHS Good Fair Poor # Bridges";
            Report.SheetPageSetup(Sheet, title, 50d, 20d, 10d, "", DateTime.Now.ToLongDateString(), "Page &P", 13);
            WriteHeader("NHS Good/Fair/Poor Number Bridges");
            WriteAnalysisInformationFooter("A29");
            WriteAnalysisYears("Category");
            WriteAnalysisData();
            CreateStackedColumnBarGraph(3);
        }



        private void WriteAnalysisData()
        {
            var oData = new object[1, NumberYears + 1];

            var good = Analysis.GetCountBetween(10, 7, true, true);
            var fair = Analysis.GetCountBetween(7, 4, false, true);
            var poor = Analysis.GetCountBetween(4, 0, true, true);
           
            oData[0, 0] = "Poor (<=4)";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = poor[i];
            }


            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, "A4", false, true);
            Report.ClearDataArray(ref oData);
            var oR = Sheet.get_Range("A4", Report.GetCellAtOffset("A4", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";



            oData[0, 0] = "Fair (<7 and >4)";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = fair[i];
            }
            Report.WriteObjectArrayToExcel(oData, Sheet, "A5", false, true);
            oR = Sheet.get_Range("A5", Report.GetCellAtOffset("A5", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";


            oData[0, 0] = "Good (>=7)";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = good[i];
            }


            Report.WriteObjectArrayToExcel(oData, Sheet, "A6", false, true);
            Report.ClearDataArray(ref oData);
            oR = Sheet.get_Range("A6", Report.GetCellAtOffset("A6", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";
            
        }
    }
}
