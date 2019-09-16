using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace Reports.PennDotBridge
{
    internal class Bridge6To5Nhs:PennDotBaseReport,IReportActions
    {
        public Bridge6To5Nhs(string networkId, string simulationId, _Worksheet oSheet,BridgeAnalysis analysis)
            : base(networkId, simulationId, oSheet,analysis)
        {

        }
        public void CreateReport()
        {
            const string title = "Bridge 6 to 5 NHS";
            Report.SheetPageSetup(Sheet, title, 50d, 20d, 10d, "", DateTime.Now.ToLongDateString(), "Page &P", 9);
            WriteHeader(title);
            WriteAnalysisInformationFooter("A26");
            WriteAnalysisYears("Network");
            WriteAnalysisData();
            CreateScatter(title, "", "Count", 1);
        }

        private void WriteAnalysisData()
        {
            var oData = new object[1, NumberYears + 1];

            var network1 = Analysis.GetTransition(6, "1");
            var network2 = Analysis.GetTransition(6, "2");
            var networkSum = new List<double>();

            for (var i = 0; i < network1.Count; i++)
            {
                networkSum.Add(network1[i] + network2[i]);
            }

            oData[0, 0] = "NHS";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = networkSum[i];
            }

            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, "A4", false, true);
            Report.ClearDataArray(ref oData);
            var oR = Sheet.get_Range("A4", Report.GetCellAtOffset("A4", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";
        }
    }
}
