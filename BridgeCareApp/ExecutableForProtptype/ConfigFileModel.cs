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
        public string ConnectionString { get; set; }

        [JsonProperty("connection")]
        public Connection Connection { get; set; }
    }

    public class Connection
    {
        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("dataSource")]
        public string DataSource { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
