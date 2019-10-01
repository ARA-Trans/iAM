using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("INVESTMENTS")]
    public class InvestmentsEntity
    {
        [Key]
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        public int? FIRSTYEAR { get; set; }
        public int? NUMBERYEARS { get; set; }
        public double? INFLATIONRATE { get; set; }
        public double? DISCOUNTRATE { get; set; }
        public string BUDGETORDER { get; set; }

        public virtual SimulationEntity SIMULATION { get; set; }

        public InvestmentsEntity() { }

        public InvestmentsEntity(InvestmentLibraryModel model)
        {
            INFLATIONRATE = model.InflationRate;
            BUDGETORDER = model.BudgetOrder != null
                ? string.Join(",", model.BudgetOrder)
                : "Rehabilitation,Maintenance,Construction";

            var years = model.BudgetYears.Any()
                ? model.BudgetYears.OrderBy(by => by.Year)
                    .Select(by => by.Year).Distinct().ToList()
                : new List<int>();

            FIRSTYEAR = years.Any() ? years[0] : DateTime.Now.Year;
            NUMBERYEARS = years.Any() ? years.Count() : 1;
        }

        public static void DeleteEntry(InvestmentsEntity entity, BridgeCareContext db)
        {
            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}