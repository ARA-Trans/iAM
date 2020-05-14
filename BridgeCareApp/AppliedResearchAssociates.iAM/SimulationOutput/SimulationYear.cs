using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.SimulationOutput
{
    public sealed class SimulationYear
    {
        public SimulationYear(int year) => Year = year;

        //TODO
        public object ConditionGoalsProgress { get; }

        public int Year { get; }

        public List<SectionDetail> DetailsOfSections { get; } = new List<SectionDetail>();
    }
}
