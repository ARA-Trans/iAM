namespace AppliedResearchAssociates.iAM
{
    public abstract class ConditionGoal
    {
        public NumberAttribute Attribute { get; set; }

        public Criterion Criterion { get; }

        public string Name { get; }

        public abstract bool IsMet(double actual);
    }
}
