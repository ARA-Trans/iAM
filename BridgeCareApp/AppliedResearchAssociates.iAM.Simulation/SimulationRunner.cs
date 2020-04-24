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

        private IReadOnlyCollection<Treatment> ActiveTreatments { get; set; }

        private ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; set; }

        private IDictionary<Section, ICollection<Project>> ProjectsPerSection { get; set; } // output only, I think.

        private IReadOnlyCollection<SectionContext> SectionContexts { get; set; }

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

        private double ApplyTreatment(SectionContext context, Treatment treatment, int year)
        {
            treatment.Consequences.Channel(
                consequence => consequence.Criterion.Evaluate(context),
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
                .Select(consequence => consequence.GetRecalculator(context, Simulation.AnalysisMethod.AgeAttribute))
                .ToArray();

            foreach (var consequenceAction in consequenceActions)
            {
                consequenceAction();
            }

            foreach (var scheduling in treatment.Schedulings)
            {
                var schedulingYear = year + scheduling.OffsetToFutureYear;
                context.ScheduledTreatmentPerYear[schedulingYear] = scheduling.Treatment;
            }

            context.LastYearOfShadowForAnyTreatment = year + treatment.ShadowForAnyTreatment;
            context.SetLastYearOfShadowForSameTreatment(treatment, year + treatment.ShadowForSameTreatment);

            // TODO: compute cost, use weighting.

            // TODO: create a project and add it to context.
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

        private ProjectionSummary GetProjection(SectionContext context, Treatment treatment, int initialYear)
        {
            Func<double, double> limitBenefit;
            switch (Simulation.AnalysisMethod.Benefit.Deterioration)
            {
            case Deterioration.Decreasing:
                limitBenefit = benefit => benefit - Simulation.AnalysisMethod.BenefitLimit;
                break;

            case Deterioration.Increasing:
                limitBenefit = benefit => Simulation.AnalysisMethod.BenefitLimit - benefit;
                break;

            default:
                throw InvalidDeterioration;
            }

            double getBenefit()
            {
                var rawBenefit = context.GetNumber(Simulation.AnalysisMethod.Benefit.Name);
                var limitedBenefit = limitBenefit(rawBenefit);
                var benefit = Math.Max(0, limitedBenefit);
                return benefit;
            }

            var summary = new ProjectionSummary
            {
                Cost = ApplyTreatment(context, treatment, initialYear),
                Benefit = getBenefit()
            };

            foreach (var year in Enumerable.Range(initialYear + 1, 100))
            {
                // TODO: Account for committed projects.

                ApplyPerformanceCurves(context);

                if (context.ScheduledTreatmentPerYear.TryGetValue(year, out var scheduledTreatment))
                {
                    var treatmentCost = ApplyTreatment(context, scheduledTreatment, year);
                    summary.Cost += treatmentCost;
                }

                var benefit = getBenefit();
                summary.Benefit += benefit;
            }

            return summary;
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
                        var baselineProjection = GetProjection(baselineContext, Simulation.DesignatedPassiveTreatment, currentYear);

                        foreach (var treatment in availableTreatments)
                        {
                            // TODO: Account for "number of years before..." restrictions.

                            var treatmentContext = new SectionContext(context);
                            var treatmentProjection = GetProjection(treatmentContext, treatment, currentYear);

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
