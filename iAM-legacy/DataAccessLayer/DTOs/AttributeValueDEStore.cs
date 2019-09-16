using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class AttributeValueDEStore
    {
        string _omsObjectUserIDHierarchy;
        string _value;
        string _attributeField;
        DateTime _lastEntry;

        /// <summary>
        /// OMS ObjectUserID Hierarchy
        /// </summary>
        public string OmsObjectUserIDHierarchy
        {
            get { return _omsObjectUserIDHierarchy; }
            set { _omsObjectUserIDHierarchy = value; }
        }

        /// <summary>
        /// Value from the OMS database (null is OK)
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Date of last entry for this column
        /// </summary>
        public DateTime LastEntry
        {
            get { return _lastEntry; }
            set { _lastEntry = value; }
        }


        /// <summary>
        /// Attribute field added because not always equal to OmsObjectUserIDHierarchy
        /// </summary>
        public string AttributeField
        {
            get { return _attributeField; }
            set { _attributeField = value; }
        }


        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public AttributeValueDEStore()
        {
        }

        /// <summary>
        /// Object used by Decision engine to initialize for non ConditionIndex
        /// </summary>
        /// <param name="omsObjectUserIDHierarchy">OMS ObjectUserIDHierarchy</param>
        /// <param name="value">OMS data value for asset.property</param>
        /// <param name="lastEntry">The last date that this value was valid for.</param>
        public AttributeValueDEStore(string omsObjectUserIDHierarchy, string value, DateTime lastEntry, string attributeField)
        {
            _omsObjectUserIDHierarchy = omsObjectUserIDHierarchy;
            _value = value;
            _lastEntry = lastEntry;
            _attributeField = attributeField;
        }

        /// <summary>
        /// Object used by Decision engine to initialize for ConditionIndex
        /// </summary>
        /// <param name="omsObjectUserIDHierarchy">OMS ObjectUserIDHierarchy</param>
        /// <param name="value">OMS data value for asset.property</param>
        /// <param name="lastEntry">The last date that this value was valid for.</param>
        public AttributeValueDEStore(string omsObjectUserIDHierarchy, string value, DateTime lastEntry)
        {
            _omsObjectUserIDHierarchy = omsObjectUserIDHierarchy;
            _value = value;
            _lastEntry = lastEntry;
        }


        public override string ToString()
        {
            string value = _omsObjectUserIDHierarchy + "=";
            if (_value != null) value += _value;
            return value + "(" + _lastEntry.ToString() + ")" ;
        }
    }
}
