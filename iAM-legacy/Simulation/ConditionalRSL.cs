using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class ConditionalRSL
    {
        public  Dictionary<Int32, Double> BinValues { get; set; }
        Double[] _values = null;
        Criterias _criteria;
        public String Attribute { get; private set; }
        private Boolean _isAscending = false;

        //Criteria where this conditional RSL is valid for
        public Criterias Criteria
        {
            get { return _criteria; }
        }

        public double[] Values
        {
            get { return _values; }
        }
     



        public ConditionalRSL(String attribute, Criterias criteria)
        {
            BinValues = new Dictionary<int, double>();
            _values = null;
            this.Attribute = attribute;
            _criteria = criteria;
            _isAscending = SimulationMessaging.GetAttributeAscending(attribute);
        }


        public int GetRSL(double value)
        {
            if (_values == null) CalculateValues();
            for (int i = 0; i < _values.Length; i++)
            {
                if(_isAscending)
                {
                    if (_values[i] > value)
                    {
                        return i;
                    }
                        
                        
                }
                else//Values that get larger with deterioration.
                {
                    if (_values[i] < value) return i;
                }
                
            }
            return _values.Length - 1;
        }

        
        private void CalculateValues()
        {
            int maxKey = 0;
            //Find the maximum key
            foreach(Int32 key in BinValues.Keys)
            {
                if (key > maxKey) maxKey = key;
            }

            //Set the values matrix to the proper size
            _values = new Double[maxKey+1];

            for (int i = 0; i < _values.Length; i++ )
            {
                _values[i] = Double.MinValue;
            }


            //Fill in the values that are available.
            foreach (Int32 key in BinValues.Keys)
            {
                _values[key] = BinValues[key];
            }

            int lowerKey = -1;
            bool lowerKeyFound = false;


            for(int i = 0; i < maxKey + 1; i++)
            {
                //We don't do anything with null values.  Need an upper key to interpolate.
                if (_values[i] < 0) continue;

                //If lower key hasn't been found and we find a value, it is the lower key.
                if (_values[i] >= 0 && !lowerKeyFound)
                {
                    lowerKey = i;
                    lowerKeyFound = true;
                    continue;
                }

                //If the lower key has been found, but the next higher key is not null, then set it to the new lower key.
                if (_values[i] >= 0 && lowerKeyFound && i - 1 == lowerKey)
                {
                    lowerKey = i;
                    lowerKeyFound = true;
                    continue;
                }

                //If we get here we are looking for (and found!) an upper key.
                int upperKey = i;
                double upperValue = _values[i];
                double lowerValue = _values[lowerKey];
                double keyDifference = (double)(i - lowerKey);
                double valueDifference = upperValue - lowerValue;
                double increment = valueDifference / keyDifference;

                for(int j = lowerKey + 1; j < i; j++)
                {
                    _values[j] = lowerValue + (increment * ((double)(j - lowerKey)));
                }

                lowerKey = i;
                lowerKeyFound = true;
            }
            //Upon exiting this loop all values should be calculated (for bins)
        }

        public override string ToString()
        {
            return Attribute + "(" + _criteria.Criteria + ")";
        }
    }
}
