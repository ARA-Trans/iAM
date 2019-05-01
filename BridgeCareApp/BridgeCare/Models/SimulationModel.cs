using System;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class SimulationModel
    {
        [Required]
        public int SimulationId { get; set; }
        public string SimulationName { get; set; }
        public string NetworkName { get; set; }
        [Required]
        public int NetworkId { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastRun { get; set; }
    }
}