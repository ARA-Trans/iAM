using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    public class AttributeStore
    {
        private string _assetType;
        private string _omsTable;
        private string _attributeField;
        private List<string> _lookupValues;
        private string _initialValue;
        private string _displayName;
        private string _description;
        private string _format;
        private string _omsDisplayNameHierarchy;
        private string _omsObjectUserIDHierarchy;
        private string _omsOIDHierarchy;
        private string _unit;
        private string _lookupOID;
        private bool _isConditionCategory = false;
        private float _minimum = float.MinValue;
        private float _maximum = float.MaxValue;
        private bool _ascending = true;
        private string _fieldType;
        private string _OID;
        private ErrorStore _error;


        #region Properties
        [DataMember(Order = 0)]
        public string OID
        {
            get { return _OID; }
            set { _OID = value; }
        }


        [DataMember(Order=1)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [DataMember(Order = 2)]
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        [DataMember(Order = 3)]
        public string InitialValue
        {
            get { return _initialValue; }
            set { _initialValue = value; }
        }

        [DataMember(Order = 5)]
        public List<string> LookupValues
        {
            get { return _lookupValues; }
            set { _lookupValues = value; }
        }

        [DataMember(Order = 6)]
        public string OmsTable
        {
            get { return _omsTable; }
            set { _omsTable = value; }
        }

        [DataMember(Order = 7)]
        public string AttributeField
        {
            get { return _attributeField; }
            set { _attributeField = value; }
        }

        [DataMember(Order = 8)]
        public string AssetType
        {
            get { return _assetType; }
            set { _assetType = value; }
        }

        [DataMember(Order = 9)]
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        [DataMember(Order = 10)]
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        [DataMember(Order = 11)]
        public string OmsHierarchy
        {
            get { return _omsDisplayNameHierarchy; }
            set { _omsDisplayNameHierarchy = value; }
        }

        [DataMember(Order = 12)]
        public string LookupOID
        {
            get { return _lookupOID; }
            set { _lookupOID = value; }
        }

        [DataMember(Order = 13)]
        public string OmsObjectUserIDHierarchy
        {
            get { return _omsObjectUserIDHierarchy; }
            set { _omsObjectUserIDHierarchy = value; }
        }

        [DataMember(Order = 14)]
        public bool IsConditionCategory
        {
            get { return _isConditionCategory; }
            set { _isConditionCategory = value; }
        }
       
        [DataMember(Order = 15)]
        public float Minimum
        {
            get { return _minimum; }
            set { _minimum = value; }
        }
        
        [DataMember(Order = 16)]
        public float Maximum
        {
            get { return _maximum; }
            set { _maximum = value; }
        }

        [DataMember(Order = 17)]
        public bool Ascending
        {
            get { return _ascending; }
            set { _ascending = value; }
        }

        [DataMember(Order = 18)]
        public string FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; }
        }


        [DataMember(Order = 19)]
        public string OmsOIDHierarchy
        {
            get { return _omsOIDHierarchy; }
            set { _omsOIDHierarchy = value; }
        }


        [DataMember(Order = 20)]
        public ErrorStore Error
        {
            get { return _error; }
            set { _error = value; }
        }

#endregion

        public AttributeStore()
        {
            // Empty constructor for serialization
        }

        public AttributeStore(string OID, string assetType, string omsTable, string attributeField, string displayName, string description, 
            List<string> lookupValues, string initialValue, string format, string unit, string omsDisplayNameHierarchy, string omsObjectUserIDHierarchy, string omsOIDHierarchy, string lookupOID,string fieldType)
        {
            this._OID = OID;
            this.AssetType = assetType;
            this.OmsTable = omsTable;
            this.AttributeField = attributeField;
            this.DisplayName = displayName;
            this.Description = description;
            this.LookupValues = lookupValues;
            this.InitialValue = initialValue;
            this.Format = format;
            this.Unit = unit;
            this._omsDisplayNameHierarchy = omsDisplayNameHierarchy;
            this._omsObjectUserIDHierarchy = omsObjectUserIDHierarchy;
            this._omsOIDHierarchy = omsOIDHierarchy;
            this._lookupOID = lookupOID;
            this._isConditionCategory = false;
            this._fieldType = fieldType;
        }


        /// <summary>
        /// Creates ConditionCategory attribute for storing conditionIndices in DecisionEngine.  Do not use for standard attribute.
        /// </summary>
        /// <param name="attributeField"></param>
        /// <param name="displayName"></param>
        public AttributeStore(string assetType,string attributeField, string displayName, string omsTable)
        {
            this.AssetType = assetType;
            this.OmsTable = omsTable;
            this.AttributeField = attributeField;
            this.DisplayName = displayName;
            this.Description = null;
            this.LookupValues = null;
            this.InitialValue = "100";
            this.Format = "f1";
            this.Unit = "percent";
            this._omsDisplayNameHierarchy = displayName;
            this._omsObjectUserIDHierarchy = attributeField;
            this._lookupOID = null;
            this._isConditionCategory = true;
            this._fieldType = "NUMBER";
            this._maximum = 100;
            this.Minimum = 0;
        }




        public override string ToString()
        {
            return _omsDisplayNameHierarchy;
        }
    }
}
