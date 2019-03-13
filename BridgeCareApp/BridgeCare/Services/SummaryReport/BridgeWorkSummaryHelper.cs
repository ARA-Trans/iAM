using BridgeCare.Models;
using System;
using System.Collections.Generic;

namespace BridgeCare.Services
{
    public static class BridgeWorkSummaryHelper
    {
        const string NoTreatment = "No Treatment";

        public static double CalculateCost(List<SimulationDataModel> simulationDataModels, int year, string project)
        {
            double cost = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && y.Project == project);
                cost = cost + (yearData != null ? yearData.Cost : 0);
            }

            return cost;
        }

        public static int CalculateNoTreatmentCountForCulverts(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == NoTreatment && !y.CulvD.Equals("N"))).Count;
        }

        public static int CalculatePreservationPoorFixCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Culv == "Y" && y.SD == "N")).Count;
        }

        public static int CalculateCountByProject(List<SimulationDataModel> simulationDataModels, int year, string project)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == project)).Count;
        }

        public static int CalculateNoTreatmentCountForBridges(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == NoTreatment && y.CulvD.Equals("N"))).Count;
        }

        public static int CalculatePoorBridgeCount(List<SimulationDataModel> simulationDataModels, int year, string type)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.PoorOnOffRate == type)).Count;
        }

        public static int CalculateTotalPoorBridgesCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.SD == "Y")).Count;
        }

        public static double CalculateTotalPoorBridgesDeckArea(List<SimulationDataModel> simulationDataModels, int year)
        {
            double sum = 0;            
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && y.SD == "Y");
                sum = sum + (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public  static int CalculateTotalBridgeGoodCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) >= 7)).Count;
        }

        public static int CalculateTotalBridgePoorCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && Convert.ToDouble(y.MinC) < 5)).Count;
        }

        public static double CalculateTotalGoodDeckArea(List<SimulationDataModel> simulationDataModels, int year)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) >= 7);
                sum = sum + (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public static double CalculateTotalPoorDeckArea(List<SimulationDataModel> simulationDataModels, int year)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && Convert.ToDouble(y.MinC) < 5);
                sum = sum + (yearData != null ? Convert.ToDouble(simulationDataModel.DeckArea) : 0);
            }

            return sum;
        }

        public static double CalculateTotalDeckArea(List<SimulationDataModel> simulationDataModels)
        {
            double sum = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                sum = sum + Convert.ToDouble(simulationDataModel.DeckArea);
            }

            return sum;
        }
    }
}