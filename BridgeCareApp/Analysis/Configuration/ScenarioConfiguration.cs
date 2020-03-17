using System;

namespace ARA.iAM.Analysis
{
    class ScenarioConfiguration
    {
        private AnalysisConfiguration analysisConfiguration;
        private InvestmentConfiguration investmentConfiguration;
        private PerformanceConfiguration performanceConfiguration;
        private TreatmentConfiguration treatmentConfiguration;
        private PriorityConfiguration priorityConfiguration;
        private TargetConfiguration targetConfiguration;
        private DeficientConfiguration deficientConfiguration;
        private RemainingLifeConfiguration remainingLifeConfiguration;
        private CashFlowConfiguration cashFlowConfiguration;

        public CompiledScenario Compiled
        {
            get
            {
                // compile all subconfigurations
                // compile area eq
                throw new NotImplementedException();
            }
            private set
            {
                throw new NotImplementedException();
            }
        }
    }
}
