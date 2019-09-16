using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class CommittedExport
    {
        String m_sTreatmentName;
        String m_strFacility;
        String m_strSection;
        String m_strBegin;
        String m_strEnd;
        String m_strDirection;
        String m_strYear;
        String m_strArea;

        String m_strCost;
        String m_sBudget;
        String m_strAny;
        String m_strSame;
        String m_sConsequenceID;
        List<ConsequenceExport> consequence;

        public String Treatment
        {
            get { return m_sTreatmentName; }
            set { m_sTreatmentName = value; }
        }

        public String Cost
        {
            get { return m_strCost; }
            set { m_strCost = value; }
        }

        public String Budget
        {
            get { return m_sBudget; }
            set { m_sBudget = value; }
        }

        public String Year
        {
            get { return m_strYear; }
            set { m_strYear = value; }
        }

        public String Any
        {
            get { return m_strAny; }
            set { m_strAny = value; }
        }

        public String Area
        {
            get { return m_strArea; }
            set { m_strArea = value; }
        }
        public String Same
        {
            get { return m_strSame; }
            set { m_strSame = value; }
        }

        public String ConsequenceID
        {
            get { return m_sConsequenceID; }
            set { m_sConsequenceID = value; }
        }

        public List<ConsequenceExport> Consequence
        {
            get { return consequence; }
            set { consequence = value; }
        }

        public String Facility
        {
            get { return m_strFacility; }
            set { m_strFacility = value; }
        }

        public String Section
        {
            get { return m_strSection; }
            set { m_strSection = value; }
        }
        public String BeginStation
        {
            get { return m_strBegin; }
            set { m_strBegin = value; }
        }
        public String EndStation
        {
            get { return m_strEnd; }
            set { m_strEnd = value; }
        }
        public String Direction
        {
            get { return m_strDirection; }
            set { m_strDirection = value; }
        }


    }
}
