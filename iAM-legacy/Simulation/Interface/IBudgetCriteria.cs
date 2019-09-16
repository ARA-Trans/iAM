using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation.Interface
{
    interface IBudgetCriteria
    {
        int BudgetCriteriaId { get; }
        string BudgetName { get; }
        Criterias Criteria { get; }
    }
}
