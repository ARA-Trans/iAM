using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSAssetConditionIndexStore
    {
        string _assetName;
        string _conditionCategoryTable;
        List<OMSConditionIndexStore> _conditionIndexes;
        List<OCIWeightStore> _weights;
        List<string> _uniqueCriteria = new List<string>();


        public string AssetName
        {
            get { return _assetName; }
            set { _assetName = value; }
        }

        public string ConditionCategoryTable
        {
            get { return _conditionCategoryTable; }
            set { _conditionCategoryTable = value; }
        }

        public List<OMSConditionIndexStore> ConditionIndexes
        {
            get { return _conditionIndexes; }
            set { _conditionIndexes = value; }
        }

        public List<OCIWeightStore> Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        public OMSAssetConditionIndexStore(AttributeStore conditionCategory, List<OCIWeightStore> weights)
        {
            _assetName = conditionCategory.AssetType;
            _conditionCategoryTable = conditionCategory.OmsTable;
            _conditionIndexes = new List<OMSConditionIndexStore>();
            _weights = weights;
            foreach (OCIWeightStore ows in _weights)
            {
                OMSConditionIndexStore ciExists = _conditionIndexes.Find(delegate(OMSConditionIndexStore ci) { return ci.ConditionCategory == ows.ConditionCategory; });
                if (ciExists == null)
                {
                    OMSConditionIndexStore ci = new OMSConditionIndexStore(ows.ConditionCategory);
                    _conditionIndexes.Add(ci);
                }
                if(!_uniqueCriteria.Contains(ows.Criteria))
                {
                    _uniqueCriteria.Add(ows.Criteria);
                }
            }
        }


         public override string ToString()
        {
            string equation = "";

            foreach (string criteria in _uniqueCriteria)
            {
                equation += "(";
                bool addPlus = false;
                for (int i = 0; i < _conditionIndexes.Count; i++)
                {

                    OCIWeightStore weight = _weights.Find(delegate(OCIWeightStore w) { return w.Criteria == criteria && w.ConditionCategory == _conditionIndexes[i].ConditionCategory; });
                    if (weight != null)
                    {
                        if (addPlus) equation += "+";
                        equation += "[" + ConditionIndexes[i].AttributeDE + "]*" + Weights[i].Weight;
                        addPlus = true;
                    }
                }
                equation += ")";
            }
            return equation;
        }
    }
}
