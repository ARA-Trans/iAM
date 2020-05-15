using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.SimulationOutput
{
    /// <summary>
    ///     Serialization-friendly DTO for capturing simulation output data.
    /// </summary>
    public sealed class SimulationYearDetail
    {
        public SimulationYearDetail(int year) => Year = year;

        public List<DeficientConditionGoalDetail> DetailsOfDeficientConditionGoals { get; } = new List<DeficientConditionGoalDetail>();

        public List<TargetConditionGoalDetail> DetailsOfTargetConditionGoals { get; } = new List<TargetConditionGoalDetail>();

        public List<SectionDetail> SectionDetails { get; } = new List<SectionDetail>();

        public int Year { get; }
    }
}
