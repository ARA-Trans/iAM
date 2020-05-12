using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExecutableForProtptype
{
    public class AttributeList
    {
        [JsonProperty("AttributeList")]
        public List<ConfigFileModel> AttributeConfigData { get; set; }
    }
    public class ConfigFileModel
    {
        public string AttributeName { get; set; }
        public string DefaultValue { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string Location { get; set; }
        public string DataType { get; set; }
        public Connection Connection { get; set; }
    }

    public class Connection
    {
        public string Server { get; set; }
        public string DataSource { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
