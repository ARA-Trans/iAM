using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace BridgeCare.EntityClasses
{
    [Table("COSTS")]
    public partial class CostsEntity
    {
        [Key]
        public int COSTID { get; set; }
        public int TREATMENTID { get; set; }
        public string COST_ { get; set; }
        public string UNIT { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
        public byte[] BINARY_COST { get; set; }
        public bool? ISFUNCTION { get; set; }

        [ForeignKey("TREATMENTID")]
        public virtual TreatmentsEntity TREATMENT { get; set; }

        public CostsEntity() { }

        public CostsEntity(int treatmentId, CostModel costModel)
        {
            TREATMENTID = treatmentId;
            COST_ = costModel.Equation;
            CRITERIA = costModel.Criteria;
            ISFUNCTION = costModel.IsFunction ?? false;
        }

        public CostsEntity(CostModel costModel)
        {
            COST_ = costModel.Equation;
            CRITERIA = costModel.Criteria;
            ISFUNCTION = costModel.IsFunction ?? false;
        }

        public static void DeleteEntry(CostsEntity costsEntity, BridgeCareContext db)
        {
            db.Entry(costsEntity).State = EntityState.Deleted;
        }
    }
}