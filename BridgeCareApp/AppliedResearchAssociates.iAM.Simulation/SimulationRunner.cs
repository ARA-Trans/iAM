using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class SimulationRunner
    {
        public void Run() => CompileSimulation();

        private readonly Simulation Simulation;

        public SimulationRunner(Simulation simulation)
        {
            Simulation = simulation ?? throw new System.ArgumentNullException(nameof(simulation));
        }

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
                    foreach (var equation in Simulation.PerformancCurves)
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

            foreach (var year in Enumerable.Range(
                Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod,
                1 + Simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod))
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
