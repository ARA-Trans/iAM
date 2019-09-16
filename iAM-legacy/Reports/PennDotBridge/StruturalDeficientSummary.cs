using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Reports.PennDotBridge
{
    public class StruturalDeficientSummary
    {
        public int Year { get; }
        public string BridgeType { get; }
        public double Sub { get; }
        public double Sup { get; }
        public double Deck { get; }

        public double Culv { get; }
        public double Sd
        {
            get
            {
                if (BridgeType == "C")
                {
                    return Culv;
                }
                if (Deck <= Sup && Deck <= Sub) return Deck;
                if (Sup <= Deck && Sup <= Sub) return Sup;
                return Sub;
            }
        }

        public StruturalDeficientSummary(int year, string bridgeType, double sub, double sup, double deck, double culv)
        {
            Year = year;
            BridgeType = bridgeType;
            Sub = sub;
            Sup = sup;
            Deck = deck;
            Culv = culv;
        }
    }
}
