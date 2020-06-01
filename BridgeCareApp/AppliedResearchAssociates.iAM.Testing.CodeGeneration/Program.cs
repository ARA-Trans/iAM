using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AppliedResearchAssociates.Validation;
using Microsoft.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Testing.CodeGeneration
{
    internal class Program
    {
        private static readonly string FormattedCommandText = @"
select type_, attribute_, default_value, ascending, minimum_, maximum from attributes_ where calculated is null
;
select attribute_, equation, criteria from attributes_calculated
;
select facility, section, area, units, sectionid from section_13
;
select * from segment_13_ns0
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

            using var connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=iAMBridgeCare;Integrated Security=True");
            using var command = new SqlCommand(CommandText, connection);
            connection.Open();
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            var simulation = new Explorer().AddNetwork().AddSimulation();
            var sectionById = new Dictionary<int, Section>();

            time(createRawAttributes);
            _ = reader.NextResult();
            time(createCalculatedFields);
            _ = reader.NextResult();
            time(createSections);
            _ = reader.NextResult();
            time(fillSectionHistories);

            foreach (var result in simulation.Network.Explorer.GetAllValidationResults())
            {
                Console.WriteLine($"[{result.Status}] {result.Message} :: {result.Target.Object}, {result.Target.Key}");
            }

            //---

            void createRawAttributes()
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
                        var numberAttribute = simulation.Network.Explorer.AddNumberAttribute(name);
                        numberAttribute.DefaultValue = double.Parse(reader.GetString(2));
                        numberAttribute.IsDecreasingWithDeterioration = reader.GetBoolean(3);
                        numberAttribute.Minimum = reader.GetNullableDouble(4);
                        numberAttribute.Maximum = reader.GetNullableDouble(5);
                        break;

                    case TYPE_STRING:
                        var textAttribute = simulation.Network.Explorer.AddTextAttribute(name);
                        textAttribute.DefaultValue = reader.GetNullableString(2);
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid attribute type \"{type}\".");
                    }
                }
            }

            void createCalculatedFields()
            {
                var calculatedFieldByName = new Dictionary<string, CalculatedField>(StringComparer.OrdinalIgnoreCase);

                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    if (!calculatedFieldByName.TryGetValue(name, out var calculatedField))
                    {
                        calculatedField = simulation.Network.Explorer.AddCalculatedField(name);
                        calculatedFieldByName.Add(name, calculatedField);
                    }

                    var source = calculatedField.AddValueSource();
                    source.Equation.Expression = reader.GetString(1);
                    source.Criterion.Expression = reader.GetNullableString(2);
                }
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
                var attributeNames = simulation.Network.Explorer.AllAttributes.Select(attribute => attribute.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var columnData = reader.GetColumnSchema()
                    .Where(column => !string.Equals(column.ColumnName, "sectionid", StringComparison.OrdinalIgnoreCase) && !attributeNames.Contains(column.ColumnName))
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
                    var sectionId = reader.GetInt32("sectionid");
                    var section = sectionById[sectionId];

                    fillAttributeHistories(simulation.Network.Explorer.NumberAttributes, reader.GetDouble);
                    fillAttributeHistories(simulation.Network.Explorer.TextAttributes, reader.GetString);

                    void fillAttributeHistories<T>(IEnumerable<Attribute<T>> attributes, Func<int, T> getValue)
                    {
                        foreach (var attribute in attributes)
                        {
                            var history = section.GetHistory(attribute);
                            foreach (var (columnOrdinal, year, _) in columnData[attribute.Name])
                            {
                                if (!reader.IsDBNull(columnOrdinal))
                                {
                                    var value = getValue(columnOrdinal);
                                    history.Add(year, value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
