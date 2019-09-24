using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class SimulationAnalysisModel
    {
        public int Id { get; set; }
        public int StartYear { get; set; }
        public int AnalysisPeriod { get; set; }
        public string OptimizationType { get; set; }
        public string BudgetType { get; set; }
        public double BenefitLimit { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
        public string BenefitAttribute { get; set; }
        public string WeightingAttribute { get; set; }

        public SimulationAnalysisModel() { }

        public SimulationAnalysisModel(SimulationEntity simulationEntity)
        {
            Id = simulationEntity.SIMULATIONID;
            StartYear = simulationEntity.COMMITTED_START;
            AnalysisPeriod = simulationEntity.COMMITTED_PERIOD;
            OptimizationType = simulationEntity.ANALYSIS;
            BudgetType = simulationEntity.BUDGET_CONSTRAINT;
            BenefitLimit = simulationEntity.BENEFIT_LIMIT;
            Description = simulationEntity.COMMENTS;
            Criteria = simulationEntity.JURISDICTION;
            BenefitAttribute = simulationEntity.BENEFIT_VARIABLE;
            WeightingAttribute = simulationEntity.WEIGHTING;
        }

        public void UpdateSimulationAnalysis(SimulationEntity simulationEntity)
        {
            simulationEntity.JURISDICTION = Criteria;
            simulationEntity.ANALYSIS = OptimizationType;
            simulationEntity.BENEFIT_LIMIT = BenefitLimit;
            simulationEntity.BUDGET_CONSTRAINT = BudgetType;
            simulationEntity.COMMENTS = Description;
            simulationEntity.COMMITTED_PERIOD = AnalysisPeriod;
            simulationEntity.COMMITTED_START = StartYear;
            simulationEntity.BENEFIT_VARIABLE = BenefitAttribute;
            simulationEntity.WEIGHTING = WeightingAttribute;
        }
  }
}