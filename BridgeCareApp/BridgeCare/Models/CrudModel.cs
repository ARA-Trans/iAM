using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class CrudModel
    {
        public CrudModel()
        {
            matched = false;
        }

        [IgnoreDataMember]
        public bool matched { get; set; }
    }
}