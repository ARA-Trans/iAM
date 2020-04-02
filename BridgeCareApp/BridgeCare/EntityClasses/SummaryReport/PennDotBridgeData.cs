using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace BridgeCare.EntityClasses
{
    [Table("PENNDOT_BRIDGE_DATA")]
    public class PennDotBridgeData
    {      
        [Key]
        public int BRKEY { get; set; }

        //[Column(TypeName = "VARCHAR")]        
        public int BRIDGE_FAMILY_ID { get; set; }

        //[Column(TypeName = "VARCHAR")]        
        //public string CONDITION_BASED_AGE { get; set; }

        public string BridgeCulvert { get; private set; }
    }
}
