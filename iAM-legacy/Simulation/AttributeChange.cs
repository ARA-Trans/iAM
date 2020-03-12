using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    public class AttributeChange
    {
        /// <summary>
        /// Attribute to be changed as consequence
        /// </summary>
        public String Attribute { get; set; }


        /// <summary>
        /// Change to attribute as part of consequence.
        /// </summary>
        public String Change { get; set; }
        /// <summary>
        /// Minimum value after attribute change
        /// </summary>
        public String Minimum { get; set; }


        /// <summary>
        /// Maximum value after attribute change
        /// </summary>
        public String Maximum { get; set; }
     

        public AttributeChange(string attribute, string change)
        {
            Attribute = attribute;
            Change = change;
        }
        public AttributeChange()
        {

        }


        public object ApplyChange(object value)
        {
            if (SimulationMessaging.GetAttributeType(this.Attribute) == "STRING")
            {
                return this.Change;
            }
            else
            {
                bool bAdd = false;
                bool bSubtract = false;
                bool bPercent = false;

                String strChange = this.Change;

                if(strChange.IndexOf('+') > -1)
                {
                    bAdd = true;
                    strChange = strChange.Replace("+","");
                }


                if(strChange.IndexOf('-') > -1)
                {
                    bSubtract = true;
                    strChange = strChange.Replace("-","");
                }


                if(strChange.IndexOf('%') > -1)
                {
                    bPercent = true;
                    strChange = strChange.Replace("%","");
                }

                double dChange = 0;
                double dValue = 0;
                try
                {
                    dValue = Convert.ToDouble(value);
                    dChange = double.Parse(strChange);
                }
                catch
                {
                    return value;
                }

                if(bAdd && bPercent) dValue = dValue + (dChange/100)*dValue;
                else if(bSubtract && bPercent) dValue = dValue - (dChange/100)*dValue;
                else if(bPercent) dValue = dValue * (dChange/100);
                else if(bAdd) dValue = dValue + dChange;
                else if(bSubtract) dValue = dValue - dChange;
                else dValue = dChange;


				if (!String.IsNullOrEmpty(Minimum))
				{

					if (double.Parse(Minimum) > dValue)
					{
						dValue = double.Parse(Minimum);
					}
				}

				if (!String.IsNullOrEmpty(Maximum))
				{
					if (double.Parse(Maximum) < dValue)
					{
						dValue = double.Parse(Maximum);
					}
				}
                return dValue;
            }
        }
    }
}
