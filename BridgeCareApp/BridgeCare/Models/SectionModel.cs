using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class SectionModel
    {
        [Range(1, int.MaxValue)]
        public int SectionId { get; set; }

        public string ReferenceId { get; set; }
        public string ReferenceKey { get; set; }

        [Range(1, int.MaxValue)]
        public int NetworkId { get; set; }
    }
}