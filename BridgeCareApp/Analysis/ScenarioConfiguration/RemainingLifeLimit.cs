using System.Collections.Generic;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    class RemainingLifeLimit: ICriteriaDriven
    {
        public Criterion Criterion { get; }
        private string attribute;
        private double limit;
    }
}
