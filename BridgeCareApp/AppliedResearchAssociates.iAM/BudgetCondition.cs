namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetCondition
    {
        public Budget Budget { get; set; }

        public Criterion Criterion { get; } = new Criterion();
    }
}
