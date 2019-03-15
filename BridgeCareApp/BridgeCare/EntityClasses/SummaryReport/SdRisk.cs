using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace BridgeCare.EntityClasses
{
    [Table("SD_RISK")]
    public class SdRisk
    {
        [Key]
        public int BRKEY { get; set; }

        [Column(TypeName = "NCHAR")]
        [StringLength(16)]
        public string SD_RISK { get; set; }
    }
}