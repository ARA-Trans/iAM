using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;
using System.CodeDom.Compiler;
using RoadCareGlobalOperations;

namespace CompileEquationCriteria
{
	public class BinaryLoader
	{
		private List<string> m_listCriteria;
		private List<string> m_listEquations;
		private Dictionary<string, string> m_attributeToType;
		private string errors;

		public BinaryLoader()
		{
			m_attributeToType = new Dictionary<string, string>();
			DataSet attributeTypes = DBMgr.ExecuteQuery("SELECT ATTRIBUTE_, TYPE_ FROM ATTRIBUTES_");
			foreach (DataRow attributeType in attributeTypes.Tables[0].Rows)
			{
				m_attributeToType.Add(attributeType["ATTRIBUTE_"].ToString(), attributeType["TYPE_"].ToString());
			}

			m_listCriteria = new List<string>();
			m_listEquations = new List<string>();

			// These tables contain only criteria columns.
			m_listCriteria.Add("CONSEQUENCES");
			m_listCriteria.Add("COSTS");
			m_listCriteria.Add("FEASIBILITY");
			m_listCriteria.Add("DEFICIENTS");
			m_listCriteria.Add("TARGETS");
			m_listCriteria.Add("PRIORITY");
			m_listCriteria.Add("PERFORMANCE");
			m_listCriteria.Add("ATTRIBUTES_CALCULATED");

			// These tables contain both criteria and equation columns.
			m_listEquations.Add("PERFORMANCE");
			m_listEquations.Add("ATTRIBUTES_CALCULATED");

			errors = "";
		}

		public void CreateDLLs()
		{
			// First we need to get the criteria and equations from each table in the database
			// The member variable m_listCriteriaEquations will contain those table names
			List<CalculateEvaluate.CalculateEvaluate> compiledEquations = new List<CalculateEvaluate.CalculateEvaluate>();
			List<CalculateEvaluate.CalculateEvaluate> compiledCriteria = new List<CalculateEvaluate.CalculateEvaluate>();
			foreach (String tableName in m_listEquations)
			{
				CompileEquations(tableName);
			}
			foreach (String tableName in m_listCriteria)
			{
				CompileCriteria(tableName);
			}
			CompileCostEquations();

			// After successful compilation of the binaries, we update the database table with
			// the new DLL assembly information.
			
		}

		private void CompileCostEquations()
		{
			try
			{
				List<string> columnNames = DBMgr.GetTableColumns("COSTS");
				string queryEquations = "SELECT " + columnNames[0].ToString() + ", COST_ FROM COSTS";
				DataSet equations = DBMgr.ExecuteQuery(queryEquations);
				string equation;
				string id;

				// Compile each equation in the table.
				CalculateEvaluate.CalculateEvaluate calcEval;
				foreach (DataRow rowEquation in equations.Tables[0].Rows)
				{
					equation = rowEquation["COST_"].ToString();
                    if(equation.Contains("COMPOUND_TREATMENT")) continue;
					id = rowEquation[0].ToString();

					calcEval = new CalculateEvaluate.CalculateEvaluate();
					calcEval.BuildClass(equation, true);
					calcEval.CompileAssembly();
					//UpdateCosts(id, calcEval);
				}
			}
			catch (Exception exc)
			{
				errors += "\n Error occured while getting and compiling equation in table COSTS. " + exc.Message;
			}
			return;
		}

		private void CompileEquations(string tableName)
		{
			// Get the equation information out of the table
			try
			{
				List<string> columnNames = DBMgr.GetTableColumns(tableName);
				string queryEquations = "SELECT " + columnNames[0].ToString() + ", EQUATION FROM " + tableName;
				DataSet equations = DBMgr.ExecuteQuery(queryEquations);
				string equation;
				string id;
				
				// Compile each equation in the table.
				CalculateEvaluate.CalculateEvaluate calcEval;
				foreach (DataRow rowEquation in equations.Tables[0].Rows)
				{
					equation = rowEquation["EQUATION"].ToString();
					id = rowEquation[0].ToString();

					calcEval = new CalculateEvaluate.CalculateEvaluate();
					calcEval.BuildClass(equation, true);
					calcEval.CompileAssembly();
					UpdateEquation(tableName, id, columnNames[0].ToString(), calcEval);
				}
			}
			catch (Exception exc)
			{
				errors += "\n Error occured while getting and compiling equation in table " + tableName + ". " + exc.Message;
			}
			return;
		}

		private void CompileCriteria(string tableName)
		{
			// Get the criteria statement out of the database and in the correct format.
			try
			{
				List<string> columnNames = DBMgr.GetTableColumns(tableName);
				string queryCriteria = "SELECT " + columnNames[0].ToString() + ", CRITERIA FROM " + tableName;
				DataSet criterium = DBMgr.ExecuteQuery(queryCriteria);
				string criteria;
				string id;

				// Compile each criteria in the table.
				CalculateEvaluate.CalculateEvaluate calcEval;
				foreach (DataRow rowCriteria in criterium.Tables[0].Rows)
				{
					if (rowCriteria["CRITERIA"] != null && rowCriteria["CRITERIA"].ToString() != "")
					{
						id = rowCriteria[0].ToString();
						criteria = rowCriteria["CRITERIA"].ToString().Replace("|", "'").ToUpper();

						List<String> listAttribute = ParseAttribute(criteria);
						foreach (String str in listAttribute)
						{
							String strType = m_attributeToType[str].ToString();
							if (strType == "STRING")
							{
								String strOldValue = "[" + str + "]";
								String strNewValue = "[@" + str + "]";
								criteria = criteria.Replace(strOldValue, strNewValue);
							}
						}

						calcEval = new CalculateEvaluate.CalculateEvaluate();
						calcEval.BuildClass(criteria, false,tableName + "_BINARY_CRITERIA_" + id);
						calcEval.CompileAssembly();
						//UpdateCriteria(tableName, id, columnNames[0].ToString(), calcEval);
					}

				}
			}
			catch (Exception exc)
			{
				errors += "\n Error occured while getting and compiling criteria in table " + tableName + ". " + exc.Message;
			}
			return;
		}

		private void UpdateCriteria(string tableName, string ID, string IDFieldName, CalculateEvaluate.CalculateEvaluate calcEval)
		{
            //byte[] assembly = RoadCareGlobalOperations.AssemblySerialize.SerializeObjectToByteArray(calcEval);
            //GlobalDatabaseOperations.SaveBinaryObjectToDatabase(ID, IDFieldName, tableName, "BINARY_CRITERIA", assembly);
		}

		private void UpdateEquation(string tableName, string ID, string IDFieldName, CalculateEvaluate.CalculateEvaluate calcEval)
		{
            //byte[] assembly = RoadCareGlobalOperations.AssemblySerialize.SerializeObjectToByteArray(calcEval);
            //GlobalDatabaseOperations.SaveBinaryObjectToDatabase(ID, IDFieldName, tableName, "BINARY_EQUATION", assembly);
		}

		private void UpdateCosts(string ID, CalculateEvaluate.CalculateEvaluate calcEval)
		{
            //byte[] assembly = RoadCareGlobalOperations.AssemblySerialize.SerializeObjectToByteArray(calcEval);
            //GlobalDatabaseOperations.SaveBinaryObjectToDatabase(ID, "COSTID", "COSTS", "BINARY_COST", assembly);
		}

		private List<String> ParseAttribute(string strQuery)
		{
            List<String> list = new List<String>();
            String strAttribute;
            int nOpen = -1;

            for (int i = 0; i < strQuery.Length; i++)
            {
                if (strQuery.Substring(i, 1) == "[")
                {
                    nOpen = i;
                    continue;
                }

                if (strQuery.Substring(i, 1) == "]" && nOpen > -1)
                {
                    //Get the value between open and (i)
                    strAttribute = strQuery.Substring(nOpen + 1, i - nOpen - 1);

                    if (!list.Contains(strAttribute))
                    {
                        if (m_attributeToType[strAttribute].ToString() == "")
                        {
                            errors += "\n Attribute - " + strAttribute + " is not included in the RoadCare Attribute types.  Please check spelling.";
                        }
                        else
                        {
                            list.Add(strAttribute);
                        }
                    }
                }
            }
            return list;
		}
	}
}
