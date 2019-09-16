using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Collections;
using RoadCareGlobalOperations;

namespace Simulation
{
    public class CommittedEquation
    {
        string m_strEquation;
        CalculateEvaluate.CalculateEvaluate calculate;
        List<string> m_listAttributesEquation;
        CompilerResults m_crEquation;
        string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Get/Set Cost equation as String
        /// </summary>
        public String Equation
        {
            get { return m_strEquation; }
            set
            {
                m_strEquation = value;
                // Get list of attributes
                m_listAttributesEquation = SimulationMessaging.ParseAttribute(m_strEquation);


                byte [] assembly = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "COMMITTED", "BINARY_EQUATION", _id, null);
                SimulationMessaging.AddMessage(new SimulationMessage("Compiling committed equation " +  m_strEquation));


                if (assembly != null && assembly.Length > 0)
                {
                    calculate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assembly);
                    if (calculate.OriginalInput != m_strEquation)
                    {
                        calculate = null;
                    }
                }
                 
                
                if (calculate == null)
                {
                    calculate = new CalculateEvaluate.CalculateEvaluate();
                    calculate.BuildClass(m_strEquation, true, cgOMS.Prefix + "COMMITTED_BINARY_EQUATION_" + _id);

    



                    if (calculate.m_cr == null)
                    {
                        m_crEquation = calculate.CompileAssembly();
                        SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + "COMMITTED", "BINARY_EQUATION", _id, calculate);
                    }
                }
            }
        }

        public bool HasErrors
        {
            get
            {
                if (calculate.m_cr.Errors.Count > 0) return true;
                else return false;
            }

        }



        public List<string> CommittedAttributes
        {
            get { return m_listAttributesEquation; }
        }

        public CommittedEquation(string equation, string id)
        {
            _id = id;
            this.Equation = equation;
        }
        
        public double GetConsequence(Hashtable hashAttributeValue)
        {

            int i = 0;
            object[] input = new object[m_listAttributesEquation.Count];
            foreach (String str in m_listAttributesEquation)
            {
                input[i] = hashAttributeValue[str];
                i++;
            }
            try
            {
                object result = calculate.RunMethod(input);
                return (double)result;
            }
            catch (Exception exc)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod.   " + calculate.OriginalInput + " " + exc.Message));
                return 0;
            }
        }


    }
}
