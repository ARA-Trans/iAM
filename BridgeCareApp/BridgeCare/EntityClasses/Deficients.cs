using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
  [Table("DEFICIENTS")]
  public class DEFICIENTS
  {
    [Key]
    public int ID_ { get; set; }
    [ForeignKey("SIMULATION")]
    public int SIMULATIONID { get; set; }
    public string ATTRIBUTE_ { get; set; }
    public string DEFICIENTNAME { get; set; }
    public double DEFICIENT { get; set; }
    public double PERCENTDEFICIENT { get; set; }
    public string CRITERIA { get; set; }
    public byte[] BINARY_CRITERIA { get; set; }

    public virtual SIMULATION SIMULATION { get; set; }
  }
}