using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace BridgeCare.EntityClasses
{
    [Table("PennDot_Report_A")]
    public class PennDotReportAData
    {
        [Key]
        public int BRKEY { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string BRIDGE_ID { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string DISTRICT { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string DECK_AREA { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string NHS_IND { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string BUS_PLAN_NETWORK { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string FUNC_CLASS { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string YEAR_BUILT { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string ADTTOTAL { get; set; }

        public string StructureLength { get; private set; }

        public string StructureType { get; private set; }

        public string PlanningPartner { get; private set; }

        public string Posted { get; private set; }

        public int P3 { get; private set; }

        public int ParallelBridge { get; private set; }

        public string OwnerCode { get; private set; }
    }
}
