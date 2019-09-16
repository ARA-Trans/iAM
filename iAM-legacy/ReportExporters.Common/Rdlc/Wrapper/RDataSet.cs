using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Collections.ObjectModel;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	public class RDataSet : IRdlElement
	{
		#region SubClasses

		public class RDataSetField : IRdlElement
		{
			private string Name;
			private string TypeName;
			private string DataField;

			public RDataSetField(string name, string typeName, string dataField)
			{
				Name = name;
				TypeName = typeName;
				DataField = dataField;
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Field");
				{
					xmlWriter.WriteAttributeString("Name", Name);
					xmlWriter.WriteElementString("rd:TypeName", TypeName);
					xmlWriter.WriteElementString("DataField", DataField);
				}
				xmlWriter.WriteEndElement();
			}
		}

		public class RQuery : IRdlElement
		{
			private string CommandText;
			private bool UseGenericDesigner;
			private string DataSourceName;

			public RQuery(string commandText, bool useGenericDesigner, string dataSourceName)
			{
				CommandText = commandText;
				UseGenericDesigner = useGenericDesigner;
				DataSourceName = dataSourceName;
			}
			
			public RQuery()
			{
				UseGenericDesigner = true;
				DataSourceName = "DummyDataSource";
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Query");
				{
					xmlWriter.WriteAttributeString("rd:UseGenericDesigner", RdlcWrapper.RdlcValueConverter.GetBoolean(UseGenericDesigner));
					xmlWriter.WriteElementString("CommandText", CommandText);
					xmlWriter.WriteElementString("DataSourceName", DataSourceName);
				}
				xmlWriter.WriteEndElement();
			}
		}

		public class RDataSetInfo : IRdlElement
		{
			private string ObjectDataSourceSelectMethod;
			private string DataSetName;
			private string ObjectDataSourceType;
			private string TableName;

			public RDataSetInfo()
			{
			}

			public RDataSetInfo(string objectDataSourceSelectMethod, string dataSetName, string objectDataSourceType, string tableName)
			{
				ObjectDataSourceSelectMethod = objectDataSourceSelectMethod;
				DataSetName = dataSetName;
				ObjectDataSourceType = objectDataSourceType;
				TableName = tableName;
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("rd:DataSetInfo");
				{
					xmlWriter.WriteElementString("rd:ObjectDataSourceSelectMethod", ObjectDataSourceSelectMethod);
					xmlWriter.WriteElementString("rd:DataSetName", DataSetName);
					//xmlWriter.WriteElementString("rd:ObjectDataSourceType", ObjectDataSourceType);
					xmlWriter.WriteElementString("rd:TableName", TableName);
				}
				xmlWriter.WriteEndElement();
			}
		}

		#endregion

		public string Name;
		public RDataSetInfo DataSetInfo1;
		public RQuery Query1;
		public List<RDataSetField> Fields;

		//public RDataSet(string name)
		//{
		//  Name = name;
		//  Fields = new List<RDataSetField>();
		//  Query1 = new RQuery();
		//  DataSetInfo1 = new RDataSetInfo();
		//}

		/// <summary>
		/// Create RDataSet from DataTable
		/// </summary>
		/// <param name="dTable"></param>
		public RDataSet(DataTable dTable) 
		{
			if (dTable.TableName == "")
			{
				throw new Exception("Please set table name!");
			}

			Query1 = new RQuery();
			Name = GetDataSetName(dTable);
			DataSetInfo1 = new RDataSetInfo(dTable.TableName, dTable.DataSet.DataSetName, "", dTable.TableName);
			Fields = new List<RDataSetField>();

			foreach (DataColumn dtCol in dTable.Columns)
			{
				RDataSetField field = new RDataSetField(dtCol.ColumnName, dtCol.DataType.ToString(), dtCol.ColumnName);
				Fields.Add(field);
			}
		}

		public static string GetDataSetName(DataTable dTable)
		{
			return string.Format("{0}_{1}", dTable.DataSet.DataSetName, dTable.TableName);
		}

		public void WriteTo(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("DataSet");
			{
				xmlWriter.WriteAttributeString("Name", Name);

				this.DataSetInfo1.WriteTo(xmlWriter);
				this.Query1.WriteTo(xmlWriter);

				xmlWriter.WriteStartElement("Fields");
				{
					foreach (RDataSetField field in Fields)
					{
						field.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}
	}

	internal class RDataSets : Collection<RDataSet>, IRdlElement
	{
		#region IRdlElement Members

		public void WriteTo(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("DataSets");
			{
				foreach (RDataSet dataSet in this)
				{
					dataSet.WriteTo(xmlWriter);
				}
			}
			xmlWriter.WriteEndElement();
		}

		#endregion
	}

}
