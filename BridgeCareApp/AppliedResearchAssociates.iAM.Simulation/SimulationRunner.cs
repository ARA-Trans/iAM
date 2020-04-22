using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using MathNet.Numerics.Integration;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class SimulationRunner
    {
        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public void Run() => CompileSimulation();

        private readonly Simulation Simulation;

        private IDictionary<Section, ICollection<Project>> ProjectsPerSection { get; set; }

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
                    var context = new SectionContext(section);

                    //- fill in with NON-ROLL-FORWARD data (per Gregg's memo, to apply jurisdiction criterion).

                    foreach (var calculatedField in Simulation.Network.Explorer.CalculatedFields)
                    {
                        context.SetNumber(calculatedField.Name, () => calculatedField.Calculate(context));
                    }

                    return context;
                }).ToList();

                _ = sectionContexts.RemoveAll(context => !Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context));

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
                        var projectsForThisSection = ProjectsPerSection[context.Section];
                        var sectionShouldDeteriorate = projectsForThisSection.All(project => year < project.FirstYear || project.LastYear < year);
                        if (sectionShouldDeteriorate)
                        {
                            ApplyPerformanceCurves(context);
                        }
                    }
                }

                determineBenefitCost();
                void determineBenefitCost()
                {
                    foreach (var context in SectionContexts)
                    {
                        var feasibleTreatments = Simulation.Treatments
                            .Where(treatment => treatment.FeasibilityCriterion.Evaluate(context) ?? false)
                            .ToArray();

                        var supersededTreatments = feasibleTreatments
                            .SelectMany(treatment => treatment.Supersessions
                                .Where(supersession => supersession.Criterion.Evaluate(context) ?? false)
                                .Select(supersession => supersession.Treatment))
                            .ToArray();

                        var availableTreatments = feasibleTreatments
                            .Except(supersededTreatments)
                            .ToArray();

                        var baselineContext = new SectionContext(context.Section);
                        context.CopyTo(baselineContext);

                        var baselineBenefit = GetTotalBenefit(baselineContext);

                        foreach (var treatment in availableTreatments)
                        {
                            var treatmentContext = new SectionContext(context.Section);
                            context.CopyTo(treatmentContext);

                            // apply consequences.

                            var treatmentBenefit = GetTotalBenefit(treatmentContext);
                            treatmentBenefit -= baselineBenefit;

                            var treatmentCost = 0d;
                        }
                    }
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

        private double GetTotalBenefit(SectionContext context)
        {
            var result = Enumerable.Range(default, 100).Sum(delegate
            {
                // TODO: Account for scheduled and committed projects.
                ApplyPerformanceCurves(context);

                var benefit = context.GetNumber(Simulation.AnalysisMethod.Benefit.Name);

                double limitedBenefit;
                switch (Simulation.AnalysisMethod.Benefit.Deterioration)
                {
                case Deterioration.Decreasing:
                    limitedBenefit = benefit - Simulation.AnalysisMethod.BenefitLimit;
                    break;
                case Deterioration.Increasing:
                    limitedBenefit = Simulation.AnalysisMethod.BenefitLimit - benefit;
                    break;
                default:
                    throw InvalidDeterioration;
                }

                return Math.Max(0, limitedBenefit);
            });

            return result;
        }

        private void ApplyPerformanceCurves(SectionContext context)
        {
            var dataUpdates = CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, curves =>
            {
                curves.Channel(
                    curve => curve.Criterion.Evaluate(context),
                    result => result ?? false,
                    result => !result.HasValue,
                    out var applicableCurves,
                    out var defaultCurves);

                var operativeCurves = applicableCurves.Count > 0 ? applicableCurves : defaultCurves;

                if (operativeCurves.Count > 1)
                {
                    // TODO: A warning should be emitted when more than one curve is valid.
                }

                switch (curves.Key.Deterioration)
                {
                case Deterioration.Decreasing:
                    return operativeCurves.Min(curve => curve.Equation.Calculate(context));
                case Deterioration.Increasing:
                    return operativeCurves.Max(curve => curve.Equation.Calculate(context));
                default:
                    throw InvalidDeterioration;
                }
            });

            foreach (var (key, value) in dataUpdates)
            {
                context.SetNumber(key, value);
            }
        }

        private static Exception InvalidDeterioration => new InvalidOperationException("Invalid deterioration.");
    }
}
