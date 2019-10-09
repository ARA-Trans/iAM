using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("FEASIBILITY")]
    public class FeasibilityEntity
    {
        [Key]
        public int FEASIBILITYID { get; set; }
        public int TREATMENTID { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }

        [ForeignKey("TREATMENTID")]
        public virtual TreatmentsEntity TREATMENT { get; set; }

        public FeasibilityEntity() { }

        public FeasibilityEntity(int treatmentId, FeasibilityModel feasibilityModel)
        {
            TREATMENTID = treatmentId;
            CRITERIA = feasibilityModel.Criteria;
        }

        public FeasibilityEntity(int treatmentId)
        {
            TREATMENTID = treatmentId;
            CRITERIA = "";
        }

        public static void DeleteEntry(FeasibilityEntity feasibilityEntity, BridgeCareContext db)
        {
            db.Entry(feasibilityEntity).State = EntityState.Deleted;
        }
    }
}
