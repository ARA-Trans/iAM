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

        public SimulationAnalysisModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID;
            StartYear = entity.COMMITTED_START;
            AnalysisPeriod = entity.COMMITTED_PERIOD;
            OptimizationType = entity.ANALYSIS;
            BudgetType = entity.BUDGET_CONSTRAINT;
            BenefitLimit = entity.BENEFIT_LIMIT;
            Description = entity.COMMENTS;
            Criteria = entity.JURISDICTION;
            BenefitAttribute = entity.BENEFIT_VARIABLE;
            WeightingAttribute = entity.WEIGHTING;
        }

        public void UpdateSimulationAnalysis(SimulationEntity entity)
        {
            entity.JURISDICTION = Criteria;
            entity.ANALYSIS = OptimizationType;
            entity.BENEFIT_LIMIT = BenefitLimit;
            entity.BUDGET_CONSTRAINT = BudgetType;
            entity.COMMENTS = Description;
            entity.COMMITTED_PERIOD = AnalysisPeriod;
            entity.COMMITTED_START = StartYear;
            entity.BENEFIT_VARIABLE = BenefitAttribute;
            entity.WEIGHTING = WeightingAttribute;
        }

        public void PartialUpdateSimulationAnalysis(SimulationEntity entity, bool updateWeighting)
        {
            entity.JURISDICTION = Criteria;
            entity.COMMENTS = Description;
            entity.COMMITTED_PERIOD = AnalysisPeriod;
            entity.COMMITTED_START = StartYear;
            entity.WEIGHTING = updateWeighting ? WeightingAttribute : entity.WEIGHTING;
        }
  }
}