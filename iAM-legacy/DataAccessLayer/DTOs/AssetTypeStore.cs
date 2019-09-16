using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataAccessLayer.DTOs
{
    public class AssetTypeStore
    {
        private string _assetName;
        private string _OID;

        public string OID
        {
            get { return _OID; }
            set { _OID = value; }
        }

        public string AssetName
        {
            get { return _assetName; }
            set { _assetName = value; }
        }

        public AssetTypeStore()
        {
            // For serialization
        }

        public AssetTypeStore(string assetName, string OID)
        {
            _OID = OID;
            _assetName = assetName;
        }

        public override string ToString()
        {
            return _assetName;
        }
    }
}
