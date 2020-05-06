namespace AppliedResearchAssociates.iAM
{
    public sealed class DeficientConditionGoal : ConditionGoal
    {
        public double AllowedDeficientPercentage { get; }

        public double DeficientLevel { get; }

        public override bool IsMet(double actualDeficientPercentage) => actualDeficientPercentage <= AllowedDeficientPercentage;
    }
}
