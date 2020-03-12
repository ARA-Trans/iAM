using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using CalculateEvaluate;
using System.Collections;
namespace Simulation
{
    /// <summary>
    /// A class that stores Criteria for Treatments, Costs, Consequences and Priorites
    /// </summary>
    public class Criterias
    {
        private String m_strCriteria;
        List<String> m_listAttributesCriteria;
        private CompilerResults m_crCriteria;
        private List<String> m_listError = new List<String>();
        CalculateEvaluate.CalculateEvaluate evaluate;

        string _tableName = null;
        string _columnName = null;
        string _ID = null;

        public Criterias(string tableName, string columnName, string ID)
        {
            _tableName = tableName;
            _columnName = columnName;
            _ID = ID;
        }

        public Criterias()
        {

        }


        public List<String> Errors
        {
            get{ return m_listError;}

        }


		public CalculateEvaluate.CalculateEvaluate Evaluate
		{
			set
			{
				evaluate = value;
			}
			get
			{
				return evaluate;
			}
		}


        /// <summary>
        /// Get/Set Criteria for Deterioration/Performance curve
        /// </summary>
        public String Criteria
        {
            get { return m_strCriteria; }
            set
            {
                m_strCriteria = value;
              //  m_strCriteria = m_strCriteria.ToUpper();
                if (m_strCriteria.TrimEnd().Length == 0)
                {
                    m_listAttributesCriteria = new List<String>();
                    return;
                }

                m_listAttributesCriteria = SimulationMessaging.ParseAttribute(m_strCriteria);
                if (m_listAttributesCriteria == null)
                {
                    return;
                }

                String strCriteria = m_strCriteria;
                foreach (String str in m_listAttributesCriteria)
                {
                    String strType = SimulationMessaging.GetAttributeType(str);
                    if (strType == "STRING")
                    {
                        String strOldValue = "[" + str + "]";
                        String strNewValue = "[@" + str + "]";
                        strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                    }
                    else if (strType == "DATETIME")
                    {
                        String strOldValue = "[" + str + "]";
                        String strNewValue = "[$" + str + "]";
                        strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                    }
                }
				if( evaluate == null )
				{
					evaluate = new CalculateEvaluate.CalculateEvaluate();
                    evaluate.BuildClass(strCriteria, false,  _tableName + "_" + _columnName + "_" + _ID);
                    
					m_crCriteria = evaluate.CompileAssembly();
                    if (_tableName != null)
                    {
                        SimulationMessaging.SaveSerializedCalculateEvaluate(_tableName, _columnName, _ID, evaluate);
                    }
					if( evaluate.m_listError.Count > 0 )
					{
						foreach( String str in evaluate.m_listError )
						{
							m_listError.Add( str );

						}
					}
                    evaluate.OriginalInput = m_strCriteria;
				}
            }
        }
        /// <summary>
        /// Return the compiled criteria.
        /// </summary>
        public CompilerResults CompiledCriteria
        {
            get { return m_crCriteria; }
        }

        /// <summary>
        /// Return list of attributes which make up this criteria
        /// </summary>
        public List<String> CriteriaAttributes
        {
            get { return m_listAttributesCriteria; }
        }


        public bool IsCriteriaMet(Hashtable hashAttributeValue)
        {
            if (string.IsNullOrWhiteSpace(m_strCriteria))
            {
                return true;
            }

            object[] input = new object[this.m_listAttributesCriteria.Count];

            int i = 0;
            foreach (String str in this.m_listAttributesCriteria)
            {
                input[i] = hashAttributeValue[str];
                i++;
            }
			try
			{
                if (evaluate.m_methodInfo == null)
                {
                    if (evaluate.m_cr == null)
                    {
                        evaluate.BuildClass(evaluate.Expression, false, _tableName + "_" + _columnName + "_" + _ID);
                        evaluate.CompileAssembly();
                        if (_tableName != null)
                        {
                            SimulationMessaging.SaveSerializedCalculateEvaluate(_tableName, _columnName, _ID, evaluate);
                        }
                    }
                }

				Object result = this.Evaluate.RunMethod(input);
				return bool.Parse(result.ToString());
			}
			catch(Exception exc)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod.   " + Evaluate.OriginalInput + " " + exc.Message));
                return false;
			}
        }

    }
}
