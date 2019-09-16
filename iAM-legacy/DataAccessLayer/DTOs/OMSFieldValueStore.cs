using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSFieldValueStore
    {
        string _fieldName;
        string _fieldValue;
        DateTime _entryDate;
        
        #region Properties
        public DateTime EntryDate
        {
            get { return _entryDate; }
            set { _entryDate = value; }
        }
        
        public string FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
        }

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }
        #endregion

        /// <summary>
        /// Ties an asset field to its corresponding value.
        /// </summary>
        /// <param name="fieldName">The name of the asset field.</param>
        /// <param name="fieldValue">The value of the asset field.</param>
        public OMSFieldValueStore(string fieldName, string fieldValue, DateTime entryDate)
        {
            _fieldName = fieldName;
            _fieldValue = fieldValue;
            _entryDate = entryDate;
        }
    }
}
