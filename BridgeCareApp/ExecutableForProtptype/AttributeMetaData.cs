using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExecutableForProtptype
{
    public class AttributeMetaDatum
    {
        public string AttributeName { get; set; }
        public string DefaultValue { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string Location { get; set; }
        public string DataType { get; set; }
        public string ConnectionString { get; set; }
        public string DataRetrievalCommand { get; set; }
    }
}
