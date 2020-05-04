using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype
{
    class AttributeModel
    {
        public List<AttributesProperties> AttributeList { get; set; }
    }

    class AttributesProperties
    {
        public int attributeID { get; set; }
        public string attributeName { get; set; }
        public string rollupType { get; set; }
		public bool	isCalculated { get; set; }
		public string server { get; set; }
		public string DataSource { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }
}
