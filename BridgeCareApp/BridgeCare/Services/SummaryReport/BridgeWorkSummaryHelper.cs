using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Services
{
    public class BridgeWorkSummaryComputationHelper
    {
        const string NoTreatment = "No Treatment";

        public double CalculateCost(List<SimulationDataModel> simulationDataModels, int year, string project)
        {
            double cost = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && y.Project == project);
                cost += (yearData != null ? yearData.Cost : 0);
            }

            return cost;
        }

        public int CalculateNoTreatmentCountForCulverts(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == NoTreatment && !y.CulvD.Equals("N"))).Count;
        }

        public int CalculatePreservationPoorFixCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Culv == "Y" && y.SD == "N")).Count;
        }

        public int CalculateCountByProject(List<SimulationDataModel> simulationDataModels, int year, string project)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == project)).Count;
        }

        public int CalculateNoTreatmentCountForBridges(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == NoTreatment && y.CulvD.Equals("N"))).Count;
        }

        public int CalculatePoorBridgeCount(List<SimulationDataModel> simulationDataModels, int year, string type)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.PoorOnOffRate == type)).Count;
        }

        public int CalculateTotalPoorBridgesCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.SD == "Y")).Count;
        }

        public double CalculateTotalPoorBridgesDeckArea(List<SimulationDataModel> simulationDataModels, int year)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && y.SD == "Y");
                sum += (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public int CalculateTotalBridgeGoodCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) >= 7)).Count;
        }

        public int CalculateTotalBridgePoorCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) < 5)).Count;
        }

        public double CalculateTotalGoodDeckArea(List<SimulationDataModel> simulationDataModels, int year)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) >= 7);
                sum += (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public double CalculateTotalPoorDeckArea(List<SimulationDataModel> simulationDataModels, int year)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) < 5);
                sum += (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public double CalculateTotalDeckArea(List<SimulationDataModel> simulationDataModels)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                sum += Convert.ToDouble(simulationDataModel.DeckArea);
            }

            return sum;
        }

        public double CalculateNHSBridgePoorDeckArea(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            double sum = 0;
            var filteredBridgeDataModels = bridgeDataModels.FindAll(b => b.NHS == "Y");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => filteredBridgeDataModels.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) < 5);
                sum += (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }
            return sum;
        }

        public double CalculateNHSBridgeGoodDeckArea(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            double sum = 0;
            var filteredBridgeDataModels = bridgeDataModels.FindAll(b => b.NHS == "Y");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => filteredBridgeDataModels.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) >= 7);
                sum += (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }
            return sum;
        }

        public int CalculateNHSBridgeGoodCount(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var filteredBridgeDataModels = bridgeDataModels.FindAll(b => b.NHS == "Y");
            var goodCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) >= 7) && filteredBridgeDataModels.Exists(b => b.BRKey == s.BRKey)).Count;
            return goodCount;
        }

        public int CalculateNHSBridgePoorCount(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var filteredBridgeDataModels = bridgeDataModels.FindAll(b => b.NHS == "Y");
            var poorCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) < 5) && filteredBridgeDataModels.Exists(b => b.BRKey == s.BRKey)).Count;
            return poorCount;
        }

        #region posted and closed bridge count functions
        internal int CalculatePostedAndClosedBridgeCountForBPN13(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string bpn, string posted)
        {
            var postedBridges = bridgeDataModels.FindAll(b => b.Posted.ToUpper() == posted && b.BPN == bpn);
            var postedCount = 0;
            if (posted == "Y")
            {
                postedCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) <= 4.75 && Convert.ToDouble(y.MinC) > 3.25) && postedBridges.Exists(b => b.BRKey == s.BRKey)).Count;
            }
            else
            {
                postedCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) <= 3.25) && postedBridges.Exists(b => b.BRKey == s.BRKey)).Count;
            }
            return postedCount;
        }
        internal int CalculatePostedAndClosedBridgeCountForBPN2H(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string posted)
        {
            var postedBridges = bridgeDataModels.FindAll(b => b.Posted.ToUpper() == posted && b.BPN == "2" || b.BPN == "H");
            var postedCount = 0;
            if (posted == "Y")
            {
                postedCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) <= 4.75 && Convert.ToDouble(y.MinC) > 3.25) && postedBridges.Exists(b => b.BRKey == s.BRKey)).Count;
            }
            else
            {
                postedCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) <= 3.25) && postedBridges.Exists(b => b.BRKey == s.BRKey)).Count;
            }
            return postedCount;
        }
        internal int CalculatePostedAndClosedBridgeCountForRemaining(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string posted)
        {
            var postedBridges = bridgeDataModels.FindAll(b => b.Posted.ToUpper() == posted && b.BPN != "2" && b.BPN != "H" && b.BPN != "1" && b.BPN != "3");
            var postedCount = 0;
            if (posted == "Y")
            {
                postedCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) <= 4.75 && Convert.ToDouble(y.MinC) > 3.25) && postedBridges.Exists(b => b.BRKey == s.BRKey)).Count;
            }
            else
            {
                postedCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) <= 3.25) && postedBridges.Exists(b => b.BRKey == s.BRKey)).Count;
            }
            return postedCount;
        }
        #endregion

        #region posted and closed deck area functions
        internal double CalculatePostedAndClosedDeckAreaForBPN13(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string bpn, string postStatus)
        {
            var sum = 0.0;
            var postedBridges = bridgeDataModels.FindAll(b => b.Posted == postStatus && b.BPN == bpn);
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => postedBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                if(postStatus == "Y")
                {
                    var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) <= 4.75 && Convert.ToDouble(y.MinC) > 3.25);
                    sum += yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0;
                }
                else
                {
                    var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) <= 3.25);
                    sum += yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0;
                }
            }
            return sum;
        }
        internal double CalculatePostedAndClosedDeckAreaForBPN2H(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string postStatus)
        {
            var sum = 0.0;
            var postedBridges = bridgeDataModels.FindAll(b => b.Posted == postStatus && b.BPN == "2" || b.BPN == "H");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => postedBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                if (postStatus == "Y")
                {
                    var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) <= 4.75 && Convert.ToDouble(y.MinC) > 3.25);
                    sum += yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0;
                }
                else
                {
                    var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) <= 3.25);
                    sum += yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0;
                }
            }
            return sum;
        }
        internal double CalculatePostedAndClosedDeckAreaForRemainingBPN(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string postStatus)
        {
            var sum = 0.0;
            var postedBridges = bridgeDataModels.FindAll(b => b.Posted == postStatus && b.BPN != "2" && b.BPN != "H" && b.BPN != "1" && b.BPN != "3");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => postedBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                if (postStatus == "Y")
                {
                    var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) <= 4.75 && Convert.ToDouble(y.MinC) > 3.25);
                    sum += yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0;
                }
                else
                {
                    var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) <= 3.25);
                    sum += yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0;
                }
            }
            return sum;
        }
        #endregion

        internal double CalculateMoneyNeededByBPN13(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string bpn)
        {
            var sum = 0.0;
            var filteredBPNBridges = bridgeDataModels.FindAll(b => b.BPN == bpn);
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => filteredBPNBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year);
                sum += yearData != null ? yearData.Cost : 0;
            }
            return sum;
        }
        internal double CalculateMoneyNeededByBPN2H(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var sum = 0.0;
            var filteredBPNBridges = bridgeDataModels.FindAll(b => b.BPN == "2" || b.BPN == "H");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => filteredBPNBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year);
                sum += yearData != null ? yearData.Cost : 0;
            }
            return sum;
        }
        internal double CalculateMoneyNeededByRemainingBPN(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var sum = 0.0;
            var filteredBPNBridges = bridgeDataModels.FindAll(b => b.BPN != "2" && b.BPN != "H" && b.BPN != "1" && b.BPN != "3");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => filteredBPNBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var simulationDataModel in filteredSimulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year);
                sum += yearData != null ? yearData.Cost : 0;
            }
            return sum;
        }

        #region poor deck area functions
        internal double CalculatePoorDeckAreaForBPN13(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year, string bpn)
        {
            var sum = 0.0;
            var postedBridges = bridgeDataModels.FindAll(b =>  b.BPN == bpn);
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => postedBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var model in filteredSimulationDataModels)
            {
                var yearData = model.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) < 5);
                sum += yearData != null ? Convert.ToDouble(model.DeckArea) : 0;
            }
            return sum;
        }

        internal double CalculatePoorDeckAreaForBPN2H(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var sum = 0.0;
            var postedBridges = bridgeDataModels.FindAll(b => b.BPN == "2" || b.BPN == "H");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => postedBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var model in filteredSimulationDataModels)
            {
                var yearData = model.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) < 5);
                sum += yearData != null ? Convert.ToDouble(model.DeckArea) : 0;
            }
            return sum;
        }

        internal double CalculatePoorDeckAreaForRemainingBPN(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var sum = 0.0;
            var postedBridges = bridgeDataModels.FindAll(b => b.BPN != "2" && b.BPN != "H" && b.BPN != "1" && b.BPN != "3");
            var filteredSimulationDataModels = simulationDataModels.FindAll(s => postedBridges.Exists(b => b.BRKey == s.BRKey));
            foreach (var model in filteredSimulationDataModels)
            {
                var yearData = model.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) < 5);
                sum += yearData != null ? Convert.ToDouble(model.DeckArea) : 0;
            }
            return sum;
        }
        #endregion
    }
}
