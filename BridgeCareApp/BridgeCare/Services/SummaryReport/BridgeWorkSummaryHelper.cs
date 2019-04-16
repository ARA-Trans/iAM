using BridgeCare.Models;
using System;
using System.Collections.Generic;

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
                cost = cost + (yearData != null ? yearData.Cost : 0);
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
                sum = sum + (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
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
                sum = sum + (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public double CalculateTotalPoorDeckArea(List<SimulationDataModel> simulationDataModels, int year)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) < 5);
                sum = sum + (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public double CalculateTotalDeckArea(List<SimulationDataModel> simulationDataModels)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                sum = sum + Convert.ToDouble(simulationDataModel.DeckArea);
            }

            return sum;
        }

        public int CalculateNHSBridgeGoodCount(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var bridgeData = bridgeDataModels.FindAll(b => b.NHS == "Y");
            var goodCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) >= 7) && bridgeData.Exists(b => b.BRKey == s.BRKey)).Count;
            return goodCount;
        }

        public int CalculateNHSBridgePoorCount(List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int year)
        {
            var bridgeData = bridgeDataModels.FindAll(b => b.NHS == "Y");
            var poorCount = simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) < 5) && bridgeData.Exists(b => b.BRKey == s.BRKey)).Count;
            return poorCount;
        }
    }
}