using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
  public class CreateSimulationDataModel
  {
    [Required]
    public int NetworkId { get; set; }
    [Required]
    public string Name { get; set; }
    public string Owner { get; set; }
  }
}