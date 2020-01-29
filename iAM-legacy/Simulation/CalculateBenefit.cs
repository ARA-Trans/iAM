using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace Simulation
{
    internal class CalculateBenefit
    {
        private int Year { get; }
        private Treatments Treatment { get; }
        private List<Deteriorate> Deteriorates { get; }
        private List<CalculatedAttribute> CalculatedAttributes { get; }
        private List<Consequences> NoTreatmentConsequences { get; }
        private List<Committed> CommittedProjects { get; }
        private Dictionary<string, List<AttributeChange>> CommittedConsequences { get; }
        private Dictionary<string, CommittedEquation> CommittedEquations { get; }
        private List<Deteriorate> CurrentDeteriorate { get; }
        
        private List<Treatments> SimulationTreatments { get; }


        private bool IsBenefitAttributeAscending { get; }
        private bool IsBenefitCalculated { get; }

        private bool _isNoTreatmentCircular ;

        public CalculateBenefit(int year, 
            Treatments treatmentToEvaluate,
            List<Deteriorate> deteriorates,
            List<CalculatedAttribute> calculatedAttributes,
            List<Consequences> noTreatmentConsequences,
            List<Committed> committedProjects,
            Dictionary<string, List<AttributeChange>> committedConsequences, 
            Dictionary<string,CommittedEquation> committedEquation,
            List<Treatments> simulationTreatments)
        {
            Year = year;
            Treatment = treatmentToEvaluate;
            Deteriorates = deteriorates;
            CalculatedAttributes = calculatedAttributes;
            NoTreatmentConsequences = noTreatmentConsequences;
            CommittedProjects = committedProjects;
            CommittedConsequences = committedConsequences;
            CommittedEquations = committedEquation;
            IsBenefitAttributeAscending =  SimulationMessaging.GetAttributeAscending(SimulationMessaging.Method.BenefitAttribute);
            IsBenefitCalculated = calculatedAttributes.Any(a => a.Attribute == SimulationMessaging.Method.BenefitAttribute);
            CurrentDeteriorate = new List<Deteriorate>();
            SimulationTreatments = simulationTreatments;

            //Need to input Deficient
            _isNoTreatmentCircular = false;

            //See if any of the no treatment consequences effect the criteria.
            //If they do not there is no need recalculate/resolve
            var attributesConsequence = new List<string>();
            foreach (var consequence in NoTreatmentConsequences)
            {
                foreach (var attribute in consequence.Attributes)
                {
                    if (!attributesConsequence.Contains(attribute))
                    {
                        attributesConsequence.Add(attribute);
                    }
                }
                if(consequence.Criteria?.CriteriaAttributes != null)
                { 
                    foreach (var attribute in consequence.Criteria.CriteriaAttributes)
                    {
                        if (_isNoTreatmentCircular) break;

                        foreach (var calculated in CalculatedAttributes)
                        {
                            if (calculated.Attribute != attribute) continue;
                            //If a attribute is calculated it could result in a circular condition (maybe)
                            _isNoTreatmentCircular = true;
                            break;
                        }
                        //If the criteria is modified by a consequence it can change (even with no treatment)
                        if (!attributesConsequence.Contains(attribute)) continue;
                        _isNoTreatmentCircular = true;
                        
                    }
                }
            }
        }


        public double Solve(Hashtable hashAttributeValue, out Hashtable nextAttributeValue, out string rlHash)
        {
            return  SolveBenefitCost(hashAttributeValue, out nextAttributeValue,out rlHash);
        }

        private double SolveRemainingLife(Hashtable hashAttributeValue)
        {
            //Get rid of this and calculate RL in Benefit.
            throw new NotImplementedException();
        }

        private List<Consequences> GetReducedSetConsequences(Hashtable attributeValue)
        {
            if (_isNoTreatmentCircular) return NoTreatmentConsequences;

            var noTreatmentConsequences = new List<Consequences>();
            foreach (var consequence in NoTreatmentConsequences)
            {
                if (consequence.Criteria.IsCriteriaMet(attributeValue))
                {
                    noTreatmentConsequences.Add(consequence);
                }
            }
           
         
            return noTreatmentConsequences;
        }



        private double SolveBenefitCost(Hashtable attributeValue, out Hashtable nextAttributeValue,out string rlHash)
        {
            rlHash = "";
            //Set remaining life to 99 years in the case no attribute has a valid remaining life.
            var hashRemainingLife = new Dictionary<RemainingLife, double>();
            var previousAttributeValue = new Dictionary<RemainingLife, double>();
            var ascendingAttribute = new Dictionary<string, bool>();
            foreach(var remainingLife in SimulationMessaging.RemainingLifes)
            {
                if(!hashRemainingLife.ContainsKey(remainingLife))
                {
                    hashRemainingLife.Add(remainingLife, 99);
                }
                if(!ascendingAttribute.ContainsKey(remainingLife.Attribute))
                {
                    ascendingAttribute.Add(remainingLife.Attribute, SimulationMessaging.GetAttributeAscending(remainingLife.Attribute));
                }
            }

            //Apply consequence of treatment
            nextAttributeValue = ApplyConsequences(Treatment.ConsequenceList, attributeValue);
            nextAttributeValue = SolveCalculatedFields(nextAttributeValue);
            var currentAttributeValue = new Hashtable();
            foreach (var key in attributeValue.Keys)
            {
                currentAttributeValue.Add(key, nextAttributeValue[key]);
            }



            double sumBenefit = 0;
            UpdateCurrentDeteriorate(currentAttributeValue);

            var apparentAgeHints = new Dictionary<string, int>();
            foreach (var deteriorate in CurrentDeteriorate)
            {
                apparentAgeHints.Add(deteriorate.Attribute, 0);
            }


            var noTreatmentConsequences = GetReducedSetConsequences(nextAttributeValue);

            //Calculate the 100 year benefit
            //Change to 50 year benefit
            for (var i = 0; i < 100; i++)
            {
                //Make sure the current deterioration equations are current (meet all criteria).
                UpdateCurrentDeteriorate(attributeValue);

                //// Deteriorate current deterioration model.
                foreach (var deteriorate in CurrentDeteriorate)
                {
                    if (!apparentAgeHints.ContainsKey(deteriorate.Attribute)) apparentAgeHints.Add(deteriorate.Attribute, 0);
                    currentAttributeValue[deteriorate.Attribute] =
                        deteriorate.IterateOneYear(currentAttributeValue, apparentAgeHints[deteriorate.Attribute], out double apparentAge);

                    apparentAgeHints[deteriorate.Attribute] = (int) apparentAge;
                }


                // Apply No Treatment or Committed Consequences (or Scheduled)
                var committed = CommittedProjects.Find(c => c.Year == Year + i);
                var scheduled = Treatment.Scheduleds.Find(s => s.ScheduledYear == i);


                //If both a committed and scheduled.  No benefit.  Can't do both!
                if (scheduled != null && committed != null)
                {
                    return 0;
                }

                //Only get here if scheduled and committed do notcollide.
                if (committed != null)
                {
                    if (!string.IsNullOrWhiteSpace(committed.ScheduledTreatmentId))
                    {
                        var scheduledTreatment =
                            SimulationTreatments.Find(f => f.TreatmentID == committed.ScheduledTreatmentId);
                        currentAttributeValue = ApplyConsequences(scheduledTreatment.ConsequenceList, currentAttributeValue);
                        noTreatmentConsequences = GetReducedSetConsequences(currentAttributeValue);
                    }
                    else if(!string.IsNullOrWhiteSpace(committed.SplitTreatmentId) && committed.YearSplitTreatmentComplete == Year + i)
                    {
                        var splitTreatment =
                            SimulationTreatments.Find(f => f.TreatmentID == committed.SplitTreatmentId);
                        currentAttributeValue = ApplyConsequences(splitTreatment.ConsequenceList, currentAttributeValue);
                        noTreatmentConsequences = GetReducedSetConsequences(currentAttributeValue);
                    }
                    else
                    {

                        currentAttributeValue = ApplyCommittedConsequences(currentAttributeValue, committed);
                        noTreatmentConsequences = GetReducedSetConsequences(currentAttributeValue);
                    }
                }
                else if (scheduled != null)
                {
                    currentAttributeValue = ApplyConsequences(scheduled.Treatment.ConsequenceList, currentAttributeValue);
                    noTreatmentConsequences = GetReducedSetConsequences(currentAttributeValue);
                }
                else
                {
                    currentAttributeValue = ApplyConsequences(noTreatmentConsequences, currentAttributeValue);
                }


                //// Solve calculated fields
                currentAttributeValue = SolveCalculatedFields(currentAttributeValue);
                //Look up Method.Benefit variable
                if (IsBenefitAttributeAscending)
                {
                    //For attributes that get smaller with deterioration.
                    if ((double)currentAttributeValue[SimulationMessaging.Method.BenefitAttribute] >
                        SimulationMessaging.Method.BenefitLimit)
                    {
                        sumBenefit += (double)currentAttributeValue[SimulationMessaging.Method.BenefitAttribute] -
                                      SimulationMessaging.Method.BenefitLimit;
                    }

                }
                else
                {
                    //For attributes that get larger with deterioration.
                    if ((double)currentAttributeValue[SimulationMessaging.Method.BenefitAttribute] <
                        SimulationMessaging.Method.BenefitLimit)
                    {
                        sumBenefit += SimulationMessaging.Method.BenefitLimit -
                                      (double)currentAttributeValue[SimulationMessaging.Method.BenefitAttribute.ToUpper()];

                    }
                }


                //Calculate remaining life if criteria matches.
                foreach (var remainingLife in SimulationMessaging.RemainingLifes)
                {
                    var storePrevious = false;
                    if(remainingLife.Criteria.IsCriteriaMet(currentAttributeValue))
                    {
                        if (ascendingAttribute[remainingLife.Attribute] &&  Convert.ToDouble(currentAttributeValue[remainingLife.Attribute]) > remainingLife.RemainingLifeLimit)
                        {
                            hashRemainingLife[remainingLife] = i;
                            //Need to store value of current so that when threshold is crossed can calculate partial.
                            storePrevious = true;
                        }
 
                        if (!ascendingAttribute[remainingLife.Attribute] && Convert.ToDouble(currentAttributeValue[remainingLife.Attribute]) < remainingLife.RemainingLifeLimit)
                        {
                            hashRemainingLife[remainingLife] = i;
                            storePrevious = true;
                        }

                        if (storePrevious)//The previous value was updated this loop.
                        {
                            if (!previousAttributeValue.ContainsKey(remainingLife))
                            {
                                previousAttributeValue.Add(remainingLife, Convert.ToDouble(currentAttributeValue[remainingLife.Attribute]));
                            }
                            else
                            {
                                previousAttributeValue[remainingLife] = Convert.ToDouble(currentAttributeValue[remainingLife.Attribute]);
                            }
                        }
                        else //Previous value was not updated.  If the previous value exists, calculate the exact remaining life now.
                        {
                            if(previousAttributeValue.ContainsKey(remainingLife))
                            {
                                var delta = (previousAttributeValue[remainingLife] - remainingLife.RemainingLifeLimit) / (previousAttributeValue[remainingLife] - Convert.ToDouble(currentAttributeValue[remainingLife.Attribute]));

                                hashRemainingLife[remainingLife] = hashRemainingLife[remainingLife] + delta;

                                previousAttributeValue.Remove(remainingLife); //So it does not recalculate remaining life.
                            }
                        }
                    }
                }
            }


            var minimumAttributeRemainingLife = new Dictionary<string, double>();
            foreach(var key in hashRemainingLife.Keys)
            {
                if(!minimumAttributeRemainingLife.ContainsKey(key.Attribute))
                {
                    minimumAttributeRemainingLife.Add(key.Attribute, hashRemainingLife[key]);
                }
                else
                {
                    if(minimumAttributeRemainingLife[key.Attribute] > hashRemainingLife[key])
                    {
                        minimumAttributeRemainingLife[key.Attribute] = hashRemainingLife[key];
                    }
                }
            }

            double minimumRemainingLife = 99;
            foreach(var key in minimumAttributeRemainingLife.Keys)
            {
                if(minimumAttributeRemainingLife[key] < minimumRemainingLife)
                {
                    minimumRemainingLife = minimumAttributeRemainingLife[key];
                }
                rlHash += key + "\t" + minimumAttributeRemainingLife[key].ToString("0.0") + "\n";
            }

            if(SimulationMessaging.Method.IsRemainingLife)
            {
                return minimumRemainingLife;
            }
            else
            {
                return sumBenefit;

            }
       
        }

        private void UpdateCurrentDeteriorate(Hashtable hashInput)
        {
            var needsUpdate = false;

            if (CurrentDeteriorate.Count > 0)
            {
                foreach (var deteriorate in CurrentDeteriorate)
                {
                    if (!deteriorate.IsCriteriaMet(hashInput))
                    {
                        needsUpdate = true;
                    }
                }

            }
            else
            {
                needsUpdate = true;
            }

            if (!needsUpdate) return;

            CurrentDeteriorate.Clear();
            // Deteriorate each non default (may override previous step).
            foreach (var deteriorate in Deteriorates)
            {
                if (deteriorate.Default) continue;

                if (deteriorate.IsCriteriaMet(hashInput))
                {
                    CurrentDeteriorate.Add(deteriorate);
                }
            }

            //Deteriorate each value that did not have a non-default set (will not override).
            foreach (var deteriorate in Deteriorates)
            {
                if (!deteriorate.Default) continue;

                if(CurrentDeteriorate.All(d => d.Attribute != deteriorate.Attribute))
                { 
                    CurrentDeteriorate.Add(deteriorate);
                }
            }
        }






        /// <summary>
        /// Apply consequences from treatment (or no treatment).
        /// </summary>
        /// <param name="consequences">List of consequences from treatment</param>
        /// <param name="hashInput">Input values for determining criteria and equations</param>
        /// <returns></returns>
        private Hashtable ApplyConsequences(List<Consequences> consequences, Hashtable hashInput)
        {
            object sValue;
            //Otherwise do the whole thing.
            var hashOutput = new Hashtable();

            //No Treatment usually only increments age.  If this is the case (and is the only effect of No Treatment solve it quickly.
            if (consequences.Count == 1 && consequences[0].AttributeChange.Count == 1 && consequences[0].AttributeChange[0].Attribute == "AGE")
            {
                foreach (var key in hashInput.Keys)
                {
                    hashOutput.Add(key, hashInput[key]);
                }

                if (consequences[0].AttributeChange[0].Attribute == "AGE")
                {
                    sValue = hashOutput["AGE"];
                    hashOutput["AGE"] = consequences[0].AttributeChange[0].ApplyChange(sValue);
                    return hashOutput;
                }
            }




            var hashConsequences = new Hashtable();
            foreach (var consequence in consequences)
            {
                if (consequence.Default)
                {
                    if (!hashConsequences.Contains(consequence.Attributes[0]))
                    {
                        hashConsequences.Add(consequence.Attributes[0], consequence);
                    }
                }
                else
                {
                    if (consequence.Criteria.IsCriteriaMet(hashInput))
                    {
                        if (hashConsequences.Contains(consequence.Attributes[0]))
                        {
                            var consequenceIsDefault = (Consequences)hashConsequences[consequence.Attributes[0]];
                            if (consequenceIsDefault.Default != true) continue;
                            hashConsequences.Remove(consequence.Attributes[0]);
                            hashConsequences.Add(consequence.Attributes[0], consequence);
                        }
                        else
                        {
                            hashConsequences.Add(consequence.Attributes[0], consequence);
                        }
                    }
                }
            }

            // Get all of this years deteriorated values.
            foreach (var key in hashInput.Keys)
            {
                sValue = hashInput[key];
                if (hashConsequences.Contains(key))
                {
                    var consequence = (Consequences)hashConsequences[key];

                    if (consequence.IsEquation)
                    {
                        sValue = consequence.GetConsequence(hashInput);
                    }
                    else
                    {
                        var change = consequence.AttributeChange[0];
                        if (change != null)
                        {
                            sValue = change.ApplyChange(sValue);
                        }
                    }
                }
                hashOutput.Add(key, sValue);
            }


            return hashOutput;
        }

        private Hashtable ApplyCommittedConsequences(Hashtable hashInput, Committed commit)
        {
            var hashOutput = new Hashtable();


            var hashConsequences = new Hashtable();
            var committedConsequences = CommittedConsequences[commit.ConsequenceID];

            foreach (AttributeChange attributeChange in committedConsequences)
            {
                if (SimulationMessaging.AttributeMinimum.Contains(attributeChange.Attribute))
                    attributeChange.Minimum =
                        SimulationMessaging.AttributeMinimum[attributeChange.Attribute].ToString();
                if (SimulationMessaging.AttributeMaximum.Contains(attributeChange.Attribute))
                    attributeChange.Maximum =
                        SimulationMessaging.AttributeMaximum[attributeChange.Attribute].ToString();
                hashConsequences.Add(attributeChange.Attribute, attributeChange);
            }

            // Get all of this years deteriorated values.
            foreach (String key in hashInput.Keys)
            {
                object sValue = hashInput[key];

                if (hashConsequences.Contains(key))
                {
                    AttributeChange change = (AttributeChange) hashConsequences[key];
                    if (change != null && CommittedEquations.ContainsKey(change.Change))
                    {
                        CommittedEquation ce = CommittedEquations[change.Change];
                        if (!ce.HasErrors) sValue = ce.GetConsequence(hashInput);
                    }
                    else
                    {
                        if (change != null)
                        {
                            sValue = change.ApplyChange(sValue);
                        }
                    }
                }

                hashOutput.Add(key, sValue);

            }

            return hashOutput;
        }



        public Hashtable SolveCalculatedFields(Hashtable hashInput)
        {
            var hashOutput = new Hashtable();
            foreach (var calculatedattribute in CalculatedAttributes)
            {
                if (!calculatedattribute.Default) continue;
                var calculated = calculatedattribute.Calculate(hashInput);
                if (hashOutput.Contains(calculatedattribute.Attribute))
                {
                    hashOutput[calculatedattribute.Attribute] = calculated;
                }
                else
                {
                    hashOutput.Add(calculatedattribute.Attribute, calculated);
                }
            }

            foreach (var calculatedattribute in CalculatedAttributes)
            {
                if (calculatedattribute.Default) continue;
                if (!calculatedattribute.IsCriteriaMet(hashInput)) continue;
                var calculated = calculatedattribute.Calculate(hashInput);
                if (hashOutput.Contains(calculatedattribute.Attribute))
                {
                    hashOutput[calculatedattribute.Attribute] = calculated;
                }
                else
                {
                    hashOutput.Add(calculatedattribute.Attribute, calculated);
                }
            }

            foreach (string key in hashInput.Keys)
            {
                if (hashOutput.ContainsKey(key)) continue;
                hashOutput.Add(key,hashInput[key]);
            }


            return hashOutput;
        }
    }
}
