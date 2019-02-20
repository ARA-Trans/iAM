using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.Models
{
    public class SectionDataModel
    {
        public int SECTIONID { get; set; }

        public string FACILITY { get; set; }
                
        public string SECTION { get; set; }
    }
}