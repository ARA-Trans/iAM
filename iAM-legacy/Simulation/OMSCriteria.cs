using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public static class  OMSCriteria
    {
        const string DATETIMEVALUE = "yyyymmddhhmmss";//This is unique string to replace date with so we can keep track of date in string manipulations.
        const string DATETIMEVARIABLE = "omscgdedatetimevariable";//This is unique string to replace date with so we can keep track of date in string manipulations.
        const string OMSVALUE = "omscgdevalue";
        const string OMSVARIABLE = "omscgdevariable";


        public static string GetDecisionEngineCritera(string omsCriteria)
        {
            omsCriteria = omsCriteria.Replace("[]", "cgde_timeperiodof");// Square brackets denote variables.  [] messes this up.
            string deCriteria = "";
            List<string> expressions = new List<string>();//List of omsExpressions

            int lastOpen = -1;
            int i = 0;
            while (i < omsCriteria.Length)//Find all expressions
            {
                if (omsCriteria[i] == '(')
                {
                    lastOpen = i;
                }
                if (omsCriteria[i] == ')' && lastOpen >= 0)
                {
                    string expression = omsCriteria.Substring(lastOpen, i - lastOpen + 1);
                    if (expression.Contains("["))
                    {
                        byte numberExpressions = (byte)expressions.Count;
                        string placeHolder = numberExpressions.ToString("x2");
                        expressions.Add(expression);
                        omsCriteria = omsCriteria.Replace(expression, placeHolder);
                        i = 0;
                    }
                    lastOpen = -1;
                }
                else
                {
                    i++;
                }
            }

            omsCriteria = omsCriteria.Replace("AND", "&&").Replace("OR", "||").Replace("NOT", "!");

            if (CheckMatchingParentheses(omsCriteria))
            {
                byte index = 0;
                deCriteria = omsCriteria;
                foreach (string expression in expressions)
                {
                    string variable = GetVariable(expression);
                    string fieldType = SimulationMessaging.GetAttributeType(variable);
                    if (fieldType != null)
                    {
                        string placeholder = index.ToString("x2");
                        string deExpression = ConvertToDecisionEngine(expression, fieldType);
                        deCriteria = deCriteria.Replace(placeholder, deExpression);
                    }
                    else
                    {
                        deCriteria = "Variable = " + variable + " not found.";
                        break;
                    }
                    index++;
                }
            }
            else
            {
                deCriteria = "Parentheses in input criteria do not match.";
            }
            return deCriteria;
        }




        public static string ConvertToDecisionEngine(string omsCriteria, string omsType)
        {
            string deCriteria = null;

            switch (omsType)
            {
                case "DATETIME":
                    deCriteria = ParseDateCriteria(omsCriteria);
                    break;
                case "STRING":
                    deCriteria = ParseTextCriteria(omsCriteria);
                    break;
                case "NUMBER":
                    deCriteria = ParseNumberCriteria(omsCriteria);
                    break;
                case "YesNo":
                    deCriteria = omsCriteria.Replace("=", "==");
                    break;
                case "Time":
                    break;

                default:
                    break;
            }

            return deCriteria;
        }

        #region DateParsing

        private static string ParseDateCriteria(string omsDateCriteria)
        {
            string deDateCriteria = null;

            string dateTimeValue = GetDate(omsDateCriteria);//Get datetime string from criteria.  Will be enclosed in #.  Example #12/15/2013#
            string dateTimeVariable = "[" + GetVariable(omsDateCriteria) + "]";//Get the date time variable.  Will be enclosed in [].  Example [Survey Date]

            omsDateCriteria = omsDateCriteria.Trim();//Remove leading and trailing white space because we are splitting on white space.
            if (dateTimeVariable != null)//Date will be null if unable to parse.  If dateTimeVariable is null, criteria cannot be parsed.
            {
                omsDateCriteria = omsDateCriteria.Replace(dateTimeVariable, DATETIMEVARIABLE);
                if (dateTimeValue != null)
                {
                    omsDateCriteria = omsDateCriteria.Replace("#" + dateTimeValue + "#", DATETIMEVALUE);
                }
                omsDateCriteria = RemoveDoubleWhiteSpace(omsDateCriteria); //Removes double white space which would effect parse.
                omsDateCriteria = omsDateCriteria.ToLower();//Parser only works on lower case letters.
                omsDateCriteria = omsDateCriteria.Replace("(", "").Replace(")", "");
                string[] parts = omsDateCriteria.Split(' ');
                if (parts.Length > 2) //parts[0] should always be DATETIMEVARIABLE, parts[1] should always be " is".  
                {
                    switch (parts[2])
                    {
                        case "after":
                            deDateCriteria = GetDateAfter(parts);  //We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "before":
                            deDateCriteria = GetDateBefore(parts);//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "from":
                            deDateCriteria = GetDateFrom(parts);//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "not":
                            deDateCriteria = GetDateNot(parts);//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "null":
                            deDateCriteria = DATETIMEVARIABLE + " == null";//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "on":
                            deDateCriteria = DATETIMEVARIABLE + ".Date == Convert.ToDateTime(\"" + DATETIMEVALUE + "\").Date";//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "through":
                            if (parts[3] == DATETIMEVALUE)
                            {
                                deDateCriteria = DATETIMEVARIABLE + ".Date <= Convert.ToDateTime(\"" + DATETIMEVALUE + "\").Date";//We do not need to check for DateTime null because we use MinValue instead of null
                            }
                            else if (parts[3] == "today")
                            {
                                deDateCriteria = DATETIMEVARIABLE + ".Date <= DateTime.Today.Date";//We do not need to check for DateTime null because we use MinValue instead of null
                            }
                            break;
                        case "today":
                            deDateCriteria = DATETIMEVARIABLE + ".Date == DateTime.Today.Date";//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "within":
                            deDateCriteria = GetDateWithin(parts, 3);//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        default:
                            deDateCriteria = null;
                            break;
                    }
                }
            }
            if (deDateCriteria != null)
            {
                deDateCriteria = deDateCriteria.Replace(DATETIMEVARIABLE, dateTimeVariable);
                if (dateTimeValue != null)
                {
                    deDateCriteria = deDateCriteria.Replace(DATETIMEVALUE, dateTimeValue);
                }
            }
            return deDateCriteria;
        }

        private static string GetDateAfter(string[] parts)
        {
            string deDateCriteria = null;
            parts = SubParts(parts, 3);
            switch (parts[0])
            {
                case DATETIMEVALUE:
                    deDateCriteria = DATETIMEVARIABLE + ".Date > Convert.ToDateTime(\"" + DATETIMEVALUE + "\").Date";
                    break;
                case "today":
                    deDateCriteria = DATETIMEVARIABLE + ".Date > Convert.ToDateTime(DateTime.Now).Date";
                    break;
                default:
                    deDateCriteria = DATETIMEVARIABLE + ".Date > " + GetPastOrFutureDate(parts);
                    break;
            }
            return deDateCriteria;
        }

        private static string GetDateBefore(string[] parts)
        {
            string deDateCriteria = null;
            parts = SubParts(parts, 3);
            switch (parts[0])
            {
                case DATETIMEVALUE:
                    deDateCriteria = DATETIMEVARIABLE + ".Date < Convert.ToDateTime(\"" + DATETIMEVALUE + "\").Date";
                    break;
                case "today":
                    deDateCriteria = DATETIMEVARIABLE + ".Date < Convert.ToDateTime(DateTime.Now).Date";
                    break;
                default:
                    deDateCriteria = DATETIMEVARIABLE + ".Date < " + GetPastOrFutureDate(parts);
                    break;
            }
            return deDateCriteria;
        }

        private static string GetDateFrom(string[] parts)
        {
            string deDateCriteria = null;
            parts = SubParts(parts, 3);
            switch (parts[0])
            {
                case DATETIMEVALUE:
                    deDateCriteria = DATETIMEVARIABLE + ".Date >= Convert.ToDateTime(\"" + DATETIMEVALUE + "\").Date";
                    break;
                case "today":
                    deDateCriteria = DATETIMEVARIABLE + " >= DateTime.Now.Date";
                    break;
            }
            return deDateCriteria;
        }

        private static string GetDateNot(string[] parts)
        {
            string deDateCriteria = null;
            parts = SubParts(parts, 3);

            switch (parts[0])
            {
                case "null":
                    //Removed for not including nulls for standalon
                    deDateCriteria = "0==1";// DATETIMEVARIABLE + " = null ";
                    break;
                case "today":
                    deDateCriteria = DATETIMEVARIABLE + ".Date != DateTime.Now.Date";
                    break;
                case "on":
                    deDateCriteria = DATETIMEVARIABLE + ".Date != Convert.ToDateTime(\"" + DATETIMEVALUE + "\").Date";
                    break;
                case "within":
                    deDateCriteria = "!(" + GetDateWithin(parts, 1) + ")";
                    break;
            }
            return deDateCriteria;
        }

        private static string GetDateWithin(string[] parts, int indexWithin)
        {
            string deDateCriteria = null;
            parts = SubParts(parts, indexWithin); // removes up to within,

            if (parts[0] == "last")
            {
                switch (parts[2])
                {
                    case "days":
                        deDateCriteria = DATETIMEVARIABLE + ".Date <= DateTime.Now.Date && " + DATETIMEVARIABLE + ".Date >= DateTime.Now.AddDays(-" + parts[3] + ").Date";
                        break;
                    case "months":
                        deDateCriteria = DATETIMEVARIABLE + ".Date <= DateTime.Now.Date && " + DATETIMEVARIABLE + ".Date >= DateTime.Now.AddMonths(-" + parts[3] + ").Date";
                        break;
                    case "years":
                        deDateCriteria = DATETIMEVARIABLE + ".Date <= DateTime.Now.Date && " + DATETIMEVARIABLE + ".Date >= DateTime.Now.AddYears(-" + parts[3] + ").Date";
                        break;
                }
            }
            else if (parts[0] == "next")
            {
                switch (parts[2])
                {
                    case "days":
                        deDateCriteria = DATETIMEVARIABLE + ".Date >= DateTime.Now.Date && " + DATETIMEVARIABLE + ".Date <= DateTime.Now.AddDays(" + parts[3] + ").Date";
                        break;
                    case "months":
                        deDateCriteria = DATETIMEVARIABLE + ".Date >= DateTime.Now.Date && " + DATETIMEVARIABLE + ".Date <= DateTime.Now.AddMonths(" + parts[3] + ").Date";
                        break;
                    case "years":
                        deDateCriteria = DATETIMEVARIABLE + ".Date >= DateTime.Now.Date && " + DATETIMEVARIABLE + ".Date <= DateTime.Now.AddYears(" + parts[3] + ").Date";
                        break;
                }

            }
            return deDateCriteria;
        }

        private static string GetPastOrFutureDate(string[] parts)
        {
            string futureDate = null;
            if (parts.Length == 4)
            {
                if (parts[1] == "days" && parts[2] == "ago")
                {
                    futureDate = "DateTime.Now.AddDays(-" + parts[3] + ").Date";
                }
                else if (parts[1] == "months" && parts[2] == "ago")
                {
                    futureDate = "DateTime.Now.AddMonths(-" + parts[3] + ").Date";
                }
                else if (parts[1] == "years" && parts[2] == "ago")
                {
                    futureDate = "DateTime.Now.AddYears(-" + parts[3] + ").Date";
                }
            }
            else if (parts.Length == 5)
            {
                if (parts[1] == "days" && parts[2] == "from" && parts[3] == "now")
                {
                    futureDate = "DateTime.Now.AddDays(" + parts[4] + ").Date";
                }
                else if (parts[1] == "months" && parts[2] == "from" && parts[3] == "now")
                {
                    futureDate = "DateTime.Now.AddMonths(" + parts[4] + ").Date";
                }
                else if (parts[1] == "years" && parts[2] == "from" && parts[3] == "now")
                {
                    futureDate = "DateTime.Now.AddYears(" + parts[4] + ").Date";
                }
            }
            return futureDate;
        }

        #endregion

        #region TextParsing

        private static string ParseTextCriteria(string omsTextCriteria)
        {
            string deTextCriteria = null;

            string textValue = GetText(omsTextCriteria);//Get Text string from criteria.  Will be enclosed in '.  Example "Champaign'
            string textVariable = "[" + GetVariable(omsTextCriteria) + "]";//Get the text variable.  Will be enclosed in [].  Example [Survey Date]

            omsTextCriteria = omsTextCriteria.Trim();//Remove leading and trailing white space because we are splitting on white space.
            if (textVariable != null)//Date will be null if unable to parse.  If dateTimeVariable is null, criteria cannot be parsed.
            {
                omsTextCriteria = omsTextCriteria.Replace(textVariable, OMSVARIABLE);
                if (textValue != null)
                {
                    textValue = "\"" + textValue + "\"";
                    omsTextCriteria = omsTextCriteria.Replace(textValue, OMSVALUE);
                }
                omsTextCriteria = RemoveDoubleWhiteSpace(omsTextCriteria); //Removes double white space which would effect parse.
                omsTextCriteria = omsTextCriteria.ToLower();//Parser only works on lower case letters.
                omsTextCriteria = omsTextCriteria.Replace("(", "").Replace(")", "");
                string[] parts = omsTextCriteria.Split(' ');
                if (parts.Length > 1) //parts[0] should always be TEXTVARIABLE  
                {
                    switch (parts[1])
                    {
                        case "contains":
                            deTextCriteria = OMSVARIABLE + " != null && " + OMSVARIABLE + ".Contains(" + OMSVALUE + ")";
                            break;
                        case "does":
                            deTextCriteria = TextDoesNot(parts);
                            break;
                        case "ends":
                            if (parts[2] == "with")
                            {
                                deTextCriteria = OMSVARIABLE + " != null && " + OMSVARIABLE + ".EndsWith(" + OMSVALUE + ")";
                            }
                            break;
                        case "is":
                            deTextCriteria = TextIs(parts);
                            break;
                        case "starts":
                            if (parts[2] == "with")
                            {
                                deTextCriteria = OMSVARIABLE + " != null && " + OMSVARIABLE + ".StartsWith(" + OMSVALUE + ")";
                            }
                            break;
                        default:
                            deTextCriteria = null;
                            break;
                    }
                }
            }
            if (deTextCriteria != null)
            {
                deTextCriteria = deTextCriteria.Replace(OMSVARIABLE, textVariable);
                if (textValue != null)
                {
                    deTextCriteria = deTextCriteria.Replace(OMSVALUE, textValue);
                }
            }
            return deTextCriteria;
        }

        private static string TextIs(string[] parts)
        {
            string deTextCriteria = null;
            parts = SubParts(parts, 2);// We now have a list after the 'is'
            switch (parts[0])
            {
                case "equal":
                    deTextCriteria = OMSVARIABLE + " == " + OMSVALUE + "";
                    break;
                case "greater":
                    deTextCriteria = "string.Compare(" + OMSVARIABLE + "," + OMSVALUE + ") > 0";
                    break;
                case "less":
                    deTextCriteria = OMSVARIABLE + " == null || string.Compare(" + OMSVARIABLE + "," + OMSVALUE + ") < 0";
                    break;
                case "not":
                    if (parts[1] == "null")
                    {
                        deTextCriteria = OMSVARIABLE + " != null";

                    }
                    else if (parts[1] == "equal")
                    {
                        deTextCriteria = OMSVARIABLE + " != " + OMSVALUE + "";
                    }
                    break;
                case "null":
                    deTextCriteria = OMSVARIABLE + " == null";
                    break;
            }

            return deTextCriteria;
        }

        private static string TextDoesNot(string[] parts)
        {
            string deTextCriteria = null;
            parts = SubParts(parts, 3);// We now have a list after the 'does not'
            switch (parts[0])
            {
                case "contain":
                    deTextCriteria = OMSVARIABLE + " != null &&  !(" + OMSVARIABLE + ".Contains(" + OMSVALUE + "))";
                    break;
                case "end":
                    if (parts[1] == "with")
                    {
                        deTextCriteria = OMSVARIABLE + " != null &&  !(" + OMSVARIABLE + ".EndsWith(" + OMSVALUE + "))";
                    }
                    break;
                case "start":
                    if (parts[1] == "with")
                    {
                        deTextCriteria = OMSVARIABLE + " != null &&  !(" + OMSVARIABLE + ".StartsWith(" + OMSVALUE + "))";
                    }
                    break;
            }

            return deTextCriteria;
        }
        #endregion

        #region NumberParsing

        private static string ParseNumberCriteria(string omsNumberCriteria)
        {
            string deNumberCriteria = null;

            string numberVariable = "[" + GetVariable(omsNumberCriteria) + "]";//Get the date time variable.  Will be enclosed in [].  Example [Survey Date]
            omsNumberCriteria = omsNumberCriteria.Trim();//Remove leading and trailing white space because we are splitting on white space.

            if (numberVariable != null)//  If numberVariable is null, criteria cannot be parsed.
            {
                omsNumberCriteria = omsNumberCriteria.Replace(numberVariable, OMSVARIABLE);
                omsNumberCriteria = RemoveDoubleWhiteSpace(omsNumberCriteria); //Removes double white space which would effect parse.
                omsNumberCriteria = omsNumberCriteria.ToLower();//Parser only works on lower case letters.
                omsNumberCriteria = omsNumberCriteria.Replace("(", "").Replace(")", "");
                string[] parts = omsNumberCriteria.Split(' ');
                if (parts[1] == "is")
                {
                    if (parts[2] == "null")
                    {
                        //To allow standalone roadcare to parse OMS
                        deNumberCriteria = OMSVARIABLE + " == null";
                    }
                    else if (parts[2] == "not") //not null
                    {
                        //To allow standalone roadcare to parse OMS
                        deNumberCriteria = OMSVARIABLE + " != null";
                    }
                }
                else if (parts[1] == "=")
                {
                    deNumberCriteria = omsNumberCriteria.Replace("=", "==");
                }
                else if (parts[1] == "<>")
                {
                    deNumberCriteria = omsNumberCriteria.Replace("<>", "!=");
                }
                else //Numbers do not requier extra parsing (unless Cartegraph wraps them in special string).  If so parse omsNumberValue above.
                {
                    deNumberCriteria = omsNumberCriteria;
                }

            }
            if (deNumberCriteria != null)
            {
                deNumberCriteria = deNumberCriteria.Replace(OMSVARIABLE, numberVariable);
            }
            return deNumberCriteria;
        }

        #endregion

        #region TimeParsing

        private static string ParseTimeCriteria(string omsTimeCriteria)
        {
            string deTimeCriteria = null;

            string dateTimeValue = GetDate(omsTimeCriteria);//Get datetime string from criteria.  Will be enclosed in #.  Example #12/15/2013#
            string dateTimeVariable = GetVariable(omsTimeCriteria);//Get the date time variable.  Will be enclosed in [].  Example [Survey Date]

            omsTimeCriteria = omsTimeCriteria.Trim();//Remove leading and trailing white space because we are splitting on white space.
            if (dateTimeVariable != null)//Date will be null if unable to parse.  If dateTimeVariable is null, criteria cannot be parsed.
            {
                omsTimeCriteria = omsTimeCriteria.Replace(dateTimeVariable, DATETIMEVARIABLE);
                if (dateTimeValue != null)
                {
                    dateTimeValue = "#" + dateTimeValue + "#";
                    omsTimeCriteria = omsTimeCriteria.Replace(dateTimeValue, DATETIMEVALUE);
                }
                omsTimeCriteria = RemoveDoubleWhiteSpace(omsTimeCriteria); //Removes double white space which would effect parse.
                omsTimeCriteria = omsTimeCriteria.ToLower();//Parser only works on lower case letters.

                string[] parts = omsTimeCriteria.Split(' ');
                if (parts.Length > 2) //parts[0] should always be DATETIMEVARIABLE, parts[1] should always be " is".  
                {
                    DateTime.Now.TimeOfDay.ToString();
                    switch (parts[2])
                    {
                        case "after":
                            if (parts.Length == 4)
                            {
                                deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay > Convert.ToDateTime(\"" + DATETIMEVALUE + "\").TimeOfDay";
                            }
                            else if (parts.Length == 6)//hours ago
                            {
                                deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay > Convert.ToDateTime(\"" + DATETIMEVALUE + "\").AddHours(-" + parts[4] + ").TimeOfDay";
                            }
                            else if (parts.Length == 7) //hours from now
                            {
                                deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay > Convert.ToDateTime(\"" + DATETIMEVALUE + "\").AddHours(" + parts[4] + ").TimeOfDay";
                            }
                            break;
                        case "before":

                            if (parts.Length == 4)
                            {
                                deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay < Convert.ToDateTime(\"" + DATETIMEVALUE + "\").TimeOfDay";
                            }
                            else if (parts.Length == 6)//hours ago
                            {
                                deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay < Convert.ToDateTime(\"" + DATETIMEVALUE + "\").AddHours(-" + parts[4] + ").TimeOfDay";
                            }
                            else if (parts.Length == 7) //hours from now
                            {
                                deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay < Convert.ToDateTime(\"" + DATETIMEVALUE + "\").AddHours(" + parts[4] + ").TimeOfDay";
                            }
                            break;
                        case "from":
                            deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay >= Convert.ToDateTime(\"" + DATETIMEVALUE + "\").TimeOfDay";
                            break;
                        case "not":
                            if (parts[3] == "null")
                            {   //To allow standalone roadcare to parse OMS
                                deTimeCriteria = "1==1";//DATETIMEVARIABLE + " != null";

                            }
                            else if (parts[3] == "now")
                            {
                                deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay != Convert.ToDateTime(\"" + DATETIMEVALUE + "\").TimeOfDay";
                            }
                            break;
                        case "null":
                            //To allow standalone roadcare to parse OMS
                            deTimeCriteria = DATETIMEVARIABLE + " == null ";//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        case "now":
                        case "during":
                            deTimeCriteria = DATETIMEVARIABLE + ".TimeOfDay == Convert.ToDateTime(\"" + DATETIMEVALUE + "\").TimeOfDay";//We do not need to check for DateTime null because we use MinValue instead of null
                            break;
                        default:
                            deTimeCriteria = null;
                            break;
                    }
                }
            }
            if (deTimeCriteria != null)
            {
                deTimeCriteria = deTimeCriteria.Replace(DATETIMEVARIABLE, dateTimeVariable);
                if (dateTimeValue != null)
                {
                    deTimeCriteria = deTimeCriteria.Replace(DATETIMEVALUE, dateTimeValue);
                }
            }
            return deTimeCriteria;
        }





        #endregion




        #region ParsingTools

        private static string RemoveDoubleWhiteSpace(string input)
        {
            while (input.Contains("  "))
            {
                input.Replace("  ", " ");
            }
            return input;
        }


        private static string[] SubParts(string[] parts, int indexStart)
        {
            int newLength = parts.Length - indexStart;
            string[] subparts = null;
            if (newLength > 0)
            {
                subparts = new string[newLength];
                for (int i = indexStart; i < parts.Length; i++)
                {
                    subparts[i - indexStart] = parts[i];
                }
            }
            return subparts;
        }

        private static string GetDate(string omsDateCriteria)
        {
            string dateTimeString = null;
            if (omsDateCriteria != null)
            {
                int indexFirst = omsDateCriteria.IndexOf('#');
                int indexLast = omsDateCriteria.LastIndexOf('#');
                if (indexFirst > 0 && indexLast > indexFirst)//Datetime # delimiters not found.  Return null for valid datetimeString
                {
                    dateTimeString = omsDateCriteria.Substring(indexFirst + 1, indexLast - indexFirst - 1);
                    try
                    {
                        DateTime valid_datetime = DateTime.Parse(dateTimeString);
                    }
                    catch
                    {
                        dateTimeString = null;//Datetime string is not valid if it does not parse.  Return a null for valid dateTimeString
                    }
                }
            }
            return dateTimeString;
        }

        private static string GetText(string omsTextCriteria)
        {
            string textString = null;
            if (omsTextCriteria != null)
            {
                int indexFirst = omsTextCriteria.IndexOf('\"');
                int indexLast = omsTextCriteria.LastIndexOf('\"');
                if (indexFirst > 0 && indexLast > indexFirst)//Text ' delimiters not found.  Return null 
                {
                    textString = omsTextCriteria.Substring(indexFirst + 1, indexLast - indexFirst - 1);
                }
            }
            return textString;
        }


        private static string GetVariable(string omsCriteria)
        {
            string dateTimeVariableString = null;
            if (omsCriteria != null)
            {
                int indexFirst = omsCriteria.IndexOf('[');
                int indexLast = omsCriteria.LastIndexOf(']');
                if (indexFirst > 0 && indexLast > indexFirst)//Datetime # delimiters not found.  Return null for valid datetimeString
                {
                    dateTimeVariableString = omsCriteria.Substring(indexFirst + 1, indexLast - indexFirst - 1);
                }
            }
            return dateTimeVariableString;
        }

        public static bool CheckMatchingParentheses(string criteria)
        {
            bool isMatchingParentheses = true;
            int nOpen = 0;
            for (int i = 0; i < criteria.Length; i++)
            {
                if (criteria[i] == '(')
                {
                    nOpen++;
                }
                if (criteria[i] == ')')
                {
                    nOpen--;
                }
                if (nOpen < 0)
                {
                    isMatchingParentheses = false;
                    break;
                }
            }

            if (nOpen != 0) isMatchingParentheses = false;
            return isMatchingParentheses;
        }
        #endregion

    }
}
