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

        public String NumberOfHits(String criteria, BridgeCareContext db)
        {
            var strNetworkID = db.NETWORKS.FirstOrDefault().NETWORKID.ToString();
            String strFrom = " FROM SECTION_" + strNetworkID + " INNER JOIN SEGMENT_" + strNetworkID + "_NS0 ON SECTION_" + strNetworkID + ".SECTIONID=SEGMENT_" + strNetworkID + "_NS0.SECTIONID";

            string strWhere = criteria.Replace("[", "");
            strWhere = strWhere.Replace("]", "");
            strWhere = strWhere.Replace("@", "");

            //oracle chokes on non-space whitespace
            Regex whiteSpaceMechanic = new Regex(@"\s+");
            strWhere = whiteSpaceMechanic.Replace(strWhere, " ");

            String strSelect = "SELECT COUNT(*)" + strFrom;
            strSelect += " WHERE ";
            strSelect += strWhere;

            var connection = new SqlConnection(db.Database.Connection.ConnectionString);
            try
            {
                SqlCommand cmd = new SqlCommand(strSelect, connection);
                connection.Open();
                string count = cmd.ExecuteScalar().ToString();
                connection.Close();
                return count + " results match query";
            }
            catch (SqlException e)
            {
                throw new System.InvalidOperationException(e.Message);
            }
            catch (Exception e2)
            {
                throw new System.InvalidOperationException(e2.Message);
            }
            catch
            {
                throw new System.InvalidOperationException("Failed SQL Query:" + strSelect);
            }
        }
    }
}