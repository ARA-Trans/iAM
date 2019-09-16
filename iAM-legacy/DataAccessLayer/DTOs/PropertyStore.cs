using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    class PropertyStore
    {
        /// <summary>
        /// Asset type of attribute (Sign, Pavement, Pump, etc.)
        /// </summary>
        public string Asset { get; set; }
        /// <summary>
        /// Cartegraph property, Roadcare attribute
        /// </summary>
        public string Attribute { get; set; }
        /// <summary>
        /// Type of attribute (string, number, date)
        /// </summary>
        public string Type {get;set;}
        /// <summary>
        /// Possible values for Attribute (from lookup table - can be null)
        /// </summary>
        public List<string> Lookups { get; set; }
        /// <summary>
        /// Default unit of the attribute.
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// List of validation rules.
        /// </summary>
        public List<string> ValidationRules {get; set;}

    }
}
