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
        private string _equation;
        CalculateEvaluate.CalculateEvaluate calculate;
        public string Id { get; private set; } 


        /// <summary>
        /// Get/Set Cost equation as String
        /// </summary>
        public String Equation
        {
            get { return _equation; }
            set
            {
                _equation = value;
                // Get list of attributes
                CommittedAttributes = SimulationMessaging.ParseAttribute(_equation);


                byte [] assembly = SimulationMessaging.GetSerializedCalculateEvaluate("COMMITTED", "BINARY_EQUATION", Id, null);
                SimulationMessaging.AddMessage(new SimulationMessage("Compiling committed equation " + _equation));


                if (assembly != null && assembly.Length > 0)
                {
                    calculate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assembly);
                    if (calculate.OriginalInput != _equation)
                    {
                        calculate = null;
                    }
                }
                 
                
                if (calculate == null)
                {
                    calculate = new CalculateEvaluate.CalculateEvaluate();
                    calculate.BuildClass(_equation, true, "COMMITTED_BINARY_EQUATION_" + Id);
                    if (calculate.m_cr == null)
                    {
                        calculate.CompileAssembly();
                        SimulationMessaging.SaveSerializedCalculateEvaluate("COMMITTED", "BINARY_EQUATION", Id, calculate);
                    }
                }
            }
        }

        public bool HasErrors
        {
            get
            {
                if (calculate?.m_cr.Errors.Count > 0) return true;
                else return false;
            }

        }


        public List<string> CommittedAttributes { get; private set; }


        public CommittedEquation(string equation, string id)
        {
            Id = id;
            Equation = equation;
        }
        
        public double GetConsequence(Hashtable hashAttributeValue)
        {
            var i = 0;
            var input = new object[CommittedAttributes.Count];
            foreach (String str in CommittedAttributes)
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
