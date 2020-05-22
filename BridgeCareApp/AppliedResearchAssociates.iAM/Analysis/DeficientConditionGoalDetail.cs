namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class DeficientConditionGoalDetail : ConditionGoalDetail
    {
        public double ActualDeficientPercentage { get; set; }

        public double AllowedDeficientPercentage { get; set; }

        public double DeficientLimit { get; set; }
    }
}
