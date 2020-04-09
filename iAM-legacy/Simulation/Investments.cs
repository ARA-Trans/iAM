using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DatabaseManager;
using System.Collections;
using System.Data.SqlClient;
using Simulation.Interface;

namespace Simulation
{
    public class Investments
    {
        // Investment parameters
        private String m_strInvestmentID;
        private String m_strInvestmentName;
        private String m_strInvestmentComment;
        private int m_nFirstYear;
        private int m_nNumberYear;
        private float m_fDiscount;
        private float m_fInflation;
        private List<String> m_listBudgetOrder = new List<string>();
        private Hashtable m_hashBudgetYearOriginal = new Hashtable();
        private Hashtable m_hashBudgetYear = new Hashtable();
        private String m_strBudgetOrder;
        private DateTime m_datetimeStart;

        public DateTime StartDate
        {
            get { return m_datetimeStart; }
            set { m_datetimeStart = value; }
        }

        public List<String> BudgetOrder
        {
            get { return m_listBudgetOrder; }
            set { m_listBudgetOrder = value; }

        }

        public String BudgetOrderString
        {
            get { return m_strBudgetOrder; }
            set { m_strBudgetOrder = value; }

        }

        /// <summary>
        /// Hash of original available budget for a given year (for priority calculations).
        /// </summary>
        public Hashtable BudgetYearOriginal
        {
            get { return m_hashBudgetYearOriginal; }
            set { m_hashBudgetYearOriginal = value; }
        }

        /// <summary>
        /// Hash of available budget for a given year.
        /// </summary>
        public Hashtable BudgetYear
        {
            get { return m_hashBudgetYear; }
            set { m_hashBudgetYear = value; }
        }

        /// <summary>
        /// Retrieve InvestmentID for current simulation
        /// </summary>
        public String InvestmentID
        {
            get { return m_strInvestmentID; }
            set { m_strInvestmentID = value; }
        }
        
        
        /// <summary>
        /// Name of the Investment
        /// </summary>
        public String Name
        {
            get { return m_strInvestmentName; }
            set { m_strInvestmentName = value; }
        }

        /// <summary>
        /// Description of the Investment
        /// </summary>
        public String Comment
        {
            get { return m_strInvestmentComment; }
            set { m_strInvestmentComment = value; }
        }

        /// <summary>
        /// Year to begin analysis
        /// </summary>
        public int StartYear
        {
            get { return m_nFirstYear; }
            set { m_nFirstYear = value; }
        }


        /// <summary>
        /// Number of years to run simualtion for
        /// </summary>
        public int AnalysisPeriod
        {
            get { return m_nNumberYear; }
            set { m_nNumberYear = value; }
        }
    
        public float Discount
        {
            get { return m_fDiscount; }
            set { m_fDiscount = value; }
        }

        public float Inflation
        {
            get { return m_fInflation; }
            set
            {   float fRate = value;
                m_fInflation = fRate / 100;
            }
        }

        private int MaximumYear { get; set; }

        public void LoadBudgets()
        {
            string[] budgets = BudgetOrderString.Split(',');
            String strBudget;
            for(int i = 0; i <budgets.Length; i++)
            {
                strBudget = budgets[i];
                m_listBudgetOrder.Add(strBudget);
            }

            String strSelect = "SELECT YEAR_,BUDGETNAME,AMOUNT FROM " + cgOMS.Prefix + "YEARLYINVESTMENT WHERE SIMULATIONID='" + this.InvestmentID + "' ORDER BY YEAR_";
            DataSet ds = null;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect); 
            }
            catch(Exception ex)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Loading YEARLYINVESTMENT table." + ex.Message));
                return;
            }

            String strYear;

            String strAmount;
            MaximumYear = Int32.MinValue;

            foreach(DataRow row in ds.Tables[0].Rows)
            {
                strYear = row[0].ToString();
                var year = Convert.ToInt32(strYear);
                if (year > MaximumYear)
                {
                    MaximumYear = year;
                }
                strBudget = row[1].ToString();
                strAmount = row[2].ToString();

                if (BudgetYear.Contains(strBudget))
                {
                    Hashtable hashYearAmount = (Hashtable)BudgetYear[strBudget];
                    Hashtable hashYearAmountOriginal = (Hashtable)BudgetYearOriginal[strBudget];

                    m_hashBudgetYear.Remove(strBudget);
                    m_hashBudgetYearOriginal.Remove(strBudget);

                    if (strAmount == "")
                    {
                        hashYearAmount.Add(strYear, 0.0);
                        hashYearAmountOriginal.Add(strYear, 0.0);
                    }
                    else
                    {

                        hashYearAmount.Add(strYear, float.Parse(strAmount));
                        hashYearAmountOriginal.Add(strYear, float.Parse(strAmount));
                    }

                    BudgetYear.Add(strBudget, hashYearAmount);
                    BudgetYearOriginal.Add(strBudget, hashYearAmountOriginal);
                }
                else
                {
                    Hashtable hashYearAmount = new Hashtable();
                    Hashtable hashYearAmountOriginal = new Hashtable();

                    if (strAmount == "")
                    {
                        hashYearAmount.Add(strYear, 0.0);
                        hashYearAmountOriginal.Add(strYear, 0.0);
                    }
                    else
                    {
                        hashYearAmount.Add(strYear, float.Parse(strAmount));
                        hashYearAmountOriginal.Add(strYear, float.Parse(strAmount));
                    }



                    BudgetYear.Add(strBudget, hashYearAmount);
                    BudgetYearOriginal.Add(strBudget, hashYearAmountOriginal);
                }
            }

            foreach(var budget in BudgetYear.Keys)
            {
                var hashYearAmount = (Hashtable)BudgetYear[budget];
                var hashYearAmountOriginal = (Hashtable)BudgetYearOriginal[budget];

                for(int year = MaximumYear + 1; year < MaximumYear + 101; year++)
                {
                    hashYearAmount.Add(year.ToString(), hashYearAmount[MaximumYear.ToString()]);
                    hashYearAmountOriginal.Add(year.ToString(), hashYearAmountOriginal[MaximumYear.ToString()]);
                }
            }
        }

        /// <summary>
        /// Check if budget can support treatment.  Checks for each priority.
        /// </summary>
        /// <param name="fAmount"></param>
        /// <param name="strBudget"></param>
        /// <param name="strYear"></param>
        /// <param name="hashAttributeValue"></param>
        /// <param name="priority"></param>
        /// <param name="limits">Limits where treatments are split over multiple years</param>
        /// <returns></returns>
        public string IsBudgetAvailable(float fAmount, string strBudget, String strYear, Hashtable hashAttributeValue,Priorities priority, List<ISplitTreatmentLimit> limits, string budgetLimitType, out string budgetHash, out ISplitTreatmentLimit splitTreatmentLimit, out string reasonNoBudget)
        {
            reasonNoBudget = "";
            string noBudgetAvailable = "";
            splitTreatmentLimit = limits[0];

            //Check priority.  If not possible just return null.
            if (!(priority.IsAllSections || priority.Criteria.IsCriteriaMet(hashAttributeValue)))
            {
                budgetHash = "";
                reasonNoBudget = "Priority (" + priority.PriorityLevel  + ") criteria not met for year " + strYear;
                return noBudgetAvailable;
            }

            string[] possibleBudgets;


            foreach (var limit in limits)
            {
                if (limit.Amount > fAmount)
                {
                    splitTreatmentLimit = limit;
                    break;
                }
            }

           

            budgetHash = "";
            try
            {
                possibleBudgets = strBudget.Split('|');

                for(int i = 0; i < possibleBudgets.Length; i++)
                {
                    possibleBudgets[i] = possibleBudgets[i].Trim();
                }

            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Budget is null." + e.Message));
                throw e;
            }

            //If there are no budget criteria, then ignore.  The list is enforced.

            var budgets = new List<string>();
            if (SimulationMessaging.BudgetCriterias.Count > 0)
            {
                var listAvailableBudget = new List<BudgetCriteria>();
                foreach (var budgetCriteria in SimulationMessaging.BudgetCriterias)
                {
                    if (budgetCriteria.Criteria.IsCriteriaMet(hashAttributeValue))
                    {
                        listAvailableBudget.Add(budgetCriteria);
                    }
                }


                foreach (var budget in possibleBudgets)
                {
                    var available = listAvailableBudget.Find(b => b.BudgetName == budget);
                    if (available != null && !budgets.Contains(available.BudgetName))
                    {
                        budgets.Add(available.BudgetName);
                    }
                }
            }
            else
            {
                foreach(var budget in possibleBudgets)
                {
                    budgets.Add(budget);
                }
            }

            if(budgets.Count == 0)
            {
                reasonNoBudget = "Budget (" + strBudget + ") did not meet budget criteria.";
            }

            foreach (string budget in budgets)
            {
                string budgetCheck = budget.Trim();
                Hashtable hashYearAmount;
                Hashtable hashYearAmountOriginal;
                float fAvailable;
                float fOriginal;
                float fAfterSpending;
                float fPercent;

                //Budget not defined
                try
                {
                    if (!BudgetYear.Contains(budgetCheck)) continue;
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Checking budget year. " + e.Message));
                    throw e;
                }


                try
                {
                    hashYearAmount = (Hashtable)BudgetYear[budgetCheck];
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving budget year. " + e.Message));
                    throw e;
                }

                try
                {
                    if (!BudgetYearOriginal.Contains(budgetCheck)) continue;
                }
                catch (Exception e)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error: Checking budget year original. " + e.Message));
                    throw e;
                }

                bool currentBudgetAvailable = true;

                if (budgetLimitType.ToLower() == "unlimited")//If the budget is available (possible budget) and is unlimited... we are good.
                {
                    return budgetCheck;
                }


                //Starting here need to loop through year amounts.
                var year = Convert.ToInt32(strYear);
                for (int i = 0; i < splitTreatmentLimit.Percentages.Count; i++)
                {
                    var currentYearAmount = fAmount * splitTreatmentLimit.Percentages[i] / 100;
                    var currentYear = year + i;

                    //Priority must be all years, match the current year or the current year must be greater than maximum year and the Maximum year must be a priority.
                    if (!(priority.IsAllYears || priority.Years == currentYear || (currentYear > MaximumYear && priority.Years == MaximumYear)))
                    {

                        reasonNoBudget = "Priority is not set for year " + currentYear;
                        return null;
                    }

                    try
                    {
                        //This occurs when a split treament goes pas then end.
                        if (!hashYearAmount.Contains(currentYear.ToString())) continue;
                        //Not past end of analysis.
                        fAvailable = (float)hashYearAmount[currentYear.ToString()];
                    }
                    catch (Exception e)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving year amount. " + e.Message));
                        throw e;
                    }


                    //This gets a hash of all years and the amount of budgets originally available.                    
                    try
                    {
                        hashYearAmountOriginal = (Hashtable)BudgetYearOriginal[budgetCheck];
                        //This occurs when a split treament goes pas then end.
                        if (!hashYearAmountOriginal.ContainsKey(currentYear.ToString())) continue;
                    }
                    catch (Exception e)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving budget year original. " + e.Message));
                        throw e;
                    }


                    try
                    {
                        if (!hashYearAmountOriginal.Contains(strYear)) continue;
                        fOriginal = (float)hashYearAmountOriginal[strYear];
                    }
                    catch (Exception e)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving year amount original. " + e.Message));
                        throw e;
                    }

                    fAfterSpending = fAvailable - fAmount;
                    if (fOriginal <= 0) continue;
                    fPercent = (1 - fAfterSpending / fOriginal) * 100;


                    try
                    {
                        float fPercentLimit = (float)priority.BudgetPercent[budgetCheck];
                        var original = (float)priority.BudgetPercent[budgetCheck] * fOriginal / 100;
                        budgetHash = fAmount + "/" + fAvailable.ToString("f0") + "/" + original.ToString("f0");
                        //if (fPercent < fPercentLimit) return budgetCheck;
                        if (fPercent > fPercentLimit)
                        {
                            currentBudgetAvailable = false;
                            reasonNoBudget = "Percent after spending greater than priority limit " + fPercent.ToString("f2")  + "/" + fPercentLimit.ToString("f2") + " for priorty (" + priority.PriorityLevel + ")";

                        }
                    }
                    catch (Exception e)
                    {
                        SimulationMessaging.AddMessage(new SimulationMessage("Error: Evaluating priority. " + e.Message));
                        throw e;
                    }
                }
                //To be true here, there must be money available for each year.
                if (currentBudgetAvailable)
                {
                    return budgetCheck;
                }
                
            }
            return noBudgetAvailable;
        }



        public void SpendBudget(float fAmount, String strBudget, String strYear)
        {
            if (strBudget == null) return;
            if (!BudgetYear.Contains(strBudget)) return;
            Hashtable hashYearAmount = (Hashtable)BudgetYear[strBudget];
            if (!hashYearAmount.Contains(strYear)) return;
            float fAvailable = (float)hashYearAmount[strYear];
            hashYearAmount.Remove(strYear);
            fAvailable = fAvailable - fAmount;
            hashYearAmount.Add(strYear, fAvailable);
        }


        public void MoveBudgetAcross(string budget, string year,  Priorities priority)
        {
            //Get next budget.
            var nextBudget = GetNextBudget(budget);

            //Add what is left from current budget (and priority) to the the next budget.

            string budgetCheck = budget.Trim();
            Hashtable hashYearAmount;
            Hashtable hashYearAmountOriginal;
            float available = 0;
            float original = 0;

            //Budget not defined
            try
            {
                if (!BudgetYear.Contains(budgetCheck)) return;
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Checking budget year. " + e.Message));
                throw e;
            }


            try
            {
                hashYearAmount = (Hashtable)BudgetYear[budgetCheck];
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving budget year. " + e.Message));
                throw e;
            }
            try
            {
                if (!hashYearAmount.Contains(year)) return;
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Checking year amount. " + e.Message));
                throw e;
            }

            try
            {
                available = (float)hashYearAmount[year];
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving year amount. " + e.Message));
                throw e;
            }

            try
            {
                if (!BudgetYearOriginal.Contains(budgetCheck)) return;
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Checking budget year original. " + e.Message));
                throw e;
            }

            try
            {
                hashYearAmountOriginal = (Hashtable)BudgetYearOriginal[budgetCheck];
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving budget year original. " + e.Message));
                throw e;
            }


            try
            {
                if (!hashYearAmountOriginal.Contains(year)) return;
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Checking year amount original. " + e.Message));
                throw e;
            }

            try
            {
                original = (float)hashYearAmountOriginal[year];
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving year amount original. " + e.Message));
                throw e;
            }


            if (original <= 0) return;
            var percent = (1 - available / original) * 100;

            try
            {
                var percentLimit = (float)priority.BudgetPercent[budgetCheck];
                if (percent < percentLimit)
                {
                    var difference = percentLimit - percent;
                    var budgetToMove = original * difference / 100;
                    hashYearAmount[year] = (float)hashYearAmount[year] - budgetToMove;

                    hashYearAmount = (Hashtable)BudgetYear[nextBudget];
                    hashYearAmount[year] = (float)hashYearAmount[year] + budgetToMove;


                }
            }
            catch (Exception e)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving budget year original. " + e.Message));
                throw e;
            }
        }

        private string GetNextBudget(string budget)
        {
            var index = m_listBudgetOrder.FindIndex(b=>b == budget);
            if (index < m_listBudgetOrder.Count - 1)
            {
                return m_listBudgetOrder[index + 1];
            }
            return m_listBudgetOrder[m_listBudgetOrder.Count - 1];
        }
    }
}
