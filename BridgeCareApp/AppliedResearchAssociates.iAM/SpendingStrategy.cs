namespace AppliedResearchAssociates.iAM
{
    // [REVIEW] Might be worth applying a "GetBehavior extension method" pattern to this enum type,
    // to keep the serialization benefits of the enum while still allowing the cleanliness benefits
    // of abstraction.
    public enum SpendingStrategy
    {
        NoSpending,
        UnlimitedSpending,
        AsBudgetPermits,
        UntilTargetAndDeficientConditionGoalsMet,
        UntilTargetOrDeficientConditionGoalsMet,
        UntilTargetConditionGoalsMet,
        UntilDeficientConditionGoalsMet,
    }
}
