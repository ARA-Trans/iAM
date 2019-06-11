using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
  public class PriorityFundModel : CrudModel
  {
    public int PriorityId { get; set; }
    public int Id { get; set; }
    public string Budget { get; set; }
    public double Funding { get; set; }

    public PriorityFundModel(PRIORITYFUND priorityFund)
    {
      PriorityId = priorityFund.PRIORITYID;
      Id = priorityFund.PRIORITYFUNDID;
      Budget = priorityFund.BUDGET ?? "";
      Funding = priorityFund.FUNDING ?? 0;
    }
  }
}