using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Base class for storing budget information for scenarios
    /// </summary>
    public abstract class BudgetStore
    {
        [DataMember]
        public virtual string Key { get; set; }

        [DataMember]
        public virtual string Target { get; set; }

        [DataMember]
        public virtual string Spent { get; set; }
    }
}
