using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationDataAccess.DTO
{
    public class Piecewise
    {
        List<AgeValue> m_ageValueList;
        bool m_isAscending; // True when value is larger for better performance (example PCI).
        List<AgeValue> m_yearlyAgeValue;


        bool _isOMS = false;
        public string Errors { get; set; }

        public List<AgeValue> AgeValueList
        {
            get { return m_ageValueList; }
        }

        public List<AgeValue> YearlyAgeValue
        {
            get { return m_yearlyAgeValue; }
            set { m_yearlyAgeValue = value; }
        }

        /// <summary>
        /// Stores a list of age value pairs for a given performance equation.
        /// </summary>
        /// <param name="piecewise"></param>
        public Piecewise(string piecewise)
        {
            CreateList(piecewise);
            MakeMonotonic();
            MakeYearlyAgeValue();
            _isOMS = false;
        }

        

        /// <summary>
        /// Turn the input AgeValue pairs into a 100 year record.
        /// </summary>
        private void MakeYearlyAgeValue()
        {
            m_yearlyAgeValue = new List<AgeValue>();
            if (m_ageValueList.Count == 0)
            {
                this.Errors = "NO VALID AGE/VALUE PAIRS;";
                return;
            }
            double dAgeMinimum = m_ageValueList[0].Age;
            double dAgeMaximum = m_ageValueList[m_ageValueList.Count - 1].Age;

            int nAge = 0;
            while ((double)nAge < dAgeMinimum)
            {
                m_yearlyAgeValue.Add(new AgeValue((double)nAge, m_yearlyAgeValue[0].Value));
                nAge++;
            }
            
            int current = 0;
            while ((double)nAge < dAgeMaximum &&  nAge < 1000 && current < m_ageValueList.Count - 1)
            {
                double ageCurrent = m_ageValueList[current].Age;
                double ageNext = m_ageValueList[current + 1].Age;
                double valueCurrent = m_ageValueList[current].Value;
                double valueNext = m_ageValueList[current + 1].Value;

                if (ageCurrent <= (double)nAge && (double)nAge <= ageNext)
                {
                    //Calculate
                    double deltaAge = ageNext - ageCurrent;
                    double deltaValue = valueNext - valueCurrent;
                    double delta = (double)nAge - ageCurrent;

                    double value = valueCurrent + (delta * (deltaValue / deltaAge));
                    m_yearlyAgeValue.Add(new AgeValue((double)nAge, value));           
                    //Get next age
                    nAge++;
                }
                else
                {
                    //Add current
                    current++;
                }
            }

            while ((double)nAge >= dAgeMaximum && nAge < 1000)
            {
                m_yearlyAgeValue.Add(new AgeValue((double)nAge, m_ageValueList[m_ageValueList.Count - 1].Value));
                nAge++;
            }


        }

        /// <summary>
        /// Creates piecewise list from string
        /// </summary>
        /// <param name="piecewise">(age0, value0)(age1, value1)...</param>
        private void CreateList(string piecewise)
        {
            m_ageValueList = new List<AgeValue>();
            string[] pairs = piecewise.Split(')');
            foreach (string pair in pairs)
            {
                if (!string.IsNullOrWhiteSpace(pair))
                {
                    AgeValue ageValue = new AgeValue(pair);
                    if (ageValue.Error != null)
                    {
                        this.Errors += ageValue.Error;
                    }
                    m_ageValueList.Add(ageValue);
                }
            }
            //Sort by age.
            m_ageValueList.Sort((x, y) => x.Age.CompareTo(y.Age));

            //Remove bad values
            m_ageValueList.RemoveAll(delegate(AgeValue av) { return av.Age < 0; });
            m_ageValueList.RemoveAll(delegate(AgeValue av) { return av.Value == double.NaN; });

            SortByAgeAndValues();
        }

        /// <summary>
        /// Piecewise curves are either ascending or descending.
        /// </summary>
        private void DetermineAscendingDescending()
        {
            int count = m_ageValueList.Count;
            if (count <= 1) m_isAscending = true;
            else
            {
                if (m_ageValueList[0].Value > m_ageValueList[count - 1].Value) m_isAscending = true;
                else m_isAscending = false;
            }
        }

        /// <summary>
        /// Sorts by age and value.
        /// </summary>
        private void SortByAgeAndValues()
        {
            DetermineAscendingDescending();
            //Now resort with ascending/descending in mind
            m_ageValueList.Sort(
                delegate(AgeValue av1, AgeValue av2)
                {
                    int compareAge = av1.Age.CompareTo(av2.Age);
                    if (compareAge == 0)
                    {
                        if (m_isAscending)
                        {
                            return av2.Value.CompareTo(av1.Value);
                        }
                        else
                        {
                            return av1.Value.CompareTo(av2.Value);
                        }
                    }
                    return compareAge;
                }
            );
        }

        /// <summary>
        /// All piecewise curves must be monotonically increasing or descreasing.  This function enforces that.
        /// </summary>
        private void MakeMonotonic()
        {
            if (m_ageValueList.Count <= 1) return;//Montonic by definition.
            List<AgeValue> listRemove = new List<AgeValue>();
            //Create list of of duplicate ages.
            for(int i= 0; i < m_ageValueList.Count-1; i++)
            {
                if (m_ageValueList[i].Age == m_ageValueList[i + 1].Age) listRemove.Add(m_ageValueList[i]);
            }
            //Remove duplicate
            foreach (AgeValue ageValue in listRemove)
            {
                this.Errors += "DUPLICATE AGE" + ageValue.ToString() + ";";
                m_ageValueList.Remove(ageValue);
            }
            
            //Now remove values that are not descreasing (or increasing if !m_isAscending)
            listRemove = new List<AgeValue>();

            if (m_isAscending)
            {
                int nCurrent = 0;
                for (int i = 1; i < m_ageValueList.Count; i++)
                {
                    if (m_ageValueList[nCurrent].Value <= m_ageValueList[i].Value)
                    {
                        listRemove.Add(m_ageValueList[i]);
                    }
                    else
                    {
                        nCurrent = i;
                    }
                }
            }
            else
            {
                int nCurrent = 0;
                for (int i = 1; i < m_ageValueList.Count; i++)
                {
                    if (m_ageValueList[nCurrent].Value >= m_ageValueList[i].Value)
                    {
                        listRemove.Add(m_ageValueList[i]);
                    }
                    else
                    {
                        nCurrent = i;
                    }
                }
            }
            //Remove non-monotonic
            foreach (AgeValue ageValue in listRemove)
            {
                this.Errors += "NOT MONOTONICALLY INCREASING/DESCREASING " + ageValue.ToString() + ";";
                m_ageValueList.Remove(ageValue);
            }
        }

        /// <summary>
        /// Get remaining life with Shift
        /// </summary>
        /// <param name="value">Current value of dependent variable</param>
        /// <param name="deficient">Level at which dependent variable becomes difficient</param>
        /// <param name="age">Actual age with which to corret apparent age.</param>
        /// <returns></returns>
        public double GetRemainingLife(double value, double deficient, double age)
        {
            double apparentAge = GetApparentAge(value,0);
            double deficientAge = GetApparentAge(deficient,0);
            double remainingLife = deficientAge-apparentAge;
            if (apparentAge > 0)
            {
                double ratio = age / apparentAge;
                remainingLife = remainingLife * ratio;
            }
            return remainingLife;
        }
      
        /// <summary>
        /// Retrieves the number of years to move between current value and deficient value.
        /// </summary>
        /// <param name="value">Current value of the dependent variable</param>
        /// <param name="deficient">Level at which dependent variable becomes deficient</param>
        /// <returns></returns>
        public double GetRemainingLife(double value, double deficient)
        {
            double apparentAge = GetApparentAge(value,0);
            double deficientAge = GetApparentAge(deficient,0);
            double remainingLife = deficientAge - apparentAge;
            if (remainingLife < 0) remainingLife = 0;
            return remainingLife;
        }

        /// <summary>
        /// Returns the benefit for a current value using this curve
        /// </summary>
        /// <param name="value">Current value of dependent variable</param>
        /// <param name="benefitLimit">Limit at which benefit is no longer consider (very important for descending variables - example IRI</param>
        /// <returns></returns>
        public double GetBenefit(double value, double benefitLimit)
        {
            return GetBenefit(value, benefitLimit, 0, false);
        }
        
        /// <summary>
        /// Returns the benefit for a current value using this curve
        /// </summary>
        /// <param name="value">Current value of dependent variable</param>
        /// <param name="benefitLimit">Limit at which benefit is no longer consider (very important for descending variables)</param>
        /// <param name="weighting">Weighting variable for determination of total benefit</param>
        /// <param name="isWeighted">Does this analysis use a weighting variable</param>
        /// <returns>Weighted total benefit</returns>
        public double GetBenefit(double value, double benefitLimit , double weighting, bool isWeighted)
        {
            double benefit = 0;
            if (!isWeighted) weighting = 1;

            for (int i = 0; i < m_yearlyAgeValue.Count - 1; i++)
            {
                if (m_isAscending && m_yearlyAgeValue[i].Value > m_yearlyAgeValue[i + 1].Value && m_yearlyAgeValue[i].Value <= value)
                {
                    double incrementalBenefit = m_yearlyAgeValue[i].Value - benefitLimit; 
                    if(incrementalBenefit > 0) benefit += weighting * incrementalBenefit;
                }
                else if (!m_isAscending && m_yearlyAgeValue[i].Value < m_yearlyAgeValue[i + 1].Value && m_yearlyAgeValue[i].Value >= value)
                {
                    double incrementalBenefit = benefitLimit - benefitLimit;
                    if(incrementalBenefit > 0) benefit += weighting * incrementalBenefit;
                }
            }
            return benefit;
        }


        /// <summary>
        /// Gets the benefit in case of shift without weighting
        /// </summary>
        /// <param name="value">Current value of dependent variable</param>
        /// <param name="benefitLimit">Limit at which dependent variable is not accrued.</param>
        /// <param name="age">Current age</param>
        /// <returns></returns>
        public double GetBenefit(double value, double benefitLimit, double age)
        {
            return GetBenefit(value, benefitLimit, 0, false, age);
        }


        /// <summary>
        /// Gets benefit in case of shift.
        /// </summary>
        /// <param name="value">Current value of dependent variable</param>
        /// <param name="benefitLimit">Limit at which dependent variable is not accrued.</param>
        /// <param name="weighting">Weighting variable</param>
        /// <param name="isWeighted">Is this variable weighted</param>
        /// <param name="age">Current Age</param>
        /// <returns>Shifted benefit</returns>
        public double GetBenefit(double value, double benefitLimit, double weighting, bool isWeighted, double age)
        {
            double apparentAge = GetApparentAge(value,0);
            double benefit = GetBenefit(value, benefitLimit, weighting, isWeighted);
            if (apparentAge > 0)
            {
                double ratio = age/apparentAge;
                benefit = benefit * ratio;
            }
            return benefit;
        }


        public bool GetOMSApparentAge(double value, out int age, out double ratio)
        {
            age = 0;
            ratio = 0;
            for (int i = 0; i < m_yearlyAgeValue.Count - 1; i++)
            {
                if (m_yearlyAgeValue[i].Value == value)
                {
                    age = i;
                    ratio = 0;
                    return true;
                }
                else if (m_isAscending && m_yearlyAgeValue[i].Value >= m_yearlyAgeValue[i + 1].Value && m_yearlyAgeValue[i].Value >= value && value > m_yearlyAgeValue[i + 1].Value)
                {
                    double deltaBottom = m_yearlyAgeValue[i + 1].Value - m_yearlyAgeValue[i].Value;
                    double deltaTop = value - m_yearlyAgeValue[i].Value;
                    ratio = deltaTop / deltaBottom;
                    age = i;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Returns the current apparent age of input value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double GetApparentAge(double value, int hint)
        {
            for (int i = hint; i < m_yearlyAgeValue.Count - 1; i++)
            {
                if (m_yearlyAgeValue[i].Value == value)
                {
                    return (double)i;
                }
                else if (m_isAscending && m_yearlyAgeValue[i].Value >= m_yearlyAgeValue[i + 1].Value && m_yearlyAgeValue[i].Value >= value && value > m_yearlyAgeValue[i + 1].Value)
                {
                    double deltaBottom = m_yearlyAgeValue[i + 1].Value - m_yearlyAgeValue[i].Value;
                    double deltaTop = value - m_yearlyAgeValue[i].Value;
                    double ratio = deltaTop / deltaBottom;
                    return (double)i + ratio;
                }
                else if (!m_isAscending && m_yearlyAgeValue[i].Value <= m_yearlyAgeValue[i + 1].Value && m_yearlyAgeValue[i].Value <= value && value < m_yearlyAgeValue[i + 1].Value)
                {
                    double deltaBottom = m_yearlyAgeValue[i + 1].Value - m_yearlyAgeValue[i].Value;
                    double deltaTop = value - m_yearlyAgeValue[i].Value;
                    double ratio = deltaTop / deltaBottom;
                    return (double)i + ratio;
                }
            }


            for (int i = 0; i < hint; i++)
            {
                if (m_yearlyAgeValue[i].Value == value)
                {
                    return (double)i;
                }
                else if (m_isAscending && m_yearlyAgeValue[i].Value >= m_yearlyAgeValue[i + 1].Value && m_yearlyAgeValue[i].Value >= value && value > m_yearlyAgeValue[i + 1].Value)
                {
                    double deltaBottom = m_yearlyAgeValue[i + 1].Value - m_yearlyAgeValue[i].Value;
                    double deltaTop = value - m_yearlyAgeValue[i].Value;
                    double ratio = deltaTop / deltaBottom;
                    return (double)i + ratio;
                }
                else if (!m_isAscending && m_yearlyAgeValue[i].Value <= m_yearlyAgeValue[i + 1].Value && m_yearlyAgeValue[i].Value <= value && value < m_yearlyAgeValue[i + 1].Value)
                {
                    double deltaBottom = m_yearlyAgeValue[i + 1].Value - m_yearlyAgeValue[i].Value;
                    double deltaTop = value - m_yearlyAgeValue[i].Value;
                    double ratio = deltaTop / deltaBottom;
                    return (double)i + ratio;
                }
            }



            return 0;
        }



        /// <summary>
        /// Returns the current value adjusted for AGE (shift).
        /// </summary>
        /// <param name="value">Current value of dependent variable</param>
        /// <param name="age">Current age</param>
        /// <returns>Shifted value</returns>
        public AgeValue GetNextValue(double value, double age, double span, out double apparentAge)
        {
            AgeValue ageValue = null;
            apparentAge = GetApparentAge(value,0);
            if (age > 0)
            {
                double ratio = apparentAge / age;
                double nextAge = apparentAge + ratio*span;
                double nextValue = GetValue(nextAge);
                ageValue = new AgeValue(nextAge, nextValue);
            }
            else
            {
                ageValue =  m_yearlyAgeValue[0];
            }
            return ageValue;
        }

        ///<summary>
        /// Used in ConditionIndex to calculate conditionIndex at age + fractionAge
        /// </summary>
        /// <param name="age"></param>
        /// <param name="_fractionalYear"></param>
        /// <returns></returns>
        public double GetValue(int age, double _fractionalYear)
        {
            if (age >= m_yearlyAgeValue.Count - 1) return m_yearlyAgeValue[m_yearlyAgeValue.Count - 1].Value;
            int nextAge = age + 1;
            double deltaValue = m_yearlyAgeValue[age].Value - m_yearlyAgeValue[nextAge].Value;
            double value = m_yearlyAgeValue[age].Value - (_fractionalYear * deltaValue);
            return value;
        }

        /// <summary>
        /// Given a particular age what is corresponding value.
        /// </summary>
        /// <param name="age">Apparent age to lookup</param>
        /// <returns></returns>
        public double GetValue(double age)
        {
            int nAge = (int)age;
            double delta = age - (double)nAge;

            if (nAge >= m_yearlyAgeValue.Count-1) return m_yearlyAgeValue[m_yearlyAgeValue.Count - 1].Value;

            int nextAge = nAge + 1;
            double deltaValue = m_yearlyAgeValue[nAge].Value - m_yearlyAgeValue[nextAge].Value;
            double value = m_yearlyAgeValue[nAge].Value - (delta * deltaValue);

            return value;
        }
       

        /// <summary>
        /// Gets next value on curve. Age a single apparent year
        /// </summary>
        /// <param name="value">Current value</param>
        /// <returns>Value after aging an apparent year</returns>
        public AgeValue GetNextValue(double value, double span)
        {
            double apparentAge = GetApparentAge(value,0);
            apparentAge += span;//Age one year
            return new AgeValue(apparentAge,GetValue(apparentAge));
        }

        public AgeValue GetNextValue(double value, double span, int apparentAgeHint, out double apparentAge)
        {
            apparentAge = GetApparentAge(value, apparentAgeHint);
            apparentAge += span;//Age one year
            return new AgeValue(apparentAge, GetValue(apparentAge));
        }


        public override string ToString()
        {
            string piecewise = "";
            foreach (AgeValue av in m_ageValueList)
            {
                piecewise += av.ToString();
            }
            return piecewise;
        }


    }

    /// <summary>
    /// Age value pair from piecewise equation
    /// </summary>
    public class AgeValue
    {
        public double Age { get; set; }
        public double Value { get; set; }
        public string Error { get; set; }


        public AgeValue()
        {
        }

        public AgeValue(double age, double value)
        {
            this.Age = age;
            this.Value = value;
        }
        
        /// <summary>
        /// Stores an age value pair from a piecewise equation
        /// </summary>
        /// <param name="ageValue">Takes string of form (age,value)</param>
        public AgeValue(string ageValue)
        {
            ageValue = ageValue.Replace("(", "").Replace(")", "").Trim(); ;
            try
            {
                string[] values = ageValue.Split(',');
                
                this.Age = Convert.ToDouble(values[0]);
                this.Value = Convert.ToDouble(values[1]);
                
                this.Error = null;
            }
            catch// 
            {
                this.Error = "PARSE ERROR (" + ageValue + ");"; 
                this.Age = -1;
                this.Value = double.NaN;
            }
        }

        /// <summary>
        /// Outputs Age and Value in form (age,value)
        /// </summary>
        /// <returns>String of value (age,value)</returns>
        public override string ToString()
        {
            return "(" + Age.ToString() + "," + Value.ToString() + ")";
        }
    }
}
