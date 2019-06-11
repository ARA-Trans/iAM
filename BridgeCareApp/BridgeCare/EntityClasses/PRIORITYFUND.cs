using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BridgeCare.EntityClasses
{
  [Table("PRIORITYFUND")]
  public class PRIORITYFUND
  {
    [Key]
    public int PRIORITYFUNDID { get; set; }
    public string BUDGET { get; set; }
    public double? FUNDING { get; set; }
    [ForeignKey("PRIORITY")]
    public int PRIORITYID { get; set; }

    public virtual PRIORITY PRIORITY { get; set; }
  }
}