using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class SimulationRunner
    {
        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public void Run() => CompileSimulation();

        private readonly Simulation Simulation;

        private static Exception InvalidDeterioration => new InvalidOperationException("Invalid deterioration.");

        private ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; set; }

        private IDictionary<Section, ICollection<Project>> ProjectsPerSection { get; set; }

        private IReadOnlyCollection<SectionContext> SectionContexts { get; set; }

        private IReadOnlyCollection<Treatment> ActiveTreatments { get; set; }

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

                double calculate(PerformanceCurve curve) => curve.Equation.Compute(context, Simulation.AnalysisMethod.AgeAttribute);

                switch (curves.Key.Deterioration)
                {
                case Deterioration.Decreasing:
                    return operativeCurves.Min(calculate);

                case Deterioration.Increasing:
                    return operativeCurves.Max(calculate);

                default:
                    throw InvalidDeterioration;
                }
            });

            foreach (var (key, value) in dataUpdates)
            {
                context.SetNumber(key, value);
            }
        }

        private void CompileSimulation()
        {
            // fill OCI weights.

            ActiveTreatments = Simulation.GetActiveTreatments();

            CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);

            SectionContexts = Simulation.Network.Sections
                .Select(CreateContext)
                .Where(IsWithinJurisdiction)
                .ToArray();

            foreach (var context in SectionContexts)
            {
                //- fill in with ROLL-FORWARD data and committed projects.
            }

            // drop previous simulation.

            // get simulation method.

            // if conditional RSL, load conditional RSL.

            // get simulation attributes.

            RunSimulation();
        }

        private SectionContext CreateContext(Section section)
        {
            var context = new SectionContext(section);

            //- fill in with NON-ROLL-FORWARD data (per Gregg's memo, to apply jurisdiction criterion).

            foreach (var calculatedField in Simulation.Network.Explorer.CalculatedFields)
            {
                double calculate() => calculatedField.Calculate(context, Simulation.AnalysisMethod.AgeAttribute);
                context.SetNumber(calculatedField.Name, calculate);
            }

            return context;
        }

        private void DeteriorateSections(int year)
        {
            foreach (var context in SectionContexts)
            {
                // This needs to use context schedule.

                var projectsForThisSection = ProjectsPerSection[context.Section];
                var sectionShouldDeteriorate = projectsForThisSection.All(project => year < project.FirstYear || project.LastYear < year);

                if (sectionShouldDeteriorate)
                {
                    ApplyPerformanceCurves(context);
                }
            }
        }

        private double GetTotalBenefit(SectionContext context, int initialYear)
        {
            var result = Enumerable.Range(initialYear, 100).Sum(year =>
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

        private bool IsWithinJurisdiction(SectionContext context) => Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context) ?? true;

        private void RunSimulation()
        {
            //FillSectionList();

            //FillCommittedProjects();

            //CalculateAreaOfEachSection();

            //UpdateSimulationAttributes();

            //SpendAllCommittedProjects();

            foreach (var currentYear in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                DeteriorateSections(currentYear);

                determineBenefitCost();
                void determineBenefitCost()
                {
                    foreach (var context in SectionContexts)
                    {
                        var feasibleTreatments = ActiveTreatments
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

                        var baselineContext = new SectionContext(context);
                        var baselineBenefit = GetTotalBenefit(baselineContext, currentYear);

                        foreach (var treatment in availableTreatments)
                        {
                            // TODO: Account for "number of years before..." restrictions. And schedulings. And scheduled projects.

                            var treatmentContext = new SectionContext(context);

                            treatment.Consequences.Channel(
                                consequence => consequence.Criterion.Evaluate(treatmentContext),
                                result => result ?? false,
                                result => !result.HasValue,
                                out var applicableConsequences,
                                out var defaultConsequences);

                            var operativeConsequences = applicableConsequences.Count > 0 ? applicableConsequences : defaultConsequences;

                            operativeConsequences = operativeConsequences
                                .GroupBy(consequence => consequence.Attribute)
                                .Select(group => group.Single()) // It's (currently) an error when one attribute has multiple valid consequences.
                                .ToArray();

                            var consequenceActions = operativeConsequences
                                .Select(consequence => consequence.GetRecalculator(treatmentContext, Simulation.AnalysisMethod.AgeAttribute))
                                .ToArray();

                            foreach (var consequenceAction in consequenceActions)
                            {
                                consequenceAction();
                            }

                            var treatmentBenefit = GetTotalBenefit(treatmentContext, currentYear);
                            treatmentBenefit -= baselineBenefit;

                            var treatmentCost = 0d;

                            // switch on optimization strategy.
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
    }
}
