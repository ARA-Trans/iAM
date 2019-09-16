using System;
using System.Collections.Generic;
using System.Text;
using Simulation;
using System.Collections;
using RoadCareGlobalOperations;

namespace RoadCare3
{
	public class CompoundTreatmentElement
	{
		private string _compoundTreatmentID;
		private string _compoundTreatmentName;
		private string _attributeFrom;
		private string _attributeTo;

		private List<string> _relevantAttributes = new List<string>();

		CalculateEvaluate.CalculateEvaluate _extent;
		CalculateEvaluate.CalculateEvaluate _quantity;
		CalculateEvaluate.CalculateEvaluate _cost;
		Criterias _criteria;

		#region Properties
		public CalculateEvaluate.CalculateEvaluate Quantity
		{
			get { return _quantity; }
		}

		public string CompoundTreatmentID
		{
			get { return _compoundTreatmentID; }
		}

		public Criterias CriteriaEquation
		{
			get { return _criteria; }
		}

		public CalculateEvaluate.CalculateEvaluate CostEquation
		{
			get { return _cost; }
		}

		public string CompoundTreatmentName
		{
			get { return _compoundTreatmentName; }
		}
		
		public CalculateEvaluate.CalculateEvaluate ExtentEquation
		{
			get { return _extent; }
			set { _extent = value; }
		}

		public string AttributeTo
		{
			get { return _attributeTo; }
		}

		public string AttributeFrom
		{
			get { return _attributeFrom; }
		}

		public List<string> RelevantAttributes
		{
			get { return _relevantAttributes; }
		}
		#endregion

		public CompoundTreatmentElement(string compoundTreatmentID, string compoundTreatmentName, string attributeFrom, string attributeTo, string costEquation, string extentEquation, string quantityCriteria, string criteriaString)
		{
			_compoundTreatmentID = compoundTreatmentID;
			_compoundTreatmentName = compoundTreatmentName;

			// Begin building the relavent attribute list.  This list contains all attributes used in the various
			// criteria and equations.
			_attributeFrom = attributeFrom;
			if (!_relevantAttributes.Contains(_attributeFrom)) _relevantAttributes.Add(_attributeFrom);

			_attributeTo = attributeTo;
			if (!_relevantAttributes.Contains(_attributeTo)) _relevantAttributes.Add(_attributeTo);

			// Add cost attributes


            byte[] assembly = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "COMPOUNDTREATMENT", "COST", compoundTreatmentID, null);
            if (assembly != null && assembly.Length > 0)
            {
                _cost = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assembly);
                if (SimulationMessaging.Area.OriginalInput != costEquation)
                {
                    _cost = null;
                }
            }
            if (_cost == null)
            {
                _cost = new CalculateEvaluate.CalculateEvaluate();
                _cost.BuildClass(costEquation, true,cgOMS.Prefix + "COMPOUNDTREATMENT_COST_" + compoundTreatmentID);
                _cost.CompileAssembly();
                SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + "COMPOUNDTREATMENT", "COST", compoundTreatmentID, _cost);
            }
            foreach(string parameter in _cost.m_listParameters)
			{
				if(!_relevantAttributes.Contains(parameter)) _relevantAttributes.Add(parameter);
			}


            assembly = null;
            assembly = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "COMPOUNDTREATMENT", "EXTENT", compoundTreatmentID, null);
            if (assembly != null && assembly.Length > 0)
            {
                _extent = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assembly);
                if (SimulationMessaging.Area.OriginalInput != extentEquation)
                {
                    _extent = null;
                }
            }
            if (_extent == null)
            {
                // Add extent attributes
                _extent = new CalculateEvaluate.CalculateEvaluate();
                _extent.BuildClass(extentEquation, true, cgOMS.Prefix + "COMPOUNDTREATMENT_EXTENT_" + compoundTreatmentID);
                _extent.CompileAssembly();
                SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + "COMPOUNDTREATMENT", "EXTENT", compoundTreatmentID, _extent);
            }
			foreach (string parameter in _extent.m_listParameters)
			{
				if (!_relevantAttributes.Contains(parameter)) _relevantAttributes.Add(parameter);
			}




            assembly = null;
            assembly = SimulationMessaging.GetSerializedCalculateEvaluate(cgOMS.Prefix + "COMPOUNDTREATMENT", "QUANTITY", compoundTreatmentID, null);
            if (assembly != null && assembly.Length > 0)
            {
                _quantity = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assembly);
                if (SimulationMessaging.Area.OriginalInput != extentEquation)
                {
                    _quantity = null;
                }
            }
            if (_quantity == null)
            {
                // Add quantity attributes
                _quantity = new CalculateEvaluate.CalculateEvaluate();
                _quantity.BuildClass(quantityCriteria, true, cgOMS.Prefix + "COMPOUNDTREATMENT_QUANTITY_" + compoundTreatmentID);
                _quantity.CompileAssembly();
                SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + "COMPOUNDTREATMENT", "QUANTITY", compoundTreatmentID, _quantity);
            }
			foreach (string parameter in _quantity.m_listParameters)
			{
				if (!_relevantAttributes.Contains(parameter)) _relevantAttributes.Add(parameter);
			}

			// Add any criteria attributes
			_criteria = new Criterias();
			if (!string.IsNullOrEmpty(criteriaString))
			{
				_criteria.Criteria = criteriaString;
				foreach (string parameter in _criteria.CriteriaAttributes)
				{
					if (!_relevantAttributes.Contains(parameter)) _relevantAttributes.Add(parameter);
				}
			}
		}

		public double GetExtent(Hashtable hashAttributeValue)
		{
			int i = 0;
			object[] input = new object[_extent.m_listParameters.Count];
			foreach (String str in _extent.m_listParameters)
			{
				if (str == "AREA")
				{
					input[i] = double.Parse(hashAttributeValue[str].ToString());
				}
				else if (str == "LENGTH")
				{
					input[i] = double.Parse(hashAttributeValue[str].ToString());
				}

				else
				{
					if (SimulationMessaging.GetAttributeType(str) == "STRING")
					{
						input[i] = hashAttributeValue[str].ToString();
					}
					else
					{
						input[i] = double.Parse(hashAttributeValue[str].ToString());
					}
				}
				i++;
			}
			try
			{
				object result = _extent.RunMethod(input);
				return (double)result;
			}
			catch (Exception exc)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod. " + exc.Message));
				return 0;
			}
		}

		public double GetQuantity(Hashtable hashAttributeValue)
		{
			int i = 0;
			object[] input = new object[_quantity.m_listParameters.Count];
			foreach (String str in _quantity.m_listParameters)
			{
				if (str == "AREA")
				{
					input[i] = double.Parse(hashAttributeValue[str].ToString());
				}
				else if (str == "LENGTH")
				{
					input[i] = double.Parse(hashAttributeValue[str].ToString());
				}

				else
				{
					if (SimulationMessaging.GetAttributeType(str) == "STRING")
					{
						input[i] = hashAttributeValue[str].ToString();
					}
					else
					{
						input[i] = double.Parse(hashAttributeValue[str].ToString());
					}
				}
				i++;
			}
			try
			{
				object result = _quantity.RunMethod(input);
				return (double)result;
			}
			catch (Exception exc)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod. " + exc.Message));
				return 0;
			}
		}

		public bool IsCriteriaMet(Hashtable hashAttributeValue)
		{
			return _criteria.IsCriteriaMet(hashAttributeValue);
		}

		public double Cost(Hashtable hashAttributeValue)
		{
			//return this.UnitCost * GetQuantity(hashAttributeValue);
			double cost = 0.0;
			try
			{
				cost = double.Parse(_cost.m_strResult);
			}
			catch(Exception exc)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Could not parse cost to a double. Cost set to zero." + exc.Message));
			}
			return cost;
		}
	}
}
