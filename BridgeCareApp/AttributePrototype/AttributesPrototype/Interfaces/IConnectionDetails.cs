using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype
{
    interface IConnectionDetails
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Bearer { get; set; }
        public string Server { get; set; }
        public string DataSource { get; set; }
    }
}
