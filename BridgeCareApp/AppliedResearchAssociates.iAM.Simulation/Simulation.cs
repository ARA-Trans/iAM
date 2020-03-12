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

        public List<PerformanceEquation> PerformanceEquations { get; }

        public List<SimulationResult> Results { get; }

        public List<Treatment> Treatments { get; }

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
            var sections = Enumerable.Empty<Section>();

            //FillCommittedProjects();

            //CalculateAreaOfEachSection();

            //UpdateSimulationAttributes();

            //SpendAllCommittedProjects();

            foreach (var year in Enumerable.Range(InvestmentPlan.FirstYearOfAnalysisPeriod, 1 + InvestmentPlan.NumberOfYearsInAnalysisPeriod))
            {
                // apply "deteriorate"/performance curves.
                foreach (var section in sections)
                {
                    var projects = Enumerable.Empty<Project>();
                    foreach (var project in projects)
                    {
                        if (project.Section == section && /* project is multi-year && project is in progress re current year */)
                        {

                        }
                    }
                }

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
