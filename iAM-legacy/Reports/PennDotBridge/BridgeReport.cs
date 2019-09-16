using System;

namespace Reports.PennDotBridge
{
    public class BridgeReport
    {
        private string _networkId;
        private string _simulationId;
        private BridgeAnalysis _bridgeAnalysis;

        public BridgeReport(string strNetworkID, string strSimulationID)
        {
            _networkId = strNetworkID;
            _simulationId = strSimulationID;
            _bridgeAnalysis = new BridgeAnalysis(_networkId,strSimulationID);
        }

        public void CreateReport()
        {
            
            Report.XL.Visible = false;
            Report.XL.UserControl = false;
            var oWorkBook = Report.CreateWorkBook();


            //% Poor NHS # Bridges
            var percentPoorNhsNumberBridges = new PercentPoorNhsNumberBridges(_networkId,_simulationId,(Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet,_bridgeAnalysis);
            percentPoorNhsNumberBridges.CreateReport();
     
            //% Poor NHS Deck Area
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var percentPoorNhsDeckArea = new PercentPoorNhsDeckArea(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            percentPoorNhsDeckArea.CreateReport();
     

            //% Poor Entire State # Bridges
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var percentPoorEntireStateNumberBridges = new PercentPoorEntireStateNumberBridges(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            percentPoorEntireStateNumberBridges.CreateReport();

            
            //% Poor Entire State Deck Area
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var percentPoorEntireStateDeckArea = new PercentPoorEntireStateDeckArea(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            percentPoorEntireStateDeckArea.CreateReport();

            
            //Poor Bridges by Count
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var poorBridgesByCount = new PoorBridgesByCount(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            poorBridgesByCount.CreateReport();

            
            //Poor Bridges by Deck Area
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var poorBridgesByDeckArea = new PoorBridgesByDeckArea(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            poorBridgesByDeckArea.CreateReport();
            

            //SD On Rate NHS
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var sdOnRateNhs = new SdOnRateNhs(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            sdOnRateNhs.CreateReport();

            
            //SD on Rate Non-NHS
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var sdOnRateNonNhs = new SdOnRateNonNhs(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            sdOnRateNonNhs.CreateReport();

            
            //Bridge 6 to 5 NHS
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var bridge6To5Nhs = new Bridge6To5Nhs(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            bridge6To5Nhs.CreateReport();

            
            //Bridge 6 to 5 Non-NHS
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var bridge6To5NonNhs = new Bridge6To5NonNhs(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            bridge6To5NonNhs.CreateReport();


            //Bridge 5 to 4 NHS
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var bridge5To4Nhs = new Bridge5To4Nhs(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            bridge5To4Nhs.CreateReport();


            //Bridge 5 to 4 Non-NHS
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var bridge5To4NonNhs = new Bridge5To4NonNhs(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            bridge5To4NonNhs.CreateReport();

            
            //NHS Good Fair Poor # Bridges
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var nhsGoodFairPoorNumberBridges = new NhsGoodFairPoorNumberBridges(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            nhsGoodFairPoorNumberBridges.CreateReport();

            //NHS Good Fair Poor Deck Area
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var nhsGoodFairPoorDeckArea = new NhsGoodFairPoorDeckArea(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            nhsGoodFairPoorDeckArea.CreateReport();
       

            //NonNHS Good Fair Poor # Bridges
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var nonNhsGoodFairPoorNumberBridges = new NonNhsGoodFairPoorNumberBridges(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            nonNhsGoodFairPoorNumberBridges.CreateReport();
       
            //NonNHS Good Fair Poor Deck Area
            oWorkBook.Sheets.Add(After: oWorkBook.Sheets[oWorkBook.Sheets.Count]);
            var nonNhsGoodFairPoorDeckArea = new NonNhsGoodFairPoorDeckArea(_networkId, _simulationId, (Microsoft.Office.Interop.Excel._Worksheet)oWorkBook.ActiveSheet, _bridgeAnalysis);
            nonNhsGoodFairPoorDeckArea.CreateReport();
       

            Report.XL.Visible = true;
            Report.XL.UserControl = true;
            
        }
    }
}
