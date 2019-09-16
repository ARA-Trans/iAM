using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace Reports.PennDotBridge
{
    internal class SdOnRateNonNhs:PennDotBaseReport, IReportActions
    {
        public SdOnRateNonNhs(string networkId, string simulationId, _Worksheet oSheet,BridgeAnalysis analysis)
            : base(networkId, simulationId, oSheet,analysis)
        {

        }
        public void CreateReport()
        {
            const string title = "SD on Rate Non-NHS";
            Report.SheetPageSetup(Sheet, title, 50d, 20d, 10d, "", DateTime.Now.ToLongDateString(), "Page &P", 8);
            WriteHeader(title);
            WriteAnalysisInformationFooter("A28");
            WriteAnalysisYears("Network");
            WriteAnalysisData();
            CreateScatter(title, "", "Count", 3);
        }

        private void WriteAnalysisData()
        {
            var oData = new object[1, NumberYears + 1];

            var network3 = Analysis.GetSdCount("3");
            var network4 = Analysis.GetSdCount("4");
            var networkSum = new List<double>();

            for (var i = 0; i < network3.Count; i++)
            {
                networkSum.Add(network3[i] + network4[i]);
            }

            oData[0, 0] = "3";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = network3[i];
            }


            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, "A4", false, true);
            Report.ClearDataArray(ref oData);
            var oR = Sheet.get_Range("A4", Report.GetCellAtOffset("A4", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";



            oData[0, 0] = "4";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = network4[i];
            }
            Report.WriteObjectArrayToExcel(oData, Sheet, "A5", false, true);
            oR = Sheet.get_Range("A5", Report.GetCellAtOffset("A5", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";


            oData[0, 0] = "Sum 3 and 4";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = networkSum[i];
            }


            Report.WriteObjectArrayToExcel(oData, Sheet, "A6", false, true);
            Report.ClearDataArray(ref oData);
            oR = Sheet.get_Range("A6", Report.GetCellAtOffset("A6", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";

        }

    }
}
