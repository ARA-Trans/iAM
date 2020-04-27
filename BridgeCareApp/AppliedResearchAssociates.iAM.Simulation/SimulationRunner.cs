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

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments { get; set; }

        private ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; set; }

        private IReadOnlyCollection<SectionContext> SectionContexts { get; set; }

        private void ApplyPerformanceCurves(SectionContext context, int year)
        {
            if (context.YearHasOngoingTreatment(year))
            {
                return;
            }

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
                    throw new InvalidOperationException("Invalid deterioration.");
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
                .Where(context => Simulation.AnalysisMethod.JurisdictionCriterion.Evaluate(context) ?? true) // Can jurisdiction criterion be blank?
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
            var context = new SectionContext(section, Simulation);

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
                ApplyPerformanceCurves(context, year);
            }
        }

        private TreatmentOutlook GetTreatmentOutlook(SectionContext context, SelectableTreatment treatment, int initialYear)
        {
            var outlookContext = new SectionContext(context);
            var outlook = new TreatmentOutlook(outlookContext);

            outlook.ApplyTreatment(treatment, initialYear);
            outlook.AccumulateBenefit();

            foreach (var year in Enumerable.Range(initialYear + 1, 100)) // Should this be capped at the last formal year of analysis?
            {
                ApplyPerformanceCurves(context, year);

                if (context.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment))
                {
                    outlook.ApplyTreatment(scheduledTreatment, year);
                }

                outlook.AccumulateBenefit();
            }

            return outlook;
        }

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
                        var baselineOutlook = GetTreatmentOutlook(context, Simulation.DesignatedPassiveTreatment, currentYear);
                        var selectedOutlook = baselineOutlook;

                        if (!context.YearIsWithinShadowForAnyTreatment(currentYear))
                        {
                            var feasibleTreatments = ActiveTreatments.ToHashSet();

                            _ = feasibleTreatments.RemoveWhere(treatment => context.YearIsWithinShadowForSameTreatment(currentYear, treatment));
                            _ = feasibleTreatments.RemoveWhere(treatment => treatment.FeasibilityCriterion.Evaluate(context) ?? false);

                            var supersededTreatments = feasibleTreatments
                                .SelectMany(treatment => treatment.Supersessions
                                    .Where(supersession => supersession.Criterion.Evaluate(context) ?? false)
                                    .Select(supersession => supersession.Treatment));

                            feasibleTreatments.ExceptWith(supersededTreatments);

                            foreach (var treatment in feasibleTreatments)
                            {
                                // TODO: Account for "number of years before..." restrictions.

                                var treatmentOutlook = GetTreatmentOutlook(context, treatment, currentYear);

                                // switch on optimization strategy. re-assign selectedOutlook if this one's better.
                            }
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
