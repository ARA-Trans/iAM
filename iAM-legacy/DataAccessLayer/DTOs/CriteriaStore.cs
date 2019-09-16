using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    public class CriteriaStore
    {
        string _attribute;
        bool _isEquality;
        bool _isString;
        bool _isDate;
        List<string> _listValues;
        double _minimum = double.NaN;
        double _maximum = double.NaN;
        DateTime _minimumDate = DateTime.MinValue;
        DateTime _maximumDate = DateTime.MinValue;

        [DataMember]
        public string Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }

        [DataMember]
        public bool IsString
        {
            get { return _isString; }
            set { _isString = value; }
        }

        [DataMember]
        public bool IsEquality
        {
            get { return _isEquality; }
            set { _isEquality = value; }
        }

        [DataMember]
        public bool IsDate
        {
            get { return _isDate; }
            set { _isDate = value; }
        }
        
        [DataMember]
        public List<string> Values
        {
            get { return _listValues; }
            set { _listValues = value; }

        }

        public CriteriaStore()
        {
            _listValues = new List<string>();
        }

        public CriteriaStore(string clause)
        {
            // All clauses only have one Attribute when they get here.  Store it.
            _listValues = new List<string>();
            _attribute = SimulationComponents.FindAttribute(clause, 0);

            //We remove all the attributes from clause.  This elimates AND and OR in the attributes.
            string bracket = "[" + _attribute + "]";
            clause = clause.Replace(bracket, "");

            //Search for <>.  If found IsEquality is false.
            if (clause.Contains("<>")) _isEquality = false;
            else
            {
                int gtIndex = clause.IndexOf(">=");
                int ltIndex = clause.IndexOf("<=");

                if (ltIndex < gtIndex)
                {
                    _isEquality = false;
                }
                else// both -1 (in case of only =)  or normal order
                {
                    _isEquality = true;
                }
            }
            //All strings have an ( in them
            if (!clause.Contains("(")) _isString = false;
            else
            {
                _isString = true;
                clause = clause.Replace("(", "").Replace(")", "");
            }
            //Remove all equality or inequality.

            clause = clause.Replace(">=","").Replace("<=","").Replace("=", "").Replace("<>","");
            
            //Replace AND and OR with & -> just for parsing

            clause = clause.Replace("AND", "&").Replace("OR", "&");

            string[] values = clause.Split('&');

            int index = 0;
            foreach(string value in values)
            {
                string clean = value.Replace("|", "").Trim();
                if (_isString)
                {
                    _listValues.Add(clean);
                }
                else if(!_isString && index == 0)
                {
                    if (DateTime.TryParse(clean, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out _minimumDate))
                    {
                        _isDate = true;
                        _listValues.Add(_minimumDate.ToString());
                    }
                    else
                    {
                        _minimum = Convert.ToDouble(clean);
                        _listValues.Add(_minimum.ToString());
                    }
                }
                else if (!_isString && index == 1)
                {
                    if (DateTime.TryParse(clean, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out _maximumDate))
                    {
                        _isDate = true;
                        _listValues.Add(_maximumDate.ToString());
                    }
                    else
                    {
                        _maximum = Convert.ToDouble(clean);
                        _listValues.Add(_maximum.ToString());
                    }
                }
                index++;
            }
        }



        public override string ToString()
        {
            string whereClause = "";
            string attribute = "[" + _attribute + "]";

            if (_isString)
            {
                whereClause += "(";
                foreach (string value in this.Values)
                {
                    if (whereClause.Length > 1)
                    {
                        if (_isEquality) whereClause += " OR ";
                        else whereClause += " AND ";
                    }
                    whereClause += attribute;
                    if (_isEquality) whereClause += "=";
                    else whereClause += "<>";

                    whereClause += "|" + value + "|";
                }
                whereClause += ")";
            }
            else
            {
                if (_isDate)
                {
                    if (_isEquality)
                        whereClause = attribute + ">=|" + _minimumDate.ToString() + "| AND " + attribute + "<=|" + _maximumDate.ToString() + "|";
                    else
                        whereClause = attribute + "<=|" + _minimumDate.ToString() + "| AND " + attribute + ">=|" + _maximumDate.ToString() + "|";
                }
                else
                {
                    if (_isEquality)
                        whereClause = attribute + ">=" + _minimum.ToString() + " AND " + attribute + "<=" + _maximum.ToString();
                    else
                        whereClause = attribute + "<=" + _minimum.ToString() + " AND " + attribute + ">=" + _maximum.ToString();
                }
            }
            return whereClause;
        }
    }
}
