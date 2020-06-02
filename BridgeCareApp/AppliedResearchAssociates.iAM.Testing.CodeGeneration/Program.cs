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
select simulation, jurisdiction, analysis, budget_constraint, weighting, benefit_variable, benefit_limit, committed_start, committed_period, use_cumulative_cost, use_across_budget from simulations where networkid = {NetworkId} and simulationid = {SimulationId}
";

        private static string CommandText => FormattedCommandText.Replace(Environment.NewLine, " ");

        private static void Main()
        {
            var timer = new Stopwatch();
            void time(Action action)
            {
                timer.Restart();
                action();
                Console.WriteLine(timer.Elapsed);
            }

            var simulation = new Explorer().AddNetwork().AddSimulation();
            var sectionById = new Dictionary<int, Section>();

            using var connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=iAMBridgeCare;Integrated Security=True");
            using var command = new SqlCommand(CommandText, connection);
            connection.Open();
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            time(createAttributes);
            _ = reader.NextResult();
            time(fillCalculatedFields);
            _ = reader.NextResult();
            time(fillNetwork);
            _ = reader.NextResult();
            time(createSections);
            _ = reader.NextResult();
            time(fillSectionHistories);
            _ = reader.NextResult();
            time(fillFromSimulationsTable);

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

            void fillFromSimulationsTable()
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
                simulation.InvestmentPlan.FirstYearOfAnalysisPeriod = reader.GetInt32(7);
                simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod = reader.GetInt32(8);
                simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts = reader.GetNullableBoolean(9) ?? false;
                simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets = reader.GetNullableBoolean(10) ?? false;
            }

            void fillFromInvestmentsTable()
            {

            }
        }
    }
}
