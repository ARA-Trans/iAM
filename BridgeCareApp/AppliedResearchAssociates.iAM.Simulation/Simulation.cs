using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Simulation
    {
        public AnalysisMethod AnalysisMethod { get; }

        // ??? Some kind of result data, only accessible after a simulation is run.
        public object Committed { get; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; }

        public Network Network { get; }

        public List<PerformanceCurve> PerformanceEquations { get; }

        public List<SimulationResult> Results { get; }

        public List<Treatment> Treatments { get; }

        public void Run() => CompileSimulation();

        private IEnumerable<Project> Projects { get; }

        private IEnumerable<Section> Sections { get; }

        private void ApplyDeterioration(int year)
        {
            foreach (var section in Sections)
            {
                var projectsForThisSection = Projects.Where(project => project.Section == section);

                var sectionShouldDeteriorate = projectsForThisSection.All(project => year < project.FirstYear || project.LastYear < year);

                if (sectionShouldDeteriorate)
                {
                    foreach (var equation in PerformanceEquations)
                    {
                        section.ApplyDeteriorate(equation, year);
                    }
                }

                //section.ApplyNonDeteriorate
            }
        }

        private void CompileSimulation()
        {
            // fill OCI weights.

            // load attributes.

            // drop previous simulation.

            // get simulation method.

            // if conditional RSL, load conditional RSL.

            // get simulation attributes.

            RunSimulation();
        }

        private void RunSimulation()
        {
            //FillSectionList();

            //FillCommittedProjects();

            //CalculateAreaOfEachSection();

            //UpdateSimulationAttributes();

            //SpendAllCommittedProjects();

            foreach (var year in Enumerable.Range(InvestmentPlan.FirstYearOfAnalysisPeriod, 1 + InvestmentPlan.NumberOfYearsInAnalysisPeriod))
            {
                ApplyDeterioration(year);

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
        }
    }
}
