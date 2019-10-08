using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using BridgeCare.EntityClasses;

namespace BridgeCare.DataAccessLayer
{
    public class Validation : IValidation
    {
        public void ValidateEquation(ValidateEquationModel data, BridgeCareContext db)
        {
            CalculateEvaluate calcEval = new CalculateEvaluate();

            if (data.IsPiecewise)
            {
                checkPiecewise(data.Equation);
            }
            else
            {
                string equation = data.Equation.Trim();
                equation = checkAttributes(equation, data.IsFunction, db);
                if (data.IsFunction)
                {
                    calcEval.BuildFunctionClass(equation, "double", null);
                }
                else
                {
                    calcEval.BuildTemporaryClass(equation, true);
                }
                calcEval.CompileAssembly();
            }
        }

        public string ValidateCriteria(string data, BridgeCareContext db)
        {
            string criteria = data.Replace("|", "'").ToUpper();
            criteria = checkAttributes(criteria, true, db);
            CalculateEvaluate calcEval = new CalculateEvaluate();
            calcEval.BuildClass(criteria, false);
            calcEval.CompileAssembly();

            return NumberOfHits(criteria, db);
        }

        private List<AttributesEntity> GetAllowedAttributes(bool isFunction, BridgeCareContext db)
        {
            if (isFunction)
            {
                return db.Attributes.ToList();
            }
            else
            {
                return db.Attributes.Where(e => e.Type_ == "NUMBER").ToList();
            }
        }

        private string checkAttributes(string target, bool isFunction, BridgeCareContext db)
        {
            List<AttributesEntity> attributes = GetAllowedAttributes(isFunction, db);
            target = target.Replace('[', '?');
            foreach (AttributesEntity allowedAttribute in attributes)
            {
                if (target.IndexOf("?" + allowedAttribute.ATTRIBUTE_ + "]") >= 0)
                {
                    if (allowedAttribute.Type_ == "STRING")
                    {
                        target = target.Replace("?" + allowedAttribute.ATTRIBUTE_ + "]", "[@" + allowedAttribute.ATTRIBUTE_ + "]");
                    }
                    else
                    {
                        target = target.Replace("?" + allowedAttribute.ATTRIBUTE_ + "]", "[" + allowedAttribute.ATTRIBUTE_ + "]");
                    }
                }
            }
            if (target.Count(f => f == '?') > 0)
            {
                int start = target.IndexOf('?');
                int end = target.IndexOf(']');

                throw new InvalidOperationException("Unsupported Attribute " + target.Substring(start + 1, end - 1));
            }
            return target;
        }

        private void checkPiecewise(string piecewise)
        {
            Dictionary<int, double> ageValues = new Dictionary<int, double>();
            piecewise = piecewise.Trim();
            if (piecewise.IndexOf("((") >= 0 || piecewise.IndexOf("))") >= 0)
            {
                throw new System.InvalidOperationException("Syntax error, enclose pairs in single parentheses'( )'. ");
            }
            if (piecewise.IndexOf(") ") >= 0 ||
                piecewise.IndexOf(" )") >= 0 ||
                piecewise.IndexOf(" (") >= 0 ||
                piecewise.IndexOf("( ") >= 0 ||
                piecewise.IndexOf(", ") >= 0 ||
                piecewise.IndexOf(" ,") >= 0)
            {
                throw new System.InvalidOperationException("Syntax error, remove spaces within array");
            }
            string[] pieces = piecewise.Split(new char[] { '(' });

            foreach (string piece in pieces)
            {
                if (piece.Length == 0)
                {
                    continue;
                }
                string commaDelimitedPair = piece.TrimEnd(')');
                string[] AgeValuePair = commaDelimitedPair.Split(',');
                int age = -1;
                double value = double.NaN;

                try
                {
                    age = Convert.ToInt32(AgeValuePair[0]);
                    value = Convert.ToDouble(AgeValuePair[1]);
                }
                catch
                {
                    throw new System.InvalidOperationException("Failure to convert AGE,VALUE pair to (int,double) :" + commaDelimitedPair);
                }

                if (age < 0)
                {
                    throw new System.InvalidOperationException("Values for [AGE] must be 0 or greater.");
                }

                if (!ageValues.ContainsKey(age))
                {
                    ageValues.Add(age, value);
                }
                else
                {
                    throw new System.InvalidOperationException("Only unique integer values for [AGE] are allowed.");
                }
            }
            if (ageValues.Count < 1)
            {
                throw new System.InvalidOperationException("At least one Age,Value pair must be entered.");
            }
            return;
        }

        public string NumberOfHits(string criteria, BridgeCareContext db)
        {
            // create the sql select statement
            var strNetworkID = db.NETWORKS.FirstOrDefault().NETWORKID.ToString();
            string strFrom = "FROM SECTION_" + strNetworkID + " INNER JOIN SEGMENT_" + strNetworkID + "_NS0 ON SECTION_" + strNetworkID + ".SECTIONID=SEGMENT_" + strNetworkID + "_NS0.SECTIONID";
            // Get Attributes
            List<AttributesEntity> attributes = GetAllowedAttributes(true, db);
            //oracle chokes on non-space whitespace
            Regex whiteSpaceMechanic = new Regex(@"\s+");
            // modify the critiera string replacing all special characters and white space
            var modifiedCriteria = whiteSpaceMechanic.Replace(criteria.Replace("[", "").Replace("]", "").Replace("@", ""), " ");
            // parameterize the criteria pedicate and add it to the select
            var parameterizedCriteriaData = parameterizeCriteria(modifiedCriteria, attributes);
            string strSelect = $"SELECT COUNT(*) {strFrom} WHERE {parameterizedCriteriaData.ParameterizedPredicatesString}";
            // create a sql connection
            var connection = new SqlConnection(db.Database.Connection.ConnectionString);
            try
            {
                // open the connection
                connection.Open();
                // create a sql command with the select string and the connection
                SqlCommand cmd = new SqlCommand(strSelect, connection);
                // add the command parameters
                cmd.Parameters.AddRange(parameterizedCriteriaData.SqlParameters.ToArray());
                // execute the query
                var dataReader = cmd.ExecuteReader();
                // get the returned count
                var count = dataReader.HasRows && dataReader.Read() ? dataReader.GetValue(0) : 0;
                // close the data reader
                dataReader.Close();
                // close the connection
                connection.Close();
                // return the results
                return count + " results match query";
            }
            catch (SqlException e)
            {
                throw new System.InvalidOperationException($"Failed SQL Query: {strSelect}, Error Message: {e.Message}");
            }
            catch (Exception e2)
            {
                throw new System.InvalidOperationException(e2.Message);
            }
        }

        public ParameterizedCriteriaData parameterizeCriteria(string criteria, List<AttributesEntity> attributes)
        {
            var parameterizedCriteriaPredicates = new List<string>();
            var sqlParameters = new List<SqlParameter>();
            var operators = new List<string>(){"<=", ">=", "<>", "=", "<", ">"};
            var operatorsRegex = new Regex(@"<=|>=|<>|=|<|>");
            var parameterCount = 0;
            var startingIndex = 0;
            List<string> predicates = new List<string>();
            var spacedString = 0;
            var indexForSpacedString = 0;

            while (startingIndex < criteria.Length)
            {
                var index = criteria.IndexOf(" ", startingIndex);

                if (index == -1)
                {
                    if (spacedString == 0)
                    {
                        var length = criteria.Length - startingIndex;
                        predicates.Add(criteria.Substring(startingIndex, length));
                    }
                    else
                    {
                        var lengthForCustomString = criteria.Length - indexForSpacedString;
                        predicates.Add(criteria.Substring(indexForSpacedString, lengthForCustomString));
                        spacedString = 0;
                    }
                    break;
                }

                if (criteria[index + 1] == '(' || criteria[index + 1] == '[')
                {
                    if (spacedString == 0)
                    {
                        var length = index - startingIndex;
                        predicates.Add(criteria.Substring(startingIndex, length));
                    }
                    else
                    {
                        var lengthForCustomString = index - indexForSpacedString;
                        predicates.Add(criteria.Substring(indexForSpacedString, lengthForCustomString));
                        spacedString = 0;
                    }
                    startingIndex = index + 1;
                }
                else if (criteria.Substring(index + 1, 3) == "AND" || criteria.Substring(index + 1, 2) == "OR")
                {
                    if (spacedString == 0)
                    {
                        var length = index - startingIndex;
                        predicates.Add(criteria.Substring(startingIndex, length));
                    }
                    else
                    {
                        var lengthForCustomString = index - indexForSpacedString;
                        predicates.Add(criteria.Substring(indexForSpacedString, lengthForCustomString));
                        spacedString = 0;
                    }
                    startingIndex = index + 1;
                }
                else
                {
                    if (spacedString == 0)
                    {
                        indexForSpacedString = startingIndex;
                        spacedString++;
                    }
                    startingIndex = index + 1;
                    continue;
                }
            }
            //var predicates = criteria.Split(' ');
            foreach (var predicate in predicates)
            {
                if (operatorsRegex.IsMatch(predicate))
                {
                    foreach (var @operator in operators)
                    {
                        if (predicate.Contains(@operator))
                        {
                            // count number of closed parentheses
                            var closedParenthesesCount = predicate.Count(x => x == ')');
                            // split the predicate at the operator
                            var splitPredicate = predicate.Split(new[] { @operator }, StringSplitOptions.None);
                            // create the parameter name
                            var parameterName = $"@value{++parameterCount}";
                            // create the parameter value
                            var value = splitPredicate[1].Replace(")", "").Replace("'", "");

                            var parameterizedPredicate = "";
                            if (!attributes.Exists(_ => _.ATTRIBUTE_ == value))
                            {
                                // create a parameterized predicate string
                                parameterizedPredicate = $"{splitPredicate[0]} {@operator} {parameterName}";
                            }
                            else
                            {
                                // create a parameterized predicate string
                                parameterizedPredicate = $"{splitPredicate[0]} {@operator} {value}";
                            }
                            // add a number of closed parentheses equal to closedParenthesesCount to end of parameterizedPredicate
                            if (closedParenthesesCount > 0)
                            {
                                for (int i = 0; i < closedParenthesesCount; i++)
                                {
                                    parameterizedPredicate += ")";
                                }
                            }
                            // add the parameterizedPredicate to the criteriaParameterizedPredicates list
                            parameterizedCriteriaPredicates.Add(parameterizedPredicate);
                            // create a new sql parameter with parameterName and value
                            sqlParameters.Add(new SqlParameter()
                            {
                                ParameterName = parameterName,
                                Value = value
                            });
                            break;
                        }
                    }
                }
                else
                {
                    parameterizedCriteriaPredicates.Add(predicate);
                }
            }

            return new ParameterizedCriteriaData(string.Join(" ", parameterizedCriteriaPredicates), sqlParameters);
        }
    }
}