using System;
using System.Collections.Generic;
using System.Text;
using CalculateEvaluate;

namespace Simulation
{
    public class Committed
    {
        public int Year { get; set; }
        public String Priority { get; set; }
        public String CommitOrder { get; set; }
        public String RemainingLife { get; set; }
        public String Benefit { get; set; }
        public String BenefitCost { get; set; }
        public String RemainingLifeHash { get; set; }
        public String Treatment { get; set; }
        public float Cost { get; set; }
        public String Budget { get; set; }
        public int Any { get; set; }
        public int Same { get; set; }
        public String ConsequenceID { get; set; }
        public Consequences Consequence { get; set; }
        public bool IsCommitted { get; set; }

        //If this is set. The committed project is a ScheduledTreatment and not a true committed project.
        public string ScheduledTreatmentId { get; set; }
        public string SplitTreatmentId { get;  }
        public int YearSplitTreatmentComplete { get;  }


        public Committed(int year, int yearSplitTreatmentComplete, string budget, string splitTreatmentId,float cost,string treatment )
        {
            Year = year;
            YearSplitTreatmentComplete = yearSplitTreatmentComplete;
            Budget = budget;
            SplitTreatmentId = splitTreatmentId;
            Cost = cost;
            Treatment = treatment;
            Priority = "0";
            CommitOrder = "0";
            RemainingLife = "100";
            Benefit = "0";
            BenefitCost = "0";
            RemainingLifeHash = "";
            ConsequenceID = "0";
        }

        public Committed()
        {
            Priority = "0";
            CommitOrder = "0";
            RemainingLife = "100";
            Benefit = "0";
            BenefitCost = "0";
            RemainingLifeHash = "";
            ConsequenceID = "0";
        }
    }
}
