using System;
using System.Collections.Generic;
using System.Text;
using DatabaseManager;
using System.Collections;
using System.Data;
using System.IO;

namespace Simulation
{
    class Sections
    {
        String m_strSectionID;
        String m_strFacility;
        String m_strSection;

        int m_nNumberTreatment;
        float m_fBeginStation=-1;
        float m_fEndStation=-1;
        String m_sDirection;
        float m_fArea=-1;
        //bool m_bRemainingLife;
        //bool m_bBenefit;

        public Hashtable m_hashAttributeYearValues = new Hashtable();// Key Attribute. Object Hashtable of Year/Value pairs (for roll Forward).
        public Hashtable m_hashYearAttributeValues = new Hashtable();// Key year.  Object hash attribute  value. (for simulation).

        Hashtable m_hashYearTreatment = new Hashtable(); // Key is Year. Object is a Treatments String.
        public List<SameTreatment> m_listSame = new List<SameTreatment>();// List of SameTreatments
        int m_nAny = 0; //If the current year is <= than m_nAny, no treatment can occur.

        int m_nStartYear;

        double m_dBaseBenefit;
        double m_dRemainingLife;
        Hashtable m_hashYearRemainingLife = new Hashtable();//Key - year Object Hash Attribute Remaining life for each attribute.
        public Hashtable m_hashNextAttributeValue = new Hashtable(); //Attribute value for current analysis year.
 //       Hashtable m_hashYearCommitted = new Hashtable();
        List<Committed> _yearCommitted = new List<Committed>();

        public int NumberTreatment
        {
            get { return m_nNumberTreatment; }
            set { m_nNumberTreatment = value; }
        }
        
        
        public bool Treated { get; set; }


        public List<Committed> YearCommit { get; set; }

        /// <summary>
        /// SectionID from RoadCare SECTION_networkid TABLE
        /// </summary>
        public String SectionID
        {
            get { return m_strSectionID; }
            set { m_strSectionID = value; }
        }


        /// <summary>
        /// Section display name.
        /// </summary>
        public String Facility
        {
            get { return m_strFacility; }
            set { m_strFacility = value; }
        } 
        
        /// <summary>
        /// Section display name.
        /// </summary>
        public String Section
        {
            get { return m_strSection; }
            set { m_strSection = value; }
        }


        /// <summary>
        /// Begin station
        /// </summary>
        public float Begin
        {
            get { return m_fBeginStation; }
            set { m_fBeginStation = value; }
        }


        /// <summary>
        /// End station
        /// </summary>
        public float End
        {
            get { return m_fEndStation; }
            set { m_fEndStation = value; }
        }

        /// <summary>
        /// Area
        /// </summary>
        public String Direction
        {
            get { return m_sDirection; }
            set { m_sDirection = value; }
        }


        /// <summary>
        /// Area
        /// </summary>
        public float Area
        {
            get { return m_fArea; }
            set { m_fArea = value; }
        }

        /// <summary>
        /// Area.
        /// </summary>
        public float Length
        {
            get { return m_fEndStation-m_fBeginStation; }
        }
        /// <summary>
        /// Is Benefit being calculated
        /// </summary>
        //public bool IsBenefit
        //{
        //    get { return m_bBenefit; }
        //    set { m_bBenefit = value; }
        //}


        /// <summary>
        /// Is Remaining life being calculated
        /// </summary>
        //public bool IsRemainingLife
        //{
        //    get { return m_bRemainingLife; }
        //    set { m_bRemainingLife = value; }
        //}
        
        /// <summary>
        /// Base benefit for no treatment
        /// </summary>
        public double BaseBenefit
        {
            get { return m_dBaseBenefit; }
            set { m_dBaseBenefit = value; }
        }

        /// <summary>
        /// Base remaining life for no treatement
        /// </summary>
        public double RemainingLife
        {
            get { return m_dRemainingLife; }
            set { m_dRemainingLife = value; }
        }

        public int AnyYear
        {
            get { return m_nAny; }
            set { m_nAny = value; }
        }
        //Year that we are rolling forward to.
        public int RollToYear
        {
            get { return m_nStartYear; }
            set { m_nStartYear = value-1; }
        }



        public void AddRemainingLifeHash(int nYear)
        {
            if (m_hashYearRemainingLife.Contains(nYear))
            {
                Hashtable hash = (Hashtable)m_hashYearRemainingLife[nYear];
                hash.Clear();
            }
            else
            {
                Hashtable hash = new Hashtable();
                m_hashYearRemainingLife.Add(nYear, hash);
            }
        }

        ///Default constructor
        public Sections()
        {

        }

        /// <summary>
        /// Constructor for Single year run to reload previous information
        /// </summary>
        /// <param name="row">DataRow from SIMULATION_ table return</param>
        /// <param name="nStartYear">Start year of analysis</param>
        /// <param name="nAnalysisPeriod">Number of years for analysis</param>
        /// <param name="m_listAttribute">List of attributes in analysis</param>
        public Sections(DataRow row, int nStartYear, int nAnalysisPeriod, List<String> listAttribute)
        {
            this.SectionID = row["SECTIONID"].ToString();
            this.Facility = row["FACILITY"].ToString();
            this.Section = row["SECTION"].ToString();
            this.Direction = row["DIRECTION"].ToString();

            if (row["BEGIN_STATION"].ToString().Trim() != "") this.Begin = float.Parse(row["BEGIN_STATION"].ToString());
            if (row["END_STATION"].ToString().Trim() != "") this.End = float.Parse(row["END_STATION"].ToString());
            if (row["AREA"].ToString().Trim() != "") this.Area = float.Parse(row["AREA"].ToString());
            
            for (int nYear = nStartYear; nYear < nStartYear + nAnalysisPeriod; nYear++)
            {
                String strYear = nYear.ToString();
                Hashtable hashAttributeValue = new Hashtable();
                foreach (String attribute in listAttribute)
                {
                    object value = row[attribute + "_" + strYear];
                    hashAttributeValue.Add(attribute, value);
                }
                m_hashYearAttributeValues.Add(int.Parse(strYear), hashAttributeValue);
            }
        }


        public Sections(string sectionID)
        {
            this.SectionID = sectionID;
        }



        /// <summary>
        /// Returns true if a Treatment has not already been set or withing Same/Any shadow
        /// </summary>
        /// <returns></returns>
        public bool IsTreatmentAllowed(String sTreatment,String sAny,String sSame, int nYear)
        {

            //Already treated.
            if (Treated) return false;
            int nAny = int.Parse(sAny);
            int nSame = int.Parse(sSame);
            // Check if cast shadow on committed.
            List<Committed> currentYearCommitted = _yearCommitted.FindAll(delegate(Committed c) { return c.Year == nYear; });
            foreach(Committed committed in currentYearCommitted)
            {
                int key = committed.Year;
                if (key > nYear && key < nYear + nAny) return false;
                if (key > nYear && key < nYear + committed.Same && committed.Treatment == sTreatment) return false;
            }

            //Check if it falls in shadow of previous treatment.
            if (this.AnyYear > nYear) return false;

            foreach (SameTreatment sameTreatment in m_listSame)
            {
                if (sameTreatment.strTreatment == sTreatment)
                {
                    if (nYear < sameTreatment.nYear) return false;
                }
            }
            return true;
        }


        public void AddAttributeValue(String strColumn, object value)
        {
            String strAttribute = strColumn.Substring(0, strColumn.Length - 5);
            String strYear = strColumn.Substring(strColumn.Length - 4);
            Hashtable hash = (Hashtable)m_hashAttributeYearValues[strAttribute];
            if (hash == null)
            {
                hash = new Hashtable();
                m_hashAttributeYearValues.Add(strAttribute, hash);
            }

               value = SimulationMessaging.ConvertToRoadCareObject(value, strAttribute);
               if (value != null)
               {
                   hash.Add(strYear, value);
               }
        }

        public void RollForward(List<Deteriorate> listDeteriorate, List<String> listAttribute, List<CalculatedAttribute> listCalculatedAttributes)
        {
            //All possible data is now in Section.  Do a quick check to see if all data is
            //present in current year.  If it is we are done.

            int nYear = this.RollToYear;
            object valueRollForward = null;
            Hashtable hash;
            Hashtable hashRollForward = new Hashtable();
            List<String> listMissing = new List<String>();
            foreach (String str in listAttribute)
            {
                if (str == "SECTIONID")
                {
                    hashRollForward.Add("SECTIONID", this.SectionID);
                    continue;
                }

                hash = (Hashtable) m_hashAttributeYearValues[str.ToUpper()];
                if (hash != null && hash.Contains(nYear.ToString()))
                {
                    if (hash[nYear.ToString()] != null && !hashRollForward.Contains(str))
                    {
                        valueRollForward = hash[nYear.ToString()];
                        hashRollForward.Add(str, valueRollForward);
                    }
                    else //"" Blank (and need to roll forward.
                    {
                        listMissing.Add(str);
                    }
                }
                else
                {
                    listMissing.Add(str);
                }
            }

            List<String> listAttributeDeteriorate = new List<String>();

            foreach (String str in listMissing)
            {
                //For each attribute WITHOUT a deterioration curve, find lastest value.
                if (!SimulationMessaging.DeteriorateAttributes.Contains(str))
                {
                    int nMaximumYear = 0;
                    valueRollForward = null;

                    hash = (Hashtable) m_hashAttributeYearValues[str];
                    if (hash != null)
                    {
                        foreach (String year in hash.Keys)
                        {


                            if (hash[year] != null && int.Parse(year) > nMaximumYear)
                            {
                                valueRollForward = hash[year];
                                nMaximumYear = int.Parse(year);
                            }
                        }
                    }

                    //At least one value was found
                    if (nMaximumYear > 0)
                    {
                        if (!hashRollForward.Contains(str))
                        {
                            hashRollForward.Add(str, valueRollForward);
                        }
                    }
                    else
                    {
                        if (!hashRollForward.Contains(str))
                        {
                            if (str == "AREA")
                            {
                                valueRollForward = this.Area;
                            }
                            else if (str == "LENGTH")
                            {
                                valueRollForward = this.Length;
                            }
                            else
                            {
                                valueRollForward =
                                    SimulationMessaging.ConvertToRoadCareObject(
                                        SimulationMessaging.GetAttributeDefault(str), str);
                            }

                            hashRollForward.Add(str, valueRollForward);
                            //SimulationMessaging.AddMessage("Default value for " + str + " rolled forward for " + this.Facility + " " + this.Section);
                        }
                    }

                }
                else
                {
                    listAttributeDeteriorate.Add(str);
                }
            }

            Hashtable hashYearForward = new Hashtable();

            foreach (String str in listAttributeDeteriorate)
            {
                //Find the last year the in which this attribute has a valid value.

                int nMaximumYear = 0;
                valueRollForward = null;
                hash = (Hashtable) m_hashAttributeYearValues[str];
                if (hash != null)
                {
                    foreach (String year in hash.Keys)
                    {
                        if (hash[year] != null && int.Parse(year) > nMaximumYear)
                        {
                            valueRollForward = hash[year];
                            nMaximumYear = int.Parse(year);
                        }
                    }
                }

                if (nMaximumYear > 0)
                {
                    int nYearForward = this.RollToYear - nMaximumYear;
                    //Set this value in the hashRollForward
                    hashRollForward.Add(str, valueRollForward);
                    hashYearForward.Add(str, nYearForward);
                }
                else // Value is not there.
                {
                    valueRollForward =
                        SimulationMessaging.ConvertToRoadCareObject(SimulationMessaging.GetAttributeDefault(str), str);
                    hashRollForward.Add(str, valueRollForward);
                    hashYearForward.Add(str, 0);
                }
            }

            var originalValues = new Hashtable();
            foreach (var key in hashRollForward.Keys)
            {
                originalValues.Add(key,hashRollForward[key]);
            }
            //Run performance equations for 
            foreach (String str in listAttributeDeteriorate)
            {
                //List of possible rolled forwards
                var possibleValues = new Dictionary<string, double>();

                foreach (Deteriorate deteriorate in listDeteriorate)
                {
                    if (deteriorate.Attribute == str)
                    {
                        if (deteriorate.IsCriteriaMet(hashRollForward))
                        {

                            hashRollForward[str] = originalValues[str];


                            int nYearForward = (int) hashYearForward[str];
                            for (int j = 0; j < nYearForward; j++)
                            {
                                bool bOutOfRange;
                                double dValue = deteriorate.IterateOneYear(hashRollForward, out bOutOfRange);
                                hashRollForward.Remove(str);
                                hashRollForward.Add(str, dValue);
                            }

                            possibleValues.Add(deteriorate.Criteria,Convert.ToDouble(hashRollForward[str]));
                            hashRollForward[str] = originalValues[str];

                        }
                    }
                }


                //Now figure out which of the possible to use.
                //Use the least conservative that is not blank.

                var isAscending = SimulationMessaging.GetAttributeAscending(str);
                var leastConservative = double.MaxValue;
                if (isAscending) leastConservative = double.MinValue;
                var found = false;
                foreach (var key in possibleValues.Keys)
                {
                    if (key != "")
                    {
                        if (isAscending)
                        {
                            if (possibleValues[key] > leastConservative)
                            {
                                leastConservative = possibleValues[key];
                                found = true;
                            }
                        }
                        else
                        {
                            if (possibleValues[key] < leastConservative)
                            {
                                leastConservative = possibleValues[key];
                                found = true;
                            }
                        }
                    }
                }

                if (!found && possibleValues.ContainsKey(""))
                {
                    leastConservative = possibleValues[""];
                    found = true;
                }

                if (found)
                {
                    hashRollForward.Remove(str);
                    hashRollForward.Add(str, leastConservative);
                }
            }

            if (m_hashAttributeYearValues.ContainsKey(RollToYear))
            { 
                m_hashYearAttributeValues.Remove(RollToYear);
            }

            // Update CalculatedAttributes for RollForward.

            foreach (var calculatedAttribute in listCalculatedAttributes)
            {
                if (!hashRollForward.ContainsKey(calculatedAttribute.Attribute)) continue;
                if (calculatedAttribute.IsCriteriaMet(hashRollForward))
                {
                    hashRollForward[calculatedAttribute.Attribute] = calculatedAttribute.Calculate(hashRollForward);
                }
            }


            //Store all attributes by year.  This is the first for this simulation.
            m_hashYearAttributeValues.Add(this.RollToYear, hashRollForward);

            if (m_hashAttributeYearValues.ContainsKey(0))
            {
                m_hashYearAttributeValues.Remove(0);
            }

            m_hashYearAttributeValues.Add(0, hashRollForward);//Value after rolled forward.
           
            //Delete the data used to created the year hash to to save memory.
            //TODO: Make this permanent?

            foreach (String key in m_hashAttributeYearValues.Keys)
            {
                hash = (Hashtable)m_hashAttributeYearValues[key];
                hash.Clear();
            }
            m_hashAttributeYearValues.Clear();
        }

		public void ClearAttributeValues(String strColumn)
		{
			String strAttribute = strColumn.Substring(0, strColumn.Length - 5);
			Hashtable hash = ( Hashtable )m_hashAttributeYearValues[strAttribute];
            if (hash != null)
            {
				hash.Clear();
			}
		}



        public void ApplyDeteriorate(Deteriorate deteriorate, int nYear)
        {
            int nPreviousYear = nYear - 1;
            Hashtable hashAttributeValue = null;
            hashAttributeValue = (Hashtable)m_hashYearAttributeValues[nPreviousYear];
            
            
            Hashtable hashAttributeRemainingLife = (Hashtable)m_hashYearRemainingLife[nYear];//This was set on 

            double dValue = 0;
            double dBenefit = 0;
            double dRemainingLife = 0;
            double dDeficient = 0;
            if(deteriorate.IsCriteriaMet(hashAttributeValue))
            {
                bool bOutOfRange = false;
                dValue = deteriorate.IterateOneYear(hashAttributeValue, out bOutOfRange);//Iterate for RoadCare all years and OMS after first. Can use hashAttribute for all equations.  No OMS COUNTS
            

                if (bOutOfRange)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Warning:Attribute:" + deteriorate.Attribute + " is out of the PERFORMANCE equation range for " + this.Facility + " " + this.Section + ". The value for AGE=0 used."));
                }
                // For SHIFT equation calculation of Remaining life and Benefit
                if (deteriorate.Shift)
                {
                    
                    if (hashAttributeValue != null && hashAttributeValue.Contains("AGE"))
                    {
                        if (!m_hashNextAttributeValue.Contains("AGE"))
                        {
                            m_hashNextAttributeValue.Add("AGE", hashAttributeValue["AGE"].ToString());
                        }
                    }
                }

                if (m_hashNextAttributeValue.Contains(deteriorate.Attribute))
                {
                    if (deteriorate.Default) return; //If a value has been already calculated, don't calculate default.
                    //Multiple valid deterioration curves.
                    SimulationMessaging.AddMessage(new SimulationMessage("Warning: Multiple valid performance/deterioration curves for " + deteriorate.Attribute + " for " + this.Facility + " " + this.Section + ".  The less consevative solution selected."));
                    String strValue = m_hashNextAttributeValue[deteriorate.Attribute].ToString();
                    if (SimulationMessaging.GetAttributeAscending(deteriorate.Attribute))
                    {
                        //Current value is larger than the newly calculated variable.
                        if (double.Parse(strValue) > dValue)
                        {
                            m_hashNextAttributeValue.Remove(deteriorate.Attribute);
                            m_hashNextAttributeValue.Add(deteriorate.Attribute, dValue);

                            if (SimulationMessaging.Method.IsBenefitCost && SimulationMessaging.Method.BenefitAttribute == deteriorate.Attribute)
                            {
                                dBenefit = deteriorate.CalculateBenefit(m_hashNextAttributeValue);
                            }
                            if (SimulationMessaging.Method.IsRemainingLife && SimulationMessaging.GetDeficientLevel(deteriorate.Attribute, hashAttributeValue, out dDeficient))
                            {
                                deteriorate.CalculateRemainingLife(m_hashNextAttributeValue,hashAttributeValue, out dRemainingLife);
                            }
                            m_dBaseBenefit = dBenefit;
                            hashAttributeRemainingLife.Remove(deteriorate.Attribute);
                            hashAttributeRemainingLife.Add(deteriorate.Attribute, dRemainingLife);
                        }
                    }
                    else
                    {
                        //Current value is larger than the newly calculated variable.
                        if (double.Parse(strValue) < dValue)
                        {
                            m_hashNextAttributeValue.Remove(deteriorate.Attribute);
                            m_hashNextAttributeValue.Add(deteriorate.Attribute, dValue);

                            if (SimulationMessaging.Method.IsBenefitCost && SimulationMessaging.Method.BenefitAttribute == deteriorate.Attribute)
                            {
                                dBenefit = deteriorate.CalculateBenefit(m_hashNextAttributeValue);
                            }
                            if (SimulationMessaging.Method.IsRemainingLife && SimulationMessaging.GetDeficientLevel(deteriorate.Attribute, hashAttributeValue, out dDeficient))
                            {
                                deteriorate.CalculateRemainingLife(m_hashNextAttributeValue,hashAttributeValue, out dRemainingLife);
                            }


                            m_dBaseBenefit = dBenefit;
                            hashAttributeRemainingLife.Remove(deteriorate.Attribute);
                            hashAttributeRemainingLife.Add(deteriorate.Attribute, dRemainingLife);
                        }
                    }
                }
                else
                {
                    m_hashNextAttributeValue.Add(deteriorate.Attribute, dValue);


                    if (SimulationMessaging.Method.IsBenefitCost && SimulationMessaging.Method.BenefitAttribute == deteriorate.Attribute)
                    {
                        dBenefit = deteriorate.CalculateBenefit(m_hashNextAttributeValue);
                    }

                    if (SimulationMessaging.Method.BenefitAttribute == deteriorate.Attribute)
                    {
                        this.BaseBenefit = dBenefit;
                    }


                    if (SimulationMessaging.Method.IsRemainingLife && SimulationMessaging.GetDeficientLevel(deteriorate.Attribute, hashAttributeValue, out dDeficient))
                    {
                        deteriorate.CalculateRemainingLife(m_hashNextAttributeValue, hashAttributeValue, out dRemainingLife);
                    }

                    if (SimulationMessaging.GetDeficientLevel(deteriorate.Attribute, hashAttributeValue, out dDeficient))
                    {
                        hashAttributeRemainingLife.Add(deteriorate.Attribute, dRemainingLife);
                        this.RemainingLife = 100;
                        foreach (String key in hashAttributeRemainingLife.Keys)
                        {
                            dRemainingLife = (double)hashAttributeRemainingLife[key];
                            if (dRemainingLife < RemainingLife)
                            {
                                RemainingLife = dRemainingLife;
                            }
                        }
                    }
                }
            }
        }


        public void ResetSectionForNextYear()
        {
            m_dBaseBenefit = 0;
            m_dRemainingLife = 0;
            m_hashNextAttributeValue.Clear();
            m_nNumberTreatment = 0;
            this.Treated = false;
        }

        public void ApplyNonDeteriorate(int nYear,Investments investment)
        {
            int nPreviousYear = nYear - 1;
            Hashtable hashAttributeValue = null;
            if (SimulationMessaging.IsOMS && investment.StartDate.Year == nYear)
            {
                hashAttributeValue = (Hashtable)m_hashYearAttributeValues[nYear];
            }
            else
            {
                hashAttributeValue = (Hashtable)m_hashYearAttributeValues[nPreviousYear];
            }


            foreach (String strKey in hashAttributeValue.Keys)
            {
                if (!m_hashNextAttributeValue.Contains(strKey))
                {
                    m_hashNextAttributeValue.Add(strKey, hashAttributeValue[strKey]);
                }
            }
        }

        public void CalculateArea(int nYear)
        {
            //Area recalculate only if there is a begin station otherwi.
            if(!SimulationMessaging.IsOMS && Convert.ToInt32(SectionID) < 1000000 && m_fBeginStation < 0) return;

            object[] input = new object[SimulationMessaging.ListArea.Count];

            int i = 0;
            Hashtable hashAttributeValue = (Hashtable)m_hashYearAttributeValues[nYear];
            foreach (String str in SimulationMessaging.ListArea)
            {
                if (str == "LENGTH")
                {
                    input[i] = this.End - this.Begin;

                }
                else
                {
                    input[i] = float.Parse(hashAttributeValue[str].ToString());
                }
                i++;
            }

			try
			{
				object result = SimulationMessaging.Area.RunMethod(input);
				try
				{
					this.Area = float.Parse(result.ToString());
				}
				catch
				{
					this.Area = this.Area;
				}
			}
			catch(Exception exc)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod.   " + SimulationMessaging.Area.OriginalInput + " " + exc.Message)); 
			}

        }

        public Hashtable CommitProject(Committed commit, out String strBudget, out float fAmount, Dictionary<string,CommittedEquation> dictionaryCommittedEquation, out string changeHash)
        {
            Hashtable hashAttributeValue;// This is the attribute value pair for this year.
            changeHash = "";
            int nYear = commit.Year;
            if (m_hashAttributeYearValues.Contains(nYear))
            {
                hashAttributeValue = (Hashtable)m_hashAttributeYearValues[nYear];
                m_hashNextAttributeValue.Clear();
                foreach (string key in hashAttributeValue.Keys)
                {
                    m_hashNextAttributeValue.Add(key, hashAttributeValue[key]);
                }
                hashAttributeValue.Clear();
            }
            else
            {
                hashAttributeValue = new Hashtable();
                m_hashAttributeYearValues.Add(nYear, hashAttributeValue);
            }

            
            
            strBudget = commit.Budget;
            fAmount = commit.Cost;

            this.AnyYear = nYear + commit.Any;
            this.Treated = true;


            //String sValue;
            //// SELECT TO GET ATTRIBUTE CHANGE FOR THIS YEAR AND SECTION - It exists becuase Committed commit exists.
            //// Get all of this years deteriorated values.
            foreach (String key in m_hashNextAttributeValue.Keys)
            {
                if (!hashAttributeValue.ContainsKey(key)) //No consequence, just keep value
                {
                    hashAttributeValue.Add(key, m_hashNextAttributeValue[key]);
                }
            }
           

            this.Treated = true;

            return hashAttributeValue;
        }


        public void WriteSimulation(int nYearStart, int nYearEnd, Dictionary<string,int> attributeTable, List<TextWriter> textWriters)
        {
            Hashtable hashAttributeValue = null;
            foreach(var tw in textWriters)
            {
                tw.Write(SectionID);
            }

            foreach (String sAttribute in SimulationMessaging.ListAttributes)
            {
                if (sAttribute == "SECTIONID") continue;
                for (int nYear = nYearStart; nYear <= nYearEnd; nYear++)
                {
                    hashAttributeValue = (Hashtable)m_hashYearAttributeValues[nYear];

                    textWriters[attributeTable[sAttribute]].Write("\t");
                    if(hashAttributeValue[sAttribute] != null) //Values can remain null in OMS analysis
                    {
                        String sValue = hashAttributeValue[sAttribute].ToString();
                        if(SimulationMessaging.GetAttributeType(sAttribute) == "NUMBER")
                        {
                            String sFormat = SimulationMessaging.GetAttributeFormat(sAttribute);
                            float fValue = float.NaN;
                            try
                            {
                                fValue = float.Parse(sValue);
                            }
                            catch
                            {
                                fValue = float.NaN;
                            }
                            if (fValue == 0) sValue = "0";
                            else sValue = fValue.ToString(sFormat);
                            textWriters[attributeTable[sAttribute]].Write(sValue);
                        }
                        else
                        {
                            if (hashAttributeValue[sAttribute] != null)
                            {
                                textWriters[attributeTable[sAttribute]].Write(hashAttributeValue[sAttribute].ToString());
                            }
                        }
                    }
                }

                int year = 0;//Stores rolled forward values
                textWriters[attributeTable[sAttribute]].Write("\t");
                hashAttributeValue = (Hashtable)m_hashYearAttributeValues[year];
                {
                    String sValue = hashAttributeValue[sAttribute].ToString();
                    if (SimulationMessaging.GetAttributeType(sAttribute) == "NUMBER")
                    {
                        String sFormat = SimulationMessaging.GetAttributeFormat(sAttribute);
                        float fValue = float.NaN;
                        try
                        {
                            fValue = float.Parse(sValue);
                        }
                        catch
                        {
                            fValue = float.NaN;
                        }
                        if (fValue == 0) sValue = "0";
                        else sValue = fValue.ToString(sFormat);
                        textWriters[attributeTable[sAttribute]].Write(sValue);
                    }
                    else
                    {
                        if (hashAttributeValue[sAttribute] != null)
                        {
                            textWriters[attributeTable[sAttribute]].Write(hashAttributeValue[sAttribute].ToString());
                        }
                    }
                }
            }
            foreach (var tw in textWriters)
            {
                tw.WriteLine("");
            }
            return;
        }
 


        public override string ToString()
        {
            return this.SectionID.ToString();
        }
    }

    public class SameTreatment
    {
        public String strTreatment;
        public int nYear;
        public bool isExclusive = false;
        public bool isNotAllowed = false;
    }
}
