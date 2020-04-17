using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class SimulationRunner
    {
        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public void Run() => CompileSimulation();

        private readonly Simulation Simulation;

        private const string AGE = "age";

        private IEnumerable<Project> Projects { get; }

        private IEnumerable<Section> Sections { get; } // filtered by jurisdiction criterion

        private IReadOnlyDictionary<NumberAttribute, PerformanceCurve> AttributeCurves { get; set; } // must be "seeded" at the start of each run.

        private void ApplyDeterioration(int year)
        {
            foreach (var section in Sections)
            {
                var projectsForThisSection = Projects.Where(project => project.Section == section);

                var sectionShouldDeteriorate = projectsForThisSection.All(project => year < project.FirstYear || project.LastYear < year);

                if (sectionShouldDeteriorate)
                {
                    foreach (var equation in Simulation.PerformanceCurves)
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
                //ApplyDeterioration(year);
                //for each section:
                //  for each performance curve:
                //    if curve criterion is met:
                //      if another curve's criterion was met and that curve has the same target attribute:
                //        emit warning, take "worst" curve result (depends on attribute's deterioration direction).
                //      else:
                //        curve attribute value for current year = curve equation result
                foreach (var section in Simulation.Network.Sections)
                {
                    var data = SectionData[section];
                    data.Number[AGE] += 1;

                    var applicablePerformanceCurves = Simulation.PerformanceCurves
                        .Where(curve => curve.Criterion.Evaluate(data))
                        .GroupBy(curve => curve.Attribute)
                        .Select(curve => curve.Equation.Calculate(data))
                        .ToArray();
                    foreach (var curve in Simulation.PerformanceCurves)
                    {
                        if (curve.Criterion.Evaluate(SectionData[section]))
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
        }
    }
}
