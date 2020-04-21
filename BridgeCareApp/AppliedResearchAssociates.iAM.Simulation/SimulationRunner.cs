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

        private IReadOnlyCollection<SectionContext> SectionContexts { get; set; }

        private ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; set; }

        private void CompileSimulation()
        {
            // fill OCI weights.

            loadAttributes();
            void loadAttributes()
            {
                CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);

                var sectionContexts = Simulation.Network.Sections.Select(section =>
                {
                    var data = new CalculateEvaluateArgument();

                    //- fill in with NON-ROLL-FORWARD data (per Gregg's memo, to apply jurisdiction criterion).

                    foreach (var calculatedField in Simulation.Network.Explorer.CalculatedFields)
                    {
                        data.SetNumber(calculatedField.Name, () => calculatedField.Calculate(data));
                    }

                    return new SectionContext(section, data);
                }).ToList();

                _ = sectionContexts.RemoveAll(context => !Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context.Data));

                foreach (var context in sectionContexts)
                {
                    //- fill in with ROLL-FORWARD data.
                }

                SectionContexts = sectionContexts;
            }

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

            foreach (var year in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                applyDeterioration();
                void applyDeterioration()
                {
                    foreach (var context in SectionContexts)
                    {
                        var section = context.Section;
                        var data = context.Data;

                        var projectsForThisSection = ProjectsPerSection[section];
                        var sectionShouldDeteriorate = projectsForThisSection.All(project => year < project.FirstYear || project.LastYear < year);
                        if (sectionShouldDeteriorate)
                        {
                            var dataUpdates = CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, curves =>
                            {
                                curves.Channel(
                                    curve => curve.Criterion.Evaluate(data),
                                    result => result ?? false,
                                    result => !result.HasValue,
                                    out var applicableCurves,
                                    out var defaultCurves);

                                var operativeCurves = applicableCurves.Count > 0 ? applicableCurves : defaultCurves;

                                if (operativeCurves.Count > 1)
                                {
                                    throw new NotImplementedException("A warning should be emitted when more than one curve is valid.");
                                }

                                switch (curves.Key.Deterioration)
                                {
                                case Deterioration.Decreasing:
                                    return operativeCurves.Min(curve => curve.Equation.Calculate(data));
                                case Deterioration.Increasing:
                                    return operativeCurves.Max(curve => curve.Equation.Calculate(data));
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
                }

                determineBenefitCost();
                void determineBenefitCost()
                {
                    //- collect all feasible treatments for this section.
                    //- collect all superseded treatments from these feasible treatments.
                    //- remove these superseded treatments from the feasible treatments.
                    //- for each remaining treatment, determine benefit & cost...
                }

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
