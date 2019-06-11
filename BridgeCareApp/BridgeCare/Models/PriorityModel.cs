using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
  public class PriorityModel : CrudModel
  {
    public int ScenarioId { get; set; }
    public int Id { get; set; }
    public int PriorityLevel { get; set; }
    public int Year { get; set; }
    public string Criteria { get; set; }
    public List<PriorityFundModel> PriorityFunds { get; set; }

    public PriorityModel(PRIORITY priority)
    {
      ScenarioId = priority.SIMULATIONID;
      Id = priority.PRIORITYID;
      PriorityLevel = priority.PRIORITYLEVEL ?? 1;
      Year = priority.YEARS ?? DateTime.Now.Year;
      Criteria = priority.CRITERIA;
      PriorityFunds = new List<PriorityFundModel>();
      if (priority.PRIORITYFUNDS.Any())
      {
        priority.PRIORITYFUNDS.ToList().ForEach(pf => PriorityFunds.Add(new PriorityFundModel(pf)));
      }
    }
  }
}