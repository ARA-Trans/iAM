using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    public class AppliedTreatment
    {
        String m_sTreatmentName;
        String m_sSectionID;
        float m_fCost;
        String m_sBudget;
        int m_nAny;
        int m_nSame;
        String m_sTreatmentID;
        double m_dBCRatio;
        int m_nTargetDeficient;
        double m_dRemaining;
        double m_dBenefit;
        String m_sRLHash;
        double m_dSelectionWeight;
        int m_nYear;
        bool m_bAvailable;
        String m_strChangeHash;
        AppliedTreatment _multipleTreatment;//Stores a follow up treatment.
        bool _isExclusive;
        //For split treatment (before split).
        public float TreatmentOnlyCost { get; set; }
        public Dictionary<string, float> ScheduledCost {get;set;}


        public bool IsExclusive
        {
            get { return _isExclusive; }
            set { _isExclusive = value; }
        }

        public AppliedTreatment MultipleTreatment
        {
            get { return _multipleTreatment; }
            set { _multipleTreatment = value; }
        }


        public String Treatment
        {
            get { return m_sTreatmentName; }
            set { m_sTreatmentName = value; }
        }

        public String SectionID
        {
            get { return m_sSectionID; }
            set { m_sSectionID = value; }
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

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        public bool Available
        {
            get { return m_bAvailable; }
            set { m_bAvailable = value; }
        }


        public String TreatmentID
        {
            get { return m_sTreatmentID; }
            set { m_sTreatmentID = value; }
        }

        public double BenefitCostRatio
        {
            get { return m_dBCRatio; }
            set { m_dBCRatio = value; }
        }

        public int NumberTreatmentDeficient
        {
            get { return m_nTargetDeficient; }
            set { m_nTargetDeficient = value; }
        }

        public double Benefit
        {
            get { return m_dBenefit; }
            set { m_dBenefit = value; }
        }

        public double RemainingLife
        {
            get { return m_dRemaining; }
            set { m_dRemaining = value; }
        }
        public String RemainingLifeHash
        {
            get { return m_sRLHash; }
            set { m_sRLHash = value; }
        }
        public double SelectionWeight
        {
            get { return m_dSelectionWeight; }
            set { m_dSelectionWeight = value; }

        }
        public String ChangeHash
        {
            get { return m_strChangeHash; }
            set { m_strChangeHash = value; }
        }

        public double TotalBenefitCostRatio
        {
            get
            {
                return TotalBenefit() / TotalCost();
            }
        }



        private double TotalBenefit()
        {
            double totalBenefit = this.Benefit;
            if (_multipleTreatment != null)
            {
                totalBenefit += _multipleTreatment.TotalBenefit();
            }
            return totalBenefit;
        }

        private double TotalCost()
        {
            double totalCost = this.Cost;
            if (_multipleTreatment != null)
            {
                totalCost += _multipleTreatment.TotalCost();
            }
            return totalCost;
        }

        public override string ToString()
        {

            string appliedTreatment = this.SectionID.ToString() + " - " + m_sTreatmentName + "(" +TotalBenefitCostRatio.ToString("f0") + ")";
            if (_multipleTreatment != null)
            {
                appliedTreatment = _multipleTreatment.ToString() + "|" + appliedTreatment;
            }
            return appliedTreatment;
        }


        public bool SetUnavailableIfCommitted(string treatment)
        {
            if(this._multipleTreatment != null)
            {
                m_bAvailable = _multipleTreatment.SetUnavailableIfCommitted(treatment);
            }
            if(m_sTreatmentName == treatment)
            {
                m_bAvailable = false;
            }
            return m_bAvailable;
        }
    }
}
