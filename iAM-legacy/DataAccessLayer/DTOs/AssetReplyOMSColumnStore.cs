using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Columns that make up the tables, that make up the reply to the DecisionEngine.
    /// </summary>
    public class AssetReplyOMSColumnStore
    {
        string _omsObjectUserIDHierarchy;
        string _value;
        string _attributeField;

        /// <summary>
        /// OMS ObjectUserIDHierarchy
        /// </summary>
        public string OmsObjectUserIDHierarchy
        {
            get { return _omsObjectUserIDHierarchy; }
            set { _omsObjectUserIDHierarchy = value; }
        }

        /// <summary>
        /// Value for an asset property (null allowed)
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string AttributeField
        {
            get { return _attributeField; }
            set { _attributeField = value; }
        }


        public AssetReplyOMSColumnStore()
        {

        }

        /// <summary>
        /// Return from the OMS database for a single asset
        /// </summary>
        /// <param name="omsObjectUserIDHierarchy">OMS Hierarchy UserObjectID</param>
        /// <param name="value">Value of the input (null allowed)</param>
        public AssetReplyOMSColumnStore(string omsObjectUserIDHierarchy, string value, string attributeField)
        {
            _omsObjectUserIDHierarchy = omsObjectUserIDHierarchy;
            _value = value;
            _attributeField = attributeField;
        }

        public override string ToString()
        {
            System.Diagnostics.Debug.WriteLine(_value);
            if (_value == null)
                return "null";
            else
                return _value;
        }
    }
}
