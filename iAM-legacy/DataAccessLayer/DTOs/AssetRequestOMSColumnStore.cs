using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class AssetRequestOMSColumnStore
    {
        string _omsColumn;
        string _omsObjectUserIDHierarchy;
        string _attributeField;



        /// <summary>
        /// OMS Column Name
        /// </summary>
        public string ColumnName
        {
            get { return _omsColumn; }
            set { _omsColumn = value; }
        }
        

        /// <summary>
        /// OMS ObjectUserIDHierarchy (from AttributeStore)
        /// </summary>
        public string OmsObjectUserIDHierarchy
        {
            get { return _omsObjectUserIDHierarchy; }
            set { _omsObjectUserIDHierarchy = value; }
        }

        public string AttributeField
        {
          get { return _attributeField; }
          set { _attributeField = value; }
        }

        //Blank constructor for serialization.
        public AssetRequestOMSColumnStore()
        {
        }


        /// <summary>
        /// A map of OMS columns to a DE built ObjectUserIDHierarchy
        /// </summary>
        /// <param name="omsColumn"></param>
        /// <param name="omsObjectUserIDHierarchy"></param>
        public AssetRequestOMSColumnStore(string omsColumn, string omsObjectUserIDHierarchy, string attributeField)
        {
            _omsColumn = omsColumn;
            _omsObjectUserIDHierarchy = omsObjectUserIDHierarchy;
            _attributeField = attributeField;
        }

        /// <summary>
        /// Called by Condition Index attributes
        /// </summary>
        /// <param name="omsColumn"></param>
        /// <param name="omsObjectUserIDHierarchy"></param>
        public AssetRequestOMSColumnStore(string omsColumn, string omsObjectUserIDHierarchy)
        {
            _omsColumn = omsColumn;
            _omsObjectUserIDHierarchy = omsObjectUserIDHierarchy;
        }


        public override string ToString()
        {
            return _omsColumn + "(" + _omsObjectUserIDHierarchy + ")";
        }
    }
}
