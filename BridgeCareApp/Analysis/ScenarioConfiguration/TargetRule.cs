using System.Collections.Generic;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    class TargetRule: ICriteriaDriven
    {
        public Criterion Criterion { get; }
        private string name;
        private string attribute;
        private int year;
        private double targetValue;
    }
}
