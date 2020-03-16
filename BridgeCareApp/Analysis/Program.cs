using System;
using System.Collections.Generic;
using AASHTOWare;

namespace ARA.iAM.Analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            ScenarioConfiguration configuration = null;//DataPersistence.GetConfigurationById(91);

            AnalysisResult result = configuration.Compiled.Run();

        }
    }

    interface ICompilable
    {
        public IEnumerable<string> Attributes { get; }
        public IEnumerable<CompiledCriterion> CompiledCriteria { get; }
        public IEnumerable<CompiledEquation> CompiledEquations { get; }
    }

    class Criterion
    {

    }

    class Equation
    {

    }

    class CompiledCriterion
    {

    }

    class CompiledEquation
    {

    }

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

        public CompiledAnalysis Compiled
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

    class AnalysisConfiguration: ICompilable
    {
        /// <summary>
        /// The equation used to calculate the AREA attribute for all assets
        /// </summary>
        private Equation areaEquation;
        /// <summary>
        /// The criterion for selecting which assets to include in the analysis
        /// </summary>
        private Criterion jurisdiction;
        /// <summary>
        /// The first year of the analysis
        /// </summary>
        private int startYear;
        /// <summary>
        /// The attribute used for ????
        /// </summary>
        private Option<string> weightingAttribute;
        /// <summary>
        /// The strategy used for judging which treatments are 'best'
        /// </summary>
        private OptimizationStrategy optimizationType; // Comparison function between asset-treatment pairs?
        /// <summary>
        /// The strategy used for deciding how much of the budget to spend
        /// </summary>
        private ExpenditureStrategy budgetType;
        /// <summary>
        /// The attribute by which treatments' benefits are determined
        /// </summary>
        private string benefitAttribute;
        /// <summary>
        /// The minimum value the benefit variable can have before it is considered to have no benefit
        /// </summary>
        private int benefitLimit;

        IEnumerable<string> ICompilable.Attributes { get => areaEquation.Attributes; }
        IEnumerable<Criterion> ICompilable.Criteria { get => throw new NotImplementedException();}
        IEnumerable<Equation> ICompilable.Equations { get => throw new NotImplementedException();}
    }

    class InvestmentConfiguration
    {

    }

    class PerformanceConfiguration
    {

    }

    class TreatmentConfiguration
    {

    }

    class PriorityConfiguration
    {

    }

    class TargetConfiguration
    {

    }

    class DeficientConfiguration
    {

    }

    class RemainingLifeConfiguration
    {

    }

    class CashFlowConfiguration
    {

    }

    class CompiledAnalysis
    {
        public AnalysisResult Run()
        {
            throw new NotImplementedException();
        }
    }

    class AnalysisResult
    {

    }
}
