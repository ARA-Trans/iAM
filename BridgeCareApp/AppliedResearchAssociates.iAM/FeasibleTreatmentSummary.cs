namespace AppliedResearchAssociates.iAM
{
    public sealed class FeasibleTreatmentSummary
    {
        public FeasibleTreatmentSummary(double costPerUnitArea, double benefit, double? remainingLife)
        {
            CostPerUnitArea = costPerUnitArea;
            Benefit = benefit;
            RemainingLife = remainingLife;
        }

        public double Benefit { get; }

        public double CostPerUnitArea { get; }

        public double? RemainingLife { get; }
    }
}
