using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    public class AttributeChange
    {
        private String m_strAttribute;
        private String m_strChange;
        private String m_strMinimum;
        private String m_strMaximum;
        private String m_strRedirect;

        /// <summary>
        /// Attribute to be changed as consequence
        /// </summary>
        public String Attribute
        {
            get { return m_strAttribute; }
            set { m_strAttribute = value; }
        }

        /// <summary>
        /// Change to attribute as part of consequence.
        /// </summary>
        public String Change
        {
            get { return m_strChange; }
            set { m_strChange = value; }
        }

        /// <summary>
        /// Minimum value after attribute change
        /// </summary>
        public String Minimum
        {
            get { return m_strMinimum; }
            set { m_strMinimum = value; }
        }

        /// <summary>
        /// Maximum value after attribute change
        /// </summary>
        public String Maximum
        {
            get { return m_strMaximum; }
            set { m_strMaximum = value; }
        }

        /// <summary>
        /// Where this attribute is redirected to.
        /// </summary>
        public String RedirectAttribute
        {
            get { return m_strRedirect; }
            set { m_strRedirect = value; }
        }

        public AttributeChange(string attribute, string change)
        {
            Attribute = attribute;
            Change = change;
        }
        public AttributeChange()
        {

        }
        /// <summary>
        /// Calculate change in a NUMBER attribute
        /// </summary>
        /// <param name="fValue">Current value of the attribute</param>
        /// <returns></returns>
        public float CalculateChange(float fValue)
        {
            //TODO Actually calculate the change based upon m_strChange and input value.

            return new float();
        }
        /// <summary>
        /// New value for a STRING attribute as a consequence.
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public String CalculateChange(String strValue)
        {
            return this.Change;
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
