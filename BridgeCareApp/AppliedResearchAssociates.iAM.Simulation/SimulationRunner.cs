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

        private IReadOnlyDictionary<Section, ICollection<Project>> ProjectsPerSection { get; set; }

        private IReadOnlyCollection<(Section, CalculateEvaluateArgument)> SectionContexts { get; set; }

        private ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; set; }

        private void CompileSimulation()
        {
            // fill OCI weights.

            // load attributes.
            CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);
            var sectionContexts = Simulation.Network.Sections.Select(section =>
            {
                var data = new CalculateEvaluateArgument();
                //- fill in with rolled up data (?)
                //- fill in calculated field funcs.
                return (section, data);
            }).ToList();

            // [REVIEW] "jurisdiction" should only run on non-roll-forward data? per Gregg's memo
            _ = sectionContexts.RemoveAll(context => !Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context.data));
            SectionContexts = sectionContexts;

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

            foreach (var age in Enumerable.Range(1, Simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod))
            {
                var year = Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod + age - 1;

                // "apply deterioration"
                foreach (var (section, data) in SectionContexts)
                {
                    data.SetNumber(AGE, age);

                    var projectsForThisSection = ProjectsPerSection[section];
                    var sectionShouldDeteriorate = projectsForThisSection.All(project => year < project.FirstYear || project.LastYear < year);
                    if (sectionShouldDeteriorate)
                    {
                        var dataUpdates = CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, curves =>
                        {
                            var applicableCurves = curves.Where(curve => curve.Criterion.Evaluate(data)).ToArray();
                            if (applicableCurves.Length > 1)
                            {
                                throw new NotImplementedException("A warning should be emitted when more than one curve is valid.");
                            }

                            switch (curves.Key.Deterioration)
                            {
                            case Deterioration.Decreasing:
                                return applicableCurves.Min(curve => curve.Equation.Calculate(data));
                            case Deterioration.Increasing:
                                return applicableCurves.Max(curve => curve.Equation.Calculate(data));
                            default:
                                throw new InvalidOperationException("Invalid deterioration.");
                            }
                        });

                        foreach (var (key, value) in dataUpdates)
                        {
                            data.SetNumber(key, value);
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
