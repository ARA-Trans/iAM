using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using RoadCareDatabaseOperations;

namespace Reports.PennDotBridge
{
    public class BridgeAnalysis
    {
        public List<BridgeCondition> Bridges { get; }
        public int FirstYear { get; }
        public int NumberYears { get; }


        public BridgeAnalysis(string networkId, string simulationId)
        {
            Bridges = new List<BridgeCondition>();
            var investment = DBOp.QueryInvestments(simulationId);
            FirstYear = Convert.ToInt32(investment.Tables[0].Rows[0]["FIRSTYEAR"]);
            NumberYears = Convert.ToInt32(investment.Tables[0].Rows[0]["NUMBERYEARS"]);


            var simulation = DBOp.QuerySimulationResult(networkId,simulationId);
            foreach (DataRow row in simulation.Tables[0].Rows)
            {
                Bridges.Add(new BridgeCondition(row,FirstYear,NumberYears));
            }
        }







        public List<string> GetPercentagePoorNhs()
        {
            var results = new List<string>();
            for (var year = 0; year <  NumberYears; year++)
            {
                var poorCount = 0;
                var nhsCount = 0;
                foreach (var bridge in Bridges)
                {
                    if (bridge.AnnualStructuralDeficient[year].Sd <= 4 && (bridge.BusinessPlanNetwork=="1" || bridge.BusinessPlanNetwork=="2"))
                    {
                        poorCount++;
                    }

                    if (bridge.BusinessPlanNetwork == "1" || bridge.BusinessPlanNetwork == "2")
                    {
                        nhsCount++;
                    }

                }

                if (nhsCount > 0)
                {
                    var percent = 100 * (double) poorCount / nhsCount;
                    results.Add(percent.ToString("f2") + "%");
                }
                else
                {
                    results.Add(0 + "%");
                }
            }
            return results;
        }

        public List<string> GetPercentagePoorNhsDeckArea()
        {
            var results = new List<string>();
            for (var year = 0; year < NumberYears; year++)
            {
                var poorDeckArea = 0d;
                var nhsDeckArea = 0d;
                foreach (var bridge in Bridges)
                {
                    if (bridge.AnnualStructuralDeficient[year].Sd <= 4 && (bridge.BusinessPlanNetwork == "1" || bridge.BusinessPlanNetwork == "2"))
                    {
                        poorDeckArea += bridge.DeckArea;
                    }

                    if (bridge.BusinessPlanNetwork == "1" || bridge.BusinessPlanNetwork == "2")
                    {
                        nhsDeckArea += bridge.DeckArea;
                    }

                }

                if (nhsDeckArea > 0)
                {
                    var percent = 100 * poorDeckArea / nhsDeckArea;
                    results.Add(percent.ToString("f2") + "%");
                }
                else
                {
                    results.Add(0 + "%");
                }
            }
            return results;
        }

        public List<string> GetPercentagePoorState()
        {
            var results = new List<string>();
            for (var year = 0; year < NumberYears; year++)
            {
                var poorCount = 0;
                foreach (var bridge in Bridges)
                {
                    if (bridge.AnnualStructuralDeficient[year].Sd <= 4)
                    {
                        poorCount++;
                    }
                }

                if (Bridges.Count > 0)
                {
                    var percent = 100 * (double)poorCount / Bridges.Count;
                    results.Add(percent.ToString("f2") + "%");
                }
                else
                {
                    results.Add(0 + "%");
                }
            }
            return results;
        }

        public List<string> GetPercentagePoorStateDeckArea()
        {
            var results = new List<string>();
            for (var year = 0; year < NumberYears; year++)
            {
                var poorDeckArea = 0d;
                var deckArea = 0d;
                foreach (var bridge in Bridges)
                {
                    if (bridge.AnnualStructuralDeficient[year].Sd <= 4 && (bridge.BusinessPlanNetwork == "1" || bridge.BusinessPlanNetwork == "2"))
                    {
                        poorDeckArea += bridge.DeckArea;
                    }


                    deckArea += bridge.DeckArea;

                }

                if (deckArea > 0)
                {
                    var percent = 100 * poorDeckArea / deckArea;
                    results.Add(percent.ToString("f2") + "%");
                }
                else
                {
                    results.Add(0 + "%");
                }
            }
            return results;
        }
        public List<int> GetSdCount(string businessPlanNetwork)
        {
            var results = new List<int>();
            for (var year = 0; year < NumberYears; year++)
            {
                var sdCount = 0;
                foreach (var bridge in Bridges)
                {
                    if (bridge.AnnualStructuralDeficient[year].Sd <= 4 &&
                        bridge.BusinessPlanNetwork == businessPlanNetwork)
                    {
                        sdCount++;
                    }
                }
                results.Add(sdCount);
            }
            return results;
        }


        public List<double> GetSdDeckArea(string businessPlanNetwork)
        {
            var results = new List<double>();
            for (var year = 0; year < NumberYears; year++)
            {
                var sdDeckArea = 0d;
                foreach (var bridge in Bridges)
                {
                    if (bridge.AnnualStructuralDeficient[year].Sd <= 4 &&
                        bridge.BusinessPlanNetwork == businessPlanNetwork)
                    {
                        sdDeckArea += bridge.DeckArea;
                    }
                }
                results.Add(sdDeckArea);
            }
            return results;
        }


        public List<int> GetTransition(double from, string businessPlanNetwork)
        {
            var results = new List<int>();

            for (var year = 0; year < NumberYears; year++)
            {
                var sdTransition = 0;
                foreach (var bridge in Bridges)
                {
                    if (bridge.BusinessPlanNetwork == businessPlanNetwork)
                    {
                        if (year == 0)
                        {
                            if (bridge.AnnualStructuralDeficient[bridge.AnnualStructuralDeficient.Count - 1].Sd >=
                                from &&
                                bridge.AnnualStructuralDeficient[year].Sd < from)
                            {
                                sdTransition++;
                            }
                        }
                        else
                        {
                            if (bridge.AnnualStructuralDeficient[year - 1].Sd >= from &&
                                bridge.AnnualStructuralDeficient[year].Sd < from)
                            {
                                sdTransition++;
                            }
                        }
                    }
                }
                results.Add(sdTransition);
            }
            return results;
        }

        public List<int> GetCountBetween(double high, double low, bool inclusive,bool isNhs)
        {
            var results = new List<int>();
            for (var year = 0; year < NumberYears; year++)
            {
                var count = 0;
                foreach (var bridge in Bridges)
                {
                    if(isNhs && (bridge.BusinessPlanNetwork=="1" || bridge.BusinessPlanNetwork == "2"))
                    { 
                        if (inclusive)
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd <= high &&
                                bridge.AnnualStructuralDeficient[year].Sd >= low)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd < high &&
                                bridge.AnnualStructuralDeficient[year].Sd > low)
                            {
                                count++;
                            }
                        }
                    }
                    else if (!isNhs && (bridge.BusinessPlanNetwork == "3" || bridge.BusinessPlanNetwork == "4"))
                    {
                        if (inclusive)
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd <= high &&
                                bridge.AnnualStructuralDeficient[year].Sd >= low)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd < high &&
                                bridge.AnnualStructuralDeficient[year].Sd > low)
                            {
                                count++;
                            }
                        }
                    }
                }
                results.Add(count);
            }
            return results;
        }


        public List<double> GetDeckAreaBetween(double high, double low, bool inclusive, bool isNhs)
        {
            var results = new List<double>();
            for (var year = 0; year < NumberYears; year++)
            {
                var deckArea = 0d;
                foreach (var bridge in Bridges)
                {
                    if (isNhs && (bridge.BusinessPlanNetwork == "1" || bridge.BusinessPlanNetwork == "2"))
                    {
                        if (inclusive)
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd <= high &&
                                bridge.AnnualStructuralDeficient[year].Sd >= low)
                            {
                                deckArea += bridge.DeckArea;
                            }
                        }
                        else
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd < high &&
                                bridge.AnnualStructuralDeficient[year].Sd > low)
                            {
                                deckArea += bridge.DeckArea;
                            }
                        }
                    }
                    else if (!isNhs && (bridge.BusinessPlanNetwork == "3" || bridge.BusinessPlanNetwork == "4"))
                    {
                        if (inclusive)
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd <= high &&
                                bridge.AnnualStructuralDeficient[year].Sd >= low)
                            {
                                deckArea += bridge.DeckArea;
                            }
                        }
                        else
                        {
                            if (bridge.AnnualStructuralDeficient[year].Sd < high &&
                                bridge.AnnualStructuralDeficient[year].Sd > low)
                            {
                                deckArea += bridge.DeckArea;
                            }
                        }
                    }
                }
                results.Add(deckArea);
            }
            return results;
        }

    }
}
