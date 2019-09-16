using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace Reports.PennDotBridge
{
    internal class SdOnRateNhs:PennDotBaseReport,IReportActions
    {
        public SdOnRateNhs(string networkId, string simulationId, _Worksheet oSheet,BridgeAnalysis analysis)
            : base(networkId, simulationId, oSheet,analysis)
        {

        }

        public void CreateReport()
        {
            const string title = "SD On Rate NHS";
            Report.SheetPageSetup(Sheet, title, 50d, 20d, 10d, "", DateTime.Now.ToLongDateString(), "Page &P", 7);
            WriteHeader(title);
            WriteAnalysisInformationFooter("A28");
            WriteAnalysisYears("Network");
            WriteAnalysisData();
            CreateScatter("SD On Rate NHS", "", "Count", 3);
        }

        private void WriteAnalysisData()
        {
            var oData = new object[1, NumberYears + 1];
            
            var network1 = Analysis.GetSdCount("1");
            var network2 = Analysis.GetSdCount("2");
            var networkSum = new List<double>();

            for (var i = 0; i < network1.Count; i++)
            {
                networkSum.Add(network1[i] + network2[i]);
            }

            oData[0, 0] = "1";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = network1[i];
            }


            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, "A4", false, true);
            Report.ClearDataArray(ref oData);
            var oR = Sheet.get_Range("A4", Report.GetCellAtOffset("A4", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";



            oData[0, 0] = "2";
            for (var i = 0; i < NumberYears; i++)
            {
                oData[0, i + 1] = network2[i];
            }
            Report.WriteObjectArrayToExcel(oData, Sheet, "A5", false, true);
            oR = Sheet.get_Range("A5", Report.GetCellAtOffset("A5", 0, NumberYears + 1));
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";


            oData[0, 0] = "Sum 1 and 2";
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
