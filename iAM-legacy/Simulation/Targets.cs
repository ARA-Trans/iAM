using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Simulation
{
    public class Targets
    {
        String m_sAttribute;
        int m_nYear;
        String  m_sTargetID;
        double m_dTarget;
        double m_dDeficient;
        double m_dDeficientPercentage;
        bool m_bAllYears;
        bool m_bTargets;
        bool m_bDeficiency;
        bool m_bPercentage;
        bool m_bAllSections;

        private string _table;
        private string _column;
        string _targetName;

        public int SectionCount { get; set; }

        public Hashtable m_hashYearTargetArea = new Hashtable();//Key int year, object double sum of ATTRIBUTE * AREA
        public Hashtable m_hashYearDeficientArea = new Hashtable();// Key int year, object double of sum of deficient areas meeting criteria.
        public Hashtable m_hashYearSumArea = new Hashtable();// Key int year, object double of sum of all areas meeting criteria. 


        Criterias m_criteria;

        public Targets(string id, string table)
        {
            m_sTargetID = id;
            _table = table;
            _column = "BINARY_CRITERIA";
            m_criteria = new Criterias(_table, _column, id);
        }

        /// <summary>
        /// Add area to the total area for this target.
        /// </summary>
        /// <param name="dArea">Area of the section</param>
        /// <param name="nYear">Current year</param>
        public void AddArea(double dArea, int nYear)
        {
            double dSumArea = 0;
            if (m_hashYearSumArea.Contains(nYear))
            {
                dSumArea = (double)m_hashYearSumArea[nYear];
                m_hashYearSumArea.Remove(nYear);
            }

            dSumArea += dArea;
            m_hashYearSumArea.Add(nYear, dSumArea);
        }


        /// <summary>
        /// Add weighte area to the total weigthsarea for this target.
        /// </summary>
        /// <param name="dArea">Area of the section</param>
        /// <param name="nYear">Current year</param>
        public void AddTargetArea(double dArea,double dValue, int nYear)
        {
            double dSumArea = 0;
            if (m_hashYearTargetArea.Contains(nYear))
            {
                dSumArea = (double)m_hashYearTargetArea[nYear];
                m_hashYearTargetArea.Remove(nYear);
            }

            dSumArea += dArea*dValue;
            m_hashYearTargetArea.Add(nYear, dSumArea);
        }

        /// <summary>
        /// Add area to the total deficient area for this deficient.
        /// </summary>
        /// <param name="dArea">Area of the section</param>
        /// <param name="nYear">Current year</param>
        public void AddDeficientArea(double dArea, int nYear)
        {
            double dSumArea = 0;
            if (m_hashYearDeficientArea.Contains(nYear))
            {
                dSumArea = (double)m_hashYearDeficientArea[nYear];
                m_hashYearDeficientArea.Remove(nYear);
            }

            dSumArea += dArea;
            m_hashYearDeficientArea.Add(nYear, dSumArea);
        }


        /// <summary>
        /// Subtract area to the total area for this target.
        /// </summary>
        /// <param name="dArea">Area of the section</param>
        /// <param name="nYear">Current year</param>
        public void SubtractArea(double dArea, int nYear)
        {
            double dSumArea = 0;
            if (m_hashYearSumArea.Contains(nYear))
            {
                dSumArea = (double)m_hashYearSumArea[nYear];
                m_hashYearSumArea.Remove(nYear);
            }

            dSumArea -= dArea;
            if (dSumArea < 0) dSumArea = 0;
            m_hashYearSumArea.Add(nYear, dSumArea);
        }


        /// <summary>
        /// Subtract weighted area to the total weighted area for this target.
        /// </summary>
        /// <param name="dArea">Area of the section</param>
        /// <param name="nYear">Current year</param>
        public void SubtractTargetArea(double dArea, double dValue, int nYear)
        {
            double dSumArea = 0;
            if (m_hashYearTargetArea.Contains(nYear))
            {
                dSumArea = (double)m_hashYearTargetArea[nYear];
                m_hashYearTargetArea.Remove(nYear);
            }

            dSumArea -= dArea * dValue;
            if (dSumArea < 0) dSumArea = 0;
            m_hashYearTargetArea.Add(nYear, dSumArea);
        }


        /// <summary>
        /// Subtract area to the total deficient area for this deficient.
        /// </summary>
        /// <param name="dArea">Area of the section</param>
        /// <param name="nYear">Current year</param>
        public void SubtractDeficientArea(double dArea, int nYear)
        {
            double dSumArea = 0;
            if (m_hashYearDeficientArea.Contains(nYear))
            {
                dSumArea = (double)m_hashYearDeficientArea[nYear];
                m_hashYearDeficientArea.Remove(nYear);
            }

            dSumArea -= dArea;
            if (dSumArea < 0) dSumArea = 0;
            m_hashYearDeficientArea.Add(nYear, dSumArea);

        }

        /// <summary>
        /// True if this target is met.
        /// </summary>
        /// <returns></returns>
        public bool IsTargetMet(int nYear)
        {
            double dTargetArea = 0;
            if (m_hashYearTargetArea.Contains(nYear))
            {
                dTargetArea = (double)this.m_hashYearTargetArea[nYear];
            }


            double dSumArea = 0;
            if (this.m_hashYearSumArea.Contains(nYear))
            {
                dSumArea = (double)this.m_hashYearSumArea[nYear];
            }
            if(dSumArea == 0) return true;

            double dTarget = dTargetArea / dSumArea;
            double dGoal = this.Target;

            if (SimulationMessaging.GetAttributeAscending(this.Attribute))
            {
                if (dGoal < dTarget)
                {
                    return true;

                }
            }
            else
            {
                if (dGoal > dTarget)
                {
                    return true;

                }
            }
            return false;
        }


        public double CurrentTarget(int year)
        {
            double dTargetArea = 0;
            if (m_hashYearTargetArea.Contains(year))
            {
                dTargetArea = (double)this.m_hashYearTargetArea[year];
            }


            double dSumArea = 0;
            if (this.m_hashYearSumArea.Contains(year))
            {
                dSumArea = (double)this.m_hashYearSumArea[year];
            }
            if (dSumArea == 0) return 0;

            double dTarget = dTargetArea / dSumArea;
            return dTarget;
        }




        /// <summary>
        /// True if Deficient goal is met
        /// </summary>
        /// <returns></returns>
        public bool IsDeficientMet(int nYear)
        {

            double dTargetArea = 0;

            if (m_hashYearDeficientArea.Contains(nYear))
            {
                dTargetArea = (double)this.m_hashYearDeficientArea[nYear];
            }
  
            double dSumArea = 0;
            if (m_hashYearSumArea.Contains(nYear))
            {
                dSumArea = (double)this.m_hashYearSumArea[nYear];
            }
            if (dSumArea == 0) return true;

            double dTarget = dTargetArea / dSumArea;
            double dGoal = this.DeficientPercentage/100;

            if (dGoal >= dTarget)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// Attribute for which target is set
        /// </summary>
        public String Attribute
        {
            get { return m_sAttribute; }
            set { m_sAttribute = value; }
        }

        public String ID
        {
            get { return m_sTargetID; }
            set { m_sTargetID = value; }
        }
        /// <summary>
        /// Year for which Target is for.  Left blank it is used for all years.
        /// </summary>
        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        /// <summary>
        /// Network Target (average)
        /// </summary>
        public double Target
        {
            get { return m_dTarget; }
            set { m_dTarget = value; }
        }

        /// <summary>
        /// Deficient Target (average)
        /// </summary>
        public double Deficient
        {
            get { return m_dDeficient; }
            set { m_dDeficient = value; }
        }

        /// <summary>
        /// Deficient Target (average)
        /// </summary>
        public double DeficientPercentage
        {
            get { return m_dDeficientPercentage; }
            set { m_dDeficientPercentage = value; }
        }

        /// <summary>
        /// Criteria to cause this target to be in effect
        /// </summary>
        public Criterias Criteria
        {
            get { return m_criteria; }
            set { m_criteria = value; }
        }


        /// <summary>
        /// Is true if the year column is null or blank
        /// </summary>
        public bool IsAllYears
        {
            get { return m_bAllYears; }
            set { m_bAllYears = value; }
        }

        /// <summary>
        /// Is true if the Criteria column is null or blank
        /// </summary>
        public bool IsAllSections
        {
            get { return m_bAllSections; }
            set { m_bAllSections = value; }
        }


        /// <summary>
        /// Is true if Target is NOT blank or NULL(i.e. there is a Target available)
        /// </summary>
        public bool IsTargets
        {
            get { return m_bTargets; }
            set { m_bTargets = value; }
        }

        /// <summary>
        /// Is true if Deficiency is NOT blank or NULL(i.e. the is a deficiency target available
        /// </summary>
        public bool IsDeficient
        {
            get { return m_bDeficiency; }
            set { m_bDeficiency = value; }
        }

        /// <summary>
        /// Is true if Deficiency is NOT blank or NULL
        /// </summary>
        public bool IsDeficiencyPercentage
        {
            get { return m_bPercentage; }
            set { m_bPercentage = value; }
        }


        public string TargetName
        {
            get { return _targetName; }
            set { _targetName = value; }
        }

    }
}
