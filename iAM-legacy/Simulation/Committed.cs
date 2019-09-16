using System;
using System.Collections.Generic;
using System.Text;
using CalculateEvaluate;

namespace Simulation
{
    public class Committed
    {
        String m_sTreatmentName;
        float m_fCost;
        String m_sBudget;
        int m_nAny;
        int m_nSame;
        String m_sConsequenceID="0";
        Consequences consequence;
        bool m_bCommitted;
        bool m_bIsRepeat;
        bool _omsIsExclusive;
        String m_strRemainingLife = "100";
        String m_strBenefit = "0";
        String m_strBenefitCost = "0";
        String m_strRemainingLifeHash = "";
        String m_strCommitOrder = "0";
        String m_strPriority = "0";
        bool m_bMultiYear = false;
        int _year;//Year for which project is being committed.
        Treatments _OMSTreatment;
        bool _omsIsNotAllowed;
        int _resultType;
        //If this is set. The committed project is a ScheduledTreatment and not a true committed project.
        public string ScheduledTreatmentId { get; set; }


        public int ResultType
        {
            get { return _resultType; }
            set { _resultType = value; }
        }

        public bool OMSIsNotAllowed
        {
            get { return _omsIsNotAllowed; }
            set { _omsIsNotAllowed = value; }
        }

        internal Treatments OMSTreatment
        {
            get { return _OMSTreatment; }
            set { _OMSTreatment = value; }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public bool MultipleYear
        {
            get { return m_bMultiYear; }
            set { m_bMultiYear = value; }
        }

        public String Priority
        {
            get { return m_strPriority; }
            set { m_strPriority = value; }
        }

        public String CommitOrder
        {
            get { return m_strCommitOrder; }
            set { m_strCommitOrder = value; }
        }

        public String RemainingLife
        {
            get { return m_strRemainingLife; }
            set { m_strRemainingLife = value; }
        }

        public String Benefit
        {
            get { return m_strBenefit; }
            set { m_strBenefit = value; }
        }

        public String BenefitCost
        {
            get { return m_strBenefitCost; }
            set { m_strBenefitCost = value; }
        }

        public String RemainingLifeHash
        {
            get { return m_strRemainingLifeHash; }
            set { m_strRemainingLifeHash = value; }
        }

        public String Treatment
        {
            get { return m_sTreatmentName; }
            set { m_sTreatmentName = value; }
        }

        public float Cost
        {
            get { return m_fCost; }
            set { m_fCost = value; }
        }

        public String Budget
        {
            get { return m_sBudget; }
            set { m_sBudget = value; }
        }

        public int Any
        {
            get { return m_nAny; }
            set { m_nAny = value; }
        }

        public int Same
        {
            get { return m_nSame; }
            set { m_nSame = value; }
        }

        public String ConsequenceID
        {
            get { return m_sConsequenceID; }
            set { m_sConsequenceID = value; }
        }

        public Consequences Consequence
        {
            get { return consequence; }
            set { consequence = value; }
        }

        public bool IsCommitted
        {
            get { return m_bCommitted; }
            set { m_bCommitted = value; }
        }

        public bool IsRepeat
        {
            get { return m_bIsRepeat; }
            set { m_bIsRepeat = value; }
        }

        public bool OMSIsExclusive
        {
            get { return _omsIsExclusive; }
            set { _omsIsExclusive = value; }
        }
    }
}
