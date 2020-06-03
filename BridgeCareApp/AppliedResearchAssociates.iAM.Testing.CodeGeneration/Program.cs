using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using AppliedResearchAssociates.Validation;
using Microsoft.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Testing.CodeGeneration
{
    internal static class Program
    {
        private static int NetworkId = 13;

        private static int SimulationId = 91;

        private static string FormattedCommandText => $@"
select type_, attribute_, calculated, ascending, default_value, minimum_, maximum from attributes_
;
select attribute_, equation, criteria from attributes_calculated
;
select network_name from networks where networkid = {NetworkId}
;
select facility, section, area, units, sectionid from section_{NetworkId}
;
select * from segment_{NetworkId}_ns0
;
select simulation, jurisdiction, analysis, budget_constraint, weighting, benefit_variable, benefit_limit, use_cumulative_cost, use_across_budget from simulations where networkid = {NetworkId} and simulationid = {SimulationId}
;
select firstyear, numberyears, inflationrate, discountrate, budgetorder from investments where simulationid = {SimulationId}
;
select attribute_, equationname, criteria, equation, shift from performance where simulationid = {SimulationId}
;
select c.commitid, sectionid, years, treatmentname, yearsame, yearany, budget, cost_, attribute_, change_ from committed_ c join commit_consequences cc on c.commitid = cc.commitid where simulationid = {SimulationId}
;
select t.treatmentid, treatment, beforeany, beforesame, budget, description, attribute_, change_, equation, criteria from treatments t join consequences c on t.treatmentid = c.treatmentid where simulationid = {SimulationId}
;
select t.treatmentid, cost_, criteria from treatments t join costs c on t.treatmentid = c.treatmentid where simulationid = {SimulationId}
";

        private static string CommandText => FormattedCommandText.Replace(Environment.NewLine, " ");

        private static void Main()
        {
            var simulation = new Explorer().AddNetwork().AddSimulation();
            var sectionById = new Dictionary<int, Section>();
            var treatmentById = new Dictionary<int, SelectableTreatment>();

            using var connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=iAMBridgeCare;Integrated Security=True");
            using var command = new SqlCommand(CommandText, connection);
            connection.Open();
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            var timer = new Stopwatch();
            void time(Action action, string label)
            {
                timer.Restart();
                action();
                var elapsed = timer.Elapsed;
                Console.WriteLine($"{elapsed} --- {label}");
                _ = reader.NextResult();
            }

            time(createAttributes, nameof(createAttributes));
            time(fillCalculatedFields, nameof(fillCalculatedFields));
            time(fillNetwork, nameof(fillNetwork));
            time(createSections, nameof(createSections));
            time(fillSectionHistories, nameof(fillSectionHistories));
            time(fillAnalysisMethod, nameof(fillAnalysisMethod));
            time(fillInvestmentPlan, nameof(fillInvestmentPlan));
            time(createPerformanceCurves, nameof(createPerformanceCurves));
            time(createCommittedProjects, nameof(createCommittedProjects));
            time(createTreatmentsWithConsequences, nameof(createTreatmentsWithConsequences));
            //time(fillTreatmentCosts, nameof(fillTreatmentCosts));

            foreach (var result in simulation.Network.Explorer.GetAllValidationResults())
            {
                Console.WriteLine($"[{result.Status}] {result.Message} --- {result.Target.Object}::{result.Target.Key}");
            }

            //---

            void createAttributes()
            {
                const string TYPE_NUMBER = "NUMBER";
                const string TYPE_STRING = "STRING";

                while (reader.Read())
                {
                    var type = reader.GetString(0);
                    var name = reader.GetString(1);

                    if (name == simulation.Network.Explorer.AgeAttribute.Name)
                    {
                        if (type != TYPE_NUMBER)
                        {
                            throw new InvalidOperationException("Age attribute must be numeric.");
                        }

                        continue;
                    }

                    switch (type)
                    {
                    case TYPE_NUMBER:
                        var isCalculated = reader.GetNullableBoolean(2) ?? false;
                        if (isCalculated)
                        {
                            var calculatedField = simulation.Network.Explorer.AddCalculatedField(name);
                            calculatedField.IsDecreasingWithDeterioration = reader.GetBoolean(3);
                        }
                        else
                        {
                            var numberAttribute = simulation.Network.Explorer.AddNumberAttribute(name);
                            numberAttribute.IsDecreasingWithDeterioration = reader.GetBoolean(3);
                            numberAttribute.DefaultValue = double.Parse(reader.GetString(4));
                            numberAttribute.Minimum = reader.GetNullableDouble(5);
                            numberAttribute.Maximum = reader.GetNullableDouble(6);
                        }
                        break;

                    case TYPE_STRING:
                        var textAttribute = simulation.Network.Explorer.AddTextAttribute(name);
                        textAttribute.DefaultValue = reader.GetNullableString(4);
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid attribute type \"{type}\".");
                    }
                }
            }

            void fillCalculatedFields()
            {
                var calculatedFieldByName = simulation.Network.Explorer.CalculatedFields.ToDictionary(field => field.Name, StringComparer.OrdinalIgnoreCase);

                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    if (!calculatedFieldByName.TryGetValue(name, out var calculatedField))
                    {
                        throw new InvalidOperationException("Unknown calculated field.");
                    }

                    var source = calculatedField.AddValueSource();
                    source.Equation.Expression = reader.GetString(1);
                    source.Criterion.Expression = reader.GetNullableString(2);
                }
            }

            void fillNetwork()
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Invalid network ID.");
                }

                simulation.Network.Name = reader.GetString(0);
            }

            void createSections()
            {
                var facilityByName = new Dictionary<string, Facility>();

                while (reader.Read())
                {
                    var facilityName = reader.GetString(0);
                    if (!facilityByName.TryGetValue(facilityName, out var facility))
                    {
                        facility = simulation.Network.AddFacility();
                        facility.Name = facilityName;
                        facilityByName.Add(facilityName, facility);
                    }

                    var section = facility.AddSection();
                    section.Name = reader.GetString(1);
                    section.Area = reader.GetDouble(2);
                    section.AreaUnit = reader.GetString(3);

                    var sectionId = reader.GetInt32(4);
                    sectionById.Add(sectionId, section);
                }
            }

            void fillSectionHistories()
            {
                const string SECTIONID = "sectionid";

                var attributeNames = simulation.Network.Explorer.AllAttributes.Select(attribute => attribute.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var columnData = reader.GetColumnSchema()
                    .Where(column => !string.Equals(column.ColumnName, SECTIONID, StringComparison.OrdinalIgnoreCase) && !attributeNames.Contains(column.ColumnName))
                    .Select(column =>
                    {
                        var columnOrdinal = column.ColumnOrdinal.Value;
                        var yearSeparatorIndex = column.ColumnName.LastIndexOf('_');
                        var yearString = column.ColumnName.Substring(yearSeparatorIndex + 1);
                        var year = int.Parse(yearString);
                        var attributeName = column.ColumnName.Substring(0, yearSeparatorIndex);
                        return (columnOrdinal, year, attributeName);
                    })
                    .ToLookup(columnDatum => columnDatum.attributeName);

                while (reader.Read())
                {
                    var sectionId = reader.GetInt32(SECTIONID);
                    var section = sectionById[sectionId];

                    fillHistories(simulation.Network.Explorer.NumberAttributes, reader.GetDouble);
                    fillHistories(simulation.Network.Explorer.TextAttributes, reader.GetString);

                    void fillHistories<T>(IEnumerable<Attribute<T>> attributes, Func<int, T> getValue)
                    {
                        foreach (var attribute in attributes)
                        {
                            foreach (var (columnOrdinal, year, _) in columnData[attribute.Name])
                            {
                                if (!reader.IsDBNull(columnOrdinal))
                                {
                                    var value = getValue(columnOrdinal);
                                    section.GetHistory(attribute).Add(year, value);
                                }
                            }
                        }
                    }
                }
            }

            void fillAnalysisMethod()
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Invalid simulation ID.");
                }

                var attributeByName = simulation.Network.Explorer.NumericAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);

                simulation.Name = reader.GetString(0);
                simulation.AnalysisMethod.JurisdictionCriterion.Expression = reader.GetNullableString(1);

                var optimizationStrategyLabel = reader.GetString(2);
                simulation.AnalysisMethod.OptimizationStrategy = OptimizationStrategyLookup.Instance[optimizationStrategyLabel];

                var spendingStrategyLabel = reader.GetString(3);
                simulation.AnalysisMethod.SpendingStrategy = SpendingStrategyLookup.Instance[spendingStrategyLabel];

                var weightingName = reader.GetString(4);
                if (attributeByName.TryGetValue(weightingName, out var weighting))
                {
                    simulation.AnalysisMethod.Weighting = weighting;
                }

                var benefitName = reader.GetNullableString(5);
                if (attributeByName.TryGetValue(benefitName, out var benefit))
                {
                    simulation.AnalysisMethod.Benefit.Attribute = benefit;
                }

                simulation.AnalysisMethod.Benefit.Limit = reader.GetDouble(6);
                simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts = reader.GetNullableBoolean(7) ?? false;
                simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets = reader.GetNullableBoolean(8) ?? false;
            }

            void fillInvestmentPlan()
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Invalid simulation ID.");
                }

                simulation.InvestmentPlan.FirstYearOfAnalysisPeriod = reader.GetInt32(0);
                simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod = reader.GetInt32(1);
                simulation.InvestmentPlan.InflationRatePercentage = reader.GetDouble(2);
                simulation.InvestmentPlan.DiscountRatePercentage = reader.GetDouble(3);

                var budgetOrder = reader.GetString(4);
                var budgetNamesInOrder = budgetOrder.Split(',');
                foreach (var budgetName in budgetNamesInOrder)
                {
                    var budget = simulation.InvestmentPlan.AddBudget();
                    budget.Name = budgetName;
                }
            }

            void createPerformanceCurves()
            {
                var attributeByName = simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name);

                while (reader.Read())
                {
                    var curve = simulation.AddPerformanceCurve();
                    var attributeName = reader.GetString(0);
                    curve.Attribute = attributeByName[attributeName];
                    curve.Name = reader.GetString(1);
                    curve.Criterion.Expression = reader.GetString(2);
                    curve.Equation.Expression = reader.GetString(3);
                    curve.Shift = reader.GetBoolean(4);
                }
            }

            void createCommittedProjects()
            {
                var attributeByName = simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name);
                var budgetByName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name);
                var projectById = new Dictionary<int, CommittedProject>();

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    if (!projectById.TryGetValue(id, out var project))
                    {
                        var sectionId = reader.GetInt32(1);
                        var section = sectionById[sectionId];
                        var year = reader.GetInt32(2);
                        project = simulation.CommittedProjects.GetAdd(new CommittedProject(section, year));
                        project.Name = reader.GetString(3);
                        project.ShadowForSameTreatment = reader.GetInt32(4);
                        project.ShadowForAnyTreatment = reader.GetInt32(5);
                        var budgetName = reader.GetString(6);
                        project.Budget = budgetByName[budgetName];
                        project.Cost = reader.GetDouble(7);

                        projectById.Add(id, project);
                    }

                    var consequence = project.Consequences.GetAdd(new TreatmentConsequence());
                    var attributeName = reader.GetString(8);
                    consequence.Attribute = attributeByName[attributeName];
                    consequence.Change.Expression = reader.GetString(9);
                }
            }

            void createTreatmentsWithConsequences()
            {
                var attributeByName = simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name);
                var budgetByName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name, SelectionEqualityComparer<string>.Create(name => name.Trim()));

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    if (!treatmentById.TryGetValue(id, out var treatment))
                    {
                        treatment = simulation.AddTreatment();
                        treatment.Name = reader.GetString(1);
                        treatment.ShadowForAnyTreatment = reader.GetInt32(2);
                        treatment.ShadowForSameTreatment = reader.GetInt32(3);

                        var budgetField = reader.GetString(4);
                        var budgetNames = budgetField.Split(',');
                        foreach (var budgetName in budgetNames)
                        {
                            if (budgetByName.TryGetValue(budgetName, out var budget))
                            {
                                treatment.Budgets.Add(budget);
                            }
                        }

                        treatment.Description = reader.GetString(5);

                        treatmentById.Add(id, treatment);
                    }

                    var consequence = treatment.AddConsequence();
                    var attributeName = reader.GetString(6);
                    consequence.Attribute = attributeByName[attributeName];
                    consequence.Change.Expression = reader.GetNullableString(7);
                    consequence.Equation.Expression = reader.GetNullableString(8);
                    consequence.Criterion.Expression = reader.GetNullableString(9);
                }
            }

            void fillTreatmentCosts()
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var treatment = treatmentById[id];
                    var cost = treatment.AddCost();
                    cost.Equation.Expression = reader.GetString(1);
                    cost.Criterion.Expression = reader.GetString(2);
                }
            }
        }
    }
}
