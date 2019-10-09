using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class InvestmentLibraryModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double? InflationRate { get; set; }
        public string Description { get; set; }
        public List<string> BudgetOrder { get; set; }
        public List<InvestmentLibraryBudgetYearModel> BudgetYears { get; set; }

        public InvestmentLibraryModel()
        {
            BudgetYears = new List<InvestmentLibraryBudgetYearModel>();
        }

        public InvestmentLibraryModel(SimulationEntity entity)
        {
            Id = entity.SIMULATIONID.ToString();
            Name = entity.SIMULATION;
            InflationRate = entity.INVESTMENTS?.INFLATIONRATE ?? 0;
            Description = entity.COMMENTS;
            BudgetOrder = entity.INVESTMENTS?.BUDGETORDER?.Split(',').ToList();
            BudgetYears = entity.YEARLYINVESTMENTS
                .Select(yi => new InvestmentLibraryBudgetYearModel(yi)).ToList();
        }

        public void UpdateInvestment(InvestmentsEntity entity)
        {
            entity.INFLATIONRATE = InflationRate;
            entity.BUDGETORDER = BudgetOrder != null
                ? string.Join(",", BudgetOrder)
                : "Rehabilitation,Maintenance,Construction";

            var years = BudgetYears.Any()
                ? BudgetYears.OrderBy(by => by.Year)
                    .Select(by => by.Year).Distinct().ToList()
                : new List<int>();

            entity.FIRSTYEAR = years.Any() ? years[0] : DateTime.Now.Year;
            entity.NUMBERYEARS = years.Any() ? years.Count() : 1;
        }
    }
}