using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class SimulationRunner
    {
        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        private NumberAttribute AgeAttribute => Simulation.AnalysisMethod.AgeAttribute;

        public void Run() => CompileSimulation();

        private readonly Simulation Simulation;

        private static Exception InvalidDeterioration => new InvalidOperationException("Invalid deterioration.");

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

                double calculate(PerformanceCurve curve) => curve.Equation.Compute(context, AgeAttribute);

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
            // TODO: Needs to handle cashflow/split treatments/projects.

            context.TreatmentSchedule[year] = treatment;

            var totalCost = treatment.CostEquations
                .Where(costEquation => costEquation.Criterion.Evaluate(context) ?? true)
                .Sum(costEquation => costEquation.Equation.Compute(context, AgeAttribute));

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
                .Select(consequence => consequence.GetRecalculator(context, AgeAttribute))
                .ToArray();

            foreach (var consequenceAction in consequenceActions)
            {
                consequenceAction();
            }

            foreach (var scheduling in treatment.Schedulings)
            {
                var schedulingYear = year + scheduling.OffsetToFutureYear;
                context.TreatmentSchedule.Add(schedulingYear, scheduling.Treatment);

                // Unclear what is correct when two treatments write into the same schedule slot. Or
                // when one scheduled treatment is slotted to begin before the end of another.
            }

            context.LastYearOfShadowForAnyTreatment = year + treatment.ShadowForAnyTreatment;
            context.SetLastYearOfShadowForSameTreatment(treatment, year + treatment.ShadowForSameTreatment);

            return totalCost;
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
            var context = new SectionContext(section);

            //- fill in with NON-ROLL-FORWARD data (per Gregg's memo, to apply jurisdiction criterion).

            foreach (var calculatedField in Simulation.Network.Explorer.CalculatedFields)
            {
                double calculate() => calculatedField.Calculate(context, AgeAttribute);
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

                var weight = context.GetNumber(Simulation.AnalysisMethod.Weighting.Name);
                var weightedBenefit = weight * benefit;

                return weightedBenefit;
            }

            var outlookContext = new SectionContext(context);
            var outlook = new TreatmentOutlook(outlookContext)
            {
                TotalCost = ApplyTreatment(context, treatment, initialYear),
                TotalBenefit = getBenefit()
            };

            foreach (var year in Enumerable.Range(initialYear + 1, 100))
            {
                ApplyPerformanceCurves(context, year);

                if (context.TreatmentSchedule.TryGetValue(year, out var scheduledTreatment))
                {
                    var treatmentCost = ApplyTreatment(context, scheduledTreatment, year);
                    outlook.TotalCost += treatmentCost;
                }

                var benefit = getBenefit();
                outlook.TotalBenefit += benefit;
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
                        var feasibleTreatments = ActiveTreatments.Where(treatment => treatment.FeasibilityCriterion.Evaluate(context) ?? false).ToHashSet();

                        var supersededTreatments = feasibleTreatments
                            .SelectMany(treatment => treatment.Supersessions
                                .Where(supersession => supersession.Criterion.Evaluate(context) ?? false)
                                .Select(supersession => supersession.Treatment));

                        feasibleTreatments.ExceptWith(supersededTreatments);

                        var baselineOutlook = GetTreatmentOutlook(context, Simulation.DesignatedPassiveTreatment, currentYear);

                        foreach (var treatment in feasibleTreatments)
                        {
                            // TODO: Account for "number of years before..." restrictions.

                            var treatmentOutlook = GetTreatmentOutlook(context, treatment, currentYear);

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
