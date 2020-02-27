using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Simulation
    {
        public AnalysisMethod AnalysisMethod { get; }

        // ???
        public object Committed { get; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; }

        public Network Network { get; }

        public List<PerformanceEquation> PerformanceEquations { get; } = new List<PerformanceEquation>();

        public List<SimulationResult> Results { get; } = new List<SimulationResult>();

        public List<Treatment> Treatments { get; } = new List<Treatment>();

        public void Run()
        {
            #region "Compile"

            // fill OCI weights.

            // load attributes.

            // drop previous simulation.

            // get simulation method.

            // if conditional RSL, load conditional RSL.

            // get simulation attributes.

            #endregion "Compile"

            //FillSectionList();

            //FillCommittedProjects();

            //CalculateAreaOfEachSection();

            //UpdateSimulationAttributes();

            //SpendAllCommittedProjects();

            foreach (var year in Enumerable.Range(InvestmentPlan.FirstYearOfAnalysisPeriod, 1 + InvestmentPlan.NumberOfYearsInAnalysisPeriod))
            {
                // apply "deteriorate"/performance curves.

                // determine benefit/cost.

                // load & apply committed projects.

                // calculate network averages and "deficient base" (after committed).

                // either (a) spend as budget permits or (b) spend until targets/deficient met.

                // report targets/deficient.
            }

            // create simulation table.

            // "write simulation" for each section.

            // database bulk load for each "simulation table".

            // if multi-year, "solve". (?)

            throw new NotImplementedException();
        }
    }
}
