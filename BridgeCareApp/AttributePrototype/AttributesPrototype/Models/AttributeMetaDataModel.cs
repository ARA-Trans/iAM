using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype
{
    class AttributeMetaDataModel
    {
        public List<AttributesProperties> AttributeList { get; set; }
    }

    class AttributesProperties
    {
        public int attributeID { get; set; }
        public string attributeName { get; set; }
        public string rollupType { get; set; }
		public bool	isCalculated { get; set; }
    }
}
