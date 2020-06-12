using System;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class AttributesDAL : IAttributeRepo
    {
        /// <summary>
        /// Fetches all attributes data
        /// Throws a RowNotInTableException if no attributes are found
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>AttributeModel list</returns>
        public List<AttributeModel> GetAttributes(BridgeCareContext db)
        {
            if (!db.Attributes.Any())
            {
                var log = log4net.LogManager.GetLogger(typeof(AttributesDAL));
                log.Error("No attribute data could be found.");
                throw new RowNotInTableException("No attribute data could be found.");
            }

            return db.Attributes.ToList().Select(a => new AttributeModel(a)).ToList();
        }

        public List<AttributeSelectValuesResult> GetAttributeSelectValues(NetworkAttributes model, BridgeCareContext db)
        {
            var attributeSelectValuesResults = new List<AttributeSelectValuesResult>();

            if (!db.NETWORKS.Any(network => network.NETWORKID == model.NetworkId))
                throw new RowNotInTableException($"Could not find network with id {model.NetworkId}");

            var validAttributes = GetAttributes(db).Select(m => m.Name)
                .Where(attribute => model.Attributes.Contains(attribute)).ToList();

            if (!validAttributes.Any())
                throw new RowNotInTableException($"Provided attributes are not valid");

            var attributesWithValues = new List<string>();

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString);
            sqlConnection.Open();

            var attributeCountSelects = validAttributes
                .Select(attribute => $"COUNT(DISTINCT({attribute})) AS {attribute}").ToList();
            var countsQuery = $"SELECT {string.Join(", ", attributeCountSelects)} FROM SEGMENT_{model.NetworkId}_NS0;";

            var sqlCommand = new SqlCommand(countsQuery, sqlConnection);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (var i = 0; i < validAttributes.Count; i++)
                    {
                        if (reader.GetInt32(i) > 100)
                        {
                            attributeSelectValuesResults.Add(new AttributeSelectValuesResult
                            {
                                Attribute = validAttributes[i],
                                Values = new List<string>(),
                                ResultMessage = $"Number of values for attribute {validAttributes[i]} exceeds 100; use text input"
                            });
                        }
                        else if (reader.GetInt32(i) == 0)
                        {
                            attributeSelectValuesResults.Add(new AttributeSelectValuesResult
                            {
                                Attribute = validAttributes[i],
                                Values = new List<string>(),
                                ResultMessage = $"No values found for attribute {validAttributes[i]}; use text input"
                            });
                        }
                        else
                        {
                            attributesWithValues.Add(validAttributes[i]);
                        }
                    }
                }
            }

            if (attributesWithValues.Any())
            {
                var multipleAttributeValuesSelectQueries = new List<string>();
                attributesWithValues.ForEach(attribute =>
                {
                    multipleAttributeValuesSelectQueries.Add($"SELECT DISTINCT(CAST({attribute} AS VARCHAR(255))) AS {attribute} FROM SEGMENT_{model.NetworkId}_NS0;");
                });

                sqlCommand = new SqlCommand(string.Join("", multipleAttributeValuesSelectQueries), sqlConnection);

                var index = 0;

                reader = sqlCommand.ExecuteReader();
                while (reader.HasRows)
                {
                    var values = new List<string>();

                    while (reader.Read())
                    {
                        values.Add(reader.GetString(0));
                    }

                    attributeSelectValuesResults.Add(new AttributeSelectValuesResult
                    {
                        Attribute = attributesWithValues[index],
                        Values = values,
                        ResultMessage = "Success"
                    });

                    index++;

                    reader.NextResult();
                }
            }

            sqlCommand.Dispose();
            sqlConnection.Close();

            return attributeSelectValuesResults;
        }
    }
}
