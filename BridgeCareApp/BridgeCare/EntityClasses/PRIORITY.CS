using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace BridgeCare.EntityClasses
{
  [Table("PRIORITY")]
  public class PRIORITY
  {
    [Key]
    public int PRIORITYID { get; set; }
    [ForeignKey("SIMULATION")]
    public int SIMULATIONID { get; set; }
    public int? PRIORITYLEVEL { get; set; }
    public string CRITERIA { get; set; }
    public byte?[] BINARY_CRITERIA { get; set; }
    public int? YEARS { get; set; }

    public virtual SIMULATION SIMULATION { get; set; }
    public ICollection<PRIORITYFUND> PRIORITYFUNDS { get; set; }
  }
}