using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSPrediction
    {
        string _assetName;
        string _predictionGroupName;
        string _predictionFilter;
        string _predictionGroupsOID;
        string _predictionGroupTable;
        List<OMSCategoryPrediction> _categoryPredictions;

        public string AssetName
        {
            get { return _assetName; }
            set { _assetName = value; }
        }
        
        public string PredictionGroupName
        {
            get { return _predictionGroupName; }
            set { _predictionGroupName = value; }
        }
        
        public string PredictionFilter
        {
            get { return _predictionFilter; }
            set { _predictionFilter = value; }
        }

        public string PredictionGroupsOID
        {
            get { return _predictionGroupsOID; }
            set { _predictionGroupsOID = value; }
        }
        
        public string PredictionGroupTable
        {
            get { return _predictionGroupTable; }
            set { _predictionGroupTable = value; }
        }

        public List<OMSCategoryPrediction> CategoryPredictions
        {
            get { return _categoryPredictions; }
            set { _categoryPredictions = value; }
        }


        public OMSPrediction()
        {
        }


        public OMSPrediction(string assetName, string predictionGroupName, string filter, string oid,string table)
        {
            _assetName = assetName;
            _predictionGroupName = predictionGroupName;
            _predictionFilter = filter;
            _predictionGroupsOID = oid;
            _predictionGroupTable = table;
            _categoryPredictions = new List<OMSCategoryPrediction>();
        }


        public List<AttributeStore> GetAttributes()
        {
            return OMS.ParseAttributes(_assetName, _predictionFilter);
        }
    }
}
