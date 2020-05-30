using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using AppliedResearchAssociates.Validation;
using Microsoft.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Testing.CodeGeneration
{
    internal class Program
    {
        private const string COMMAND_TEXT = @"
select type_, attribute_, default_value, ascending, minimum_, maximum from attributes_ where calculated is null
;
select attribute_, equation, criteria from attributes_calculated
;
select facility, section, area, units from section_13
";

        private static readonly string CommandText = COMMAND_TEXT.Replace(Environment.NewLine, " ");

        private static void Main()
        {
            var timer = Stopwatch.StartNew();
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
                }
            }

            void fillSectionHistories()
            {
            }
        }
    }
}
