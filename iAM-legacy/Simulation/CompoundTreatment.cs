using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DataObjects;
//using PCI;
using System.Data;
using DatabaseManager;
using RoadCare3;

namespace Simulation
{
    public class CompoundTreatment
    {
        string m_strCompoundTreatment;
		List<CompoundTreatmentElement> _compoundTreatmentElements;
        List<PCIDistressObject> m_listPCIDistress;
        List<string> m_listAttributes = new List<string>();
		List<Deteriorate> m_listCalculated;
		private string _affectedAttribute = "";
		private string _compoundTreatmentID;

		#region Properties
		public string CompoundTreatmentID
		{
			get { return _compoundTreatmentID; }
		}

		public string AffectedAttribute
		{
			get { return _affectedAttribute; }
		}

		public List<CompoundTreatmentElement> CompoundTreatmentElements
		{
			get { return _compoundTreatmentElements; }
		}

        public string CompoundTreatmentName
        {
            get { return m_strCompoundTreatment; }
        }

        public List<string> Attributes
        {
            get { return m_listAttributes; }
		}
		#endregion

		public CompoundTreatment(string strCompoundTreatmentName)
        {
            m_strCompoundTreatment = strCompoundTreatmentName;
            m_listPCIDistress = LoadPCIDistress();
            _compoundTreatmentElements = LoadCompoundTreatmentElements(strCompoundTreatmentName);
            LoadAttributes();
			GetCalculatedFieldData();
        }

        private void LoadAttributes()
        {
            foreach (CompoundTreatmentElement cta in _compoundTreatmentElements)
            {
                foreach (string attribute in cta.RelevantAttributes)
                {
					if (!m_listAttributes.Contains(attribute)) m_listAttributes.Add(attribute);
                }
            }
        }

        private List<PCIDistressObject> LoadPCIDistress()
        {
            List<PCIDistressObject> listDistress = new List<PCIDistressObject>();
            string strSelect = "SELECT DISTINCT METHOD_,DISTRESSNAME,DISTRESSNUMBER,ATTRIBUTE_,METRIC_CONVERSION FROM " + cgOMS.Prefix + "PCI_DISTRESS";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string strMethod = row["METHOD_"].ToString();
                string strDistress = row["DISTRESSNAME"].ToString();
                string strAttribute = row["ATTRIBUTE_"].ToString();
                int nDistressNumber = (int)row["DISTRESSNUMBER"];
                float fMetric = float.Parse(row["METRIC_CONVERSION"].ToString());

                PCIDistressObject pciDistress = new PCIDistressObject(strDistress, nDistressNumber, fMetric, strAttribute, strMethod);
                listDistress.Add(pciDistress);
            }
            return listDistress;
        }

        private List<CompoundTreatmentElement> LoadCompoundTreatmentElements(string strCompoundTreatmentName)
        {
			List<CompoundTreatmentElement> listCompoundTreatment = new List<CompoundTreatmentElement>();
			string strSelect = "SELECT ct.COMPOUND_TREATMENT_ID, AFFECTED_ATTRIBUTE, COMPOUND_TREATMENT_NAME ,ATTRIBUTE_FROM, ATTRIBUTE_TO,EXTENT_,QUANTITY_,COST_,CRITERIA_ FROM COMPOUND_TREATMENT_ELEMENTS cte RIGHT JOIN COMPOUND_TREATMENTS ct ON cte.COMPOUND_TREATMENT_ID = ct.COMPOUND_TREATMENT_ID WHERE COMPOUND_TREATMENT_NAME='" + strCompoundTreatmentName + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                try
                {
					string compoundTreatmentID = row["COMPOUND_TREATMENT_ID"].ToString();
					string affectedAttribute = row["AFFECTED_ATTRIBUTE"].ToString();

					// These two properties could probably be moved out of this loop, they are assigned the same value each time through.
					_affectedAttribute = affectedAttribute;
					_compoundTreatmentID = compoundTreatmentID;

					string strTreatmentName = row["COMPOUND_TREATMENT_NAME"].ToString();
					string strAttributeFrom = "";
					string strAttributeTo = "";
					string strExtent = "";
					string strQuantity = "";
					string strCriteria = "";
					string costEquation = "";

					if(!String.IsNullOrEmpty(row["ATTRIBUTE_FROM"].ToString()))
					{
						strAttributeFrom = row["ATTRIBUTE_FROM"].ToString();
					}
					if (!String.IsNullOrEmpty(row["ATTRIBUTE_TO"].ToString()))
					{
						strAttributeTo = row["ATTRIBUTE_TO"].ToString();
					}
					if (!String.IsNullOrEmpty(row["EXTENT_"].ToString()))
					{
						strExtent = row["EXTENT_"].ToString();
					}
					if (!String.IsNullOrEmpty(row["QUANTITY_"].ToString()))
					{
						strQuantity = row["QUANTITY_"].ToString();
					}
					if (!String.IsNullOrEmpty(row["CRITERIA_"].ToString()))
					{
						strCriteria = row["CRITERIA_"].ToString();
					}
					if (!String.IsNullOrEmpty(row["COST_"].ToString()))
					{
						costEquation = row["COST_"].ToString();
					}
					if (strTreatmentName != "" && strAttributeFrom != "" && strAttributeTo != "" && strExtent != "" && strQuantity != "")
					{
						listCompoundTreatment.Add(new CompoundTreatmentElement(compoundTreatmentID, strTreatmentName, strAttributeFrom, strAttributeTo, costEquation, strExtent, strQuantity, strCriteria));
					}
                }
                catch(Exception ex)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("Error parsing compound treatment." + ex.Message));
                }
                
            }
            return listCompoundTreatment;
        }

        public double GetCost(Hashtable hashAttributeValue)
        {
            double dCost = 0;

			foreach (CompoundTreatmentElement cta in _compoundTreatmentElements)
            {
                if(cta.IsCriteriaMet(hashAttributeValue))
                {
                    dCost += cta.Cost(hashAttributeValue);
                }
            }
            return dCost;
        }

        public Dictionary<string, double> GetConsequences(Hashtable hashAttributeValue)
        {
            Dictionary<string,double> dictionaryAttributeConsequences = GetPCIConsequences(hashAttributeValue);

			foreach (CompoundTreatmentElement cta in _compoundTreatmentElements)
            {
                if(cta.IsCriteriaMet(hashAttributeValue))
                {
                    //If it is in list, set to zero it as it is now changed to something else
                    if(dictionaryAttributeConsequences.ContainsKey(cta.AttributeFrom))
                    {
                        dictionaryAttributeConsequences[cta.AttributeFrom] = 0;
                    }

                    //Now insert new extent or add existing.
                    double dExtent = cta.GetExtent(hashAttributeValue);
                    if(!string.IsNullOrEmpty(cta.AttributeTo))
                    {
                        if (!dictionaryAttributeConsequences.ContainsKey(cta.AttributeTo))
                        {
                            dictionaryAttributeConsequences.Add(cta.AttributeTo, dExtent);
                        }
                        else
                        {
                            dictionaryAttributeConsequences[cta.AttributeTo] += dExtent;
                        }
                    }
                }
            }

			foreach (string key in hashAttributeValue.Keys)
			{
				if (key == "PCI")
				{
					dictionaryAttributeConsequences.Add("PCI", GetPCIConsequence(hashAttributeValue, dictionaryAttributeConsequences));
				}
				else
				{
					if(m_listCalculated.Find(x=>x.Attribute==key) != null)
					{
						dictionaryAttributeConsequences.Add(key, GetOtherConsequence(key, hashAttributeValue, dictionaryAttributeConsequences));
					}
				}
			}

            return dictionaryAttributeConsequences;
        }

		private double GetOtherConsequence(string key, Hashtable hashAttributeValue, Dictionary<string, double> dictionaryAttributeConsequences)
		{

			List<Deteriorate> listKey = m_listCalculated.FindAll(delegate(Deteriorate deteriorate) { return deteriorate.Attribute == key; });

			Deteriorate defaultCalculated = null;
			foreach(Deteriorate calculated in listKey)
			{
				if(calculated.Criteria.Trim() == "")
				{
					defaultCalculated = calculated;
				}
				else
				{
					if (calculated.IsCriteriaMet(hashAttributeValue)) 
					{
						return calculated.ApplyDeterioration(hashAttributeValue);
					}
				}
			}
			return 0;
		}



        private Dictionary<string, double> GetPCIConsequences(Hashtable hashAttributeValue)
        {
            Dictionary<string, double> dictionaryConsequences = new Dictionary<string, double>();
            string strDistress = "";
            string strSeverity = "";
            string strMethod = "";


            foreach (string key in hashAttributeValue.Keys)
            {
                if (IsPCIDistress(key, out strDistress, out strSeverity, out strMethod))
                {
                    dictionaryConsequences.Add(key,double.Parse(hashAttributeValue[key].ToString()));
                }
            }
            return dictionaryConsequences;
        }

        public bool IsPCIDistress(string strAttribute, out string strDistress, out string strSeverity, out string strMethod)
        {
            strDistress = "";
            strSeverity = "";
            strMethod = "";
            if(strAttribute.Length < 3) return false;

            string strPossibleDistress = strAttribute;
            if (strPossibleDistress.Contains("_NO_SEV"))
            {
                strPossibleDistress = strPossibleDistress.Replace("_NO_SEV", "");
                strSeverity = "NS";
            }
            else
            {
                strPossibleDistress = strAttribute.Substring(strAttribute.Length - 2, 2);
                if (strPossibleDistress.Substring(0, 1) == "_")
                {
                    strSeverity = strPossibleDistress.Substring(1, 1);
                    strPossibleDistress = strAttribute.Substring(0, strAttribute.Length - 2);
                }
            }
            PCIDistressObject pciDistress = m_listPCIDistress.Find(delegate(PCIDistressObject distress) { return distress.Attribute == strPossibleDistress; });
            if (pciDistress == null) return false;
            strMethod = pciDistress.Method;
            strDistress = pciDistress.Distress;
            strAttribute = pciDistress.Attribute;
            return true;
        }


        public double GetPCIConsequence(Hashtable hashAttributeValue, Dictionary<string, double> dictionaryConsequence)
        {
            StringBuilder sbDeduct = new StringBuilder();
            double dPCI = 100.0;
            string strMethod = "";
            foreach (String strKey in hashAttributeValue.Keys)
            {
                string strDistress = "";
                string strSeverity = "";
                
                double dExtent = 0;
                if(IsPCIDistress(strKey,out strDistress,out strSeverity,out strMethod))
                {
                    //Check if key is PCI....
                    if (dictionaryConsequence.ContainsKey(strKey))
                    {
                        dExtent = dictionaryConsequence[strKey];
                    }
                    else
                    {
                        dExtent = double.Parse(hashAttributeValue[strKey].ToString());
                    }
                    double dDeduct = CalculateCurrentRowDeducts(strMethod, strDistress, strSeverity, dExtent, 1);
                    if (dDeduct > 0)
                    {
                        if (sbDeduct.Length != 0) sbDeduct.Append(",");
                        sbDeduct.Append(dDeduct.ToString());
                    }
                }
            }
            if (sbDeduct.Length > 0)
            {
         //       dPCI = Distress.ComputePCIValue(sbDeduct.ToString(), strMethod.Trim());
            }
            return double.Parse(dPCI.ToString("f1"));
        }


		private double CalculateCurrentRowDeducts(String strMethod, String strDistress, String strSeverity, double dAmount, double dArea)
		{
			PCIDistressObject pciDistress = m_listPCIDistress.Find(delegate(PCIDistressObject distress) { return distress.Method == strMethod && distress.Distress == strDistress; });

			if (pciDistress == null) return 0;
            return 0;// Distress.pvt_ComputePCIDeduct(pciDistress.DistressNumber, strSeverity, dAmount, dArea);
		}

		private bool GetCalculatedFieldData()
		{
			if (m_listCalculated == null) m_listCalculated = new List<Deteriorate>();
			String strSelect = "SELECT ATTRIBUTE_,EQUATION,CRITERIA FROM " + cgOMS.Prefix + "ATTRIBUTES_CALCULATED";
			DataSet ds;
			try
			{
				ds = DBMgr.ExecuteQuery(strSelect);
			}
			catch (Exception exception)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Error: Retrieving Calculated Field data. SQL Message - " + exception.Message));
				return false;
			}

			foreach (DataRow row in ds.Tables[0].Rows)
			{

				Deteriorate deteriorate = new Deteriorate();

				deteriorate.Attribute = row["ATTRIBUTE_"].ToString();
				deteriorate.Equation = row["EQUATION"].ToString();
				deteriorate.Criteria = row["CRITERIA"].ToString();
				m_listCalculated.Add(deteriorate);
			}


			return true;
		}
	}
}
