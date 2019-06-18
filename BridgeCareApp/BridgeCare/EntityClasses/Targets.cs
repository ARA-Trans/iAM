using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
  [Table("TARGETS")]
  public class TARGETS
  {
    [Key]
    public int ID_ { get; set; }
    [ForeignKey("SIMULATION")]
    public int SIMULATIONID { get; set; }
    [ForeignKey("ATTRIBUTES_")]
    public string ATTRIBUTE_ { get; set; }
    public int YEARS { get; set; }
    public double TARGETMEAN { get; set; }
    public string TARGETNAME { get; set; }
    public string CRITERIA { get; set; }
    public byte[] BINARY_CRITERIA { get; set; }

    public virtual SIMULATION SIMULATION { get; set; }
    public virtual Attributes ATTRIBUTES_ { get; set; }
    }
}